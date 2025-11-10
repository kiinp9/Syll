using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syll.be.application.Form.Interfaces;
using syll.be.Controllers.Base;
using syll.be.lib.Form.Interfaces;

namespace syll.be.Controllers.Form
{
    [Route("api/core/form-template")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FormTemplateController:BaseController
    {
        private readonly IFormTemplateService _formTemplateService;
        private readonly ILogger<FormTemplateController> _logger;
        public FormTemplateController(ILogger<FormTemplateController> logger, IFormTemplateService formTemplateService) : base(logger)
        {
            _formTemplateService = formTemplateService;
            _logger = logger;
        }

        [HttpPost("form-loai/{idFormLoai}/replace")]
        public async Task<IActionResult> ReplaceWordFormTemplate([FromRoute] int idFormLoai)
        {
            try
            {
                var replaceBytes = await _formTemplateService.ReplaceWordFormTemplate(idFormLoai);
                return File(
                    replaceBytes,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    $"SoYeuLyLich_{DateTime.Now:yyyyMMddHHmmss}.docx"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error replacing word template for IdFormLoai={idFormLoai}");
                return Ok(OkException(ex));
            }
        }


        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSoYeuLyLichTemplate()
        {
            try
            {
                var templateBytes = _formTemplateService.GenerateSoYeuLyLichTemplate();

                return File(
                    templateBytes,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "SoYeuLyLich_Template.docx"
                );
            }
            catch (Exception ex)
            {

                return Ok(OkException(ex));
            }
        }
    }
}
