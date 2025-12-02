using syll.be.application.Reports.Dtos;
using syll.be.shared.HttpRequest.BaseRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Reports.Interfaces
{
    public interface IReportService
    {
        public  Task<GetReportNhanVienToChucDto> GetReportNhanVienToChuc(int idFormLoai);
        public BaseResponsePagingDto<GetThongTinToChucDanhBaReportDto> FindPagingToChucDanhBa(GetThongTinToChucDanhBaReportFindPagingDto dto,int idFormLoai);
    }
}
