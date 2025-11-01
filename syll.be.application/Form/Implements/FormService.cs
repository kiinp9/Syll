using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NLog.LayoutRenderers.Wrappers;
using syll.be.application.Base;
using syll.be.application.Form.Dtos;
using syll.be.application.Form.Interfaces;
using syll.be.infrastructure.data;
using syll.be.shared.HttpRequest.AppException;
using syll.be.shared.HttpRequest.BaseRequest;
using syll.be.shared.HttpRequest.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace syll.be.application.Form.Implements
{
    public class FormService :BaseService, IFormService 
    {
        public FormService(
            SyllDbContext syllDbContext,
            ILogger<FormService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ) : base(syllDbContext, logger, httpContextAccessor, mapper)
        {
        }

        public void Create (CreateFormLoaiDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}  dto={JsonSerializer.Serialize(dto)}");

            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var form = new domain.Form.FormLoai
            {
                TenForm = dto.TenLoaiForm,
                MoTa = dto.MoTa,
                ThoiGianBatDau= dto.ThoiGianBatDau,
                ThoiGianKetThuc= dto.ThoiGianKetThuc,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
                Deleted = false
            };
            _syllDbContext.FormLoais.Add(form);
            _syllDbContext.SaveChanges();
        }

        public void Update (UpdateFormLoaiDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}  dto={JsonSerializer.Serialize(dto)}");
            var form = _syllDbContext.FormLoais.FirstOrDefault(f => f.Id == dto.Id && !f.Deleted )
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            form.TenForm = dto.Ten;
            form.MoTa = dto.MoTa;
            form.ThoiGianBatDau = dto.ThoiGianBatDau;
            form.ThoiGianKetThuc = dto.ThoiGianKetThuc;
            _syllDbContext.FormLoais.Update(form);
            _syllDbContext.SaveChanges();

        }

        public BaseResponsePagingDto<ViewFormLoaiDto> Find (FindPagingFormLoaiDto dto)
        {
            _logger.LogInformation($"{nameof(Find)}  dto={JsonSerializer.Serialize(dto)}");
            var query = from fl in _syllDbContext.FormLoais
                        where fl.Deleted == false
                        orderby fl.CreatedDate descending
                        select fl;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewFormLoaiDto>>(data);
            return new BaseResponsePagingDto<ViewFormLoaiDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }

        public ViewFormLoaiDto GetFormLoaiById (int id)
        {
            _logger.LogInformation($"{nameof(GetFormLoaiById)}  id={id}");
            var form = _syllDbContext.FormLoais.FirstOrDefault(f => f.Id == id && !f.Deleted )
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            return new ViewFormLoaiDto
            {
                Id = form.Id,
                TenForm = form.TenForm,
                MoTa = form.MoTa,
                ThoiGianBatDau = form.ThoiGianBatDau,
                ThoiGianKetThuc = form.ThoiGianKetThuc
            };
        }

        public void Delete (int id)
        {
            _logger.LogInformation($"{nameof(Delete)}  id={id}");
            var form = _syllDbContext.FormLoais.FirstOrDefault(f => f.Id == id && !f.Deleted )
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            form.Deleted = true;
            form.DeletedBy = currentUserId;
            form.DeletedDate = vietNamNow;
            _syllDbContext.FormLoais.Update(form);
            _syllDbContext.SaveChanges();
        }

      

        /*public GetFormInforByIdDanhBaDto GetFormInforByIdDanhBa( int idFormLoai,int idDanhBa)
        {
            _logger.LogInformation($"{nameof(GetFormInforByIdDanhBa)}  idDanhBa={idDanhBa}, idFormLoai={idFormLoai}");

            var form = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            var DanhBa = _syllDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);
            var formDanhBa = _syllDbContext.FormDanhBa.FirstOrDefault(x => x.IdDanhBa == idDanhBa && x.IdFormLoai == idFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormDanhBaErrorNotFound);


            var truongDatas = _syllDbContext.FormTruongDatas
                .Where(x => x.IdFormLoai == idFormLoai && !x.Deleted)
                .ToList();

            var formDatas = _syllDbContext.FormDatas
                .Where(x => x.IdFormLoai == idFormLoai && x.IdDanhBa == idDanhBa && !x.Deleted)
                .ToList();

            var result = new GetFormInforByIdDanhBaDto
            {
                Id = form.Id,
                TenFormLoai = form.TenForm,
                MoTa = form.MoTa,
                ThoiGianBatDau = form.ThoiGianBatDau,
                ThoiGianKetThuc = form.ThoiGianKetThuc,
                Items = dauMucs.Select(dm => new GetFormDauMucDto
                {
                    Id = dm.Id,
                    TenDauMuc = dm.TenDauMuc,
                    Items = truongDatas
                        .Where(td => td.IdDauMuc == dm.Id)
                        .Select(td => new GetFormTruongDataDto
                        {
                            Id = td.Id,
                            TenTruong = td.TenTruong,
                            Item = new GetFormDataDto
                            {
                                Id = formDatas.FirstOrDefault(fd => fd.IdTruongData == td.Id)?.Id ?? 0,
                                Data = formDatas.FirstOrDefault(fd => fd.IdTruongData == td.Id)?.Data ?? string.Empty
                            }
                        }).ToList()
                }).ToList()
            };

            return result;
        }


        public void UpdateFormData(int idFormLoai, int idDanhBa, UpdateFormDataRequestDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateFormData)}  dto={JsonSerializer.Serialize(dto)}");

            using var transaction = _syllDbContext.Database.BeginTransaction();
            try
            {
                var form = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);

                var danhBa = _syllDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);

                var formDanhBa = _syllDbContext.FormDanhBa.FirstOrDefault(x => x.IdDanhBa == idDanhBa && x.IdFormLoai == idFormLoai && !x.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.FormDanhBaErrorNotFound);

                var allIdDauMucs = dto.DauMucs.Select(x => x.IdDauMuc).Distinct().ToList();
                var allIdTruongs = dto.DauMucs.SelectMany(x => x.TruongDatas.Select(t => t.IdTruong)).Distinct().ToList();

                var dauMucs = _syllDbContext.FormDauMucs
                    .Where(x => allIdDauMucs.Contains(x.Id) && x.IdFormLoai == idFormLoai && !x.Deleted)
                    .ToDictionary(x => x.Id);

                if (dauMucs.Count != allIdDauMucs.Count)
                    throw new UserFriendlyException(ErrorCodes.FormDauMucErrorNotFound);

                var truongDatas = _syllDbContext.FormTruongDatas
                    .Where(x => allIdTruongs.Contains(x.Id) && x.IdFormLoai == idFormLoai && !x.Deleted)
                    .ToDictionary(x => x.Id);

                if (truongDatas.Count != allIdTruongs.Count)
                    throw new UserFriendlyException(ErrorCodes.FormTruongDataErrorNotFound);

                foreach (var dauMucDto in dto.DauMucs)
                {
                    if (!dauMucs.ContainsKey(dauMucDto.IdDauMuc))
                        throw new UserFriendlyException(ErrorCodes.FormDauMucErrorNotFound);

                    foreach (var truongDataDto in dauMucDto.TruongDatas)
                    {
                        if (!truongDatas.TryGetValue(truongDataDto.IdTruong, out var truongData))
                            throw new UserFriendlyException(ErrorCodes.FormTruongDataErrorNotFound);

                        if (truongData.IdDauMuc != dauMucDto.IdDauMuc)
                            throw new UserFriendlyException(ErrorCodes.FormTruongDataErrorNotFound);
                    }
                }

                var existingFormDatas = _syllDbContext.FormDatas
                    .Where(x => allIdTruongs.Contains(x.IdTruongData) && x.IdDanhBa == idDanhBa && x.IdFormLoai == idFormLoai && !x.Deleted)
                    .ToDictionary(x => x.IdTruongData);

                var vietNamNow = GetVietnamTime();
                var currentUserId = getCurrentUserId();

                foreach (var dauMucDto in dto.DauMucs)
                {
                    foreach (var truongDataDto in dauMucDto.TruongDatas)
                    {
                        if (existingFormDatas.TryGetValue(truongDataDto.IdTruong, out var existingFormData))
                        {
                            existingFormData.Data = truongDataDto.Data;
                        }
                        else
                        {
                            var newFormData = new domain.Form.FormData
                            {
                                IdFormLoai = idFormLoai,
                                IdTruongData = truongDataDto.IdTruong,
                                IdDanhBa = idDanhBa,
                                Data = truongDataDto.Data,
                                CreatedBy = currentUserId,
                                CreatedDate = vietNamNow,
                                Deleted = false
                            };
                            _syllDbContext.FormDatas.Add(newFormData);
                        }
                    }
                }

                _syllDbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, $"Error in {nameof(UpdateFormData)}");
                throw;
            }*/
        }

    }

