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
        [Permission(PermissionKeys.DanhBaAdd)]
        [HttpPost("")]
        public async Task<ApiResponse> CreateDanhBa([FromBody] CreateDanhBaDto dto)
        {
            try
            {
                await _danhBaService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DanhBaUpdate)]
        [HttpPut("")]
        public async Task<ApiResponse> UpdateDanhBa([FromBody] UpdateDanhBaDto dto)
        {
            try
            {
                await _danhBaService.Update(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
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
        [Permission(PermissionKeys.DanhBaView)]
        [HttpGet("according-to-to-chuc")]
        public ApiResponse FindPagingDanhBaAccordingToChuc([FromQuery] FindPagingDanhBaAccordingToChucDto dto)
        {
            try
            {
                var result = _danhBaService.FindPagingDanhBaAccordingToChuc(dto);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DanhBaDelete)]
        [HttpDelete("{idDanhBa}/to-chuc/{idToChuc}")]
        public async Task<ApiResponse> DeleteDanhBa([FromRoute]int idDanhBa,[FromRoute] int idToChuc)
        {
            try
            {
                await _danhBaService.Delete(idToChuc, idDanhBa);
                return new();
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
