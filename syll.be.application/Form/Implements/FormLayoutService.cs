using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using syll.be.application.Base;
using syll.be.application.Form.Dtos.FormLayout;
using syll.be.application.Form.Interfaces;
using syll.be.infrastructure.data;
using syll.be.shared.Constants.Form;
using syll.be.shared.HttpRequest.AppException;
using syll.be.shared.HttpRequest.Error;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace syll.be.application.Form.Implements
{
    public class FormLayoutService : BaseService, IFormLayoutService
    {
        public FormLayoutService(
            SyllDbContext syllDbContext,
            ILogger<FormLayoutService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ) : base(syllDbContext, logger, httpContextAccessor, mapper) 
        {
        }
            
            
        public void CreateLayout(CreateLayoutDto dto)
        {
            _logger.LogInformation($"{nameof(CreateLayout)}  dto = {JsonSerializer.Serialize(dto)}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == dto.IdFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            var maxOrder = _syllDbContext.Layouts
              .Where(x => x.IdFormLoai == dto.IdFormLoai && !x.Deleted)
              .Max(x => (int?)x.Order) ?? 0;
            var layout = new domain.Form.Layout
            {
                IdFormLoai = dto.IdFormLoai,
                Ten = dto.Ten,
                Order = maxOrder + 1 ,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
            };
            _syllDbContext.Layouts.Add(layout);
            _syllDbContext.SaveChanges();

        } 

        public async Task UpdateLayOut(UpdateLayoutDto dto)
        {
            _logger.LogInformation($"{nameof(CreateLayoutDto)} dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == dto.IdFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
        
            var layouts = await _syllDbContext.Layouts
                .Where(x => x.IdFormLoai == dto.IdFormLoai && !x.Deleted)
                .OrderBy(x => x.Order)
                .ToListAsync();
            var layout = layouts.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorLayoutNotFound);
            var currentOrder = layout.Order;
            var newOrder = dto.Order;
            if (newOrder < 1 || newOrder > layouts.Count)
            {
                throw new UserFriendlyException(ErrorCodes.FormLoaiErrorLayoutOrderInvalid);
            }

            if (currentOrder != newOrder)
            {
                if (newOrder < currentOrder)
                {
                    foreach (var l in layouts)
                    {
                        if (l.Order >= newOrder && l.Order < currentOrder)
                        {
                            l.Order++;
                        }
                    }
                }
                else
                {
                    foreach (var l in layouts)
                    {
                        if (l.Order <= newOrder && l.Order > currentOrder)
                        {
                            l.Order--;
                        }
                    }
                }
            }
            layout.Ten = dto.Ten;
            layout.Order = dto.Order;
            _syllDbContext.Layouts.Update(layout);
            _syllDbContext.SaveChanges();
            
        }

        public async Task<ViewLayoutByIdDto> FindLayoutById(int id)
        {
            _logger.LogInformation($"{nameof(FindLayoutById)} id = {id}");
            var idDanhBa = await GetCurrentDanhBaId();
            var layout = _syllDbContext.Layouts
                .FirstOrDefault(l => l.Id == id && !l.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorLayoutNotFound);
            var blocks = _syllDbContext.Blocks
                .Where(b => b.IdLayout == id && !b.Deleted)
                .OrderBy(b => b.Order)
                .ToList();
            var blockIds = blocks.Select(b => b.Id).ToList();
            var rows = _syllDbContext.Rows
                .Where(r => blockIds.Contains(r.IdBlock) && !r.Deleted)
                .OrderBy(r => r.Order)
                .ToList();
            var rowIds = rows.Select(r => r.Id).ToList();
            var items = _syllDbContext.Items
                .Where(i => rowIds.Contains(i.IdRow) && !i.Deleted)
                .OrderBy(i => i.Order)
                .ToList();
            var itemIds = items.Select(i => i.Id).ToList();
            var formTruongDatas = _syllDbContext.FormTruongDatas
                .Where(f => itemIds.Contains(f.IdItem) && !f.Deleted)
                .ToList();
            var truongDataIds = formTruongDatas.Select(f => f.Id).ToList();
            var formDatas = _syllDbContext.FormDatas
                .Where(fd => truongDataIds.Contains(fd.IdTruongData) && fd.IdDanhBa == idDanhBa && !fd.Deleted)
                .ToList();
            var result = new ViewLayoutByIdDto
            {
                Id = layout.Id,
                IdFormLoai = layout.IdFormLoai,
                Ten = layout.Ten,
                Order = layout.Order,
                Items = blocks.Select(b => new GetBlockDto
                {
                    Id = b.Id,
                    Order = b.Order,
                    Items = rows.Where(r => r.IdBlock == b.Id).Select(r => new GetRowDto
                    {
                        Id = r.Id,
                        Order = r.Order,
                        Items = items.Where(i => i.IdRow == r.Id).Select(i => new GetItemDto
                        {
                            Id = i.Id,
                            Order = i.Order,
                            Type = i.Type,
                            Ratio = i.Ratio,
                            Items = formTruongDatas.Where(f => f.IdItem == i.Id).Select(f =>
                            {
                                var fd = formDatas.FirstOrDefault(fd => fd.IdTruongData == f.Id);
                                return new GetFormTruongData
                                {
                                    Id = f.Id,
                                    TenTruong = f.TenTruong,
                                    Type = f.Type,
                                    Item = fd != null ? new GetFormData
                                    {
                                        Id = fd.Id,
                                        Data = fd.Data,
                                        IndexRowTable = fd.IndexRowTable
                                    } : new GetFormData()
                                };
                            }).ToList()
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
            return result;
        }

        public void DeleteLayout(int idFormLoai, int idLayout)
        {
            _logger.LogInformation($"{nameof(DeleteLayout)} idFormLoai = {idFormLoai} idLayout ={idLayout}");
            var vietnamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            var layout = _syllDbContext.Layouts.FirstOrDefault(x => x.Id == idLayout && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorLayoutNotFound);
            layout.Deleted = true;
            layout.DeletedBy = currentUserId;
            layout.DeletedDate = vietnamNow;

            _syllDbContext.Layouts.Update(layout);
            _syllDbContext.SaveChanges();
        }

        public void CreateBlock (CreateBlockDto dto)
        {
            _logger.LogInformation($"{nameof(CreateBlock)} dto = {JsonSerializer.Serialize(dto)}");
            var vietamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var maxOrder = _syllDbContext.Blocks
                .Where(x => x.IdLayout == dto.IdLayout && !x.Deleted)
                .Max(x => (int?)x.Order) ?? 0;
            var block = new domain.Form.Block
            {
                IdLayout = dto.IdLayout,
                Order = maxOrder + 1,
                CreatedDate = vietamNow,
                CreatedBy = currentUserId,
            };
            _syllDbContext.Blocks.Add(block);
            _syllDbContext.SaveChanges();
        }

        public async Task UpdateBlock(UpdateBlockDto dto) {
            _logger.LogInformation($"{nameof(UpdateBlock)} dto = {JsonSerializer.Serialize(dto)}");
            var vietamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var layout = await _syllDbContext.Blocks.FirstOrDefaultAsync(x => x.Id == dto.IdLayout && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorLayoutNotFound);
            var blocks = await _syllDbContext.Blocks
                .Where(x => x.IdLayout == dto.IdLayout && !x.Deleted)
                .OrderBy(x => x.Order)
                .ToListAsync();
            var block = blocks.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorBlockNotFound);
            var currentOrder = block.Order;
            var newOrder = dto.Order;
            if (newOrder < 1 || newOrder > blocks.Count)
            {
                throw new UserFriendlyException(ErrorCodes.FormLoaiErrorBlockOrderInvalid);
            }

            if (currentOrder != newOrder)
            {
                if (newOrder < currentOrder)
                {
                    foreach (var b in blocks)
                    {
                        if (b.Order >= newOrder && b.Order < currentOrder)
                        {
                            b.Order++;
                        }
                    }
                }
                else
                {
                    foreach (var b in blocks)
                    {
                        if (b.Order <= newOrder && b.Order > currentOrder)
                        {
                            b.Order--;
                        }
                    }
                }
            }
            block.Order = dto.Order;

            await _syllDbContext.SaveChangesAsync();
        }

        public List<ViewBlockDto> GetBlockByIdLayout (int idLayout)
        {
            _logger.LogInformation($"{nameof(GetBlockByIdLayout)}  idLayout= {idLayout}");
            var blocks = (from b in _syllDbContext.Blocks
                         where b.IdLayout == idLayout
                         && !b.Deleted
                         orderby b.Order
                         select new ViewBlockDto
                         {
                             Id = b.Id,
                             IdLayout = idLayout,
                             Order = b.Order,
                         }).ToList();

            return blocks;
        }

        public ViewBlockDto GetBlockById(int id)
        {
            _logger.LogInformation($"{nameof(GetBlockById)}  id= {id}");
            var block = (from b in _syllDbContext.Blocks
                          where b.Id == id
                          && !b.Deleted
                          select new ViewBlockDto
                          {
                              Id = b.Id,
                              IdLayout = b.IdLayout,
                              Order = b.Order,
                          }).FirstOrDefault()
                          ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorBlockNotFound);

            return block;
        }

        public void DeleteBlock (int idLayout, int id)
        {
            _logger.LogInformation($"{nameof(DeleteBlock)}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var layout = _syllDbContext.Layouts.FirstOrDefault(x => x.Id == idLayout && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorLayoutNotFound);
            var block = _syllDbContext.Blocks.FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorBlockNotFound);
            block.Deleted = true;
            block.DeletedBy = currentUserId;
            block.DeletedDate = vietNamNow;

            _syllDbContext.Blocks.Update(block);
            _syllDbContext.SaveChanges();
        } 


        public void CreateRow (CreateRowDto dto)
        {
            _logger.LogInformation($"{nameof(CreateRow)} dto = {JsonSerializer.Serialize(dto)}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var block = _syllDbContext.Blocks.FirstOrDefault( x => x.Id == dto.IdBlock && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorBlockNotFound);
            var maxOrder = _syllDbContext.Rows
                .Where(x => x.IdBlock == dto.IdBlock && !x.Deleted)
                .Max(x => (int?)x.Order) ?? 0;
            var row = new domain.Form.Row
            {
                IdBlock = dto.IdBlock,
                Order = maxOrder + 1,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
            };
            _syllDbContext.Rows.Add(row);
            _syllDbContext.SaveChanges();


        }



        public async Task UpdateRow(UpdateRowDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateBlock)} dto = {JsonSerializer.Serialize(dto)}");
            var block = _syllDbContext.Blocks.FirstOrDefault(x => x.Id == dto.IdBlock && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorBlockNotFound);

            var rows = await _syllDbContext.Rows
                .Where(x => x.IdBlock == dto.IdBlock && !x.Deleted)
                .OrderBy(x => x.Order)
                .ToListAsync();
            var row = rows.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorRowNotFound);

            var currentOrder = row.Order;
            var newOrder = dto.Order;
            if (newOrder < 1 || newOrder > rows.Count)
            {
                throw new UserFriendlyException(ErrorCodes.FormLoaiErrorRowOrderInvalid);
            }

            if (currentOrder != newOrder)
            {
                if (newOrder < currentOrder)
                {
                    foreach (var r in rows)
                    {
                        if (r.Order >= newOrder && r.Order < currentOrder)
                        {
                            r.Order++;
                        }
                    }
                }
                else
                {
                    foreach (var r in rows)
                    {
                        if (r.Order <= newOrder && r.Order > currentOrder)
                        {
                            r.Order--;
                        }
                    }
                }

            }
            row.Order = dto.Order;
            await _syllDbContext.SaveChangesAsync();

        }

        public void DeleteRow (int idBlock, int idRow)
        {
            _logger.LogInformation($"{nameof(DeleteRow)}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId(); 
            var block = _syllDbContext.Blocks.FirstOrDefault(x => x.Id == idBlock && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorBlockNotFound);

            var row = _syllDbContext.Rows.FirstOrDefault(x => x.Id == idRow && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorRowNotFound);
            row.Deleted = true;
            row.DeletedBy = currentUserId;
            row.DeletedDate = vietNamNow;
            _syllDbContext.Rows.Update(row);
            _syllDbContext.SaveChangesAsync();
        }


        public void CreateItem(CreateItemDto dto)
        {
            _logger.LogInformation($"{nameof(CreateItem)} dto = {JsonSerializer.Serialize(dto)}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var row = _syllDbContext.Rows.FirstOrDefault(x => x.Id == dto.IdRow && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorRowNotFound);
            var maxOrder = _syllDbContext.Items
               .Where(x => x.IdRow == dto.IdRow && !x.Deleted)
               .Max(x => (int?)x.Order) ?? 0;
            var item = new domain.Form.Item
            {
                IdRow = dto.IdRow,
                Order = maxOrder + 1,
                Type = dto.Type,
                Ratio = dto.Ratio,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
            };
            _syllDbContext.Items.Add(item);
            _syllDbContext.SaveChangesAsync();
        }

        public async Task UpdateItem(UpdateItemDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateItem)} dto = {JsonSerializer.Serialize(dto)}");
            var row = _syllDbContext.Rows.FirstOrDefault(x => x.Id == dto.IdRow && !x.Deleted)
               ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorRowNotFound);

            var items = await _syllDbContext.Items
                .Where(x => x.IdRow == dto.IdRow && !x.Deleted)
                .OrderBy(x => x.Order)
                .ToListAsync();
            var item = items.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorItemNotFound);

            var currentOrder = item.Order;
            var newOrder = dto.Order;
            if (newOrder < 1 || newOrder > items.Count)
            {
                throw new UserFriendlyException(ErrorCodes.FormLoaiErrorItemOrderInvalid);
            }

            if (currentOrder != newOrder)
            {
                if (newOrder < currentOrder)
                {
                    foreach (var it in items)
                    {
                        if (it.Order >= newOrder && it.Order < currentOrder)
                        {
                            it.Order++;
                        }
                    }
                }
                else
                {
                    foreach (var it in items)
                    {
                        if (it.Order <= newOrder && it.Order > currentOrder)
                        {
                            it.Order--;
                        }
                    }
                }

            }
            if(dto.Type != ItemConstants.InputText ||  dto.Type != ItemConstants.DropDownText || dto.Type != ItemConstants.Table || dto.Type != ItemConstants.CheckBox || dto.Type != ItemConstants.DropDownDate || dto.Type != ItemConstants.ItemText)
            {
                throw new UserFriendlyException(ErrorCodes.FormLoaiErrorItemTypeInvalid);
            }
            item.Order = dto.Order;
            item.Type = dto.Type;
            item.Ratio = dto.Ratio;
            await _syllDbContext.SaveChangesAsync();

        }


        public void DeleteItem(int idRow, int id)
        {
            _logger.LogInformation($"{nameof(DeleteItem)}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var row = _syllDbContext.Rows.FirstOrDefault(x => x.Id == idRow && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorRowNotFound);

            var item = _syllDbContext.Items.FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorItemNotFound);
            row.Deleted = true;
            row.DeletedBy = currentUserId;
            row.DeletedDate = vietNamNow;
            _syllDbContext.Items.Update(item);
            _syllDbContext.SaveChangesAsync();
        }
    }

}

