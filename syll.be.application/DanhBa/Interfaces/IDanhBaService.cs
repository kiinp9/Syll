using syll.be.application.DanhBa.Dtos;
using syll.be.shared.HttpRequest.BaseRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.DanhBa.Interfaces
{
    public interface IDanhBaService
    {
        public BaseResponsePagingDto<ViewDanhBaDto> FindDanhBa(FindPagingDanhBaDto dto);
        public  Task<ImportDanhBaResponseDto> ImportDanhBa(ImportDanhBaDto dto);
        public  Task Create(CreateDanhBaDto dto);
        public  Task Update(UpdateDanhBaDto dto);
        public BaseResponsePagingDto<ViewDanhBaAccordingToChucDto> FindPagingDanhBaAccordingToChuc(FindPagingDanhBaAccordingToChucDto dto);
        public  Task Delete(int idToChuc, int idDanhBa);
    }
}
