using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.Form
{
    public class GetDropDownDataResponseDto
    {
        public int Id { get; set; }
        public string Data { get; set; } = String.Empty;
        public int Order { get; set; }
    }
}
