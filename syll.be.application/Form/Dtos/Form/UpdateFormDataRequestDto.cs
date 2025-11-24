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
        // Dùng cho các trường thường (không phải bảng)
        public List<UpdateListFormDataRequestDto> Datas { get; set; } = new List<UpdateListFormDataRequestDto>();
        // Dùng cho các trường kiểu Table
        public List<List<UpdateListFormDataRequestDto>>? TableRows { get; set; }
    }

    public class UpdateListFormDataRequestDto
    {
        public int IdData { get; set; }
        public string Data { get; set; } = String.Empty;
    }
}
