using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using syll.be.application.Base;
using syll.be.application.DanhBa.Dtos;
using syll.be.application.DanhBa.Interfaces;
using syll.be.infrastructure.data;
using syll.be.shared.HttpRequest.BaseRequest;
using syll.be.shared.HttpRequest.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace syll.be.application.DanhBa.Implements
{
    public class DanhBaService : BaseService, IDanhBaService
    {
        private readonly IConfiguration _configuration;
        /*private readonly string[] Scopes = {
               "https://www.googleapis.com/auth/drive",
               "https://www.googleapis.com/auth/drive.file",
               "https://www.googleapis.com/auth/spreadsheets"
        };*/
        //private readonly IMemoryCache _memoryCache;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public DanhBaService(
            SyllDbContext syllDbContext,
            ILogger<DanhBaService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IConfiguration configuration
            //IMemoryCache memoryCache
        )
            : base(syllDbContext, logger, httpContextAccessor, mapper)
        {
            _configuration = configuration;
            //_memoryCache = memoryCache;
        }
        public BaseResponsePagingDto<ViewDanhBaDto> FindDanhBa( FindPagingDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(FindDanhBa)} dto={JsonSerializer.Serialize(dto)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var query = from db in _syllDbContext.DanhBas
                        where  !db.Deleted
                              && (string.IsNullOrEmpty(dto.Keyword)
                                  || db.HoVaTen.Contains(dto.Keyword)
                                  || db.HoDem.Contains(dto.Keyword)
                                  || db.Ten.Contains(dto.Keyword)
                                  || db.Email.Contains(dto.Keyword))
                        orderby db.Id ascending
                        select db;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewDanhBaDto>>(data);
            var response = new BaseResponsePagingDto<ViewDanhBaDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
            return response;
        }
    }
}
