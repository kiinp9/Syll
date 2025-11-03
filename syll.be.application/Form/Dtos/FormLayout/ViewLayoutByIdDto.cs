using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.FormLayout
{
    public class ViewLayoutByIdDto
    {
        public int Id { get; set; }
        public int IdFormLoai { get; set; }
        public string Ten { get; set; } = String.Empty;
        public int Order { get; set; }

        public List<GetBlockDto> Items { get; set; } = new List<GetBlockDto>();
    }

    public class GetBlockDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public List<GetRowDto> Items { get; set; } = new List<GetRowDto>();
    }

    public class GetRowDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public List<GetItemDto> Items { get; set; } = new List<GetItemDto>();
    }

    public class GetItemDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public int Type { get; set; }
        public decimal Ratio { get; set; }
        public List<GetFormTruongData> Items { get; set; } = new List<GetFormTruongData>();    
    }

    public class GetFormTruongData
    {
        public int Id { get; set; }
        public string TenTruong { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public List<GetFormData> Items { get; set; } = new List<GetFormData>();

    }

    public class GetFormData
    {
        public int Id { get; set; }
        public string Data { get; set; } = String.Empty;
        public int? IndexRowTable { get; set; }
    }
}
