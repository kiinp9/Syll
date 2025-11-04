using syll.be.application.Form.Dtos.FormData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Interfaces
{
    public interface IFormDataService
    {
        public  Task<ImportGgSheetResponseDto> ImportDataForm(ImportGgSheetRequestDto dto);
        public  Task<ImportGgSheetResponseDto> ImportDataTableForm(ImportGgSheetTableRequestDto dto);
    }
}
