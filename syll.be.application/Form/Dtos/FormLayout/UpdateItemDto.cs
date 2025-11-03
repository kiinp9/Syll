using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos.FormLayout
{
    public class UpdateItemDto
    {
        public int Id { get; set; }
        public int IdRow { get; set; }
        public int Type { get; set; }
        public int Order {  get; set; }
        public decimal Ratio { get; set; }
    }
}
