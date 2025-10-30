using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos
{
    public class ViewFormDauMucByIdDto
    {
        public int Id { get; set; }
        public string TenDauMuc { get; set; } = String.Empty;
        public string SoDauMuc { get; set; } = String.Empty;
        public List<ViewFormTruongDto> Truongs { get; set; } = new List<ViewFormTruongDto>();
    }

    public class ViewFormTruongDto
    {
        public int IdDauMuc { get; set; }
        public string TenTruong { get; set; } = string.Empty;

    }
}
