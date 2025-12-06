using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.Form
{
    public class UpdateFormLoaiDto
    {
        public int IdChienDich { get; set; }
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
    }
}
