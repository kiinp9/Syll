using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syll.be.application.Form.Dtos.FormData;
using syll.be.application.Form.Interfaces;
using syll.be.Attributes;
using syll.be.Controllers.Base;
using syll.be.shared.Constants.Auth;
using syll.be.shared.HttpRequest;

namespace syll.be.Controllers.Form
{
    [Route("api/core/form-data")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FormDataController : BaseController
    {
        private readonly IFormDataService _formDataService;
        private readonly ILogger<FormDataController> _logger;
        public FormDataController(ILogger<FormDataController> logger, IFormDataService formDataService) : base(logger)
        {
            _formDataService = formDataService;
            _logger = logger;
        }

        [Permission(PermissionKeys.FormImport)]
        [HttpPost()]
        public async Task<ApiResponse> ImportFormData(ImportGgSheetRequestDto dto)
        {
            try
            {
                var data = await _formDataService.ImportDataForm(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.FormImport)]
        [HttpPost("table")]
        public async Task<ApiResponse> ImportFormDataTable(ImportGgSheetTableRequestDto dto)
        {
            try
            {
                var data = await _formDataService.ImportDataTableForm(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

    }
}
