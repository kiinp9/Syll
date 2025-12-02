using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Reports.Dtos
{
    public class GetThongTinDanhBaToChucReportDto
    {
        public int Id { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;

        public int Status { get; set; }
        public DateTime? LastModified { get; set; } 

        public ViewToChucDanhBaDto toChuc { get; set; } = new ViewToChucDanhBaDto();

    }

    public class ViewToChucDanhBaDto
    {
        public int Id { get; set; }
        public string TenToChuc { get; set; } = String.Empty;
    }
}
