using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.FormData
{
    public class ImportGgSheetResponseDto
    {
        public int TotalRowsImported { get; set; }
        public int TotalDataImported { get; set; }
        public int ImportTimeInSeconds { get; set; }
    }
}
