# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["syll.be/syll.be.csproj", "syll.be/"]
COPY ["syll.be.application/syll.be.application.csproj", "syll.be.application/"]
COPY ["syll.be.infrastructure.data/syll.be.infrastructure.data.csproj", "syll.be.infrastructure.data/"]
COPY ["syll.be.domain/syll.be.domain.csproj", "syll.be.domain/"]
COPY ["syll.be.shared/syll.be.shared.csproj", "syll.be.shared/"]
COPY ["syll.be.infrastructure.external/syll.be.infrastructure.external.csproj", "syll.be.infrastructure.external/"]
RUN dotnet restore "./syll.be/syll.be.csproj"
COPY . .
WORKDIR "/src/syll.be"
RUN dotnet build "./syll.be.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./syll.be.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "syll.be.dll"]