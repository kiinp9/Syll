using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog.LayoutRenderers.Wrappers;
using syll.be.application.Base;
using syll.be.application.Form.Dtos.Form;
using syll.be.application.Form.Interfaces;
using syll.be.domain.Form;
using syll.be.infrastructure.data;
using syll.be.shared.Constants.Form;
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
    public class FormService : BaseService, IFormService
    {
        public FormService(
            SyllDbContext syllDbContext,
            ILogger<FormService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ) : base(syllDbContext, logger, httpContextAccessor, mapper)
        {
        }

        public void Create(CreateFormLoaiDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}  dto={JsonSerializer.Serialize(dto)}");

            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var form = new domain.Form.FormLoai
            {
                TenForm = dto.TenLoaiForm,
                MoTa = dto.MoTa,
                ThoiGianBatDau = dto.ThoiGianBatDau,
                ThoiGianKetThuc = dto.ThoiGianKetThuc,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
                Deleted = false
            };
            _syllDbContext.FormLoais.Add(form);
            _syllDbContext.SaveChanges();
        }

        public void Update(UpdateFormLoaiDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}  dto={JsonSerializer.Serialize(dto)}");
            var form = _syllDbContext.FormLoais.FirstOrDefault(f => f.Id == dto.Id && !f.Deleted)
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

        public BaseResponsePagingDto<ViewFormLoaiDto> Find(FindPagingFormLoaiDto dto)
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

        public ViewFormLoaiDto GetFormLoaiById(int id)
        {
            _logger.LogInformation($"{nameof(GetFormLoaiById)}  id={id}");
            var form = _syllDbContext.FormLoais.FirstOrDefault(f => f.Id == id && !f.Deleted)
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

        public void Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}  id={id}");
            var form = _syllDbContext.FormLoais.FirstOrDefault(f => f.Id == id && !f.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            form.Deleted = true;
            form.DeletedBy = currentUserId;
            form.DeletedDate = vietNamNow;
            _syllDbContext.FormLoais.Update(form);
            _syllDbContext.SaveChanges();
        }

        public List<GetDropDownDataResponseDto?> GetDropDownData (int idTruongData)
        {
            _logger.LogInformation($"{nameof(GetDropDownData)}  idTruongData={idTruongData}");

            var truongData = _syllDbContext.FormTruongDatas.FirstOrDefault(x => x.Id == idTruongData && !x.Deleted);
            if (truongData == null)
            {
                return null;
            }

            var item = _syllDbContext.Items.FirstOrDefault(x => x.Id == truongData.IdItem && !x.Deleted);
            if (item == null)
            {
                return null;
            }
            if (item.Type == ItemConstants.DropDownText)
            {
                var dropDownData = (from dd in _syllDbContext.DropDowns
                                    where dd.IdTruongData == idTruongData
                                    && !dd.Deleted
                                    orderby dd.Order
                                    select new GetDropDownDataResponseDto
                                    {
                                        Id = dd.Id,
                                        Data = dd.Data,
                                        Order = dd.Order,
                                    }).ToList();
                return dropDownData;
            }
            else
            {
                return null;
            }
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
        }*/


        public async Task UpdateFormData(int idFormLoai, UpdateFormDataRequestDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateFormData)} - idFormLoai={idFormLoai}");
            using var transaction = await _syllDbContext.Database.BeginTransactionAsync();
            var idDanhBa = await GetCurrentDanhBaId();
            try
            {
                var formExists = await _syllDbContext.FormLoais
                    .AnyAsync(x => x.Id == idFormLoai && !x.Deleted);
                if (!formExists)
                    throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
                if (idDanhBa.HasValue)
                {
                    var danhBaExists = await _syllDbContext.DanhBas
                        .AnyAsync(x => x.Id == idDanhBa.Value && !x.Deleted);
                    if (!danhBaExists)
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);
                    var formDanhBaExists = await _syllDbContext.FormDanhBa
                        .AnyAsync(x => x.IdDanhBa == idDanhBa.Value && x.IdFormLoai == idFormLoai && !x.Deleted);
                    if (!formDanhBaExists)
                        throw new UserFriendlyException(ErrorCodes.FormDanhBaErrorNotFound);
                }
                var allIdTruongs = dto.TruongDatas.Select(t => t.IdTruong).Distinct().ToList();

                var truongDataCount = await _syllDbContext.FormTruongDatas
                    .Where(x => allIdTruongs.Contains(x.Id) && x.IdFormLoai == idFormLoai && !x.Deleted)
                    .CountAsync();
                if (truongDataCount != allIdTruongs.Count)
                    throw new UserFriendlyException(ErrorCodes.FormTruongDataErrorNotFound);

  
                var dataToUpdate = dto.TruongDatas
                    .SelectMany(t => t.Datas.Where(d => d.IdData > 0).Select(d => new { IdTruongData = t.IdTruong, d.IdData, d.Data }))
                    .ToList();

                var dataToInsert = dto.TruongDatas
                    .SelectMany(t => t.Datas.Where(d => d.IdData <= 0).Select(d => new { IdTruongData = t.IdTruong, d.Data }))
                    .ToList();

                var vietNamNow = GetVietnamTime();
                var currentUserId = getCurrentUserId();

    
                if (dataToUpdate.Count > 0)
                {
                    var allIdDatas = dataToUpdate.Select(d => d.IdData).Distinct().ToList();

                    var existingFormDatas = await _syllDbContext.FormDatas
                        .AsNoTracking()
                        .Where(x => allIdDatas.Contains(x.Id)
                            && (!idDanhBa.HasValue || x.IdDanhBa == idDanhBa.Value)
                            && x.IdFormLoai == idFormLoai
                            && !x.Deleted)
                        .Select(x => new { x.Id, x.Data })
                        .ToDictionaryAsync(x => x.Id, x => x.Data);

                    var updateDict = dataToUpdate.ToDictionary(d => d.IdData, d => d.Data);

                    var changedIds = new List<int>();
                    foreach (var kvp in updateDict)
                    {
                        if (existingFormDatas.TryGetValue(kvp.Key, out var oldData))
                        {
                            if (oldData != kvp.Value)
                            {
                                changedIds.Add(kvp.Key);
                            }
                        }
                    }

                    if (changedIds.Count > 0)
                    {
                        var recordsToUpdate = await _syllDbContext.FormDatas
                            .Where(x => changedIds.Contains(x.Id))
                            .ToListAsync();

                        foreach (var record in recordsToUpdate)
                        {
                            record.Data = updateDict[record.Id];
                            record.ModifiedDate = vietNamNow;
                            record.ModifiedBy = currentUserId;
                        }
                    }
                }

                if (dataToInsert.Count > 0)
                {

                    var allIdTruongsInTable = dataToInsert.Select(x => x.IdTruongData).Distinct().ToList();

                    var maxIndexRowOrder = await _syllDbContext.FormDatas
                        .Where(x => allIdTruongsInTable.Contains(x.IdTruongData)
                            && x.IdFormLoai == idFormLoai
                            && (!idDanhBa.HasValue || x.IdDanhBa == idDanhBa.Value)
                            && !x.Deleted)
                        .MaxAsync(x => (int?)x.IndexRowTable) ?? 0;
                    var newRecords = dataToInsert.Select(item => new FormData
                    {
                        IdFormLoai = idFormLoai,
                        IdDanhBa = idDanhBa ?? 0,
                        IdTruongData = item.IdTruongData,  
                        Data = item.Data,
                        CreatedDate = vietNamNow,
                        CreatedBy = currentUserId,
                        ModifiedDate = vietNamNow,
                        IndexRowTable = maxIndexRowOrder + 1,
                        //ModifiedBy = currentUserId,
                        Deleted = false
                    }).ToList();

                    await _syllDbContext.FormDatas.AddRangeAsync(newRecords);
                }

                await _syllDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error in {nameof(UpdateFormData)}");
                throw;
            }
        }

        public async Task DeleteRowTableData(DeleteRowTableDataDto dto)
        {
            _logger.LogInformation($"{nameof(DeleteRowTableData)} dto={JsonSerializer.Serialize(dto)}");

            using var transaction = await _syllDbContext.Database.BeginTransactionAsync();
            try
            {
                var idDanhBa = await GetCurrentDanhBaId();

                var allIdDatas = dto.Truongs
                    .SelectMany(t => t.Datas.Select(d => d.IdData))
                    .Distinct()
                    .ToList();

                if (allIdDatas.Count == 0)
                {
                    await transaction.CommitAsync();
                    return;
                }

                var formDatasToDelete = await _syllDbContext.FormDatas
                    .Where(x => allIdDatas.Contains(x.Id)
                        && (!idDanhBa.HasValue || x.IdDanhBa == idDanhBa.Value)
                        && !x.Deleted)
                    .ToListAsync();

                if (formDatasToDelete.Count == 0)
                {
                    await transaction.CommitAsync();
                    return;
                }

                var vietNamNow = GetVietnamTime();
                var currentUserId = getCurrentUserId();

                foreach (var record in formDatasToDelete)
                {
                    record.Deleted = true;
                    record.DeletedBy = currentUserId;
                    record.DeletedDate = vietNamNow;
                }

                await _syllDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error in {nameof(DeleteRowTableData)}");
                throw;
            }
        }
        public async Task UpdateFormDataForAdmin(int idFormLoai, int idDanhBa, UpdateFormDataRequestDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateFormDataForAdmin)}  dto={JsonSerializer.Serialize(dto)}");
            using var transaction = _syllDbContext.Database.BeginTransaction();
            try
            {
                var form = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
                var danhBa = _syllDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);
                var formDanhBa = _syllDbContext.FormDanhBa.FirstOrDefault(x => x.IdDanhBa == idDanhBa && x.IdFormLoai == idFormLoai && !x.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.FormDanhBaErrorNotFound);
                var allIdTruongs = dto.TruongDatas.Select(t => t.IdTruong).Distinct().ToList();


                var truongDataCount = await _syllDbContext.FormTruongDatas
                    .Where(x => allIdTruongs.Contains(x.Id) && x.IdFormLoai == idFormLoai && !x.Deleted)
                    .CountAsync();
                if (truongDataCount != allIdTruongs.Count)
                    throw new UserFriendlyException(ErrorCodes.FormTruongDataErrorNotFound);

                var allIdDatas = dto.TruongDatas
                    .SelectMany(t => t.Datas.Select(d => d.IdData))
                    .Distinct()
                    .ToList();

                var existingFormDatas = await _syllDbContext.FormDatas
                    .AsNoTracking()
                    .Where(x => allIdDatas.Contains(x.Id)
                        && ( x.IdDanhBa == idDanhBa)
                        && x.IdFormLoai == idFormLoai
                        && !x.Deleted)
                    .Select(x => new { x.Id, x.Data })
                    .ToDictionaryAsync(x => x.Id, x => x.Data);


                var updateDict = dto.TruongDatas
                    .SelectMany(t => t.Datas)
                    .ToDictionary(d => d.IdData, d => d.Data);

                var changedIds = new List<int>();
                foreach (var kvp in updateDict)
                {
                    if (existingFormDatas.TryGetValue(kvp.Key, out var oldData))
                    {
                        if (oldData != kvp.Value)
                        {
                            changedIds.Add(kvp.Key);
                        }
                    }

                }

                if (changedIds.Count == 0)
                {

                    await transaction.CommitAsync();
                    return;
                }


                var recordsToUpdate = await _syllDbContext.FormDatas
                    .Where(x => changedIds.Contains(x.Id))
                    .ToListAsync();

                var vietNamNow = GetVietnamTime();
                var currentUserId = getCurrentUserId();

                foreach (var record in recordsToUpdate)
                {
                    record.Data = updateDict[record.Id];
                    record.ModifiedDate = vietNamNow;
                    record.ModifiedBy = currentUserId;
                }

                await _syllDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, $"Error in {nameof(UpdateFormDataForAdmin)}");
                throw;
            }
        }


        public void CreateTruongData (CreateTruongDataDto dto)
        {
            _logger.LogInformation($"{nameof(CreateTruongData)} dto = {JsonSerializer.Serialize(dto)}");

            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();
            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == dto.IdFormLoai && !x.Deleted)
                 ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);

            var item = _syllDbContext.Items.FirstOrDefault(x => x.Id == dto.IdItem && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorItemNotFound);
            var truongData = new domain.Form.FormTruongData
            {
                IdFormLoai = dto.IdFormLoai,
                IdItem = dto.IdItem,
                TenTruong = dto.TenTruong,
                Type = "string",
                CreatedDate = vietNamNow,
                CreatedBy = currentUserId,
                Deleted = false

            };
            _syllDbContext.FormTruongDatas.Add( truongData );
            _syllDbContext.SaveChanges();

        }

    }

}

