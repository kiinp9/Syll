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
        public string Style { get; set; } = String.Empty;
        public string Class { get; set; } = String.Empty;
        public int Order { get; set; }

        public List<GetBlockDto> Items { get; set; } = new List<GetBlockDto>();
    }

    public class GetBlockDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Style { get; set; } = String.Empty;
        public string Class { get; set; } = String.Empty;
        public List<GetRowDto> Items { get; set; } = new List<GetRowDto>();
    }

    public class GetRowDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Style { get; set; } = String.Empty;
        public string Class { get; set; } = String.Empty;
        public List<GetItemDto> Items { get; set; } = new List<GetItemDto>();
    }

    public class GetItemDto
    {
        public int Id { get; set; }
        public string InputName { get; set; } = String.Empty;
        public int Order { get; set; }
        public int Type { get; set; }
        public string Style { get; set; } = String.Empty;
        public string Class { get; set; } = String.Empty;
        public decimal Ratio { get; set; }
        public List<GetFormTruongData> Items { get; set; } = new List<GetFormTruongData>();
        public List<GetTableHeader?> Headers { get; set; } = new List<GetTableHeader?>();
    }

    public class GetFormTruongData
    {
        public int Id { get; set; }
        public string TenTruong { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public GetFormData Item { get; set; } = new GetFormData();

        public List<GetDropDownData?> Items { get; set; } = new List<GetDropDownData?>();

    }

    public class GetFormData
    {
        public int Id { get; set; }
        public string Data { get; set; } = String.Empty;
        public int? IndexRowTable { get; set; }
    }


    public class GetDropDownData
    {
        public int Id { get; set; }
        public string Data { get; set; } = String.Empty;
        public int Order { get; set; }
        public string Class { get;set; } = String.Empty;
        public string Style { get; set; } = String.Empty;
    }


    public class GetTableHeader
    {
        public int Id { get; set; }
        public string Data { get; set; } = String.Empty;
        public int Order { get; set; }
        public decimal Ratio { get; set; }
        public string Type { get; set; } = String.Empty;
        public string Class { get; set; } = String.Empty;
    }
}
