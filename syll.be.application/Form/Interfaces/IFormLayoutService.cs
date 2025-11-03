using syll.be.application.Form.Dtos.FormLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Interfaces
{
    public interface IFormLayoutService
    {
        public void CreateLayout(CreateLayoutDto dto);
        public Task UpdateLayOut(UpdateLayoutDto dto);
        public  Task<ViewLayoutByIdDto> FindLayoutById(int id);
        public void DeleteLayout(int idFormLoai, int idLayout);
        public void CreateBlock(CreateBlockDto dto);
        public  Task UpdateBlock(UpdateBlockDto dto);

        public List<ViewBlockDto> GetBlockByIdLayout(int idLayout);
        public ViewBlockDto GetBlockById(int id);
        public void DeleteBlock(int idLayout, int id);
        public void CreateRow(CreateRowDto dto);
        public  Task UpdateRow(UpdateRowDto dto);
        public void DeleteRow(int idBlock, int idRow);
        public void CreateItem(CreateItemDto dto);
        public  Task UpdateItem(UpdateItemDto dto);
        public void DeleteItem(int idRow, int id);



    }
}
