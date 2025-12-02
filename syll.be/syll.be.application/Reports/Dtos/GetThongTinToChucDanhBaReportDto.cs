using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Reports.Dtos
{
    public class GetThongTinToChucDanhBaReportDto
    {
        public int IdToChuc { get; set; }
        public string TenToChuc { get; set; } = String.Empty;
        public int totalNhanVienToChucCheckForm { get; set; }
        public int totalNhanVienToChucChuaCheckForm { get; set; }
        public int totalNhanVienToChuc { get; set; }
        public decimal Progress { get; set; }

    }
}
