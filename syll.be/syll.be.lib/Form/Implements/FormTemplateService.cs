using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using syll.be.domain.Form;
using syll.be.infrastructure.data;
using syll.be.lib.Form.Dtos;
using syll.be.lib.Form.Interfaces;
using syll.be.shared.Constants.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using syll.be.shared.HttpRequest.AppException;
using syll.be.shared.HttpRequest.Error;
using syll.be.infrastructure.data.Migrations;

namespace syll.be.lib.Form.Implements
{
    public class FormTemplateService :BaseService, IFormTemplateService
    {
        public FormTemplateService(
            SyllDbContext syllDbContext,
            ILogger<FormTemplateService> logger,
            IHttpContextAccessor httpContextAccessor

        ) : base(syllDbContext, logger, httpContextAccessor)
        {

        }

        // Xuất file docx cho nhân viên
        public async Task<byte[]> ReplaceWordFormTemplate(int idFormLoai)
        {
            _logger.LogInformation($"{nameof(ReplaceWordFormTemplate)} idFormLoai = {idFormLoai}");
            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            var idDanhBa = await GetCurrentDanhBaId();

            var formDataList = await _syllDbContext.FormDatas
                .Where(x => x.IdFormLoai == idFormLoai && x.IdDanhBa == idDanhBa && !x.Deleted)
                .ToListAsync();

            var truongDataIds = formDataList.Select(x => x.IdTruongData).Distinct().ToList();

            var formTruongDataList = await _syllDbContext.FormTruongDatas
                .Where(x => truongDataIds.Contains(x.Id) && x.IdFormLoai == idFormLoai && !x.Deleted)
                .ToListAsync();

            var truongNhaOList = formTruongDataList
                .Where(x => x.BlockTruongNhanBan == TruongDataConstants.NhaO)
                .OrderBy(x => x.IndexInTemplate)
                .ToList();

            var truongDatOList = formTruongDataList
                .Where(x => x.BlockTruongNhanBan == TruongDataConstants.DatO)
                .OrderBy(x => x.IndexInTemplate)
                .ToList();

            int cloneCountNhaO = CalculateCloneCount(formDataList, truongNhaOList);
            int cloneCountDatO = CalculateCloneCount(formDataList, truongDatOList);
            //idFormLoai = 1;
            if (idFormLoai != 1)
            {
                throw new UserFriendlyException(ErrorCodes.TemplateErrorTemplateFormLoaiNotFound);
            }
            var templateBytes = GenerateSoYeuLyLichTemplate(idFormLoai);

            using var memoryStream = new MemoryStream();
            memoryStream.Write(templateBytes, 0, templateBytes.Length);
            memoryStream.Position = 0;

            using (var document = WordprocessingDocument.Open(memoryStream, true))
            {
                var body = document.MainDocumentPart.Document.Body;

                CloneFieldsForBlock(body, truongNhaOList, cloneCountNhaO, "Nhà ở");
                CloneFieldsForBlock(body, truongDatOList, cloneCountDatO, "Đất ở");

                var truongNhanBanIds = truongNhaOList.Select(x => x.Id)
                    .Concat(truongDatOList.Select(x => x.Id))
                    .ToList();

                var rowDataList = formDataList
                    .Where(x => !truongNhanBanIds.Contains(x.IdTruongData))
                    .ToList();

                foreach (var formData in rowDataList)
                {
                    var truongData = formTruongDataList.FirstOrDefault(x => x.Id == formData.IdTruongData);
                    if (truongData == null) continue;

                    int index = truongData.IndexInTemplate;
                    string data = formData.Data;

                    if (DateTime.TryParse(data, out DateTime dateValue))
                    {
                        if (index == 9)
                        {
                            ReplaceSpecialBirthdayField(body, dateValue);
                            continue;
                        }
                        else
                        {
                            data = dateValue.ToString("dd/MM/yyyy");
                        }
                    }

                    ReplaceFieldInDocument(body, index, data);
                }

                ReplaceBlockTruongNhanBanData(body, formDataList, truongNhaOList, "Nhà ở");
                ReplaceBlockTruongNhanBanData(body, formDataList, truongDatOList, "Đất ở");

                var tableItems = await _syllDbContext.Items
                    .Where(item => item.Type == 5 && !item.Deleted &&
                        _syllDbContext.Rows.Any(row => row.Id == item.IdRow && !row.Deleted &&
                            _syllDbContext.Blocks.Any(block => block.Id == row.IdBlock && !block.Deleted &&
                                _syllDbContext.Layouts.Any(layout => layout.Id == block.IdLayout &&
                                    layout.IdFormLoai == idFormLoai && !layout.Deleted))))
                    .OrderBy(item => item.Id)
                    .ToListAsync();

                var itemToTableIndexMap = new Dictionary<int, int>();
                for (int i = 0; i < tableItems.Count; i++)
                {
                    itemToTableIndexMap[tableItems[i].Id] = i + 1;
                }

                var tableItemIds = tableItems.Select(x => x.Id).ToList();

                var tableRecords = await _syllDbContext.Tables
                    .Where(x => tableItemIds.Contains(x.IdItem) && !x.Deleted)
                    .OrderBy(x => x.IdItem).ThenBy(x => x.Order)
                    .ToListAsync();

                var tableTruongDataIds = tableRecords.Select(x => x.IdTruongData).Distinct().ToList();

                var tableFormDataList = await _syllDbContext.FormDatas
                    .Where(x => tableTruongDataIds.Contains(x.IdTruongData) &&
                        x.IdDanhBa == idDanhBa &&
                        x.IndexRowTable.HasValue &&
                        !x.Deleted)
                    .ToListAsync();

                var tableDataGroups = tableFormDataList
                    .GroupBy(x => x.IdTruongData)
                    .ToList();

                foreach (var group in tableDataGroups)
                {
                    var tableRecord = tableRecords.FirstOrDefault(x => x.IdTruongData == group.Key);
                    if (tableRecord == null) continue;

                    int itemId = tableRecord.IdItem;
                    int columnIndex = tableRecord.Order;

                    var rowGroups = group.GroupBy(x => x.IndexRowTable.Value).OrderBy(x => x.Key);

                    foreach (var rowGroup in rowGroups)
                    {
                        int rowIndex = rowGroup.Key;

                        foreach (var formData in rowGroup)
                        {
                            string data = formData.Data;
                            ReplaceFieldInTable(body, columnIndex, rowIndex, data, itemId, itemToTableIndexMap);
                        }
                    }
                }

                document.MainDocumentPart.Document.Save();
            }

            return memoryStream.ToArray();
        }

        //Xuất file docx cho admin
        public async Task<byte[]> ReplaceWordFormTemplateAdmin(int idFormLoai,int idDanhBa)
        {
            _logger.LogInformation($"{nameof(ReplaceWordFormTemplate)} idFormLoai = {idFormLoai}");
            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            

            var formDataList = await _syllDbContext.FormDatas
                .Where(x => x.IdFormLoai == idFormLoai && x.IdDanhBa == idDanhBa && !x.Deleted)
                .ToListAsync();

            var truongDataIds = formDataList.Select(x => x.IdTruongData).Distinct().ToList();

            var formTruongDataList = await _syllDbContext.FormTruongDatas
                .Where(x => truongDataIds.Contains(x.Id) && x.IdFormLoai == idFormLoai && !x.Deleted)
                .ToListAsync();

            var truongNhaOList = formTruongDataList
                .Where(x => x.BlockTruongNhanBan == TruongDataConstants.NhaO)
                .OrderBy(x => x.IndexInTemplate)
                .ToList();

            var truongDatOList = formTruongDataList
                .Where(x => x.BlockTruongNhanBan == TruongDataConstants.DatO)
                .OrderBy(x => x.IndexInTemplate)
                .ToList();

            int cloneCountNhaO = CalculateCloneCount(formDataList, truongNhaOList);
            int cloneCountDatO = CalculateCloneCount(formDataList, truongDatOList);
            //idFormLoai = 1;
            if (idFormLoai != 1)
            {
                throw new UserFriendlyException(ErrorCodes.TemplateErrorTemplateFormLoaiNotFound);
            }
            var templateBytes = GenerateSoYeuLyLichTemplate(idFormLoai);

            using var memoryStream = new MemoryStream();
            memoryStream.Write(templateBytes, 0, templateBytes.Length);
            memoryStream.Position = 0;

            using (var document = WordprocessingDocument.Open(memoryStream, true))
            {
                var body = document.MainDocumentPart.Document.Body;

                CloneFieldsForBlock(body, truongNhaOList, cloneCountNhaO, "Nhà ở");
                CloneFieldsForBlock(body, truongDatOList, cloneCountDatO, "Đất ở");

                var truongNhanBanIds = truongNhaOList.Select(x => x.Id)
                    .Concat(truongDatOList.Select(x => x.Id))
                    .ToList();

                var rowDataList = formDataList
                    .Where(x => !truongNhanBanIds.Contains(x.IdTruongData))
                    .ToList();

                foreach (var formData in rowDataList)
                {
                    var truongData = formTruongDataList.FirstOrDefault(x => x.Id == formData.IdTruongData);
                    if (truongData == null) continue;

                    int index = truongData.IndexInTemplate;
                    string data = formData.Data;

                    if (DateTime.TryParse(data, out DateTime dateValue))
                    {
                        if (index == 9)
                        {
                            ReplaceSpecialBirthdayField(body, dateValue);
                            continue;
                        }
                        else
                        {
                            data = dateValue.ToString("dd/MM/yyyy");
                        }
                    }

                    ReplaceFieldInDocument(body, index, data);
                }

                ReplaceBlockTruongNhanBanData(body, formDataList, truongNhaOList, "Nhà ở");
                ReplaceBlockTruongNhanBanData(body, formDataList, truongDatOList, "Đất ở");

                var tableItems = await _syllDbContext.Items
                    .Where(item => item.Type == 5 && !item.Deleted &&
                        _syllDbContext.Rows.Any(row => row.Id == item.IdRow && !row.Deleted &&
                            _syllDbContext.Blocks.Any(block => block.Id == row.IdBlock && !block.Deleted &&
                                _syllDbContext.Layouts.Any(layout => layout.Id == block.IdLayout &&
                                    layout.IdFormLoai == idFormLoai && !layout.Deleted))))
                    .OrderBy(item => item.Id)
                    .ToListAsync();

                var itemToTableIndexMap = new Dictionary<int, int>();
                for (int i = 0; i < tableItems.Count; i++)
                {
                    itemToTableIndexMap[tableItems[i].Id] = i + 1;
                }

                var tableItemIds = tableItems.Select(x => x.Id).ToList();

                var tableRecords = await _syllDbContext.Tables
                    .Where(x => tableItemIds.Contains(x.IdItem) && !x.Deleted)
                    .OrderBy(x => x.IdItem).ThenBy(x => x.Order)
                    .ToListAsync();

                var tableTruongDataIds = tableRecords.Select(x => x.IdTruongData).Distinct().ToList();

                var tableFormDataList = await _syllDbContext.FormDatas
                    .Where(x => tableTruongDataIds.Contains(x.IdTruongData) &&
                        x.IdDanhBa == idDanhBa &&
                        x.IndexRowTable.HasValue &&
                        !x.Deleted)
                    .ToListAsync();

                var tableDataGroups = tableFormDataList
                    .GroupBy(x => x.IdTruongData)
                    .ToList();

                foreach (var group in tableDataGroups)
                {
                    var tableRecord = tableRecords.FirstOrDefault(x => x.IdTruongData == group.Key);
                    if (tableRecord == null) continue;

                    int itemId = tableRecord.IdItem;
                    int columnIndex = tableRecord.Order;

                    var rowGroups = group.GroupBy(x => x.IndexRowTable.Value).OrderBy(x => x.Key);

                    foreach (var rowGroup in rowGroups)
                    {
                        int rowIndex = rowGroup.Key;

                        foreach (var formData in rowGroup)
                        {
                            string data = formData.Data;
                            ReplaceFieldInTable(body, columnIndex, rowIndex, data, itemId, itemToTableIndexMap);
                        }
                    }
                }

                document.MainDocumentPart.Document.Save();
            }

            return memoryStream.ToArray();
        }


        private int CalculateCloneCount(List<FormData> formDataList, List<FormTruongData> truongList)
        {
            if (!truongList.Any()) return 0;

            var truongIds = truongList.Select(x => x.Id).ToList();

            var maxIndex = formDataList
                .Where(x => truongIds.Contains(x.IdTruongData) && x.IndexRowTable.HasValue)
                .Select(x => x.IndexRowTable.Value)
                .DefaultIfEmpty(0)
                .Max();

            return maxIndex > 0 ? maxIndex - 1 : 0;
        }

        private (int startIndex, int endIndex) GetBlockParagraphRange(
            List<Paragraph> paragraphs,
            List<FormTruongData> truongList)
        {
            int startIndex = -1;
            int endIndex = -1;

            var firstPattern = GetFieldPatternByIndex(truongList.OrderBy(x => x.IndexInTemplate).First().IndexInTemplate);
            var lastPattern = GetFieldPatternByIndex(truongList.OrderBy(x => x.IndexInTemplate).Last().IndexInTemplate);

            for (int i = 0; i < paragraphs.Count; i++)
            {
                var text = paragraphs[i].InnerText;

                if (startIndex == -1 && text.Contains(firstPattern))
                {
                    startIndex = i;
                }

                if (startIndex != -1 && text.Contains(lastPattern))
                {
                    endIndex = i;
                    break;
                }
            }

            return (startIndex, endIndex);
        }

        private void CloneFieldsForBlock(Body body, List<FormTruongData> truongList, int cloneCount, string blockName)
        {
            if (cloneCount == 0 || !truongList.Any())
            {
                return;
            }

            var allParagraphs = body.Elements<Paragraph>().ToList();

            var (startIndex, endIndex) = GetBlockParagraphRange(allParagraphs, truongList);

            if (startIndex == -1 || endIndex == -1)
            {
                return;
            }

            var blockParagraphs = new List<Paragraph>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                blockParagraphs.Add(allParagraphs[i]);
            }

            var lastParagraph = blockParagraphs.Last();

            for (int clone = 0; clone < cloneCount; clone++)
            {
                var insertAfter = lastParagraph;

                foreach (var paragraph in blockParagraphs)
                {
                    var clonedParagraph = (Paragraph)paragraph.CloneNode(true);
                    body.InsertAfter(clonedParagraph, insertAfter);
                    insertAfter = clonedParagraph;
                }

                lastParagraph = insertAfter;
            }
        }

        private void ReplaceBlockTruongNhanBanData(Body body, List<FormData> formDataList,
            List<FormTruongData> truongList, string blockName)
        {
            if (!truongList.Any())
            {
                return;
            }

            var truongIds = truongList.Select(x => x.Id).ToList();

            var blockDataList = formDataList
                .Where(x => truongIds.Contains(x.IdTruongData) && x.IndexRowTable.HasValue)
                .GroupBy(x => x.IndexRowTable.Value)
                .OrderBy(g => g.Key)
                .ToList();

            var allParagraphs = body.Elements<Paragraph>().ToList();

            int totalBlocks = blockDataList.Any() ? blockDataList.Max(g => g.Key) : 1;

            foreach (var group in blockDataList)
            {
                int indexRowTable = group.Key;

                foreach (var formData in group)
                {
                    var truongData = truongList.FirstOrDefault(x => x.Id == formData.IdTruongData);
                    if (truongData == null) continue;

                    string data = formData.Data;
                    if (DateTime.TryParse(data, out DateTime dateValue))
                    {
                        data = dateValue.ToString("dd/MM/yyyy");
                    }

                    ReplaceFieldOccurrenceInBlock(allParagraphs, truongData.IndexInTemplate, data, indexRowTable, blockName);
                }
            }
        }

        private void ReplaceFieldOccurrenceInBlock(List<Paragraph> paragraphs, int fieldIndex,
            string data, int occurrence, string blockName)
        {
            if (string.IsNullOrEmpty(data)) return;

            var fieldPattern = GetFieldPatternByIndex(fieldIndex);
            if (string.IsNullOrEmpty(fieldPattern)) return;

            int foundCount = 0;
            bool replaced = false;

            foreach (var paragraph in paragraphs)
            {
                if (replaced) break;

                var fullText = paragraph.InnerText;
                if (!fullText.Contains(fieldPattern)) continue;

                foundCount++;

                if (foundCount != occurrence) continue;

                var runs = paragraph.Elements<Run>().ToList();

                foreach (var run in runs)
                {
                    if (replaced) break;

                    var textElement = run.Elements<Text>().FirstOrDefault();
                    if (textElement == null) continue;

                    string text = textElement.Text;
                    if (!text.Contains(fieldPattern)) continue;

                    int patternIndex = text.IndexOf(fieldPattern);
                    int afterPatternIndex = patternIndex + fieldPattern.Length;

                    if (afterPatternIndex >= text.Length) continue;

                    var remainingText = text.Substring(afterPatternIndex);

                    var dotsPattern = @"^[:\s]*(\.{2,}[\s/]*)+";
                    var dotsMatch = System.Text.RegularExpressions.Regex.Match(remainingText, dotsPattern);

                    if (dotsMatch.Success)
                    {
                        string beforePattern = text.Substring(0, afterPatternIndex);
                        string afterAllDots = remainingText.Substring(dotsMatch.Length);

                        afterAllDots = System.Text.RegularExpressions.Regex.Replace(
                            afterAllDots,
                            @"^[\s/\.]*(\.{2,}[\s/]*)*",
                            ""
                        );

                        var trimmedAfter = afterAllDots.TrimStart();

                        var units = new[] { "%", "m2", "m²", "kg", "cm", "vnđ", "đồng", "VNĐ" };
                        bool isUnitCase = units.Any(unit => trimmedAfter.StartsWith(unit));

                        string spacer;
                        if (string.IsNullOrWhiteSpace(afterAllDots))
                        {
                            spacer = "";
                        }
                        else if (isUnitCase)
                        {
                            spacer = "";

                            foreach (var unit in units)
                            {
                                if (trimmedAfter.StartsWith(unit))
                                {
                                    var unitPattern = $@"^(\s*){System.Text.RegularExpressions.Regex.Escape(unit)}(\s*)";
                                    afterAllDots = System.Text.RegularExpressions.Regex.Replace(
                                        afterAllDots,
                                        unitPattern,
                                        $"{unit}    "
                                    );
                                    break;
                                }
                            }
                        }
                        else
                        {
                            spacer = "    ";
                        }

                        string newText = beforePattern + " " + data + spacer + afterAllDots;

                        textElement.Text = newText;
                        textElement.Space = SpaceProcessingModeValues.Preserve;
                        EnsureRunFormatting(run);

                        replaced = true;
                    }
                }
            }
        }

        private void ReplaceSpecialBirthdayField(Body body, DateTime dateValue)
        {
            foreach (var paragraph in body.Descendants<Paragraph>())
            {
                var fullText = paragraph.InnerText;
                if (fullText.Contains("3) Sinh ngày:") && fullText.Contains("tháng") && fullText.Contains("năm"))
                {
                    var runs = paragraph.Elements<Run>().ToList();

                    foreach (var run in runs)
                    {
                        var textElement = run.Elements<Text>().FirstOrDefault();
                        if (textElement == null) continue;

                        string text = textElement.Text;

                        var pattern = @"(\.{2,})\s*tháng\s*(\.{2,})\s*năm\s*(\.{2,})";
                        var regex = new System.Text.RegularExpressions.Regex(pattern);

                        if (regex.IsMatch(text))
                        {
                            text = regex.Replace(text, $"{dateValue.Day:00} tháng {dateValue.Month:00} năm {dateValue.Year}", 1);
                            textElement.Text = text;
                            textElement.Space = SpaceProcessingModeValues.Preserve;
                            EnsureRunFormatting(run);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void ReplaceFieldInDocument(Body body, int index, string data)
        {
            if (string.IsNullOrEmpty(data))
                return;

            var fieldPattern = GetFieldPatternByIndex(index);
            if (string.IsNullOrEmpty(fieldPattern))
                return;

            bool replaced = false;

            foreach (var paragraph in body.Descendants<Paragraph>())
            {
                if (replaced) break;

                var fullText = paragraph.InnerText;

                if (!fullText.Contains(fieldPattern))
                    continue;

                var runs = paragraph.Elements<Run>().ToList();

                for (int i = 0; i < runs.Count; i++)
                {
                    if (replaced) break;

                    var run = runs[i];
                    var textElement = run.Elements<Text>().FirstOrDefault();
                    if (textElement == null) continue;

                    string text = textElement.Text;

                    if (!text.Contains(fieldPattern))
                        continue;

                    int patternIndex = text.IndexOf(fieldPattern);
                    int afterPatternIndex = patternIndex + fieldPattern.Length;

                    if (afterPatternIndex >= text.Length)
                        continue;

                    var remainingText = text.Substring(afterPatternIndex);

                    var dotsPattern = @"^[:\s]*(\.{2,}[\s/]*)+";
                    var dotsMatch = System.Text.RegularExpressions.Regex.Match(remainingText, dotsPattern);

                    if (dotsMatch.Success)
                    {
                        string beforePattern = text.Substring(0, afterPatternIndex);
                        string afterAllDots = remainingText.Substring(dotsMatch.Length);

                        afterAllDots = System.Text.RegularExpressions.Regex.Replace(
                            afterAllDots,
                            @"^[\s/\.]*(\.{2,}[\s/]*)*",
                            ""
                        );

                        var trimmedAfter = afterAllDots.TrimStart();

                        var units = new[] { "%", "m2", "m²", "kg", "cm", "vnđ", "đồng", "VNĐ" };
                        bool isUnitCase = units.Any(unit => trimmedAfter.StartsWith(unit));

                        string spacer;
                        if (string.IsNullOrWhiteSpace(afterAllDots))
                        {
                            spacer = "";
                        }
                        else if (isUnitCase)
                        {
                            spacer = "";

                            foreach (var unit in units)
                            {
                                if (trimmedAfter.StartsWith(unit))
                                {
                                    var unitPattern = $@"^(\s*){System.Text.RegularExpressions.Regex.Escape(unit)}(\s*)";
                                    afterAllDots = System.Text.RegularExpressions.Regex.Replace(
                                        afterAllDots,
                                        unitPattern,
                                        $"{unit}    "
                                    );
                                    break;
                                }
                            }
                        }
                        else
                        {
                            spacer = "    ";
                        }

                        string newText;
                        if (data.Length > 50)
                        {
                            newText = beforePattern + "\n" + data + spacer + afterAllDots;
                        }
                        else
                        {
                            newText = beforePattern + " " + data + spacer + afterAllDots;
                        }

                        textElement.Text = newText;
                        textElement.Space = SpaceProcessingModeValues.Preserve;
                        EnsureRunFormatting(run);

                        replaced = true;
                    }
                }
            }
        }

        private string GetFieldPatternByIndex(int index)
        {
            var patterns = new Dictionary<int, string>
    {
        { 1, "Cơ quan quản lý viên chức:" },
        { 2, "Đơn vị sử dụng viên chức:" },
        { 3, "Số hiệu:" },
        { 4, "Mã số định danh:" },
        { 5, "SƠ YẾU LÝ LỊCH" },
        { 6, "1) Họ và tên khai sinh (viết chữ in hoa):" },
        { 7, "Giới tính:" },
        { 8, "2) Các tên gọi khác:" },
        { 9, "3) Sinh ngày:" },
        { 10, "4) Nơi sinh:" },
        { 11, "5) Quê quán (xã, phường):" },
        { 12, "tỉnh, TP):" },
        { 13, "6) Dân tộc:" },
        { 14, "7) Tôn giáo:" },
        { 15, "8) Số CCCD:" },
        { 16, "Ngày cấp:" },
        { 17, "SĐT liên hệ:" },
        { 18, "9) Số BHXH:" },
        { 19, "Số thẻ BHYT:" },
        { 20, "10) Nơi ở hiện nay:" },
        { 21, "11) Thành phần gia đình xuất thân:" },
        { 22, "12) Nghề nghiệp trước khi được tuyển dụng:" },
        { 23, "13) Ngày được tuyển dụng lần đầu:" },
        { 24, "Cơ quan, tổ chức, đơn vị tuyển dụng:" },
        { 25, "14) Ngày vào cơ quan hiện đang công tác:" },
        { 26, "15) Ngày vào Đảng Cộng sản Việt Nam:" },
        { 27, "Ngày chính thức:" },
        { 28, "16) Ngày tham gia tổ chức chính trị-xã hội đầu tiên" },
        { 29, "17) Ngày nhập ngũ:" },
        { 30, "Ngày xuất ngũ:" },
        { 31, "Quân hàm cao nhất:" },
        { 32, "18) Đối tượng chính sách:" },
        { 33, "19) Trình độ giáo dục phổ thông" },
        { 34, "20) Trình độ chuyên môn cao nhất:" },
        { 35, "21) Học hàm:" },
        { 36, "22) Danh hiệu nhà nước phong tặng:" },
        { 37, "23) Chức vụ hiện tại:" },
        { 38, "Ngày bổ nhiệm/ngày phê chuẩn:" },
        { 39, "Ngày bổ nhiệm lại/phê chuẩn nhiệm kỳ tiếp theo:" },
        { 40, "24) Được quy hoạch chức danh:" },
        { 41, "25) Chức vụ kiêm nhiệm:" },
        { 42, "26) Chức vụ Đảng hiện tại:" },
        { 43, "27) Chức vụ Đảng kiêm nhiệm:" },
        { 44, "28) Công việc chính được giao:" },
        { 45, "29) Sở trường công tác:" },
        { 46, "Công việc làm lâu nhất" },
        { 47, "30) Tiền lương" },
        { 48, "30.1) Ngạch/chức danh nghề nghiệp:" },
        { 49, "Mã số:" },
        { 50, "Ngày bổ nhiệm ngạch/chức danh nghề nghiệp:" },
        { 51, "Bậc lương:" },
        { 52, "Hệ số:" },
        { 53, "Ngày hưởng:" },
        { 54, "Phần trăm hưởng:" },
        { 55, "Phụ cấp thâm niên vượt khung:" },
        { 56, "Ngày hưởng PCTNVK:" },
        { 57, "30.2) Phụ cấp chức vụ:" },
        { 58, "Phụ cấp kiêm nhiệm" },
        { 59, "Phụ cấp khác" },
        { 60, "30.3) Vị trí việc làm:" },
        { 61, "Mã số:" },
        { 62, "Bậc lương" },
        { 63, "Lương theo mức tiền:" },
        { 64, "Ngày hưởng:" },
        { 65, "Phần trăm hưởng:" },
        { 66, "Ngày hưởng PCTNVK:" },
        { 67, "Phụ cấp thâm niên vượt khung:" },
        { 68, "31) Tình trạng sức khoẻ:" },
        { 69, "Chiều cao:" },
        { 70, "Cân nặng:" },
        { 71, "Nhóm máu:" },
        { 74, "Từ tháng/năm" },
        { 75, "Đến tháng/năm" },
        { 76, "Tên cơ sở đào tạo" },
        { 77, "Chuyên ngành đào tạo" },
        { 78, "Hình thức đào tạo" },
        { 79, "Văn bằng, trình độ" },
        { 81, "Từ tháng/năm" },
        { 82, "Đến tháng/năm" },
        { 83, "Tên cơ sở đào tạo" },
        { 84, "Hình thức đào tạo" },
        { 85, "Văn bằng được cấp" },
        { 87, "Từ tháng/năm" },
        { 88, "Đến tháng/năm" },
        { 89, "Tên cơ sở đào tạo" },
        { 90, "Chứng chỉ được cấp" },
        { 92, "Từ tháng/năm" },
        { 93, "Đến tháng/năm" },
        { 94, "Tên cơ sở đào tạo" },
        { 95, "Chứng chỉ được cấp" },
        { 97, "Từ tháng/năm" },
        { 98, "Đến tháng/năm" },
        { 99, "Tên cơ sở đào tạo" },
        { 100, "Chứng chỉ được cấp" },
        { 102, "Từ tháng/năm" },
        { 103, "Đến tháng/năm" },
        { 104, "Tên cơ sở đào tạo" },
        { 105, "Tên ngoại ngữ/ tiếng dân tộc" },
        { 106, "Chứng chỉ được cấp" },
        { 107, "Điểm số" },
        { 109, "Từ tháng/năm" },
        { 110, "Đến tháng/năm" },
        { 111, "Đơn vị công tác" },
        { 112, "Chức danh/ chức vụ" },
        { 114, "34.1- Khai rõ:" },
        { 116, "Từ tháng/năm" },
        { 117, "Đến tháng/năm" },
        { 118, "Chức danh, chức vụ, đơn vị, địa điểm đã làm việc" },
        { 120, "Từ tháng/năm" },
        { 121, "Đến tháng/năm" },
        { 122, "Tên tổ chức, địa chỉ trụ sở, công việc đã làm" },
        { 125, "Năm" },
        { 126, "Xếp loại chuyên môn" },
        { 127, "Xếp loại thi đua" },
        { 128, "Hình thức khen thưởng" },
        { 130, "Từ tháng/năm" },
        { 131, "Đến tháng/năm" },
        { 132, "Hình thức" },
        { 133, "Hành vi vi phạm chính" },
        { 134, "Cơ quan quyết định" },
        { 137, "Mối quan hệ" },
        { 138, "Họ và tên" },
        { 139, "Năm sinh" },
        { 140, "Quê quán, nghề nghiệp" },
        { 142, "Mối quan hệ" },
        { 143, "Họ và tên" },
        { 144, "Năm sinh" },
        { 145, "Quê quán, nghề nghiệp" },
        { 148, "Từ tháng/năm" },
        { 149, "Đến tháng/năm" },
        { 150, "Mã số" },
        { 151, "Bậc lương" },
        { 152, "Hệ số lương" },
        { 153, "Tiền lương theo vị trí việc làm" },
        { 155, "Từ tháng/năm" },
        { 156, "Đến tháng/năm" },
        { 157, "Loại phụ cấp" },
        { 158, "Phần trăm hưởng" },
        { 159, "Hệ số" },
        { 160, "Hình thức hưởng" },
        { 161, "Giá trị (đồng)" },
        { 163, "- Tiền lương:" },
        { 164, "- Các nguồn khác:" },
        { 165, "Nhà ở" },
        { 166, "+ Được cấp, được thuê (loại nhà):" },
        { 167, "tổng diện tích sử dụng:" },
        { 168, "Giấy chứng nhận quyền sở hữu:" },
        { 169, "+ Nhà tự mua, tự xây (loại nhà):" },
        { 170, "tổng diện tích sử dụng:" },
        { 171, "Giấy chứng nhận quyền sở hữu:" },
        { 172, "Đất ở"},
        { 173, "+ Đất được cấp:" },
        { 174, "Giấy chứng nhận quyền sử dụng:" },
        { 175, "+ Đất tự mua:" },
        { 176, "Giấy chứng nhận quyền sử dụng:" },
        { 177, "- Đất sản xuất kinh doanh:" },
        { 179, "Nhận xét, đánh giá:" }
    };

            return patterns.ContainsKey(index) ? patterns[index] : null;
        }

        private void EnsureRunFormatting(Run run)
        {
            var runProps = run.RunProperties;
            if (runProps == null)
            {
                runProps = new RunProperties();
                run.InsertAt(runProps, 0);
            }

            var existingFont = runProps.Elements<RunFonts>().FirstOrDefault();
            if (existingFont == null)
            {
                runProps.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
            }
            else
            {
                existingFont.Ascii = "Times New Roman";
                existingFont.HighAnsi = "Times New Roman";
            }

            var existingSize = runProps.Elements<FontSize>().FirstOrDefault();
            if (existingSize == null)
            {
                runProps.Append(new FontSize { Val = "24" });
            }
            else
            {
                existingSize.Val = "24";
            }
        }

        private void ReplaceFieldInTable(Body body, int columnIndex, int rowIndex, string data, int itemId, Dictionary<int, int> itemToTableIndexMap)
        {
            if (string.IsNullOrEmpty(data))
                return;

            if (!itemToTableIndexMap.TryGetValue(itemId, out int tableIndex))
            {
                return;
            }

            var tables = body.Elements<WordTable>().ToList();

            int tableArrayIndex = tableIndex;

            if (tableArrayIndex >= tables.Count || tableArrayIndex < 0)
            {
                return;
            }

            var table = tables[tableArrayIndex];
            var rows = table.Elements<TableRow>().ToList();
            if (rows.Count == 0)
            {
                return;
            }

            var headerRow = rows[0];
            var headerCells = headerRow.Elements<TableCell>().ToList();

            int cellArrayIndex = columnIndex - 1;

            if (cellArrayIndex >= headerCells.Count || cellArrayIndex < 0)
            {
                return;
            }

            int targetRowIndex = rowIndex;

            while (rows.Count <= targetRowIndex)
            {
                var newRow = CreateTableRow(headerCells.Count);
                table.Append(newRow);
                rows = table.Elements<TableRow>().ToList();
            }

            var targetRow = rows[targetRowIndex];
            var targetCells = targetRow.Elements<TableCell>().ToList();

            if (cellArrayIndex < targetCells.Count)
            {
                var targetCell = targetCells[cellArrayIndex];

                var cellProps = targetCell.GetFirstChild<TableCellProperties>();
                if (cellProps == null)
                {
                    cellProps = new TableCellProperties();
                    targetCell.InsertAt(cellProps, 0);
                }

                var verticalAlign = cellProps.GetFirstChild<TableCellVerticalAlignment>();
                if (verticalAlign == null)
                {
                    cellProps.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                }

                var paragraph = targetCell.Elements<Paragraph>().FirstOrDefault();

                if (paragraph != null)
                {
                    paragraph.RemoveAllChildren<Run>();

                    var paraProps = paragraph.GetFirstChild<ParagraphProperties>();
                    if (paraProps == null)
                    {
                        paraProps = new ParagraphProperties();
                        paragraph.InsertAt(paraProps, 0);
                    }

                    var justification = paraProps.GetFirstChild<Justification>();
                    if (justification == null)
                    {
                        paraProps.Append(new Justification { Val = JustificationValues.Center });
                    }
                    else
                    {
                        justification.Val = JustificationValues.Center;
                    }

                    var run = new Run();
                    var runProps = new RunProperties();
                    runProps.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                    runProps.Append(new FontSize { Val = "24" });
                    run.Append(runProps);
                    run.Append(new Text(data) { Space = SpaceProcessingModeValues.Preserve });
                    paragraph.Append(run);
                }
            }
        }

        private TableRow CreateTableRow(int columnCount)
        {
            var row = new TableRow();

            for (int i = 0; i < columnCount; i++)
            {
                var cell = new TableCell();
                var paragraph = new Paragraph();
                var run = new Run();
                var runProps = new RunProperties();
                runProps.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                runProps.Append(new FontSize { Val = "24" });
                run.Append(runProps);
                run.Append(new Text(""));
                paragraph.Append(run);
                cell.Append(paragraph);
                row.Append(cell);
            }

            return row;
        }

        public byte[] GenerateSoYeuLyLichTemplate(int idFormLoai)
        {
            var formLoai = _syllDbContext.FormLoais.FirstOrDefault(x => x.Id == idFormLoai && !x.Deleted)
              ?? throw new UserFriendlyException(ErrorCodes.FormLoaiErrorNotFound);
            //idFormLoai = 1;
            if(idFormLoai != 1)
            {
                throw new UserFriendlyException(ErrorCodes.TemplateErrorTemplateFormLoaiNotFound);
            }
            using var memoryStream = new MemoryStream();

            using (var document = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = mainPart.Document.AppendChild(new Body());

                SetupPageSettings(mainPart);
                AddHeader(body);
                AddMainInfoSection(body);
                AddBasicFieldsSection(body);
                AddSalarySection(body);
                AddHealthSection(body);
                AddTrainingSection(body);
                AddWorkHistorySection(body);
                AddPersonalHistorySection(body);
                AddRewardSection(body);
                AddFamilySection(body);
                AddEconomicSection(body);
                AddFooterSection(body);

                mainPart.Document.Save();
            }

            return memoryStream.ToArray();
        }

        private void SetupPageSettings(MainDocumentPart mainPart)
        {
            var sectionProperties = new SectionProperties();
            var pageSize = new PageSize { Width = 11906, Height = 16838 }; // A4
            var pageMargin = new PageMargin
            {
                Top = 1134,
                Right = 1134,
                Bottom = 1134,
                Left = 1134,
                Header = 708,
                Footer = 708,
                Gutter = 0
            };

            sectionProperties.Append(pageSize);
            sectionProperties.Append(pageMargin);
            mainPart.Document.Body.Append(sectionProperties);
        }

        private void AddHeader(Body body)
        {
            var p1 = new Paragraph();
            var run1 = new Run();
            var runProps1 = new RunProperties();
            runProps1.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
            runProps1.Append(new FontSize { Val = "24" });
            run1.Append(runProps1);
            run1.Append(new Text("Cơ quan quản lý viên chức: ") { Space = SpaceProcessingModeValues.Preserve });
            p1.Append(run1);

            var run2 = new Run();
            var runProps2 = new RunProperties();
            runProps2.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
            runProps2.Append(new FontSize { Val = "24" });
            runProps2.Append(new Bold());
            runProps2.Append(new BoldComplexScript());
            run2.Append(runProps2);
            run2.Append(new Text("TRƯỜNG ĐẠI HỌC XÂY DỰNG HÀ NỘI.") { Space = SpaceProcessingModeValues.Preserve });
            p1.Append(run2);
            body.Append(p1);

            AddParagraph(body, "Đơn vị sử dụng viên chức: ......................................", false, 24);
            AddParagraph(body, "Số hiệu: ....................", false, 24);
            AddParagraph(body, "Mã số định danh:.........................................................................................................", false, 24);
            AddParagraph(body, "", false, 24);
        }

        private void AddMainInfoSection(Body body)
        {
            var table = new WordTable();
            var tableProperties = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 }
                ),
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
            );
            table.AppendChild(tableProperties);

            var row = new TableRow();

            // Set chiều cao cố định cho row để tạo khung vuông
            var rowProperties = new TableRowProperties(
                new TableRowHeight { Val = 2400, HeightType = HeightRuleValues.AtLeast }
            );
            row.Append(rowProperties);

            // Cell ảnh - vuông
            var cellPhoto = new TableCell();
            var cellPhotoProperties = new TableCellProperties(
                new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "2400" },
                new TableCellBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                ),
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            );
            cellPhoto.Append(cellPhotoProperties);

            // Text căn giữa theo chiều ngang và dọc
            cellPhoto.Append(CreateParagraphWithText("Ảnh màu", true, 24, JustificationValues.Center));
            cellPhoto.Append(CreateParagraphWithText("(4 x 6 cm)", false, 24, JustificationValues.Center));

            row.Append(cellPhoto);

            // Khoảng cách giữa khung ảnh và nội dung
            var cellSpace = new TableCell();
            var cellSpaceProperties = new TableCellProperties(
                new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "400" }
            );
            cellSpace.Append(cellSpaceProperties);
            cellSpace.Append(CreateParagraphWithText("", false, 24));
            row.Append(cellSpace);

            // Cell thông tin
            var cellInfo = new TableCell();
            var cellInfoProperties = new TableCellProperties(
                new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "7000" }
            );
            cellInfo.Append(cellInfoProperties);

            cellInfo.Append(CreateParagraphWithText("SƠ YẾU LÝ LỊCH", true, 32, JustificationValues.Center));
            cellInfo.Append(CreateParagraphWithText("", false, 24));
            cellInfo.Append(CreateParagraphWithText("1) Họ và tên khai sinh (viết chữ in hoa):................... Giới tính:..........", false, 24));
            cellInfo.Append(CreateParagraphWithText("2) Các tên gọi khác: .............................................................................", false, 24));
            cellInfo.Append(CreateParagraphWithText("3) Sinh ngày: ...... tháng ...... năm .........................................................", false, 24));
            cellInfo.Append(CreateParagraphWithText("4) Nơi sinh:.............................................................................................", false, 24));
            cellInfo.Append(CreateParagraphWithText("5) Quê quán (xã, phường): ............ ................tỉnh, TP): ............................", false, 24));
            row.Append(cellInfo);

            table.Append(row);
            body.Append(table);
            AddParagraph(body, "", false, 24);
        }

        private void AddBasicFieldsSection(Body body)
        {
            var fields = new[]
            {
                "6) Dân tộc:........................................................................................................",
                "7) Tôn giáo:......................................................................................................",
                "8) Số CCCD:............................Ngày cấp:....../....../............SĐT liên hệ:..............",
                "9) Số BHXH:...................................................Số thẻ BHYT:........................................",
                "10) Nơi ở hiện nay:..............................................................................................",
                "11) Thành phần gia đình xuất thân:...................................................................",
                "12) Nghề nghiệp trước khi được tuyển dụng:........................................................",
                "13) Ngày được tuyển dụng lần đầu: ...../...../...... Cơ quan, tổ chức, đơn vị tuyển dụng:...",
                "14) Ngày vào cơ quan hiện đang công tác:............................................................",
                "15) Ngày vào Đảng Cộng sản Việt Nam: ....../....../...     Ngày chính thức:......./....../.....",
                "16) Ngày tham gia tổ chức chính trị-xã hội đầu tiên (ngày vào Đoàn TNCSHCM, Công đoàn, Hội):........",
                "17) Ngày nhập ngũ:.../.../......      Ngày xuất ngũ:.../.../......     Quân hàm cao nhất:....",
                "18) Đối tượng chính sách:........",
                "19) Trình độ giáo dục phổ thông (đã tốt nghiệp lớp mấy/thuộc hệ nào):..................",
                "20) Trình độ chuyên môn cao nhất: ................................................................",
                "21) Học hàm:......................................................................................................",
                "22) Danh hiệu nhà nước phong tặng:.........................................................................",
                "23) Chức vụ hiện tại:..............................................................................................",
                "Ngày bổ nhiệm/ngày phê chuẩn:.../.../ ... Ngày bổ nhiệm lại/phê chuẩn nhiệm kỳ tiếp theo:.../.../ ...",
                "24) Được quy hoạch chức danh: ...........................................................................",
                "25) Chức vụ kiêm nhiệm:........................................................................................",
                "26) Chức vụ Đảng hiện tại:..................................................................................",
                "27) Chức vụ Đảng kiêm nhiệm:.............................................................................",
                "28) Công việc chính được giao:...........................................................................",
                "29) Sở trường công tác:................................................................ Công việc làm lâu nhất...."
            };

            foreach (var field in fields)
            {
                AddParagraph(body, field, false, 24);
            }
        }

        private void AddSalarySection(Body body)
        {
            AddParagraph(body, "30) Tiền lương", true, 24);
            AddParagraph(body, "30.1) Ngạch/chức danh nghề nghiệp: .................................................. Mã số:...........", false, 24);
            AddParagraph(body, "Ngày bổ nhiệm ngạch/chức danh nghề nghiệp: ......../......../...............................", false, 24);
            AddParagraph(body, "Bậc lương: ...................... Hệ số: ..................... Ngày hưởng: ......../......../.......", false, 24);
            AddParagraph(body, "Phần trăm hưởng:....%; Phụ cấp thâm niên vượt khung:...%; Ngày hưởng PCTNVK:.../.../ ...", false, 24);
            AddParagraph(body, "30.2) Phụ cấp chức vụ: ............ Phụ cấp kiêm nhiệm .................... Phụ cấp khác.........", false, 24);
            AddParagraph(body, "30.3) Vị trí việc làm: ......................................................................... Mã số:.........", false, 24);
            AddParagraph(body, "Bậc lương: ........................... Lương theo mức tiền: .....................vnđ. Ngày hưởng: ...../...../.", false, 24);
            AddParagraph(body, "Phần trăm hưởng:...%; Phụ cấp thâm niên vượt khung:..%; Ngày hưởng PCTNVK: ...../..../...", false, 24);
        }

        private void AddHealthSection(Body body)
        {
            AddParagraph(body, "31) Tình trạng sức khoẻ:..................", false, 24);
            AddParagraph(body, "Chiều cao: .......................cm, Cân nặng: .................... kg, Nhóm máu:.......", false, 24);
        }

        private void AddTrainingSection(Body body)
        {
            AddParagraph(body, "32) QUÁ TRÌNH ĐÀO TẠO, BỒI DƯỠNG", true, 24, JustificationValues.Center);

            AddParagraph(body, "32.1- Chuyên môn (từ trung cấp trở lên cả trong nước và nước ngoài)", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Tên cơ sở đào tạo", "Chuyên ngành đào tạo", "Hình thức\nđào tạo", "Văn bằng,\ntrình độ" }, 3);

            AddParagraph(body, "32.2- Lý luận chính trị", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Tên cơ sở đào tạo", "Hình thức\nđào tạo", "Văn bằng được cấp" }, 3);

            AddParagraph(body, "32.3- Bồi dưỡng quản lý nhà nước/ chức danh nghề nghiệp/ nghiệp vụ chuyên ngành", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Tên cơ sở đào tạo", "Chứng chỉ được cấp" }, 3);

            AddParagraph(body, "32.4- Bồi dưỡng kiến thức an ninh, quốc phòng", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Tên cơ sở đào tạo", "Chứng chỉ được cấp" }, 2);

            AddParagraph(body, "32.5- Tin học", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Tên cơ sở đào tạo", "Chứng chỉ được cấp" }, 3);

            AddParagraph(body, "32.6- Ngoại ngữ/ tiếng dân tộc", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Tên cơ sở đào tạo", "Tên ngoại ngữ/\ntiếng dân tộc", "Chứng chỉ\nđược cấp", "Điểm số" }, 3);
        }

        private void AddWorkHistorySection(Body body)
        {
            AddParagraph(body, "33) TÓM TẮT QUÁ TRÌNH CÔNG TÁC", true, 24, JustificationValues.Center);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Đơn vị công tác (đảng, chính quyền,\n\nđoàn thể, tổ chức xã hội)", "Chức danh/\nchức vụ" }, 3);
        }

        private void AddPersonalHistorySection(Body body)
        {
            AddParagraph(body, "34) ĐẶC ĐIỂM LỊCH SỬ BẢN THÂN", true, 24, JustificationValues.Center);
            AddParagraph(body, "34.1- Khai rõ: bị bắt, bị tù (từ ngày tháng năm nào đến ngày tháng năm nào, ở đâu?), đã khai báo cho ai, những vấn đề gì?: .....................................................................", false, 24);

            AddParagraph(body, "34.2- Bản thân có làm việc cho chế độ cũ", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Chức danh, chức vụ, đơn vị, địa điểm đã làm việc" }, 3);

            AddParagraph(body, "34.3-Tham gia hoặc có quan hệ với các tổ chức chính trị, kinh tế, xã hội ... ở nước ngoài", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Tên tổ chức, địa chỉ trụ sở, công việc đã làm" }, 3);
        }

        private void AddRewardSection(Body body)
        {
            AddParagraph(body, "35) KHEN THƯỞNG, KỶ LUẬT", true, 24, JustificationValues.Center);

            AddParagraph(body, "35.1- Thành tích thi đua, khen thưởng", false, 24);
            AddTableWithHeaders(body, new[] { "Năm", "Xếp loại chuyên môn", "Xếp loại thi đua", "Hình thức khen\nthưởng" }, 3);

            AddParagraph(body, " 35.2-Kỷ luật Đảng/hành chính", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Hình thức", "Hành vi vi phạm chính", "Cơ quan quyết\nđịnh" }, 3);
        }

        private void AddFamilySection(Body body)
        {
            AddParagraph(body, "36) QUAN HỆ GIA ĐÌNH", true, 24, JustificationValues.Center);

            AddParagraph(body, "36.1- Về bản thân: Cha, Mẹ, Vợ (hoặc chồng), các con, anh chị em ruột", false, 24);
            AddTableWithHeaders(body, new[] { "Mối\nquan\nhệ", "Họ và\ntên", "Năm\nsinh", "Quê quán, nghề nghiệp, chức danh, chức vụ, đơn vị công\ntác, học tập, nơi ở (trong, ngoài nước); thành viên các\ntổ chức chính trị - xã hội (trong, ngoài nước); làm việc\ncho chế độ cũ, tiền án, tiền sự (nếu có)" }, 3);

            AddParagraph(body, "36.2- Cha, Mẹ, anh chị em ruột (bên vợ hoặc chồng)", false, 24);
            AddTableWithHeaders(body, new[] { "Mối quan\nhệ", "Họ và\ntên", "Năm\nsinh", "Quê quán, nghề nghiệp, chức danh, chức vụ, đơn vị công\ntác, học tập, nơi ở (trong, ngoài nước); thành viên các\ntổ chức chính trị - xã hội (trong, ngoài nước); làm việc\ncho chế độ cũ, tiền án, tiền sự (nếu có)" }, 2);
        }

        private void AddEconomicSection(Body body)
        {
            AddParagraph(body, "37) HOÀN CẢNH KINH TẾ GIA ĐÌNH", true, 24, JustificationValues.Center);

            AddParagraph(body, "37.1- Quá trình lương của bản thân", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Mã số", "Bậc lương", "Hệ số lương", "Tiền lương theo vị trí\nviệc làm" }, 2);

            AddParagraph(body, "37.2- Các loại phụ cấp khác", false, 24);
            AddTableWithHeaders(body, new[] { "Từ tháng/năm", "Đến tháng/năm", "Loại phụ cấp", "Phần trăm hưởng", "Hệ số", "Hình thức\nhưởng", "Giá trị\n(đồng)" }, 3);

            AddParagraph(body, "37.3- Nguồn thu nhập chính của gia đình hàng năm", false, 24);
            AddParagraph(body, "- Tiền lương: .................................................................................................", false, 24);
            AddParagraph(body, "- Các nguồn khác: ................................................................................................", false, 24);
            AddParagraph(body, "- Nhà ở:", false, 24);
            AddParagraph(body, "+ Được cấp, được thuê (loại nhà): ................., tổng diện tích sử dụng: ...........m2.", false, 24);
            AddParagraph(body, "Giấy chứng nhận quyền sở hữu:...............................................................", false, 24);
            AddParagraph(body, "+ Nhà tự mua, tự xây (loại nhà): .........., tổng diện tích sử dụng: ...............m2.", false, 24);
            AddParagraph(body, "Giấy chứng nhận quyền sở hữu:....", false, 24);
            AddParagraph(body, "- Đất ở:", false, 24);
            AddParagraph(body, "+ Đất được cấp: ............................. m2.", false, 24);
            AddParagraph(body, "Giấy chứng nhận quyền sử dụng:....", false, 24);
            AddParagraph(body, "+ Đất tự mua: ...................................m2.", false, 24);
            AddParagraph(body, "Giấy chứng nhận quyền sử dụng:...", false, 24);
            AddParagraph(body, "- Đất sản xuất kinh doanh: ................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................", false, 24);
        }

        private void AddFooterSection(Body body)
        {
            AddParagraph(body, "38) NHẬN XÉT, ĐÁNH GIÁ CỦA CƠ QUAN, TỔ CHỨC, ĐƠN VỊ SỬ DỤNG", true, 24, JustificationValues.Center);
            //AddParagraph(body, "Nhận xét, đánh giá", true, 24, JustificationValues.Center);
            AddParagraph(body, "Nhận xét, đánh giá: \n ................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................", false, 24);
            AddParagraph(body, "", false, 24);

            var table = new WordTable();
            var tableProperties = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0 }
                ),
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
            );
            table.AppendChild(tableProperties);

            var row = new TableRow();

            var cell1 = new TableCell();
            cell1.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "2000" }));
            cell1.Append(CreateParagraphWithText("Người khai", true, 24, JustificationValues.Center));
            cell1.Append(CreateParagraphWithText("Tôi xin cam đoan những lời", false, 24, JustificationValues.Center));
            cell1.Append(CreateParagraphWithText("khai trên đây là đúng sự thật", false, 24, JustificationValues.Center));
            //cell1.Append(CreateParagraphWithText("", false, 24));
            cell1.Append(CreateParagraphWithText("(Ký tên, ghi rõ họ tên)", false, 24, JustificationValues.Center));
            row.Append(cell1);

            var cell2 = new TableCell();
            cell2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "3000" }));
            cell2.Append(CreateParagraphWithText("............, Ngày.......tháng.........năm 20......", false, 24, JustificationValues.Center));
            //cell2.Append(CreateParagraphWithText("", false, 24));
            cell2.Append(CreateParagraphWithText("HIỆU TRƯỞNG", true, 24, JustificationValues.Center));
            //cell2.Append(CreateParagraphWithText("", false, 24));
            cell2.Append(CreateParagraphWithText("(Ký tên, đóng dấu)", false, 24, JustificationValues.Center));
            row.Append(cell2);

            table.Append(row);
            body.Append(table);
        }

        private void AddParagraph(Body body, string text, bool bold, int fontSize, JustificationValues? alignment = null)
        {
            body.Append(CreateParagraphWithText(text, bold, fontSize, alignment));
        }

        private Paragraph CreateParagraphWithText(string text, bool bold, int fontSize, JustificationValues? alignment = null)
        {
            var paragraph = new Paragraph();

            if (alignment.HasValue)
            {
                var paragraphProperties = new ParagraphProperties(new Justification { Val = alignment.Value });
                paragraph.Append(paragraphProperties);
            }

            var run = new Run();
            var runProperties = new RunProperties();
            runProperties.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
            runProperties.Append(new FontSize { Val = fontSize.ToString() });

            if (bold)
            {
                runProperties.Append(new Bold());
            }

            run.Append(runProperties);
            run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            paragraph.Append(run);

            return paragraph;
        }

        private void AddTableWithHeaders(Body body, string[] headers, int dataRows)
        {
            var table = new WordTable();
            var tableProperties = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                ),
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
            );
            table.AppendChild(tableProperties);

            var headerRow = new TableRow();
            foreach (var header in headers)
            {
                var cell = new TableCell();
                cell.Append(CreateParagraphWithText(header, true, 20, JustificationValues.Center));
                headerRow.Append(cell);
            }
            table.Append(headerRow);

            for (int i = 0; i < dataRows; i++)
            {
                var dataRow = new TableRow();
                foreach (var _ in headers)
                {
                    var cell = new TableCell();
                    cell.Append(CreateParagraphWithText("", false, 24));
                    dataRow.Append(cell);
                }
                table.Append(dataRow);
            }

            body.Append(table);
            AddParagraph(body, "", false, 24);
        }

        
    }
}