using syll.be.application.Form.Dtos.Form;
using syll.be.shared.HttpRequest.BaseRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Interfaces
{
    public  interface IFormService
    {
        public void Create(CreateFormLoaiDto dto);
        public void Update(UpdateFormLoaiDto dto);
        public BaseResponsePagingDto<ViewFormLoaiDto> Find(FindPagingFormLoaiDto dto);
        public ViewFormLoaiDto GetFormLoaiById(int id);
        public void Delete(int id);

        //public GetFormInforByIdDanhBaDto GetFormInforByIdDanhBa(int idFormLoai,int idDanhBa);
        public Task UpdateFormData(int idFormLoai, UpdateFormDataRequestDto dto);
        public void UpdateFormDataForAdmin(int idFormLoai, int idDanhBa, UpdateFormDataRequestDto dto);
        public List<GetDropDownDataResponseDto?> GetDropDownData(int idTruongData);
    }
}
