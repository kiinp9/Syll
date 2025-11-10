using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using syll.be.infrastructure.data;
using syll.be.lib.Form.Dtos;
using syll.be.lib.Form.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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


        public async Task<byte[]> ReplaceWordFormTemplate(int idFormLoai)
        {
            _logger.LogInformation($"{nameof(ReplaceWordFormTemplate)} idFormLoai = {JsonSerializer.Serialize(idFormLoai)}");
            var idDanhBa = await GetCurrentDanhBaId();

            // Lấy dữ liệu từ FormData và FormTruongData
            var formDataList = await _syllDbContext.FormDatas
                .Where(fd => fd.IdFormLoai == idFormLoai && fd.IdDanhBa == idDanhBa && !fd.Deleted)
                .ToListAsync();

            _logger.LogInformation($"[DEBUG] Tổng số FormData tìm thấy: {formDataList.Count}");

            if (!formDataList.Any())
            {
                _logger.LogWarning($"No data found for IdFormLoai={idFormLoai}, IdDanhBa={idDanhBa}");
                return GenerateSoYeuLyLichTemplate();
            }

            var idTruongDataList = formDataList.Select(fd => fd.IdTruongData).Distinct().ToList();
            _logger.LogInformation($"[DEBUG] Số lượng IdTruongData unique: {idTruongDataList.Count}");

            var formTruongDataList = await _syllDbContext.FormTruongDatas
                .Where(ftd => idTruongDataList.Contains(ftd.Id) && !ftd.Deleted)
                .ToListAsync();

            _logger.LogInformation($"[DEBUG] Số lượng FormTruongData tìm thấy: {formTruongDataList.Count}");

            // Map TenTruong -> Data cho các trường đơn giản
            var dataMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Map TenTruong -> List<Data> cho dữ liệu bảng (có IndexRowTable)
            var tableDataMapping = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            // Trong phần build mapping của ReplaceWordFormTemplate, thay thế đoạn code xử lý IndexRowTable
            foreach (var formData in formDataList)
            {
                var truongData = formTruongDataList.FirstOrDefault(ftd => ftd.Id == formData.IdTruongData);
                if (truongData == null)
                {
                    _logger.LogWarning($"[DEBUG] Không tìm thấy TruongData cho IdTruongData={formData.IdTruongData}");
                    continue;
                }

                var tenTruong = truongData.TenTruong.Trim();
                var data = formData.Data ?? "";

                if (formData.IndexRowTable.HasValue && formData.IndexRowTable.Value > 0)
                {
                    // Dữ liệu bảng - IndexRowTable bắt đầu từ 1
                    if (!tableDataMapping.ContainsKey(tenTruong))
                    {
                        tableDataMapping[tenTruong] = new List<string>();
                        _logger.LogInformation($"[DEBUG] Khởi tạo bảng cho trường: '{tenTruong}'");
                    }

                    // IndexRowTable = 1 tương ứng List[0], IndexRowTable = 2 tương ứng List[1]
                    var listIndex = formData.IndexRowTable.Value - 1;

                    // Đảm bảo có đủ rows
                    while (tableDataMapping[tenTruong].Count <= listIndex)
                    {
                        tableDataMapping[tenTruong].Add("");
                    }

                    tableDataMapping[tenTruong][listIndex] = data;
                    _logger.LogInformation($"[DEBUG] Bảng - '{tenTruong}' [IndexRowTable={formData.IndexRowTable.Value}, ListIndex={listIndex}] = '{data}'");
                }
                else
                {
                    // Dữ liệu đơn giản (IndexRowTable null hoặc 0)
                    dataMapping[tenTruong] = data;
                    _logger.LogInformation($"[DEBUG] Dòng - '{tenTruong}' = '{data}'");
                }
            }

            _logger.LogInformation($"[DEBUG] Tổng số trường dòng đơn: {dataMapping.Count}");
            _logger.LogInformation($"[DEBUG] Tổng số trường bảng: {tableDataMapping.Count}");

            // Log chi tiết các trường dòng đơn
            foreach (var kvp in dataMapping)
            {
                _logger.LogInformation($"[DEBUG] DataMapping: '{kvp.Key}' = '{kvp.Value}'");
            }

            // Log chi tiết các trường bảng
            foreach (var kvp in tableDataMapping)
            {
                _logger.LogInformation($"[DEBUG] TableDataMapping: '{kvp.Key}' có {kvp.Value.Count} dòng");
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    _logger.LogInformation($"[DEBUG]   Row {i}: '{kvp.Value[i]}'");
                }
            }

            // Generate template
            var templateBytes = GenerateSoYeuLyLichTemplate();
            _logger.LogInformation($"[DEBUG] Template size: {templateBytes.Length} bytes");

            using var memoryStream = new MemoryStream();
            memoryStream.Write(templateBytes, 0, templateBytes.Length);
            memoryStream.Position = 0;

            using (var document = WordprocessingDocument.Open(memoryStream, true))
            {
                var body = document.MainDocumentPart.Document.Body;

                // Replace dấu ... bằng dữ liệu
                _logger.LogInformation($"[DEBUG] Bắt đầu replace dữ liệu dòng đơn");
                ReplaceDotsWithData(body, dataMapping);

                // Replace data trong bảng
                _logger.LogInformation($"[DEBUG] Bắt đầu replace dữ liệu bảng");
                ReplaceTableData(body, tableDataMapping);

                _logger.LogInformation($"[DEBUG] Bắt đầu replace dữ liệu dòng đơn (lần 2 - sau bảng)");
                ReplaceDotsWithData(body, dataMapping);

                document.MainDocumentPart.Document.Save();
            }

            var resultBytes = memoryStream.ToArray();
            _logger.LogInformation($"[DEBUG] Result file size: {resultBytes.Length} bytes");

            return resultBytes;
        }

        private void ReplaceDotsWithData(Body body, Dictionary<string, string> dataMapping)
        {
            var textElements = body.Descendants<Text>().ToList();
            _logger.LogInformation($"[DEBUG] ReplaceDotsWithData: Tổng số Text elements: {textElements.Count}");

            int replacedCount = 0;

            var normalizedDbMapping = new Dictionary<string, (string OriginalKey, string Value)>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in dataMapping)
            {
                var normalizedKey = NormalizeText(kvp.Key);
                if (!string.IsNullOrWhiteSpace(normalizedKey))
                {
                    normalizedDbMapping[normalizedKey] = (kvp.Key, kvp.Value);
                }
            }

            foreach (var textElement in textElements)
            {
                if (string.IsNullOrEmpty(textElement.Text)) continue;

                var originalText = textElement.Text;
                var text = originalText;

                if (!text.Contains("...") && !text.Contains("..") && !text.Contains("…")) continue;

                _logger.LogInformation($"[DEBUG] Đang xử lý text có dots: '{text}'");

                bool textChanged = false;

                // XỬ LÝ 1: Pattern với prefix + hoặc - (BẤT KỲ VỊ TRÍ nào, không chỉ đầu dòng)
                // Sửa: Bỏ ^ để cho phép match ở bất kỳ vị trí nào
                var fieldWithPrefixPattern = @"([\+\-])\s*([^:]+?)[\s:]*([.…/]{2,})(?:\s*(m2|cm|kg|vnđ|đồng|%))?";
                var prefixMatches = System.Text.RegularExpressions.Regex.Matches(text, fieldWithPrefixPattern);

                if (prefixMatches.Count > 0)
                {
                    _logger.LogInformation($"[DEBUG] Tìm thấy {prefixMatches.Count} trường có prefix +/-");

                    // Xử lý TỪNG match (có thể có nhiều trường + hoặc - trong cùng 1 text element)
                    foreach (System.Text.RegularExpressions.Match prefixMatch in prefixMatches)
                    {
                        var prefix = prefixMatch.Groups[1].Value;
                        var templateFieldRaw = prefixMatch.Groups[2].Value.Trim();
                        var dots = prefixMatch.Groups[3].Value;
                        var unit = prefixMatch.Groups[4].Value;

                        templateFieldRaw = templateFieldRaw.TrimEnd(',', ';');
                        var normalizedTemplateField = NormalizeText(templateFieldRaw);

                        _logger.LogInformation($"[DEBUG]   Trường với prefix '{prefix}': '{templateFieldRaw}' -> Normalized: '{normalizedTemplateField}' -> Unit: '{unit}'");

                        if (normalizedDbMapping.TryGetValue(normalizedTemplateField, out var dbData))
                        {
                            var originalKey = dbData.OriginalKey;
                            var data = dbData.Value;

                            if (!string.IsNullOrWhiteSpace(data))
                            {
                                var formattedData = FormatDateIfNeeded(data);

                                // Pattern để replace: giữ nguyên prefix
                                var escapedField = System.Text.RegularExpressions.Regex.Escape(templateFieldRaw);
                                var replacePattern = string.IsNullOrWhiteSpace(unit)
                                    ? $@"[\+\-]\s*{escapedField}[\s:]*[.…/]+"
                                    : $@"[\+\-]\s*{escapedField}[\s:]*[.…/]+\s*{System.Text.RegularExpressions.Regex.Escape(unit)}";

                                var replacement = string.IsNullOrWhiteSpace(unit)
                                    ? $"{prefix} {templateFieldRaw}: {formattedData}"
                                    : $"{prefix} {templateFieldRaw}: {formattedData} {unit}";

                                var newText = System.Text.RegularExpressions.Regex.Replace(
                                    text,
                                    replacePattern,
                                    replacement,
                                    System.Text.RegularExpressions.RegexOptions.None
                                );

                                if (newText != text)
                                {
                                    _logger.LogInformation($"[DEBUG]   Replace thành công: '{originalKey}' = '{formattedData}'");
                                    _logger.LogInformation($"[DEBUG]     From: '{text}'");
                                    _logger.LogInformation($"[DEBUG]     To: '{newText}'");

                                    text = newText;
                                    replacedCount++;
                                    textChanged = true;
                                }
                            }
                            else
                            {
                                _logger.LogInformation($"[DEBUG]   Trường '{originalKey}' không có data -> Giữ nguyên dấu ...");
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"[DEBUG]   Không tìm thấy data cho trường: '{normalizedTemplateField}'");
                        }
                    }
                }

                // XỬ LÝ 2: Pattern thông thường (không có +/-)
                if (!textChanged) // Chỉ xử lý nếu chưa thay đổi bởi pattern prefix
                {
                    var fieldPattern = @"([^:\.;]+?(?:\([^)]*\))?)[\s:]*([.…/]{2,})(?:\s*(m2|cm|kg|vnđ|đồng|%))?";
                    var matches = System.Text.RegularExpressions.Regex.Matches(text, fieldPattern);

                    _logger.LogInformation($"[DEBUG] Tìm thấy {matches.Count} trường thông thường trong dòng này");

                    var fieldAfterCommaPattern = @",\s*([^:,]+?)[\s:]*([.…/]{2,})(?:\s*(m2|cm|kg|vnđ|đồng|%))?";
                    var additionalMatches = System.Text.RegularExpressions.Regex.Matches(text, fieldAfterCommaPattern);

                    var allMatches = new List<System.Text.RegularExpressions.Match>();
                    foreach (System.Text.RegularExpressions.Match m in matches)
                    {
                        allMatches.Add(m);
                    }

                    if (additionalMatches.Count > 0)
                    {
                        _logger.LogInformation($"[DEBUG] Tìm thêm {additionalMatches.Count} trường sau dấu phẩy");
                        foreach (System.Text.RegularExpressions.Match m in additionalMatches)
                        {
                            allMatches.Add(m);
                        }
                    }

                    foreach (var match in allMatches)
                    {
                        var templateFieldRaw = match.Groups[1].Value.Trim();
                        templateFieldRaw = templateFieldRaw.TrimEnd(',', ';');

                        var dots = match.Groups[2].Value;
                        var unit = match.Groups[3].Value;

                        if (string.IsNullOrWhiteSpace(templateFieldRaw) || templateFieldRaw.Length < 2)
                        {
                            continue;
                        }

                        var normalizedTemplateField = NormalizeText(templateFieldRaw);

                        _logger.LogInformation($"[DEBUG]   Trường template: '{templateFieldRaw}' -> Normalized: '{normalizedTemplateField}' -> Unit: '{unit}'");

                        if (normalizedDbMapping.TryGetValue(normalizedTemplateField, out var dbData))
                        {
                            var originalKey = dbData.OriginalKey;
                            var data = dbData.Value;

                            if (!string.IsNullOrWhiteSpace(data))
                            {
                                var formattedData = FormatDateIfNeeded(data);

                                var escapedField = System.Text.RegularExpressions.Regex.Escape(templateFieldRaw);
                                var replacePattern = string.IsNullOrWhiteSpace(unit)
                                    ? $@"{escapedField}[\s:]*[.…/]+"
                                    : $@"{escapedField}[\s:]*[.…/]+\s*{System.Text.RegularExpressions.Regex.Escape(unit)}";

                                var replacement = string.IsNullOrWhiteSpace(unit)
                                    ? $"{templateFieldRaw}: {formattedData}"
                                    : $"{templateFieldRaw}: {formattedData} {unit}";

                                var newText = System.Text.RegularExpressions.Regex.Replace(
                                    text,
                                    replacePattern,
                                    replacement
                                );

                                if (newText != text)
                                {
                                    _logger.LogInformation($"[DEBUG]   Replace thành công: '{originalKey}' = '{formattedData}'");
                                    _logger.LogInformation($"[DEBUG]     From: '{text}'");
                                    _logger.LogInformation($"[DEBUG]     To: '{newText}'");

                                    text = newText;
                                    replacedCount++;
                                    textChanged = true;
                                }
                            }
                            else
                            {
                                _logger.LogInformation($"[DEBUG]   Trường '{originalKey}' không có data -> Giữ nguyên dấu ...");
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"[DEBUG]   Không tìm thấy data cho trường: '{normalizedTemplateField}'");
                        }
                    }
                }

                if (textChanged)
                {
                    textElement.Text = text;
                }
            }

            _logger.LogInformation($"[DEBUG] ReplaceDotsWithData: Đã replace {replacedCount} lần");
        }
        private string FormatDateIfNeeded(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return data;

            // Kiểm tra format ISO date: YYYY-MM-DD hoặc YYYY-MM-DD HH:mm:ss
            var isoDatePattern = @"^(\d{4})-(\d{2})-(\d{2})";
            var match = System.Text.RegularExpressions.Regex.Match(data.Trim(), isoDatePattern);

            if (match.Success)
            {
                var year = match.Groups[1].Value;
                var month = match.Groups[2].Value;
                var day = match.Groups[3].Value;

                // Format lại thành DD/MM/YYYY
                var formattedDate = $"{day}/{month}/{year}";

                _logger.LogInformation($"[DEBUG] Format date: '{data}' -> '{formattedDate}'");

                return formattedDate;
            }

            // Không phải date thì trả về nguyên bản
            return data;
        }
        private void ReplaceTableData(Body body, Dictionary<string, List<string>> tableDataMapping)
        {
            if (!tableDataMapping.Any())
            {
                _logger.LogInformation($"[DEBUG] ReplaceTableData: Không có dữ liệu bảng nào");
                return;
            }

            var tables = body.Descendants<Table>().ToList();
            _logger.LogInformation($"[DEBUG] ReplaceTableData: Tìm thấy {tables.Count} bảng trong document");

            // Tạo normalized mapping
            var normalizedMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in tableDataMapping)
            {
                var normalizedKey = NormalizeText(kvp.Key);
                if (!string.IsNullOrWhiteSpace(normalizedKey))
                {
                    normalizedMapping[normalizedKey] = kvp.Key;
                }
            }

            int tableIndex = 0;
            foreach (var table in tables)
            {
                tableIndex++;
                _logger.LogInformation($"[DEBUG] Đang xử lý bảng #{tableIndex}");

                var rows = table.Elements<TableRow>().ToList();
                _logger.LogInformation($"[DEBUG]   Bảng có {rows.Count} dòng");

                if (rows.Count < 2)
                {
                    _logger.LogInformation($"[DEBUG]   Bỏ qua bảng vì không đủ dòng (cần ít nhất 2 dòng)");
                    continue;
                }

                // Row đầu tiên là header
                var headerRow = rows[0];
                var headerCells = headerRow.Elements<TableCell>().ToList();
                _logger.LogInformation($"[DEBUG]   Header có {headerCells.Count} cột");

                // Map index cột với TenTruong từ header (sử dụng normalized comparison)
                var columnMapping = new Dictionary<int, string>();
                for (int i = 0; i < headerCells.Count; i++)
                {
                    var headerText = string.Join(" ", headerCells[i].Descendants<Text>().Select(t => t.Text)).Trim();
                    var normalizedHeader = NormalizeText(headerText);

                    _logger.LogInformation($"[DEBUG]   Header cột {i}: '{headerText}' -> Normalized: '{normalizedHeader}'");

                    // Tìm TenTruong match với header - so sánh CHỨA nhau (bi-directional)
                    foreach (var kvp in normalizedMapping)
                    {
                        var normalizedKey = kvp.Key;
                        var originalKey = kvp.Value;

                        // Match nếu header chứa key HOẶC key chứa header
                        // Ví dụ: header "Từ tháng/năm" sẽ match với key "Từ"
                        // Hoặc: header "Họ và tên" sẽ match với key "Họ và tên khai sinh"
                        if (normalizedHeader.Contains(normalizedKey, StringComparison.OrdinalIgnoreCase) ||
                            normalizedKey.Contains(normalizedHeader, StringComparison.OrdinalIgnoreCase))
                        {
                            columnMapping[i] = originalKey; // Lưu original key
                            _logger.LogInformation($"[DEBUG]   Mapped cột {i} với trường: '{originalKey}'");
                            break;
                        }
                    }
                }

                if (!columnMapping.Any())
                {
                    _logger.LogInformation($"[DEBUG]   Bỏ qua bảng vì không tìm thấy cột nào khớp");
                    continue;
                }

                _logger.LogInformation($"[DEBUG]   Tìm thấy {columnMapping.Count} cột có dữ liệu để replace");

                // Tính số dòng data cần thiết (max của tất cả các column)
                int maxDataRows = 0;
                foreach (var colMapping in columnMapping)
                {
                    var tenTruong = colMapping.Value;
                    if (tableDataMapping.TryGetValue(tenTruong, out var dataList))
                    {
                        maxDataRows = Math.Max(maxDataRows, dataList.Count);
                    }
                }

                // Thêm dòng nếu cần (nếu DB có nhiều dòng hơn template)
                int currentDataRows = rows.Count - 1; // Trừ header row
                while (currentDataRows < maxDataRows)
                {
                    var newRow = new TableRow();

                    // Copy properties từ row cuối cùng
                    if (rows.Count > 1)
                    {
                        var lastRow = rows[rows.Count - 1];
                        var lastRowProps = lastRow.GetFirstChild<TableRowProperties>();
                        if (lastRowProps != null)
                        {
                            newRow.Append(lastRowProps.CloneNode(true));
                        }
                    }

                    // Tạo cells cho new row
                    foreach (var headerCell in headerCells)
                    {
                        var newCell = new TableCell();

                        // Copy cell properties
                        var cellProps = headerCell.GetFirstChild<TableCellProperties>();
                        if (cellProps != null)
                        {
                            newCell.Append(cellProps.CloneNode(true));
                        }

                        // Tạo empty paragraph với font size 12 (24 half-points)
                        var paragraph = new Paragraph();
                        var run = new Run();
                        var runProperties = new RunProperties();
                        runProperties.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                        runProperties.Append(new FontSize { Val = "24" }); // 12pt = 24 half-points
                        run.Append(runProperties);
                        run.Append(new Text(""));
                        paragraph.Append(run);
                        newCell.Append(paragraph);

                        newRow.Append(newCell);
                    }

                    table.Append(newRow);
                    rows.Add(newRow);
                    currentDataRows++;

                    _logger.LogInformation($"[DEBUG]   Đã thêm dòng mới, tổng dòng data: {currentDataRows}");
                }

                // Replace data trong các data rows
                int replacedCells = 0;
                for (int rowIndex = 1; rowIndex < rows.Count; rowIndex++)
                {
                    var dataRow = rows[rowIndex];
                    var dataCells = dataRow.Elements<TableCell>().ToList();

                    foreach (var colMapping in columnMapping)
                    {
                        var colIndex = colMapping.Key;
                        var tenTruong = colMapping.Value;

                        if (colIndex >= dataCells.Count)
                        {
                            _logger.LogWarning($"[DEBUG]   Row {rowIndex}, cột {colIndex} không tồn tại");
                            continue;
                        }

                        if (!tableDataMapping.TryGetValue(tenTruong, out var dataList))
                        {
                            _logger.LogWarning($"[DEBUG]   Không tìm thấy data cho trường '{tenTruong}'");
                            continue;
                        }

                        // IndexRowTable trong DB bắt đầu từ 1, List bắt đầu từ 0
                        // Nhưng trong code build mapping đã convert rồi, nên dataIndex = rowIndex - 1 là đúng
                        var dataIndex = rowIndex - 1;

                        _logger.LogInformation($"[DEBUG]   Row {rowIndex} -> Đang lấy dataIndex {dataIndex} từ dataList có {dataList.Count} phần tử");

                        if (dataIndex >= dataList.Count)
                        {
                            _logger.LogInformation($"[DEBUG]   Row {rowIndex}, dataIndex {dataIndex} vượt quá số dòng data ({dataList.Count})");
                            continue;
                        }

                        var data = dataList[dataIndex];
                        if (string.IsNullOrWhiteSpace(data))
                        {
                            _logger.LogInformation($"[DEBUG]   Row {rowIndex}, cột '{tenTruong}': data rỗng");
                            continue;
                        }

                        // Replace text trong cell
                        var cell = dataCells[colIndex];
                        var cellTexts = cell.Descendants<Text>().ToList();

                        if (cellTexts.Any())
                        {
                            // Clear existing text và set text mới
                            cellTexts[0].Text = data;
                            for (int i = 1; i < cellTexts.Count; i++)
                            {
                                cellTexts[i].Text = "";
                            }
                            _logger.LogInformation($"[DEBUG]   Row {rowIndex}, cột '{tenTruong}': replaced = '{data}'");
                            replacedCells++;
                        }
                        else
                        {
                            // Tạo paragraph và text mới nếu cell rỗng - Font size 12
                            var paragraph = new Paragraph();
                            var run = new Run();
                            var runProperties = new RunProperties();
                            runProperties.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                            runProperties.Append(new FontSize { Val = "24" }); // 12pt = 24 half-points
                            run.Append(runProperties);
                            run.Append(new Text(data));
                            paragraph.Append(run);
                            cell.Append(paragraph);
                            _logger.LogInformation($"[DEBUG]   Row {rowIndex}, cột '{tenTruong}': created new = '{data}'");
                            replacedCells++;
                        }
                    }
                }

                _logger.LogInformation($"[DEBUG]   Đã replace {replacedCells} cells trong bảng #{tableIndex}");
            }
        }
        public byte[] GenerateSoYeuLyLichTemplate()
        {
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
            var table = new Table();
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
            AddParagraph(body, "Bậc lương ........................... Lương theo mức tiền: .....................vnđ. Ngày hưởng: ...../...../.", false, 24);
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
            AddTableWithHeaders(body, new[] { "Tháng/\nnăm\nTừ", "Tháng/\nnăm\nĐến", "Đơn vị công tác (đảng, chính quyền,\n\nđoàn thể, tổ chức xã hội)", "Chức danh/\nchức vụ" }, 3);
        }

        private void AddPersonalHistorySection(Body body)
        {
            AddParagraph(body, "34) ĐẶC ĐIỂM LỊCH SỬ BẢN THÂN", true, 24, JustificationValues.Center);
            AddParagraph(body, "34.1- Khai rõ: bị bắt, bị tù (từ ngày tháng năm nào đến ngày tháng năm nào, ở đâu?), đã khai báo cho ai, những vấn đề gì?: .....................................................................", false, 24);

            AddParagraph(body, "34.2- Bản thân có làm việc cho chế độ cũ", false, 24);
            AddTableWithHeaders(body, new[] { "Tháng/\nnăm\nTừ", "Tháng/\nnăm\nĐến", "Chức danh, chức vụ, đơn vị, địa điểm đã làm việc" }, 3);

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
            AddParagraph(body, "Nhận xét, đánh giá \n ................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................", false, 24);
            AddParagraph(body, "", false, 24);

            var table = new Table();
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
            var table = new Table();
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

        private string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            // Loại bỏ xuống dòng, tab, khoảng trắng thừa
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[\r\n\t]+", " ");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");

            // Loại bỏ prefix dạng "số.số-" hoặc "số-" ở đầu (ví dụ: "0.1-", "30.1-", "32.1)")
            text = System.Text.RegularExpressions.Regex.Replace(text, @"^\d+\.?\d*[\-\)]\s*", "");

            // Xử lý trường hợp đặc biệt: "Sinh ngày ... tháng ... năm" -> "Sinh ngày/tháng/năm"
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                @"Sinh\s+ngày[:\s.]*tháng[:\s.]*năm",
                "Sinh ngày/tháng/năm",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            // Loại bỏ dấu hai chấm và tất cả các ký tự phía sau (dấu chấm, khoảng trắng...)
            // Ví dụ: "Giới tính:........." -> "Giới tính"
            // Ví dụ: "Dân tộc:........" -> "Dân tộc"
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[:.\s]+$", "");

            return text.Trim();
        }
    }
}