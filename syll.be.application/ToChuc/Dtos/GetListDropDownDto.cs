using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ToChuc.Dtos
{
    public class GetListDropDownDto
    {
        public int Id { get; set; }
        public string TenToChuc { get; set; } = String.Empty;
    }
}
