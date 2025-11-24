using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syll.be.application.Reports.Dtos;
using syll.be.application.Reports.Interfaces;
using syll.be.Attributes;
using syll.be.Controllers.Base;
using syll.be.shared.Constants.Auth;
using syll.be.shared.HttpRequest;

namespace syll.be.Controllers.Report
{
    [Route("api/core/thong-ke")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(ILogger<ReportController> logger, IReportService reportService) : base(logger)
        {
            _reportService = reportService;
            _logger = logger;

        }


        [Permission(PermissionKeys.ReportView)]
        [HttpGet("form-loai/{idFormLoai}/nhan-vien")]
        public async Task<ApiResponse> GetReportNhanVienToChuc([FromRoute] int idFormLoai)
        {
            try
            {
                var data = await _reportService.GetReportNhanVienToChuc(idFormLoai);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.ReportView)]
        [HttpGet("form-loai/{idFormLoai}/to-chuc/paging")]
        public ApiResponse FindPagingReportNhanVienToChuc([FromQuery] GetThongTinToChucDanhBaReportFindPagingDto dto, [FromRoute] int idFormLoai)
        {
            try
            {
                var data =  _reportService.FindPagingToChucDanhBa(dto,idFormLoai);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

    }
}
