using syll.be.shared.HttpRequest.BaseRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.ChienDich.Dtos
{
    public class FindPagingFormLoaiByIdChienDichDto : BaseRequestPagingDto
    {
        public int IdChienDich { get; set; }
    }
}
