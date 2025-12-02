using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ChienDich.Dtos
{
    public class UpdateChienDichDto
    {
        public int IdChienDich { get; set; }
        public string TenChienDich { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public DateTime? ThoiGíanBatDau {  get; set; }
        public DateTime? ThoiGianKetThuc {  get; set; }
        public List<UpdateFormLoaiChienDichDto> FormLoais { get; set; } = new List<UpdateFormLoaiChienDichDto>();
    }

    public class UpdateFormLoaiChienDichDto
    {
        public int IdFormLoai { get; set; }
    }
}
