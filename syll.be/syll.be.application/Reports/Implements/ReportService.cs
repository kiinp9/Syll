using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using syll.be.application.Base;
using syll.be.application.Form.Implements;
using syll.be.application.Reports.Dtos;
using syll.be.application.Reports.Interfaces;
using syll.be.infrastructure.data;
using syll.be.shared.Constants.Report;
using syll.be.shared.HttpRequest.AppException;
using syll.be.shared.HttpRequest.BaseRequest;
using syll.be.shared.HttpRequest.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace syll.be.application.Reports.Implements
{
    public class ReportService : BaseService, IReportService
    {
        public ReportService(
            SyllDbContext syllDbContext,
            ILogger<ReportService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ) : base(syllDbContext, logger, httpContextAccessor, mapper)
        {

        }

        public async Task<GetReportNhanVienToChucDto> GetReportNhanVienToChuc(int idFormLoai)
        {
            _logger.LogInformation($"{nameof(GetReportNhanVienToChuc)}");
            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            var totalNhanVien = await _syllDbContext.ToChucDanhBa
                 .Where(tdb => !tdb.Deleted)
                 .Select(tdb => tdb.IdDanhBa)
                 .Distinct()
                 .CountAsync();
            var totalNhanVienCheckForm = await _syllDbContext.ToChucDanhBa
                .Where(tdb => !tdb.Deleted)
                .Where(tdb => _syllDbContext.FormDatas
                     .Any(fd => !fd.Deleted && fd.IdDanhBa == tdb.IdDanhBa && fd.IdFormLoai == idFormLoai && fd.Modified == true))
                .Select(tdb => tdb.IdDanhBa)
                .Distinct()
                .CountAsync();
            var totalNhanVienChuaImportData = await _syllDbContext.ToChucDanhBa
                .Where(tdb => !tdb.Deleted)
                .Where(tcb => !_syllDbContext.FormDatas
                      .Any(fd => !fd.Deleted && fd.IdDanhBa == tcb.IdDanhBa && fd.IdFormLoai == idFormLoai))
                .Select(tdb => tdb.IdDanhBa)
                .Distinct()
                .CountAsync();

            var totalNhanVienChuaCheckForm = totalNhanVien - totalNhanVienCheckForm - totalNhanVienChuaImportData;

            return new GetReportNhanVienToChucDto
            {
                TotalNhanVien = totalNhanVien,
                TotalNhanVienCheckForm = totalNhanVienCheckForm,
                TotalNhanVienChuaCheckForm = totalNhanVienChuaCheckForm,
                TotalNhanVienChuaImportData = totalNhanVienChuaImportData
            };



        }


        public BaseResponsePagingDto<GetThongTinToChucDanhBaReportDto> FindPagingToChucDanhBa(GetThongTinToChucDanhBaReportFindPagingDto dto, int idFormLoai)
        {
            _logger.LogInformation($"{nameof(FindPagingToChucDanhBa)} dto={JsonSerializer.Serialize(dto)}");

            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);

            var query = from tc in _syllDbContext.ToChucs
                        where !tc.Deleted
                        orderby tc.Id
                        select tc;

            var data = query.Paging(dto).ToList();
            var toChucIds = data.Select(x => x.Id).ToList();

            var totalNhanVienToChucDict = _syllDbContext.ToChucDanhBa
                .Where(tdb => toChucIds.Contains(tdb.IdToChuc) && !tdb.Deleted)
                .GroupBy(tdb => tdb.IdToChuc)
                .Select(g => new { IdToChuc = g.Key, Count = g.Select(x => x.IdDanhBa).Distinct().Count() })
                .ToDictionary(x => x.IdToChuc, x => x.Count);

            var totalNhanVienToChucCheckFormDict = _syllDbContext.ToChucDanhBa
                .Where(tdb => toChucIds.Contains(tdb.IdToChuc) && !tdb.Deleted)
                .Where(tdb => _syllDbContext.FormDatas
                    .Any(fd => !fd.Deleted && fd.IdDanhBa == tdb.IdDanhBa && fd.IdFormLoai == idFormLoai && fd.Modified == true))
                .GroupBy(tdb => tdb.IdToChuc)
                .Select(g => new { IdToChuc = g.Key, Count = g.Select(x => x.IdDanhBa).Distinct().Count() })
                .ToDictionary(x => x.IdToChuc, x => x.Count);

            var items = data.Select(tc =>
            {
                var totalNhanVien = totalNhanVienToChucDict.GetValueOrDefault(tc.Id, 0);
                var totalCheckForm = totalNhanVienToChucCheckFormDict.GetValueOrDefault(tc.Id, 0);
                var progress = totalNhanVien > 0 ? Math.Round((decimal)totalCheckForm / totalNhanVien * 100, 2) : 0;

                return new GetThongTinToChucDanhBaReportDto
                {
                    IdToChuc = tc.Id,
                    TenToChuc = tc.TenToChuc,
                    totalNhanVienToChuc = totalNhanVien,
                    totalNhanVienToChucCheckForm = totalCheckForm,
                    totalNhanVienToChucChuaCheckForm = totalNhanVien - totalCheckForm,
                    Progress = progress
                };
            }).ToList();

            return new BaseResponsePagingDto<GetThongTinToChucDanhBaReportDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }


        public BaseResponsePagingDto<GetThongTinDanhBaToChucReportDto> FindPagingDanhBaToChuc(GetThongTinDanhBaToChucReportFindPagingDto dto, int idFormLoai, int idToChuc)
        {
            _logger.LogInformation($"{nameof(FindPagingDanhBaToChuc)} dto={JsonSerializer.Serialize(dto)}");

            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);

            var toChuc = _syllDbContext.ToChucs.FirstOrDefault(x => x.Id == idToChuc && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ToChucErrorNotFound);

            var query = from tdb in _syllDbContext.ToChucDanhBa
                        join db in _syllDbContext.DanhBas on tdb.IdDanhBa equals db.Id
                        where tdb.IdToChuc == idToChuc && !tdb.Deleted && !db.Deleted
                        select new { tdb, db };

            if (!string.IsNullOrEmpty(dto.Keyword))
            {
                query = query.Where(x => x.db.HoVaTen.Contains(dto.Keyword) || x.db.Email.Contains(dto.Keyword));
            }

            var allData = query.ToList();
            var allDanhBaIds = allData.Select(x => x.db.Id).ToList();

            var formDataDict = _syllDbContext.FormDatas
                .Where(fd => allDanhBaIds.Contains(fd.IdDanhBa) && fd.IdFormLoai == idFormLoai && !fd.Deleted)
                .GroupBy(fd => fd.IdDanhBa)
                .Select(g => new
                {
                    IdDanhBa = g.Key,
                    HasModified = g.Any(x => x.Modified == true),
                    LastModified = g.Max(x => x.ModifiedDate)
                })
                .ToDictionary(x => x.IdDanhBa);

            var allItems = allData.Select(x =>
            {
                var hasData = formDataDict.ContainsKey(x.db.Id);
                int status;
                DateTime? lastModified = null;

                if (!hasData)
                {
                    status = ReportConstants.ChuaImportData;
                }
                else if (!formDataDict[x.db.Id].HasModified)
                {
                    status = ReportConstants.ChuaCheck;
                }
                else
                {
                    status = ReportConstants.DaCheck;
                    lastModified = formDataDict[x.db.Id].LastModified;
                }

                return new GetThongTinDanhBaToChucReportDto
                {
                    Id = x.db.Id,
                    HoVaTen = x.db.HoVaTen,
                    Email = x.db.Email,
                    Status = status,
                    LastModified = lastModified,
                    toChuc = new ViewToChucDanhBaDto
                    {
                        Id = toChuc.Id,
                        TenToChuc = toChuc.TenToChuc
                    }
                };
            }).ToList();

            if (dto.Status.HasValue && dto.Status.Value != ReportConstants.TatCa)
            {
                allItems = allItems.Where(x => x.Status == dto.Status.Value).ToList();
            }

            var totalItems = allItems.Count;
            var items = allItems.Skip(dto.GetSkip()).Take(dto.PageSize).ToList();

            return new BaseResponsePagingDto<GetThongTinDanhBaToChucReportDto>
            {
                Items = items,
                TotalItems = totalItems
            };
        }


        public GetDashBoardReportDto GetDashBoardReport()
        {
            _logger.LogInformation($"{nameof(GetDashBoardReport)}");

            var totalNhanVien = _syllDbContext.ToChucDanhBa
                .Where(tdb => !tdb.Deleted)
                .Select(tdb => tdb.IdDanhBa)
                .Distinct()
                .Count();

            var totalToChuc = _syllDbContext.ToChucs
                .Where(tc => !tc.Deleted)
                .Count();

            var totalForm = _syllDbContext.FormLoais
                .Where(fl => !fl.Deleted)
                .Count();

            return new GetDashBoardReportDto
            {
                totalNhanVien = totalNhanVien,
                totalToChuc = totalToChuc,
                totalForm = totalForm
            };
        }
    }
}
