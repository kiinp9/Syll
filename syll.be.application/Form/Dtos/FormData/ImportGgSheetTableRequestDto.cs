using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.FormData
{
    public class ImportGgSheetTableRequestDto
    {
        public int IdFormLoai { get; set; }
        public int IdItem {  get; set; }
        public string Url { get; set; } = String.Empty;
        public string SheetName { get; set; } = String.Empty;
    }
}
