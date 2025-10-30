using syll.be.application.Form.Dtos;
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
        public void CreateFormDauMuc(CreateFormDauMucDto dto);
        public void UpdateFormDauMuc(UpdateFormDauMucDto dto);
        public void DeleteFormDauMuc(int idFormDauMuc, int idFormLoai);
        public List<ViewFormDauMucDto> GetFormDauMucByidFormLoai(int idFormLoai);
        public ViewFormDauMucByIdDto GetFormDauMucById(int idDauMuc);
        public GetFormInforByIdDanhBaDto GetFormInforByIdDanhBa(int idFormLoai,int idDanhBa);
        public void UpdateFormData(int idFormLoai, int idDanhBa,UpdateFormDataRequestDto dto);

    }
}
