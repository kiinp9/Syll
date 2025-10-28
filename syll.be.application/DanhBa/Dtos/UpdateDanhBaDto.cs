using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.DanhBa.Dtos
{
    public class UpdateDanhBaDto
    {
        public int Id { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public int CurrentIdToChuc { get; set; }
        public int IdToChuc { get; set; }
    }
}
