using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.Form
{
    public class UpdateFormDataRequestDto
    {
        public List<UpdateFormTruongDataRequestDto> TruongDatas { get; set; } = new List<UpdateFormTruongDataRequestDto>();

    }

    public class UpdateFormTruongDataRequestDto
    {
        public int IdTruong { get; set; }
        public string Data { get; set; } = string.Empty;


    }

}
