using syll.be.application.ChienDich.Dtos;
using syll.be.shared.HttpRequest.BaseRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ChienDich.Interfaces
{
    public interface IChienDichService
    {
        public void CreateChienDich(CreateChienDichDto dto);

        public void UpdateChienDich(UpdateChienDichDto dto);
        public BaseResponsePagingDto<ViewChienDichDto> FindPagingChienDich(FindPagingChienDichDto dto);
        public void DeleteChienDich(int id);
        public  Task<BaseResponsePagingDto<ViewFormLoaiByIdChienDichDto>> FindPagingFormLoaiByIdChienDich(FindPagingFormLoaiByIdChienDichDto dto);
    }
}
