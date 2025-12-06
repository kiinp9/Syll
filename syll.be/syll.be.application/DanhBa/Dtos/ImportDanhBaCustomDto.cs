using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.DanhBa.Dtos
{
    public class ImportDanhBaCustomDto
    {
        public int IdChienDich {  get; set; }
        public string Url { get; set; } = String.Empty;
        public string SheetName { get; set; } = String.Empty;
        public int IndexRowStartImport { get; set; }
        public int IndexRowHeader { get; set; }
        public int IndexColumnHoTen { get; set; }
        public int IndexColumnEmail { get; set; }

        //public int LoaiDanhBa { get; set; }
        public int IndexColumnMaSoToChuc { get; set; }
    }
}
