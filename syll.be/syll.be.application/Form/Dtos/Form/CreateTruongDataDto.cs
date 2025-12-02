using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.Form
{
    public class CreateTruongDataDto
    {
        public int IdFormLoai {  get; set; }
        public int IdItem { get; set; }
        public string TenTruong { get; set; } = String.Empty;
        //public string Data { get; set; } = String.Empty;
    }
}
