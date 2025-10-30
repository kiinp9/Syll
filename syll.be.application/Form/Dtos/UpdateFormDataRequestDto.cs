using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos
{
    public class UpdateFormDataRequestDto
    {
        public List<UpdateFormDauMucRequestDto> DauMucs { get; set; } = new List<UpdateFormDauMucRequestDto>();

    }

    public class UpdateFormDauMucRequestDto
    {
        public int IdDauMuc { get; set; }
        public List<UpdateFormTruongDataRequestDto> TruongDatas { get; set; } = new List<UpdateFormTruongDataRequestDto>();
    }
    public class UpdateFormTruongDataRequestDto
    {
        public int IdTruong { get; set; }
        public string Data { get; set; } = String.Empty;


    }
    

}
