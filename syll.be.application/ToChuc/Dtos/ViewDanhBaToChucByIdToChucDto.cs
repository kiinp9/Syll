using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ToChuc.Dtos
{
    public class ViewDanhBaToChucByIdToChucDto
    {
        public int Id { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public ViewRoleDanhBaToChucDto role { get; set; } = new ViewRoleDanhBaToChucDto();
        public ViewToChucDanhBaDto toChuc { get; set; } = new ViewToChucDanhBaDto();

    }

    public class ViewToChucDanhBaDto
    {
        public int Id { get; set; }
        public string TenToChuc { get; set; } = String.Empty;
    }

    public class ViewRoleDanhBaToChucDto
    {
        public string Id { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
    }
}
