using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syll.be.application.DanhBa.Interfaces;
using syll.be.application.Form.Dtos.Form;
using syll.be.application.Form.Interfaces;
using syll.be.Attributes;
using syll.be.Controllers.Base;
using syll.be.Controllers.DanhBa;
using syll.be.shared.Constants.Auth;
using syll.be.shared.HttpRequest;

namespace syll.be.Controllers.Form
{
    [Route("api/core/form")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FormController : BaseController
    {
        private readonly IFormService _formService;
        private readonly ILogger<FormController> _logger;
        public FormController(ILogger<FormController> logger, IFormService formService) : base(logger)
        {
            _formService = formService;
            _logger = logger;
        }



        [Permission(PermissionKeys.FormAdd)]
        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateFormLoaiDto dto)
        {
            try
            {
                _formService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }

        }



        [Permission(PermissionKeys.FormUpdate)]
        [HttpPut("")]
        public ApiResponse Update([FromBody] UpdateFormLoaiDto dto)
        {
            try
            {
                _formService.Update(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }



        //[Permission(PermissionKeys.FormView)]
        [HttpGet("")]
        public async Task<ApiResponse> Find([FromQuery] FindPagingFormLoaiDto dto)
        {
            try
            {
                var result = await _formService.Find(dto);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }



        [Permission(PermissionKeys.FormDelete)]
        [HttpDelete("{id}/chien-dich/{idChienDich}")]
        public ApiResponse Delete([FromRoute] int id, [FromRoute] int idChienDich)
        {
            try
            {
                _formService.Delete(id, idChienDich);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }



        //[Permission(PermissionKeys.FormView)]
        [HttpGet("{id}/chien-dich/{idChienDich}")]
        public ApiResponse GetFormLoaiById([FromRoute] int id, [FromRoute] int idChienDich)
        {
            try
            {
                var result = _formService.GetFormLoaiById(id,idChienDich);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }




        /*[Permission(PermissionKeys.FormView)]
        [HttpGet("{idFormLoai}/danh-ba/{idDanhBa}/form-content")]
        public ApiResponse GetFormInforByIdDanhBa([FromRoute] int idFormLoai, [FromRoute] int idDanhBa)
        {
            try
            {
                var result = _formService.GetFormInforByIdDanhBa(idFormLoai, idDanhBa);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }*/


        //UpdateFormDataAdmin cho nhân viên 
        [HttpPut("{idFormLoai}/form-content")]
        public async Task<ApiResponse> UpdateFormData([FromRoute] int idFormLoai, [FromBody] UpdateFormDataRequestDto dto)
        {
            try
            {
                await _formService.UpdateFormData(idFormLoai, dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }




        //UpdateFormDataAdmin cho admin
        [Permission(PermissionKeys.FormUpdate)]
        [HttpPut("admin/form-content")]
        public async Task<ApiResponse> UpdateFormDataAdmin ([FromQuery] int idFormLoai, [FromQuery] int idDanhBa ,[FromBody] UpdateFormDataRequestDto dto)
        {
            try
            {
                await _formService.UpdateFormDataAdmin(idFormLoai,idDanhBa, dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }





        [HttpGet("truong-data/{idTruongData}")]
        public ApiResponse UpdateFormDataForAdmin([FromRoute] int idTruongData)
        {
            try
            {
                var data =_formService.GetDropDownData(idTruongData);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        // DeleteRowTableData cho nhân viên 
        [HttpDelete("row-table")]
        public async Task<ApiResponse> DeleteRowTableData([FromBody] DeleteRowTableDataDto dto)
        {
            try
            {
                await _formService.DeleteRowTableData(dto);
                return new();
            }catch(Exception ex)
            {
                return OkException(ex);
            }
        }


        //DeleteRowTableData cho admin
        [HttpDelete("nhan-vien/{idDanhBa}/row-table")]
        public async Task<ApiResponse> DeleteRowTableDataAdmin([FromBody] DeleteRowTableDataDto dto, [FromRoute]int? idDanhBa)
        {
            try
            {
                await _formService.DeleteRowTableDataAdmin(dto,idDanhBa);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }



        [Permission(PermissionKeys.FormAdd)]
        [HttpPost("truong-data")]
        public ApiResponse CreateTruongData([FromBody] CreateTruongDataDto dto)
        {
            try
            {
                _formService.CreateTruongData(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }

        }

        [Permission(PermissionKeys.FormView)]
        [HttpGet("list-drop-down")]
        public ApiResponse GetListDropDownFormLoai()
        {
            try
            {
                var data = _formService.GetListDropDownFormLoai();
                return new(data);
            }catch(Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
