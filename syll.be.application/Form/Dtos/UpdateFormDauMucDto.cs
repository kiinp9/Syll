using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos
{
    public class UpdateFormDauMucDto
    {
        public int Id { get; set; }
        public int IdForm { get; set; }
        public string TenDauMuc { get; set; } = string.Empty;
        public string SoDauMuc { get; set; } = string.Empty; 
    }
}
