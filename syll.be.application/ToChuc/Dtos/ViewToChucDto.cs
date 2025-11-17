using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ToChuc.Dtos
{
    public class ViewToChucDto
    {
        public int Id { get; set; }
        public string TenToChuc { get; set; } = String.Empty;

        public string MoTa { get; set; } = String.Empty;
        public int SoNhanVien { get; set; }
        public int LoaiToChuc { get; set; }
        public string? MaSoToChuc { get; set; }
    }
}
