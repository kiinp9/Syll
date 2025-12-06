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
        public Task<BaseResponsePagingDto<ViewFormLoaiDto>> Find(FindPagingFormLoaiDto dto);

        public ViewFormLoaiDto GetFormLoaiById(int id, int idChienDich);

        public void Delete(int id, int idChienDich);

        //public GetFormInforByIdDanhBaDto GetFormInforByIdDanhBa(int idFormLoai,int idDanhBa);

        // UpdateFormData cho nhân viên 
        public Task UpdateFormData(int idFormLoai, UpdateFormDataRequestDto dto);

        //UpdateFormDataAdmin cho admin
        public Task UpdateFormDataAdmin(int idFormLoai, int? idDanhBa, UpdateFormDataRequestDto dto);
        public List<GetDropDownDataResponseDto?> GetDropDownData(int idTruongData);
        // DeleteRowTableData cho nhân viên 
        public Task DeleteRowTableData(DeleteRowTableDataDto dto);

        //DeleteRowTableData cho admin
        public  Task DeleteRowTableDataAdmin(DeleteRowTableDataDto dto, int? idDanhBa);
        public void CreateTruongData(CreateTruongDataDto dto);
        public List<GetListDropDownFormLoaiDto> GetListDropDownFormLoai();
    }
}
