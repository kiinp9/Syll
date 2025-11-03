using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syll.be.application.Form.Dtos.FormLayout;
using syll.be.application.Form.Interfaces;
using syll.be.Attributes;
using syll.be.Controllers.Base;
using syll.be.shared.Constants.Auth;
using syll.be.shared.HttpRequest;

namespace syll.be.Controllers.Form
{
    [Route("api/core/form-layout")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FormLayoutController : BaseController
    {
        private readonly IFormLayoutService _formLayoutService;
        private readonly ILogger<FormLayoutController> _logger;
        public FormLayoutController(ILogger<FormLayoutController> logger, IFormLayoutService formLayoutService) : base(logger)
        {
            _formLayoutService = formLayoutService;
            _logger = logger;
        }


        [Permission(PermissionKeys.FormAdd)]
        [HttpPost("")]
        public ApiResponse CreateLayout([FromBody] CreateLayoutDto dto)
        {
            try
            {
                _formLayoutService.CreateLayout(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        //[Permission(PermissionKeys.FormView)]
        [HttpGet("{id}")]
        public async Task<ApiResponse> FindById([FromRoute] int id)
        {
            try
            {
                var data = await _formLayoutService.FindLayoutById(id);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.FormUpdate)]
        [HttpPut("")]
        public async Task<ApiResponse> UpdateLayout([FromBody] UpdateLayoutDto dto)
        {
            try
            {
                await _formLayoutService.UpdateLayOut(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.FormDelete)]
        [HttpDelete("{idLayout}/form-loai/{idFormLoai}")]
        public ApiResponse DeleteLayout([FromRoute] int idLayout, [FromRoute] int idFormLoai)
        {
            try
            {
                _formLayoutService.DeleteLayout(idLayout, idFormLoai);
                return new();
            }catch(Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.FormAdd)]
        [HttpPost("block")]
        public ApiResponse CreateBlock([FromBody] CreateBlockDto dto)
        {
            try
            {
                _formLayoutService.CreateBlock(dto);
                return new();
            }catch(Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.FormUpdate)]
        [HttpPut("block")]
        public async Task<ApiResponse> UpdateBlock([FromBody] UpdateBlockDto dto)
        {
            try
            {
                await _formLayoutService.UpdateBlock(dto);
                return new();
            }catch(Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.FormView)]
        [HttpGet("{idLayout}/list-block")]
        public ApiResponse GetListBlockByIdLayout([FromRoute] int idLayout)
        {
            try
            {
                var data =_formLayoutService.GetBlockByIdLayout(idLayout);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.FormView)]
        [HttpGet("block/{id}")]
        public ApiResponse GetBlockById([FromRoute] int id)
        {
            try
            {
                var data = _formLayoutService.GetBlockById(id);
                return new(data);
            }catch(Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.FormDelete)]
        [HttpDelete("{idLayout}/block/{id}")]
        public ApiResponse DeleteBlock([FromRoute] int idLayout, [FromRoute] int id)
        {
            try
            {
                _formLayoutService.DeleteBlock(idLayout, id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.FormAdd)]
        [HttpPost("row")]
        public ApiResponse CreateRow([FromBody] CreateRowDto dto)
        {
            try
            {
                _formLayoutService.CreateRow(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.FormUpdate)]
        [HttpPut("row")]
        public async Task<ApiResponse> UpdateRow([FromBody] UpdateRowDto dto)
        {
            try
            {
                await _formLayoutService.UpdateRow(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }



        [Permission(PermissionKeys.FormDelete)]
        [HttpDelete("block/{idBlock}/row/{idRow}")]
        public ApiResponse DeleteRow([FromRoute] int idBlock, [FromRoute] int idRow)
        {
            try
            {
                _formLayoutService.DeleteRow(idBlock, idRow);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.FormAdd)]
        [HttpPost("item")]
        public ApiResponse CreateItem([FromBody] CreateItemDto dto)
        {
            try
            {
                _formLayoutService.CreateItem(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.FormUpdate)]
        [HttpPut("item")]
        public async Task<ApiResponse> UpdateItem([FromBody] UpdateItemDto dto)
        {
            try
            {
                await _formLayoutService.UpdateItem(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }



        [Permission(PermissionKeys.FormDelete)]
        [HttpDelete("row/{idRow}/item/{id}")]
        public ApiResponse DeleteItem([FromRoute] int idRow, [FromRoute] int id)
        {
            try
            {
                _formLayoutService.DeleteItem(idRow, id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }



    }
}
