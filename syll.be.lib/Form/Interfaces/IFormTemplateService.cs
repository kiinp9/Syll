using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.lib.Form.Interfaces
{
    public interface IFormTemplateService
    {
       // Xuất file docx cho nhân viên
       public Task<byte[]> ReplaceWordFormTemplate(int idFormLoai);
       public byte[] GenerateSoYeuLyLichTemplate(int idFormLoai);
        //Xuất file docx cho admin
       public Task<byte[]> ReplaceWordFormTemplateAdmin(int idFormLoai, int idDanhBa);
    }
}
