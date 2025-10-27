using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using syll.be.application.Base;
using syll.be.application.ToChuc.Dtos;
using syll.be.application.ToChuc.Interfaces;
using syll.be.infrastructure.data;
using syll.be.shared.Constants.ToChuc;
using syll.be.shared.HttpRequest.AppException;
using syll.be.shared.HttpRequest.BaseRequest;
using syll.be.shared.HttpRequest.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace syll.be.application.ToChuc.Implements
{
    public class ToChucService : BaseService, IToChucService
    {
        private readonly IConfiguration _configuration;
        

        public ToChucService(
            IConfiguration configuration,
            SyllDbContext syllDbContext,
            ILogger<ToChucService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
            : base(syllDbContext, logger, httpContextAccessor, mapper)
        {
            _configuration = configuration;
        }


        public void Create(CreateToChucDto dto)
        {
            var currentUserId = getCurrentUserId();
            var isSuperAdmin = IsSuperAdmin();
            _logger.LogInformation($"{nameof(Create)}  dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            if (dto.LoaiToChuc != ToChucConstants.DaiHocCongLap && dto.LoaiToChuc != ToChucConstants.PhongBan && dto.LoaiToChuc != ToChucConstants.KhoaDaoTao)
            {
                throw new UserFriendlyException(ErrorCodes.ToChucErrorLoaiToChucNotFound);
            }
            var toChuc = new domain.ToChuc.ToChuc
            {
                TenToChuc = dto.TenToChuc,
                MoTa = dto.MoTa,
                LoaiToChuc = dto.LoaiToChuc,
                MaSoToChuc = dto.MaSoToChuc,
                CreatedBy = currentUserId,
                CreatedDate = vietnamNow,
                Deleted = false,

            };
            _syllDbContext.ToChucs.Add(toChuc);
            _syllDbContext.SaveChanges();
        }
        public void Update(int idToChuc, UpdateToChucDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}  dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var isSuperAdmin = IsSuperAdmin();
            var existingToChuc = _syllDbContext.ToChucs.FirstOrDefault(x => x.Id == idToChuc && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ToChucErrorNotFound);
            if (dto.LoaiToChuc != ToChucConstants.DaiHocCongLap && dto.LoaiToChuc != ToChucConstants.PhongBan && dto.LoaiToChuc != ToChucConstants.KhoaDaoTao)
            {
                throw new UserFriendlyException(ErrorCodes.ToChucErrorLoaiToChucNotFound);
            }
            existingToChuc.TenToChuc = dto.TenToChuc;
            existingToChuc.MoTa = dto.MoTa;
            existingToChuc.LoaiToChuc = dto.LoaiToChuc;
            existingToChuc.MaSoToChuc = dto.MaSoToChuc;
            _syllDbContext.ToChucs.Update(existingToChuc);
            _syllDbContext.SaveChanges();
        }
        public void Delete(int idToChuc)
        {
            _logger.LogInformation($"{nameof(Delete)}");
            var vietnamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var isSuperAdmin = IsSuperAdmin();
            var existingToChuc = _syllDbContext.ToChucs.FirstOrDefault(x => x.Id == idToChuc && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                  ?? throw new UserFriendlyException(ErrorCodes.ToChucErrorNotFound);
            existingToChuc.Deleted = true;
            existingToChuc.DeletedDate = vietnamNow;
            _syllDbContext.ToChucs.Update(existingToChuc);
            _syllDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewToChucDto> Find(FindPagingToChucDto dto)
        {
            _logger.LogInformation($"{nameof(Find)}  dto={JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var isSuperAdmin = IsSuperAdmin();
            var query = from tc in _syllDbContext.ToChucs
                        where !tc.Deleted
                        && (isSuperAdmin || tc.CreatedBy == currentUserId)
                        orderby tc.CreatedDate descending
                        select tc;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewToChucDto>>(data);
            return new BaseResponsePagingDto<ViewToChucDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }


       
    }
}
