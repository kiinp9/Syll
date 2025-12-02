using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using syll.be.application.Base;
using syll.be.application.ChienDich.Dtos;
using syll.be.application.ChienDich.Interfaces;
using syll.be.application.Form.Dtos.Form;
using syll.be.application.Form.Implements;
using syll.be.domain.ChienDich;
using syll.be.infrastructure.data;
using syll.be.shared.HttpRequest.AppException;
using syll.be.shared.HttpRequest.BaseRequest;
using syll.be.shared.HttpRequest.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace syll.be.application.ChienDich.Implements
{
    public class ChienDichService: BaseService, IChienDichService
    {
        public ChienDichService(
            SyllDbContext syllDbContext,
            ILogger<ChienDichService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IConfiguration configuration
        ) : base(syllDbContext, logger, httpContextAccessor, mapper)
        {

        }


        public void CreateChienDich( CreateChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(CreateChienDich)}  dto = {JsonSerializer.Serialize(dto)}" );

            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var isSuperAdmin = IsSuperAdmin();


            var chienDich = new domain.ChienDich.ChienDich
            {
                TenChienDich = dto.TenChienDich,
                MoTa = dto.MoTa,
                ThoiGianBatDau = dto.ThoiGianBatDau,
                ThoiGianKetThuc = dto.ThoiGianKetThuc,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
            };

            _syllDbContext.ChienDiches.Add( chienDich );
            _syllDbContext.SaveChanges();

            var chienDichId = chienDich.Id;

            if( dto.FormLoais != null && dto.FormLoais.Any())
            {
                var chienDichFormLoaiList = dto.FormLoais.Select(x => new ChienDichFormLoai
                {
                    IdChienDich = chienDichId,
                    IdFormLoai = x.Id,
                    CreatedBy = currentUserId,
                    CreatedDate = vietNamNow,

                }).ToList();
                _syllDbContext.ChienDichFormLoais.AddRange(chienDichFormLoaiList);
                _syllDbContext.SaveChanges();
            }
        }


        public void UpdateChienDich (UpdateChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateChienDich)} dto = {JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var isSuperAdmin = IsSuperAdmin();
            var vietNamNow = GetVietnamTime();

            var chienDich = _syllDbContext.ChienDiches.FirstOrDefault(x => x.Id == dto.IdChienDich && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);

            chienDich.TenChienDich = dto.TenChienDich;
            chienDich.MoTa = dto.MoTa;
            chienDich.ThoiGianBatDau = dto.ThoiGíanBatDau;
            chienDich.ThoiGianKetThuc = dto.ThoiGianKetThuc;


            _syllDbContext.ChienDiches.Update( chienDich );
            _syllDbContext.SaveChanges();


            if (dto.FormLoais != null && dto.FormLoais.Any())
            {
                var existingChienDichFormLoaiList = _syllDbContext.ChienDichFormLoais
                                                    .Where(x => x.IdChienDich == dto.IdChienDich && !x.Deleted)
                                                    .ToList();
                

                var dtoFormLoaiIds =dto.FormLoais.Select( x => x.IdFormLoai).ToList();
                var existingFormLoaiIds = existingChienDichFormLoaiList.Select(x => x.IdFormLoai ).ToList();

                var deleteChienDichFormLoaiList = existingChienDichFormLoaiList.Where( x => !dtoFormLoaiIds.Contains(x.IdFormLoai)).ToList();
                foreach(var deleteChienDichFormLoai in deleteChienDichFormLoaiList)
                {
                    deleteChienDichFormLoai.DeletedDate = vietNamNow;
                    deleteChienDichFormLoai.Deleted = true;
                    deleteChienDichFormLoai.DeletedBy = currentUserId;
                }

                var newChienDichFormLoaiIds = dtoFormLoaiIds.Where(x => !existingFormLoaiIds.Contains(x)).ToList();
                var newChienDichFormLoai = newChienDichFormLoaiIds.Select(x => new ChienDichFormLoai
                {
                    IdChienDich = dto.IdChienDich,
                    IdFormLoai = x,
                    CreatedBy = currentUserId,
                    CreatedDate = vietNamNow,
                }).ToList();

                _syllDbContext.ChienDichFormLoais.AddRange(newChienDichFormLoai);
                _syllDbContext.SaveChanges();

            }
        }


        public BaseResponsePagingDto<ViewChienDichDto> FindPagingChienDich (FindPagingChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(FindPagingChienDich)} dto = {JsonSerializer.Serialize(dto)}");
            var query = from cd in _syllDbContext.ChienDiches
                        where !cd.Deleted 
                              &&(string.IsNullOrEmpty(dto.Keyword)
                              || cd.TenChienDich.Contains(dto.Keyword))
                              orderby cd.Id 
                              select cd;


            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewChienDichDto>>(data);
            var response = new BaseResponsePagingDto<ViewChienDichDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
            return response;
        }

        public async Task<BaseResponsePagingDto<ViewFormLoaiByIdChienDichDto>> FindPagingFormLoaiByIdChienDich(FindPagingFormLoaiByIdChienDichDto dto)
        {
            var currentUserId = await GetCurrentDanhBaId();
            _logger.LogInformation($"{nameof(FindPagingFormLoaiByIdChienDich)} dto = {JsonSerializer.Serialize(dto)}");
            var query = from fl in _syllDbContext.FormLoais
                        join cd in _syllDbContext.ChienDichFormLoais

                        on fl.Id equals cd.IdFormLoai
                        join fld in _syllDbContext.FormDanhBa
                        on fl.Id equals fld.IdFormLoai 
                        where !fl.Deleted 
                        && !cd.Deleted
                        && cd.IdChienDich == dto.IdChienDich
                        && fld.IdDanhBa == currentUserId
                        orderby fl.Order
                        select cd;

            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewFormLoaiByIdChienDichDto>>(data);

            var formLoaiIds = items.Select( x => x.Id).ToList();
            var truongCounts = _syllDbContext.FormTruongDatas
                .Where(x => formLoaiIds.Contains(x.IdFormLoai) && x.TruongCanNhap == true && !x.Deleted)
                .GroupBy(x => x.IdFormLoai)
                .Select(g => new { IdFormLoai = g.Key, Count = g.Count() })
                .ToDictionary(x => x.IdFormLoai, x => x.Count);


            var lastModifiedDates = _syllDbContext.FormDatas
                .Where(x => formLoaiIds.Contains(x.IdFormLoai) && !x.Deleted && x.IdDanhBa == currentUserId)
                .GroupBy(x => x.IdFormLoai)
                .Select(g => new
                {
                    IdFormLoai = g.Key,
                    LastModified = g.Max(x => x.ModifiedDate)
                })
                .ToDictionary(x => x.IdFormLoai, x => x.LastModified);

            foreach (var item in items)
            {
                item.TongSoTruong = truongCounts.GetValueOrDefault(item.Id, 0);
                item.ThoiGianCapNhatGanNhat = lastModifiedDates.GetValueOrDefault(item.Id);
            }

            return new BaseResponsePagingDto<ViewFormLoaiByIdChienDichDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }

        public void DeleteChienDich(int id)
        {
            _logger.LogInformation($"{nameof(DeleteChienDich)}");
            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();
            var chienDich = _syllDbContext.ChienDiches.FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);

            var chienDichFormLoaiList = _syllDbContext.ChienDichFormLoais
                                        .Where(x => x.Id == id && !x.Deleted)
                                        .ToList();

            foreach (var chienDichFormLoai in chienDichFormLoaiList)
            {
                chienDichFormLoai.Deleted = true;
                chienDichFormLoai.DeletedDate = vietNamNow;
                chienDichFormLoai.DeletedBy = currentUserId;
            }

            chienDich.DeletedBy = currentUserId;
            chienDich.Deleted = true;
            chienDich.DeletedDate = vietNamNow;

            _syllDbContext.SaveChanges();
        }


        


    }
}
