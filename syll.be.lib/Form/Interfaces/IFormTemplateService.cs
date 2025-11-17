using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.lib.Form.Interfaces
{
    public interface IFormTemplateService
    {
       public  Task<byte[]> ReplaceWordFormTemplate(int idFormLoai);
       public byte[] GenerateSoYeuLyLichTemplate(int idFormLoai);
    }
}
