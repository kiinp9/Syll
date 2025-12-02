using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.Form
{
    public  class DeleteRowTableDataDto
    {
       public List<DeleteTruongDataTable> Truongs {  get; set; } = new List<DeleteTruongDataTable>();
    }

    public class DeleteTruongDataTable
    {
        public int IdTruongData { get; set; }
        public List<DeleteDataTable> Datas { get; set; } = new List<DeleteDataTable>();
    }
    public class DeleteDataTable
    {
        public int IdData { get; set; }
    }
}
