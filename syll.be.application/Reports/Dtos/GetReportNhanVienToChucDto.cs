using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Reports.Dtos
{
    public class GetReportNhanVienToChucDto
    {
        public int TotalNhanVien { get; set; }
        public int TotalNhanVienCheckForm { get; set; }
        public int TotalNhanVienChuaCheckForm { get; set; }
        public int TotalNhanVienChuaImportData { get; set; }
    }
}
