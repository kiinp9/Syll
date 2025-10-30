using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using syll.be.application.Base;
using syll.be.application.Form.Dtos;
using syll.be.application.Form.Interfaces;
using syll.be.infrastructure.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace syll.be.application.Form.Implements
{
    public class FormService :BaseService, IFormService 
    {
        public FormService(
            SyllDbContext syllDbContext,
            ILogger<FormService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ) : base(syllDbContext, logger, httpContextAccessor, mapper)
        {
        }

        public void Create (CreateFormLoaiDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}  dto={JsonSerializer.Serialize(dto)}");

            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var form = new domain.Form.FormLoai
            {
                TenForm = dto.TenLoaiForm,
                MoTa = dto.MoTa,
                ThoiGianBatDau= dto.ThoiGianBatDau,
                ThoiGianKetThuc= dto.ThoiGianKetThuc,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
                Deleted = false
            };
            _syllDbContext.FormLoais.Add(form);
            _syllDbContext.SaveChanges();
        }

        //public void Update ()
    }
}
