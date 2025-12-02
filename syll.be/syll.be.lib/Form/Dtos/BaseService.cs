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

namespace syll.be.lib.Form.Dtos
{
    public class BaseService
    {
        public readonly SyllDbContext _syllDbContext;
        public readonly ILogger<BaseService> _logger;
        public readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public BaseService(
            SyllDbContext SyllDbContext,
            ILogger<BaseService> logger,
            IHttpContextAccessor httpContextAccessor

        )
        {
            _syllDbContext = SyllDbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

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

        protected async Task<int?> GetCurrentDanhBaId()
        {
            var username = _httpContextAccessor.HttpContext?.User.FindFirstValue(Claims.Username);

            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            var danhBa = await _syllDbContext.Set<domain.DanhBa.DanhBa>()
                .AsNoTracking()
                .Where(x => x.Email == username && !x.Deleted)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            return danhBa == 0 ? null : danhBa;
        }

        protected static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}