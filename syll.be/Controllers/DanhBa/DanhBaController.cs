using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syll.be.application.DanhBa.Dtos;
using syll.be.application.DanhBa.Interfaces;
using syll.be.Attributes;
using syll.be.Controllers.Base;
using syll.be.shared.Constants.Auth;
using syll.be.shared.HttpRequest;

namespace syll.be.Controllers.DanhBa
{

    [Route("api/core/danh-ba")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DanhBaController : BaseController
    {
        private readonly IDanhBaService _danhBaService;
        private readonly ILogger<DanhBaController> _logger;
        public DanhBaController(ILogger<DanhBaController> logger, IDanhBaService danhBaService) : base(logger)
        {
            _danhBaService = danhBaService;
            _logger = logger;
        }
        [Permission(PermissionKeys.DanhBaView)]
        [HttpGet("")]
        public ApiResponse Find([FromQuery] FindPagingDanhBaDto dto)
        {
            try
            {
                var result = _danhBaService.FindDanhBa(dto);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.DanhBaImport)]
        [HttpPost("import")]
        public async Task<ApiResponse> ImportDanhBa([FromBody] ImportDanhBaDto dto)
        {
            try
            {
                var result = await _danhBaService.ImportDanhBa(dto);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
