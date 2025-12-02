using AutoMapper;
using EFCore.BulkExtensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using syll.be.application.Base;
using syll.be.application.DanhBa.Dtos;
using syll.be.application.DanhBa.Interfaces;
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
        public async Task Create (CreateDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(Create)} dto={JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var vietnamTime = GetVietnamTime();
        
            using var transaction = await _syllDbContext.Database.BeginTransactionAsync();

            try
            {
                var parts = dto.HoVaTen.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ten = parts.Length > 0 ? parts[parts.Length - 1] : string.Empty;
                var hoDem = parts.Length > 1 ? string.Join(" ", parts.Take(parts.Length - 1)) : string.Empty;

                var danhBa = new domain.DanhBa.DanhBa
                {
                    HoVaTen = dto.HoVaTen,
                    HoDem = hoDem,
                    Ten = ten,
                    Email = dto.Email,
                    CreatedBy = currentUserId,
                    CreatedDate = vietnamTime,
                    Deleted = false
                };

                _syllDbContext.DanhBas.Add(danhBa);
                await _syllDbContext.SaveChangesAsync();

                var danhBaId = danhBa.Id;

                var toChuc = await _syllDbContext.ToChucs
                    .FirstOrDefaultAsync(tc => tc.Id == dto.IdToChuc && !tc.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.ToChucErrorNotFound);

                var toChucDanhBa = new domain.ToChuc.ToChucDanhBa
                {
                    IdToChuc = toChuc.Id,
                    IdDanhBa = danhBaId,
                    CreatedBy = currentUserId,
                    CreatedDate = vietnamTime,
                    Deleted = false
                };

                _syllDbContext.ToChucDanhBa.Add(toChucDanhBa);
                await _syllDbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task Update (UpdateDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(Update)} dto={JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var vietnamTime = GetVietnamTime();
            using var transaction = await _syllDbContext.Database.BeginTransactionAsync();

            try
            {
                var parts = dto.HoVaTen.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ten = parts.Length > 0 ? parts[parts.Length - 1] : string.Empty;
                var hoDem = parts.Length > 1 ? string.Join(" ", parts.Take(parts.Length - 1)) : string.Empty;

                var danhBa = await _syllDbContext.DanhBas
                .FirstOrDefaultAsync(db => db.Id == dto.Id && !db.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);
                var toChuc = await _syllDbContext.ToChucs.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.ToChucErrorNotFound);
                danhBa.HoVaTen = dto.HoVaTen;
                danhBa.HoDem = hoDem;
                danhBa.Ten = ten;
                danhBa.Email = dto.Email;
                _syllDbContext.DanhBas.Update(danhBa);

                var toChucDanhBa = await _syllDbContext.ToChucDanhBa.FirstOrDefaultAsync(x => x.IdToChuc == dto.CurrentIdToChuc && x.IdDanhBa == dto.Id && !x.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFoundInToChuc);
                toChucDanhBa.IdToChuc = dto.IdToChuc;
                _syllDbContext.ToChucDanhBa.Update(toChucDanhBa);
                await _syllDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch 
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public BaseResponsePagingDto<ViewDanhBaDto> FindDanhBa(FindPagingDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(FindDanhBa)} dto={JsonSerializer.Serialize(dto)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var query = from db in _syllDbContext.DanhBas
                        where !db.Deleted
                              && (string.IsNullOrEmpty(dto.Keyword)
                                  || db.HoVaTen.Contains(dto.Keyword)
                                  || db.HoDem.Contains(dto.Keyword)
                                  || db.Ten.Contains(dto.Keyword)
                                  || db.Email.Contains(dto.Keyword))
                        orderby db.Id ascending
                        select new ViewDanhBaDto
                        {
                            Id = db.Id,
                            HoVaTen = db.HoVaTen,
                            HoDem = db.HoDem,
                            Ten = db.Ten,
                            Email = db.Email,
                            Items = (from tcdb in _syllDbContext.ToChucDanhBa
                                     join tc in _syllDbContext.ToChucs on tcdb.IdToChuc equals tc.Id
                                     where !tcdb.Deleted && !tc.Deleted && tcdb.IdDanhBa == db.Id
                                     select new ViewDanhBaWithToChucDto
                                     {
                                         Id = tc.Id,
                                         TenToChuc = tc.TenToChuc,
                                         LoaiToChuc = tc.LoaiToChuc,
                                         MaSoToChuc = tc.MaSoToChuc ?? string.Empty,
                                     }).ToList()
                        };
            var data = query.Paging(dto).ToList();

            var response = new BaseResponsePagingDto<ViewDanhBaDto>
            {
                Items = data,
                TotalItems = query.Count()
            };
            return response;
        }
        public async Task Delete(int idToChuc, int idDanhBa)
        {
            _logger.LogInformation($"{nameof(Delete)} idToChuc={idToChuc}, idDanhBa={idDanhBa}");
            using var transaction = await _syllDbContext.Database.BeginTransactionAsync();

            try
            {
                var vietnamTime = GetVietnamTime();
                var currentUserId = getCurrentUserId();
                var toChucDanhBa = await _syllDbContext.ToChucDanhBa
                    .FirstOrDefaultAsync(tcdb => tcdb.IdToChuc == idToChuc && tcdb.IdDanhBa == idDanhBa && !tcdb.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFoundInToChuc);
                var danhBa = await _syllDbContext.DanhBas
                    .FirstOrDefaultAsync(db => db.Id == idDanhBa && !db.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);
                var toChuc = await _syllDbContext.ToChucs
                    .FirstOrDefaultAsync(tc => tc.Id == idToChuc && !tc.Deleted)
                    ?? throw new UserFriendlyException(ErrorCodes.ToChucErrorNotFound);
                toChucDanhBa.Deleted = true;
                toChucDanhBa.DeletedBy = currentUserId;
                toChucDanhBa.DeletedDate = vietnamTime;
                _syllDbContext.ToChucDanhBa.Update(toChucDanhBa);
                await _syllDbContext.SaveChangesAsync();
                danhBa.Deleted = true;
                danhBa.DeletedBy = currentUserId;
                danhBa.DeletedDate = vietnamTime;
                _syllDbContext.DanhBas.Update(danhBa);
                await _syllDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public BaseResponsePagingDto<ViewDanhBaAccordingToChucDto> FindPagingDanhBaAccordingToChuc (FindPagingDanhBaAccordingToChucDto dto)
        {
            _logger.LogInformation($"{nameof(FindPagingDanhBaAccordingToChuc)} dto={JsonSerializer.Serialize(dto)}");
            var query = from tcdb in _syllDbContext.ToChucDanhBa
                        join db in _syllDbContext.DanhBas on tcdb.IdDanhBa equals db.Id
                        where !tcdb.Deleted && !db.Deleted && tcdb.IdToChuc == dto.IdToChuc
                              && (string.IsNullOrEmpty(dto.Keyword)
                                  || db.HoVaTen.Contains(dto.Keyword)
                                  || db.HoDem.Contains(dto.Keyword)
                                  || db.Ten.Contains(dto.Keyword)
                                  || db.Email.Contains(dto.Keyword))
                        orderby db.Id ascending
                        select new ViewDanhBaAccordingToChucDto
                        {
                            Id = db.Id,
                            HoVaTen = db.HoVaTen,
                            HoDem = db.HoDem,
                            Ten = db.Ten,
                            Email = db.Email
                        };
            var data = query.Paging(dto).ToList();

            var response = new BaseResponsePagingDto<ViewDanhBaAccordingToChucDto>
            {
                Items = data,
                TotalItems = query.Count()
            };
            return response;
        }

        public async Task<ImportDanhBaResponseDto> ImportDanhBa(ImportDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(ImportDanhBa)} dto={JsonSerializer.Serialize(dto)}");

            var startTime = DateTime.UtcNow;
            var currentUserId = getCurrentUserId();
            var vietnamTime = GetVietnamTime();

            if (string.IsNullOrEmpty(dto.Url))
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid);
            }

            if (string.IsNullOrEmpty(dto.SheetName))
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid);
            }

            var hasPermission = await _checkGoogleSheetPermission(dto.Url);
            if (!hasPermission)
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid);
            }

            var sheetData = await _getSheetData(dto.Url, dto.SheetName);

            if (sheetData == null || sheetData.Count == 0)
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid);
            }

            if (dto.IndexRowHeader > sheetData.Count || dto.IndexRowStartImport > sheetData.Count)
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid);
            }

            var totalRowsImported = 0;
            var totalDataImported = 0;

            var emailsToImport = new List<string>();
            var importDataMap = new Dictionary<string, (string hoTen, string hoDem, string ten, string maSoToChuc)>();

            for (int i = dto.IndexRowStartImport - 1; i < sheetData.Count; i++)
            {
                var row = sheetData[i];

                if (row == null || row.Count == 0)
                {
                    continue;
                }

                var hoTen = dto.IndexColumnHoTen - 1 < row.Count && dto.IndexColumnHoTen > 0 ? row[dto.IndexColumnHoTen - 1]?.ToString() ?? string.Empty : string.Empty;
                var email = dto.IndexColumnEmail - 1 < row.Count && dto.IndexColumnEmail > 0 ? row[dto.IndexColumnEmail - 1]?.ToString() ?? string.Empty : string.Empty;
                var maSoToChuc = dto.IndexColumnMaSoToChuc - 1 < row.Count && dto.IndexColumnMaSoToChuc > 0 ? row[dto.IndexColumnMaSoToChuc - 1]?.ToString() ?? string.Empty : string.Empty;

                if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(email))
                {
                    continue;
                }

                totalRowsImported++;

                var hoTenOriginal = dto.IndexColumnHoTen - 1 < row.Count && dto.IndexColumnHoTen > 0
                    ? row[dto.IndexColumnHoTen - 1]?.ToString() ?? string.Empty
                    : string.Empty;

                var hoTenTrimmed = hoTenOriginal.Trim();
                var parts = hoTenTrimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ten = parts.Length > 0 ? parts[parts.Length - 1] : string.Empty;
                var hoDem = parts.Length > 1 ? string.Join(" ", parts.Take(parts.Length - 1)) : string.Empty;

                
                emailsToImport.Add(email);
                importDataMap[email] = (hoTenOriginal, hoDem, ten, maSoToChuc);
            }

            if (emailsToImport.Count == 0)
            {
                return new ImportDanhBaResponseDto
                {
                    TotalRowsImported = 0,
                    TotalDataImported = 0,
                    ImportTimeInSeconds = 0
                };
            }

            var existingDanhBas = await _syllDbContext.DanhBas
                .Where(db => emailsToImport.Contains(db.Email) && !db.Deleted)
                .ToListAsync();

            var existingEmails = existingDanhBas.Select(db => db.Email).ToHashSet();

            var danhBasToUpdate = new List<domain.DanhBa.DanhBa>();
            var danhBasToInsert = new List<domain.DanhBa.DanhBa>();

            foreach (var email in emailsToImport)
            {
                var data = importDataMap[email];

                if (existingEmails.Contains(email))
                {
                    var existingDanhBa = existingDanhBas.First(db => db.Email == email);
                    existingDanhBa.HoVaTen = data.hoTen;
                    existingDanhBa.HoDem = data.hoDem;
                    existingDanhBa.Ten = data.ten;
                    danhBasToUpdate.Add(existingDanhBa);
                }
                else
                {
                    var newDanhBa = new domain.DanhBa.DanhBa
                    {
                        HoVaTen = data.hoTen,
                        HoDem = data.hoDem,
                        Ten = data.ten,
                        Email = email,
                        CreatedBy = currentUserId,
                        CreatedDate = vietnamTime,
                        Deleted = false
                    };
                    danhBasToInsert.Add(newDanhBa);
                }
            }

            if (danhBasToUpdate.Count > 0)
            {
                await _syllDbContext.BulkUpdateAsync(danhBasToUpdate);
            }

            if (danhBasToInsert.Count > 0)
            {
                await _syllDbContext.BulkInsertAsync(danhBasToInsert);
                totalDataImported = danhBasToInsert.Count;
            }

            var allDanhBas = await _syllDbContext.DanhBas
                .Where(db => emailsToImport.Contains(db.Email) && !db.Deleted)
                .ToListAsync();

            var emailToIdDanhBaMap = allDanhBas.ToDictionary(db => db.Email, db => db.Id);

            var maSoToChucs = importDataMap.Values
                .Where(v => !string.IsNullOrEmpty(v.maSoToChuc))
                .Select(v => v.maSoToChuc)
                .Distinct()
                .ToList();

            var toChucs = await _syllDbContext.ToChucs
                .Where(tc => maSoToChucs.Contains(tc.MaSoToChuc) && !tc.Deleted)
                .ToListAsync();

            var maSoToIdToChucMap = toChucs.ToDictionary(tc => tc.MaSoToChuc, tc => tc.Id);

            var existingRelations = await _syllDbContext.ToChucDanhBa
                .Where(tcdb => !tcdb.Deleted)
                .ToListAsync();

            var existingRelationKeys = existingRelations
                .Select(tcdb => $"{tcdb.IdToChuc}_{tcdb.IdDanhBa}")
                .ToHashSet();

            var toChucDanhBasToInsert = new List<domain.ToChuc.ToChucDanhBa>();

            foreach (var email in emailsToImport)
            {
                var data = importDataMap[email];

                if (string.IsNullOrEmpty(data.maSoToChuc))
                {
                    continue;
                }

                if (!maSoToIdToChucMap.ContainsKey(data.maSoToChuc))
                {
                    continue;
                }

                if (!emailToIdDanhBaMap.ContainsKey(email))
                {
                    continue;
                }

                var idToChuc = maSoToIdToChucMap[data.maSoToChuc];
                var idDanhBa = emailToIdDanhBaMap[email];
                var relationKey = $"{idToChuc}_{idDanhBa}";

                if (!existingRelationKeys.Contains(relationKey))
                {
                    var newRelation = new domain.ToChuc.ToChucDanhBa
                    {
                        IdToChuc = idToChuc,
                        IdDanhBa = idDanhBa,
                        CreatedBy = currentUserId,
                        CreatedDate = vietnamTime,
                        Deleted = false
                    };
                    toChucDanhBasToInsert.Add(newRelation);
                }
            }

            

            if (toChucDanhBasToInsert.Count > 0)
            {
                await _syllDbContext.BulkInsertAsync(toChucDanhBasToInsert);
            }
            var existingFormDanhBas = await _syllDbContext.FormDanhBa
        .Where(fdb => fdb.IdFormLoai == 1 && !fdb.Deleted)
        .ToListAsync();

            var existingFormDanhBaKeys = existingFormDanhBas
                .Select(fdb => fdb.IdDanhBa)
                .ToHashSet();

            var formDanhBasToInsert = new List<domain.FormDanhBa.FormDanhBa>();

            foreach (var idDanhBa in emailToIdDanhBaMap.Values)
            {
                if (!existingFormDanhBaKeys.Contains(idDanhBa))
                {
                    var newFormDanhBa = new domain.FormDanhBa.FormDanhBa
                    {
                        IdFormLoai = 1,
                        IdDanhBa = idDanhBa,
                        CreatedBy = currentUserId,
                        CreatedDate = vietnamTime,
                        Deleted = false
                    };
                    formDanhBasToInsert.Add(newFormDanhBa);
                }
            }

            if (formDanhBasToInsert.Count > 0)
            {
                await _syllDbContext.BulkInsertAsync(formDanhBasToInsert);
            }

            var endTime = DateTime.UtcNow;
            var importTimeInSeconds = (int)(endTime - startTime).TotalSeconds;

            return new ImportDanhBaResponseDto
            {
                TotalRowsImported = totalRowsImported,
                TotalDataImported = totalDataImported,
                ImportTimeInSeconds = importTimeInSeconds
            };
        }
        private async Task<List<List<string>>> _getSheetData(string sheetUrl, string sheetName)
        {
            var serviceAccountPath = _configuration["Google:ServiceAccountPath"];

            if (string.IsNullOrEmpty(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound);
            }

            if (!Path.IsPathRooted(serviceAccountPath))
            {
                var basePath = AppContext.BaseDirectory;
                serviceAccountPath = Path.Combine(basePath, serviceAccountPath);
            }

            if (!File.Exists(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound);
            }

            var credential = GoogleCredential.FromFile(serviceAccountPath)
                .CreateScoped("https://www.googleapis.com/auth/spreadsheets.readonly");

            var service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API"
            });

            var spreadsheetId = _extractSpreadsheetId(sheetUrl);
            var range = sheetName;

            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            var values = response.Values;

            var responseData = new List<List<string>>();

            if (values != null && values.Any())
            {
                foreach (var row in values)
                {
                    var stringRow = row.Select(c => c?.ToString() ?? string.Empty).ToList();
                    responseData.Add(stringRow);
                }
            }

            return responseData;
        }
        private string _extractSpreadsheetId(string sheetUrl)
        {
            var match = System.Text.RegularExpressions.Regex.Match(sheetUrl, @"/spreadsheets/d/([a-zA-Z0-9-_]+)");
            if (!match.Success || string.IsNullOrEmpty(match.Groups[1].Value))
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid);
            }
            return match.Groups[1].Value;
        }
        private async Task<bool> _checkGoogleSheetPermission(string sheetUrl)
        {
            var serviceAccountPath = _configuration["Google:ServiceAccountPath"];

            if (string.IsNullOrEmpty(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound);
            }

            if (!Path.IsPathRooted(serviceAccountPath))
            {
                var basePath = AppContext.BaseDirectory;
                serviceAccountPath = Path.Combine(basePath, serviceAccountPath);
            }

            if (!File.Exists(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound);
            }

            var credential = GoogleCredential.FromFile(serviceAccountPath)
                .CreateScoped("https://www.googleapis.com/auth/spreadsheets");

            var service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API"
            });

            var spreadsheetId = _extractSpreadsheetId(sheetUrl);

            try
            {
                var request = service.Spreadsheets.Get(spreadsheetId);
                request.IncludeGridData = false;

                var response = await request.ExecuteAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
