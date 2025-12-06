using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ChienDich.Dtos
{
    public class ViewChienDichByIdDto
    {
        public int Id { get; set; }
        public string TenChienDich { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc {  get; set; }
        public List<ViewFormLoaisChienDichDto> FormLoais { get; set; } = new List<ViewFormLoaisChienDichDto>();
    }

    public class ViewFormLoaisChienDichDto
    {
        public int IdFormLoai { get; set; }
        public string TenFormLoai { get; set; } = String.Empty;
    }
}
