using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syll.be.application.ChienDich.Dtos;
using syll.be.application.ChienDich.Interfaces;
using syll.be.Attributes;
using syll.be.Controllers.Base;
using syll.be.shared.Constants.Auth;
using syll.be.shared.HttpRequest;

namespace syll.be.Controllers.ChienDich
{
    [Route("api/core/chien-dich")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChienDichController:BaseController
    {
        private readonly IChienDichService _chienDichService;
        private readonly ILogger<ChienDichController> _logger;


        public ChienDichController(ILogger<ChienDichController> logger,IChienDichService chienDichService) : base(logger) 
        {
            _chienDichService = chienDichService;
            _logger = logger;
        }


        [Permission(PermissionKeys.ChienDichAdd)]
        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateChienDichDto dto)
        {
            try
            {
                _chienDichService.CreateChienDich(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.ChienDichUpdate)]
        [HttpPut("")]
        public ApiResponse Update([FromBody] UpdateChienDichDto dto)
        {
            try
            {
                _chienDichService.UpdateChienDich(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        //[Permission(PermissionKeys.ChienDichView)]
        [HttpGet("")]
        public async Task<ApiResponse> FindPaging([FromQuery] FindPagingChienDichDto dto)
        {
            try
            {
                var data = await _chienDichService.FindPagingChienDich(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        //[Permission(PermissionKeys.ChienDichView)]
        [HttpGet("{idChienDich}")]
        public async Task<ApiResponse> FindChienDichById([FromRoute] int idChienDich)
        {
            try
            {
                var data = await _chienDichService.FindChienDichById(idChienDich);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }



        [Permission(PermissionKeys.ChienDichDelete)]
        [HttpDelete("{id}")]
        public ApiResponse Delete([FromRoute] int id)
        {
            try
            {
                _chienDichService.DeleteChienDich(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        //[Permission(PermissionKeys.ChienDichView)]
        [HttpGet("form")]
        public async Task<ApiResponse> FindPagingFormLoaiByIdChienDich([FromQuery] FindPagingFormLoaiByIdChienDichDto dto)
        {
            try
            {
                var data = await _chienDichService.FindPagingFormLoaiByIdChienDich(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
