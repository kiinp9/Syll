using AutoMapper;
using EFCore.BulkExtensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using syll.be.application.Base;
using syll.be.application.Form.Dtos.FormData;
using syll.be.application.Form.Interfaces;
using syll.be.domain.Form;
using syll.be.infrastructure.data;
using syll.be.shared.HttpRequest.AppException;
using syll.be.shared.HttpRequest.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace syll.be.application.Form.Implements
{
    public class FormDataService:BaseService,IFormDataService
    {
        private readonly IConfiguration _configuration;
        public FormDataService(
            SyllDbContext syllDbContext,
            ILogger<FormDataService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IConfiguration configuration
        ) : base(syllDbContext, logger, httpContextAccessor, mapper)
        {
            _configuration = configuration;
        }

        public async Task<ImportGgSheetResponseDto> ImportDataForm(ImportGgSheetRequestDto dto)
        {
            _logger.LogInformation($"{nameof(ImportDataForm)} dto={JsonSerializer.Serialize(dto)}");
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

            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == dto.IdFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);

            var totalRowsImported = 0;
            var totalDataImported = 0;
            var headers = sheetData[0];
            var dataRows = sheetData.Skip(1).ToList();

            var emailColumnIndex = headers.FindIndex(h => h.Equals("Email", StringComparison.OrdinalIgnoreCase));
            if (emailColumnIndex == -1)
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid);
            }

            var formTruongDataList = new List<domain.Form.FormTruongData>();
            var formDataList = new List<domain.Form.FormData>();

          
            var newTruongDataListToCreate = new List<domain.Form.FormTruongData>();

            for (int i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                if (header.Equals("Email", StringComparison.OrdinalIgnoreCase) ||
                    header.Equals("STT", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var newTruongData = new domain.Form.FormTruongData
                {
                    IdFormLoai = dto.IdFormLoai,
                    IdItem = 0,
                    TenTruong = header,
                    Type = "string",
                    CreatedBy = currentUserId,
                    CreatedDate = vietnamTime,
                    Deleted = false
                };
                newTruongDataListToCreate.Add(newTruongData);
            }

            await _syllDbContext.BulkInsertAsync(newTruongDataListToCreate, new BulkConfig { SetOutputIdentity = true });


            foreach (var truongData in newTruongDataListToCreate)
            {
                truongData.IdItem = truongData.Id;
            }
            await _syllDbContext.BulkUpdateAsync(newTruongDataListToCreate);

            formTruongDataList.AddRange(newTruongDataListToCreate);

           
            var allEmails = dataRows
                .Where(row => row.Count > emailColumnIndex && !string.IsNullOrEmpty(row[emailColumnIndex]))
                .Select(row => row[emailColumnIndex])
                .Distinct()
                .ToList();

            var danhBaDict = await _syllDbContext.DanhBas
                .Where(x => allEmails.Contains(x.Email) && !x.Deleted)
                .ToDictionaryAsync(x => x.Email);

            foreach (var row in dataRows)
            {
                if (row.Count <= emailColumnIndex || string.IsNullOrEmpty(row[emailColumnIndex]))
                {
                    continue;
                }

                var email = row[emailColumnIndex];

              
                if (!danhBaDict.TryGetValue(email, out var danhBa))
                {
                    continue;
                }

                int truongDataIndex = 0;
                for (int i = 0; i < headers.Count; i++)
                {
                    var header = headers[i];
                    if (header.Equals("Email", StringComparison.OrdinalIgnoreCase) ||
                        header.Equals("STT", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var data = i < row.Count ? row[i] : string.Empty;
                    var formData = new domain.Form.FormData
                    {
                        IdFormLoai = dto.IdFormLoai,
                        Data = data,
                        IdTruongData = formTruongDataList[truongDataIndex].Id,
                        IdDanhBa = danhBa.Id,
                        IndexRowTable = null,
                        CreatedBy = currentUserId,
                        CreatedDate = vietnamTime,
                        Deleted = false
                    };
                    formDataList.Add(formData);
                    truongDataIndex++;
                }
                totalRowsImported++;
            }

            if (formDataList.Any())
            {
                await _syllDbContext.BulkInsertAsync(formDataList);
                totalDataImported = formDataList.Count;
            }

            var endTime = DateTime.UtcNow;
            var importTimeInSeconds = (int)(endTime - startTime).TotalSeconds;

            return new ImportGgSheetResponseDto
            {
                TotalRowsImported = totalRowsImported,
                TotalDataImported = totalDataImported,
                ImportTimeInSeconds = importTimeInSeconds
            };
        }
        public async Task<ImportGgSheetResponseDto> ImportDataTableForm(ImportGgSheetTableRequestDto dto)
        {
            _logger.LogInformation($"{nameof(ImportDataTableForm)} dto={JsonSerializer.Serialize(dto)}");
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

            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == dto.IdFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);

            var totalRowsImported = 0;
            var totalDataImported = 0;
            var headers = sheetData[0];
            var dataRows = sheetData.Skip(1).ToList();

            var emailColumnIndex = headers.FindIndex(h => h.Equals("Email", StringComparison.OrdinalIgnoreCase));
            if (emailColumnIndex == -1)
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid);
            }

            var formTruongDataList = new List<domain.Form.FormTruongData>();
            var formDataList = new List<domain.Form.FormData>();


            var relevantHeaders = headers
                .Where(h => !h.Equals("Email", StringComparison.OrdinalIgnoreCase) &&
                            !h.Equals("STT", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var existingTruongDataDict = await _syllDbContext.FormTruongDatas
                .Where(x => relevantHeaders.Contains(x.TenTruong) &&
                            x.IdFormLoai == dto.IdFormLoai &&
                            x.IdItem == dto.IdItem &&
                            !x.Deleted)
                .ToDictionaryAsync(x => x.TenTruong);

            for (int i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                if (header.Equals("Email", StringComparison.OrdinalIgnoreCase) ||
                    header.Equals("STT", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (existingTruongDataDict.TryGetValue(header, out var existingTruongData))
                {
                    formTruongDataList.Add(existingTruongData);
                    continue;
                }

                var newTruongData = new domain.Form.FormTruongData
                {
                    IdFormLoai = dto.IdFormLoai,
                    IdItem = dto.IdItem,
                    TenTruong = header,
                    Type = "string",
                    CreatedBy = currentUserId,
                    CreatedDate = vietnamTime,
                    Deleted = false
                };
                await _syllDbContext.FormTruongDatas.AddAsync(newTruongData);
                await _syllDbContext.SaveChangesAsync();
                formTruongDataList.Add(newTruongData);

                var maxOrder = _syllDbContext.Tables
                    .Where(x => x.IdItem == dto.IdItem && !x.Deleted)
                    .Max(x => (int?)x.Order) ?? 0;

                var newTableHeader = new domain.Form.Table
                {
                    IdItem = dto.IdItem,
                    Order = maxOrder + 1,
                    Ratio = 10,
                    CreatedBy = currentUserId,
                    CreatedDate = vietnamTime,
                    Deleted = false
                };

                await _syllDbContext.Tables.AddAsync(newTableHeader);
                await _syllDbContext.SaveChangesAsync();
            }


            var allEmails = dataRows
                .Where(row => row.Count > emailColumnIndex && !string.IsNullOrEmpty(row[emailColumnIndex]))
                .Select(row => row[emailColumnIndex])
                .Distinct()
                .ToList();

            var danhBaDict = await _syllDbContext.DanhBas
                .Where(x => allEmails.Contains(x.Email) && !x.Deleted)
                .ToDictionaryAsync(x => x.Email);

            var emailIndexRowDict = new Dictionary<string, int>();

            foreach (var row in dataRows)
            {
                if (row.Count <= emailColumnIndex || string.IsNullOrEmpty(row[emailColumnIndex]))
                {
                    continue;
                }

                var email = row[emailColumnIndex];


                if (!danhBaDict.TryGetValue(email, out var danhBa))
                {
                    continue;
                }

                if (!emailIndexRowDict.ContainsKey(email))
                {
                    emailIndexRowDict[email] = 1;
                }
                else
                {
                    emailIndexRowDict[email]++;
                }

                var currentIndexRow = emailIndexRowDict[email];

                int truongDataIndex = 0;
                for (int i = 0; i < headers.Count; i++)
                {
                    var header = headers[i];
                    if (header.Equals("Email", StringComparison.OrdinalIgnoreCase) ||
                        header.Equals("STT", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var data = i < row.Count ? row[i] : string.Empty;
                    var formData = new domain.Form.FormData
                    {
                        IdFormLoai = dto.IdFormLoai,
                        Data = data,
                        IdTruongData = formTruongDataList[truongDataIndex].Id,
                        IdDanhBa = danhBa.Id,
                        IndexRowTable = currentIndexRow,
                        CreatedBy = currentUserId,
                        CreatedDate = vietnamTime,
                        Deleted = false
                    };
                    formDataList.Add(formData);
                    truongDataIndex++;
                }
                totalRowsImported++;
            }

            if (formDataList.Any())
            {
                await _syllDbContext.BulkInsertAsync(formDataList);
                totalDataImported = formDataList.Count;
            }

            var endTime = DateTime.UtcNow;
            var importTimeInSeconds = (int)(endTime - startTime).TotalSeconds;

            return new ImportGgSheetResponseDto
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
