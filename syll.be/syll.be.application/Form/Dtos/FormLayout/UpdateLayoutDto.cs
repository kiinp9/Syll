using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.FormLayout
{
    public class UpdateLayoutDto
    {
        public int IdFormLoai { get; set; }
        public int Id {  get; set; }
        public string Ten { get; set; } = String.Empty;
        public int Order { get; set; }
    }
}
