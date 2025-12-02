using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using syll.be.application.Base;
using syll.be.application.Form.Implements;
using syll.be.application.Reports.Dtos;
using syll.be.application.Reports.Interfaces;
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

namespace syll.be.application.Reports.Implements
{
    public class ReportService: BaseService,IReportService
    {
        public ReportService(
            SyllDbContext syllDbContext,
            ILogger<ReportService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ): base(syllDbContext, logger, httpContextAccessor, mapper)
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


        public BaseResponsePagingDto<GetThongTinToChucDanhBaReportDto> FindPagingToChucDanhBa(GetThongTinToChucDanhBaReportFindPagingDto dto,int idFormLoai)
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
    }
}
