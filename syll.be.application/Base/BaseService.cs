using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using syll.be.infrastructure.data;
using syll.be.shared.Constants.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace syll.be.application.Base
{
    public class BaseService
    {
        public readonly SyllDbContext _syllDbContext;
        public readonly ILogger<BaseService> _logger;
        public readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        protected readonly IMapper _mapper;
        public BaseService(
            SyllDbContext syllDbContext,
            ILogger<BaseService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
        {
            _syllDbContext = syllDbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        protected string getCurrentUserId()
        {
            var data = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(data))
            {
                data = _httpContextAccessor.HttpContext?.User.FindFirstValue(Claims.Subject);
            }
            //_logger.LogInformation($"getCurrentUserId: {data}");
            return data!;
        }
        protected string getCurrentName()
        {
            var data = _httpContextAccessor.HttpContext?.User.FindFirstValue(Claims.Name);
            return data!;
        }
        protected bool IsSuperAdmin()
        {
            var roles = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).ToList();
            var isSuperAdmin = roles?.Any(r => r.Value == RoleConstants.ROLE_SUPER_ADMIN) ?? false;
            return isSuperAdmin;
        }
        protected static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}