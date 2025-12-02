using syll.be.application.ToChuc.Dtos;
using syll.be.shared.HttpRequest.BaseRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ToChuc.Interfaces
{
    public interface  IToChucService
    {
        public void Create(CreateToChucDto dto);
        public void Update(int idToChuc, UpdateToChucDto dto);
        public void Delete(int idToChuc);
        public BaseResponsePagingDto<ViewToChucDto> Find(FindPagingToChucDto dto);
        public List<GetListDropDownDto> GetListDropDown();
        public BaseResponsePagingDto<ViewDanhBaToChucByIdToChucDto> FindPagingDanhBaToChuc(FindPagingDanhBaToChucByIdToChucDto dto);
    }
}
