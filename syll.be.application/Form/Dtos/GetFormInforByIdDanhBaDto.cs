using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos
{
    public class GetFormInforByIdDanhBaDto
    {
        public int Id { get; set; }
        public string TenFormLoai { get; set; } = String.Empty;
        public string MoTa { get; set; } = string.Empty;
        public DateTime? ThoiGianBatDau { get; set; } 
        public DateTime? ThoiGianKetThuc {  get; set; }
        public List<GetFormDauMucDto> Items { get; set; } = new List<GetFormDauMucDto>(); 
    }

    public class GetFormTruongDataDto
    {
        public int Id { get; set; }
        public string TenTruong { get; set; } = String.Empty;
        public GetFormDataDto Item { get; set; } = new GetFormDataDto(); 
    }
    public class GetFormDauMucDto 
    { 
        public int Id { get; set; }
        public string TenDauMuc { get; set; } = String.Empty ;
        public List<GetFormTruongDataDto> Items { get; set; } = new List<GetFormTruongDataDto>();
    }

    public class GetFormDataDto
    {
        public int Id { get; set; }
        public string Data { get; set; } = string.Empty;

    }

}
