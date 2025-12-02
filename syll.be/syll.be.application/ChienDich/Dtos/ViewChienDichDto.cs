using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ChienDich.Dtos
{
    public class ViewChienDichDto
    {
        public int Id { get; set; }
        public string TenChienDich { get; set; } = String.Empty;

        public string MoTa { get; set; } = String.Empty;
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc {  get; set; }
    }
}
