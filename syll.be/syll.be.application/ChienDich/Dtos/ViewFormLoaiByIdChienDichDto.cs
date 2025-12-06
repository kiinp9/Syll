using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ChienDich.Dtos
{
    public class ViewFormLoaiByIdChienDichDto
    {
        public int Id { get; set; }
        public string TenForm { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public int TongSoTruong { get; set; }
        public DateTime? ThoiGianTao {  get; set; }
        public DateTime? ThoiGianCapNhatGanNhat { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
    }
}
