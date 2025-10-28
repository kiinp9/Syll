using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.DanhBa.Dtos
{
    public class ViewDanhBaDto
    {
        public int Id { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string HoDem { get; set; } = String.Empty;
        public string Ten { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public List<ViewDanhBaWithToChucDto> Items { get; set; } = new List<ViewDanhBaWithToChucDto>();
    }
    public class ViewDanhBaWithToChucDto
    {
        public int Id { get; set; }
        public string TenToChuc { get; set; } = String.Empty;
        public int LoaiToChuc { get; set; } 
        public string MaSoToChuc { get; set; } = String.Empty;
    }
}
