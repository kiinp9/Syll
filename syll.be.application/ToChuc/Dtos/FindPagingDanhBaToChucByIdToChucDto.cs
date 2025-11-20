using syll.be.shared.HttpRequest.BaseRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ToChuc.Dtos
{
    public class FindPagingDanhBaToChucByIdToChucDto : BaseRequestPagingDto
    {
        public int IdToChuc { get; set; }
    }
}
