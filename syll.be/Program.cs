﻿using Azure.Identity;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using OpenIddict.Abstractions;
using syll.be.application.Auth.Implements;
using syll.be.application.Auth.Interfaces;
using syll.be.application.Base;
using syll.be.application.DanhBa.Implements;
using syll.be.application.DanhBa.Interfaces;
using syll.be.application.ToChuc.Implements;
using syll.be.application.ToChuc.Interfaces;
using syll.be.domain.Auth;
using syll.be.infrastructure.data;
using syll.be.infrastructure.data.Seeder;
using syll.be.infrastructure.external.BackgroundJob;
using syll.be.shared.Constants.Auth;
using syll.be.shared.Settings;
using syll.be.Workers;
using System.Text;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Starting application...");
var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("EnvironmentName => " + builder.Environment.EnvironmentName);


builder.Logging.ClearProviders();
builder.Host.UseNLog();

#region db
string connectionString = builder.Configuration.GetConnectionString("SYLL")
    ?? throw new InvalidOperationException("Không tìm thấy connection string \"SYLL\" trong appsettings.json");

string hangfireConnectionString = builder.Configuration.GetConnectionString("HANGFIRE")
    ?? throw new InvalidOperationException("Không tìm thấy connection string \"HANGFIRE\" trong appsettings.json");

builder.Services.AddDbContext<SyllDbContext>(options =>
{
    options.UseSqlServer(connectionString, options =>
    {
        //options.MigrationsAssembly(typeof(Program).Namespace);
        //options.MigrationsHistoryTable(DbSchemas.TableMigrationsHistory, DbSchemas.Core);
        options.CommandTimeout(600);
    });
    options.UseOpenIddict(); // Register OpenIddict entities
}, ServiceLifetime.Scoped);
#endregion

#region cors
string allowOrigins = builder.Configuration.GetSection("AllowedHosts")!.Value!;
//File.WriteAllText("cors.now.txt", $"CORS: {allowOrigins}");;/'\,
Console.WriteLine($"CORS: {allowOrigins}");
var origins = allowOrigins
    .Split(';')
    .Where(s => !string.IsNullOrWhiteSpace(s))
    .ToArray();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        ProgramExtensions.CorsPolicy,
        builder =>
        {
            builder
                .WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                //.AllowCredentials()
                .WithExposedHeaders("Content-Disposition");
        }
    );
    options.AddPolicy("SignalRPolicy", builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
#endregion

#region identity
// 2. Add Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<SyllDbContext>()
    .AddDefaultTokenProviders();
#endregion

#region auth
string secretKey = builder.Configuration.GetSection("AuthServer:SecretKey").Value!;
string googleClientId = builder.Configuration.GetSection("AuthServer:Google:ClientId").Value!;
string googleClientSecret = builder.Configuration.GetSection("AuthServer:Google:ClientSecret").Value!;
string googleRedirectUri = builder.Configuration.GetSection("AuthServer:Google:RedirectUri").Value!;

//string msClientId = builder.Configuration["AuthServer:MS:ClientId"];
//string msClientSecret = builder.Configuration.GetSection("AuthServer:MS:ClientSecret").Value!;
//string msRedirectUri = builder.Configuration.GetSection("AuthServer:MS:RedirectUri").Value!;

builder.Services.Configure<AuthServerSettings>(builder.Configuration.GetSection("AuthServer"));
builder.Services.Configure<AuthGoogleSettings>(builder.Configuration.GetSection("AuthServer:Google"));
//builder.Services.Configure<AuthMsSettings>(builder.Configuration.GetSection("AuthServer:MS"));
//builder.Services.Configure<CdsConnectSettings>(builder.Configuration.GetSection("CdsConnect:Url"));


builder.Services.AddOpenIddict()
    .AddCore(opt =>
    {
        opt.UseEntityFrameworkCore()
           .UseDbContext<SyllDbContext>();
    })
    // Register the OpenIddict server components.
    .AddServer(options =>
    {
        // Enable the token endpoint.
        options.SetTokenEndpointUris("connect/token")
            .SetAuthorizationEndpointUris("/connect/authorize")
        ;

        // Enable the client credentials flow.
        options.AllowClientCredentialsFlow()
                .AllowPasswordFlow()
                .AllowRefreshTokenFlow()
                .AllowAuthorizationCodeFlow()
                .RequireProofKeyForCodeExchange()
                ;

        options.AcceptAnonymousClients();
        options.DisableAccessTokenEncryption();

        options.RegisterScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.OfflineAccess, OpenIddictConstants.Scopes.Profile);

        // Register the signing and encryption credentials.
        //options.AddDevelopmentEncryptionCertificate()
        //       .AddDevelopmentSigningCertificate();

        // Development: ephemeral keys (or dev certs if you want)
        options.AddEphemeralEncryptionKey()
               .AddEphemeralSigningKey();

        // 🔑 Symmetric signing key
        var secret = Encoding.UTF8.GetBytes(secretKey);
        options.AddEncryptionKey(new SymmetricSecurityKey(secret));
        options.AddSigningKey(new SymmetricSecurityKey(secret));

        // Register the ASP.NET Core host and configure the ASP.NET Core options.
        options.UseAspNetCore()
                .EnableAuthorizationEndpointPassthrough()
               .EnableTokenEndpointPassthrough()
               .DisableTransportSecurityRequirement();

    });

builder.Services.AddAuthentication(options =>
{
    //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,        // ✅ only check exp & nbf
                ClockSkew = TimeSpan.Zero, // no extra leeway

                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                // Symmetric key (HMAC) example
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                ),

                // 👇 Accept both "JWT" and "at+jwt" as token types
                ValidTypes = new[] { "JWT", "at+jwt" }
            };
            options.RequireHttpsMetadata = false;
        }
    )
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.ReturnUrlParameter = "redirect_uri";
        //options.CallbackPath = googleRedirectUri;
    });
    /*.AddMicrosoftAccount(options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //options.ClientId = msClientId;
        //options.ClientSecret = msClientSecret;
    });*/

builder.Services.AddAuthorization();


builder.Services.AddHostedService<AuthWorker>();
#endregion

#region mapper
// Build mapper configuration
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
#endregion

#region hangfire
builder.Services.ConfigureHangfire(hangfireConnectionString);
#endregion


// Add services to the container.
#region service
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IToChucService, ToChucService>();
builder.Services.AddScoped<IDanhBaService, DanhBaService>();

#endregion

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<GraphServiceClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var options = new ClientSecretCredentialOptions
    {
        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
    };

    var clientSecretCredential = new ClientSecretCredential(
        configuration["AzureAd:TenantId"],
        configuration["AzureAd:ClientId"],
        configuration["AzureAd:ClientSecret"],
        options);

    return new GraphServiceClient(clientSecretCredential);
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });

    // 🔑 Add Bearer JWT Security Definition
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\"",
    });

    // 🔐 Add Security Requirement (apply globally to all endpoints)
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

#region Seed data
// Run seeding inside scope
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedUser.SeedAsync(userManager, roleManager);

}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(ProgramExtensions.CorsPolicy);
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard();
app.MapHealthChecks("/health");
app.Run();
