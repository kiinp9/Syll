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

        public async Task<BaseResponsePagingDto<ViewFormLoaiDto>> Find(FindPagingFormLoaiDto dto)
        {
            var currentUserId = await GetCurrentDanhBaId();


            _logger.LogInformation($"{nameof(Find)}  dto={JsonSerializer.Serialize(dto)}");

            var query = (from fl in _syllDbContext.FormLoais
                         join fld in _syllDbContext.FormDanhBa
                         on fl.Id equals fld.IdFormLoai
                         where !fl.Deleted
                         && !fld.Deleted
                         && fld.IdDanhBa == currentUserId
                         orderby fl.Id
                         select fl).Distinct();



            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewFormLoaiDto>>(data);


            var formLoaiIds = items.Select(x => x.Id).ToList();
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


        // UpdateFormData cho nhân viên 
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
                            record.Modified = true;
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




        // DeleteRowTableData cho nhân viên 
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




        //DeleteRowTableData cho admin
        public async Task DeleteRowTableDataAdmin(DeleteRowTableDataDto dto , int? idDanhBa)
        {
            _logger.LogInformation($"{nameof(DeleteRowTableDataAdmin)} dto={JsonSerializer.Serialize(dto)}");

            using var transaction = await _syllDbContext.Database.BeginTransactionAsync();
            try
            {


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
                    record.Modified = true;
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

        //UpdateFormDataAdmin cho admin
        public async Task UpdateFormDataAdmin(int idFormLoai, int? idDanhBa, UpdateFormDataRequestDto dto)
        {
            using var transaction = await _syllDbContext.Database.BeginTransactionAsync();
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

                var truongDataInfo = await _syllDbContext.FormTruongDatas
                    .Where(x => allIdTruongs.Contains(x.Id) && x.IdFormLoai == idFormLoai && !x.Deleted)
                    .Join(_syllDbContext.Items,
                        truong => truong.IdItem,
                        item => item.Id,
                        (truong, item) => new { truong.Id, truong.IdItem, item.Type })
                    .ToListAsync();

                if (truongDataInfo.Count != allIdTruongs.Count)
                    throw new UserFriendlyException(ErrorCodes.FormTruongDataErrorNotFound);

                var truongDataDict = truongDataInfo.ToDictionary(x => x.Id, x => x.Type);

                var vietNamNow = GetVietnamTime();
                var currentUserId = getCurrentUserId();

                var normalTruongs = dto.TruongDatas.Where(t =>
                    truongDataDict.ContainsKey(t.IdTruong) &&
                    truongDataDict[t.IdTruong] != ItemConstants.Table &&
                    t.Datas != null &&
                    t.Datas.Any()
                ).ToList();



                foreach (var truongData in normalTruongs)
                {
                    var dataToUpdate = truongData.Datas.Where(d => d.IdData > 0).ToList();
                    var dataToInsert = truongData.Datas.Where(d => d.IdData <= 0).ToList();

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
                                record.Modified = true;
                            }
                        }
                    }

                    if (dataToInsert.Count > 0)
                    {
                        var maxIndexRowOrder = await _syllDbContext.FormDatas
                            .Where(x => x.IdTruongData == truongData.IdTruong
                                && x.IdFormLoai == idFormLoai
                                && (!idDanhBa.HasValue || x.IdDanhBa == idDanhBa.Value)
                                && !x.Deleted)
                            .MaxAsync(x => (int?)x.IndexRowTable) ?? 0;

                        var newRecords = dataToInsert.Select(item => new FormData
                        {
                            IdFormLoai = idFormLoai,
                            IdDanhBa = idDanhBa ?? 0,
                            IdTruongData = truongData.IdTruong,
                            Data = item.Data,
                            CreatedDate = vietNamNow,
                            CreatedBy = currentUserId,
                            ModifiedDate = vietNamNow,
                            IndexRowTable = maxIndexRowOrder + 1,
                            Deleted = false
                        }).ToList();

                        await _syllDbContext.FormDatas.AddRangeAsync(newRecords);
                    }
                }

                var tableTruongs = dto.TruongDatas.Where(t =>
                    truongDataDict.ContainsKey(t.IdTruong) &&
                    truongDataDict[t.IdTruong] == ItemConstants.Table &&
                    t.TableRows != null &&
                    t.TableRows.Any()
                ).ToList();

   

                foreach (var truongData in tableTruongs)
                {
                    var idTruong = truongData.IdTruong;
                    var tableRows = truongData.TableRows;


                    var tableMapping = await _syllDbContext.Tables
                        .Where(x => x.IdItem == idTruong && !x.Deleted)
                        .OrderBy(x => x.Order)
                        .Select(x => x.IdTruongData)
                        .ToListAsync();


                    if (tableMapping.Count == 0)
                        throw new UserFriendlyException(ErrorCodes.FormLoaiErrorTableHeadersNotFound);

                    var maxIndexRowTable = await _syllDbContext.FormDatas
                        .Where(x => tableMapping.Contains(x.IdTruongData)
                            && x.IdFormLoai == idFormLoai
                            && (!idDanhBa.HasValue || x.IdDanhBa == idDanhBa.Value)
                            && !x.Deleted)
                        .MaxAsync(x => (int?)x.IndexRowTable) ?? 0;

                    int newRowCounter = 0;

                    for (int rowIdx = 0; rowIdx < tableRows.Count; rowIdx++)
                    {
                        var row = tableRows[rowIdx];
                        var cellsToUpdate = row.Where(cell => cell.IdData > 0).ToList();
                        var cellsToInsert = row.Where(cell => cell.IdData <= 0).ToList();


                        int currentIndexRowTable;

                        if (cellsToUpdate.Count > 0)
                        {
                            var firstIdData = cellsToUpdate.First().IdData;
                            var existingIndexRow = await _syllDbContext.FormDatas
                                .Where(x => x.Id == firstIdData
                                    && (!idDanhBa.HasValue || x.IdDanhBa == idDanhBa.Value)
                                    && x.IdFormLoai == idFormLoai
                                    && !x.Deleted)
                                .Select(x => x.IndexRowTable)
                                .FirstOrDefaultAsync();

                            if (!existingIndexRow.HasValue)
                                throw new UserFriendlyException(ErrorCodes.FormDataErrorNotFound);

                            currentIndexRowTable = existingIndexRow.Value;
                        }
                        else
                        {
                            newRowCounter++;
                            currentIndexRowTable = maxIndexRowTable + newRowCounter;
                        }

                        if (cellsToUpdate.Count > 0)
                        {
                            var idDatasToUpdate = cellsToUpdate.Select(c => c.IdData).ToList();

                            var existingCells = await _syllDbContext.FormDatas
                                .AsNoTracking()
                                .Where(x => idDatasToUpdate.Contains(x.Id)
                                    && (!idDanhBa.HasValue || x.IdDanhBa == idDanhBa.Value)
                                    && x.IdFormLoai == idFormLoai
                                    && !x.Deleted)
                                .Select(x => new { x.Id, x.Data })
                                .ToDictionaryAsync(x => x.Id, x => x.Data);

                            var updateCellDict = cellsToUpdate.ToDictionary(c => c.IdData, c => c.Data);

                            var changedCellIds = new List<int>();
                            foreach (var kvp in updateCellDict)
                            {
                                if (existingCells.TryGetValue(kvp.Key, out var oldData))
                                {
                                    if (oldData != kvp.Value)
                                    {
                                        changedCellIds.Add(kvp.Key);
                                    }
                                }
                            }

                            if (changedCellIds.Count > 0)
                            {
                                var recordsToUpdateCells = await _syllDbContext.FormDatas
                                    .Where(x => changedCellIds.Contains(x.Id))
                                    .ToListAsync();

                                foreach (var record in recordsToUpdateCells)
                                {
                                    record.Data = updateCellDict[record.Id];
                                    record.ModifiedDate = vietNamNow;
                                    record.ModifiedBy = currentUserId;
                                }
                            }
                        }

                        if (cellsToInsert.Count > 0)
                        {
                            if (cellsToInsert.Count != tableMapping.Count)
                            {
                                throw new UserFriendlyException(ErrorCodes.FormLoaiErrorTableCellCountMismatch);
                            }

                            var newCellRecords = new List<FormData>();
                            for (int i = 0; i < cellsToInsert.Count; i++)
                            {
                                var newCell = new FormData
                                {
                                    IdFormLoai = idFormLoai,
                                    IdDanhBa = idDanhBa ?? 0,
                                    IdTruongData = tableMapping[i],
                                    Data = cellsToInsert[i].Data,
                                    CreatedDate = vietNamNow,
                                    CreatedBy = currentUserId,
                                    ModifiedDate = vietNamNow,
                                    IndexRowTable = currentIndexRowTable,
                                    Deleted = false
                                };
                                newCellRecords.Add(newCell);
                            }

                            await _syllDbContext.FormDatas.AddRangeAsync(newCellRecords);
                        }
                    }
                }

                await _syllDbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Transaction committed successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error in {nameof(UpdateFormDataAdmin)}");
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


        public List<GetListDropDownFormLoaiDto> GetListDropDownFormLoai()
        {
            _logger.LogInformation($"{nameof(GetListDropDownFormLoai)} ");

            var query = from fl in _syllDbContext.FormLoais
                        where !fl.Deleted
                        orderby fl.Id
                        select new GetListDropDownFormLoaiDto
                        {
                            Id = fl.Id,
                            TenFormLoai = fl.TenForm,
                        };

            var data = query.ToList();
            return data;

        }

    }

}

