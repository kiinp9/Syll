using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using syll.be.lib.Form.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.lib.Form.Implements
{
    public class FormTemplateService : IFormTemplateService
    {
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
            runProps1.Append(new FontSize { Val = "22" });
            run1.Append(runProps1);
            run1.Append(new Text("Cơ quan quản lý viên chức: ") { Space = SpaceProcessingModeValues.Preserve });
            p1.Append(run1);

            var run2 = new Run();
            var runProps2 = new RunProperties();
            runProps2.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
            runProps2.Append(new FontSize { Val = "22" });
            runProps2.Append(new Bold());
            runProps2.Append(new BoldComplexScript());
            run2.Append(runProps2);
            run2.Append(new Text("TRƯỜNG ĐẠI HỌC XÂY DỰNG HÀ NỘI.") { Space = SpaceProcessingModeValues.Preserve });
            p1.Append(run2);
            body.Append(p1);

            AddParagraph(body, "Đơn vị sử dụng viên chức: ......................................", false, 22);
            AddParagraph(body, "Số hiệu: ....................", false, 22);
            AddParagraph(body, "Mã số định danh:.........................................................................................................", false, 22);
            AddParagraph(body, "", false, 22);
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

            var cellPhoto = new TableCell();
            var cellPhotoProperties = new TableCellProperties(
                new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "1700" },
                new TableCellBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                )
            );
            cellPhoto.Append(cellPhotoProperties);
            cellPhoto.Append(CreateParagraphWithText("Ảnh màu", true, 22, JustificationValues.Center));
            cellPhoto.Append(CreateParagraphWithText("(4 x 6 cm)", false, 20, JustificationValues.Center));
            row.Append(cellPhoto);

            var cellInfo = new TableCell();
            cellInfo.Append(CreateParagraphWithText("SƠ YẾU LÝ LỊCH", true, 32, JustificationValues.Center));
            cellInfo.Append(CreateParagraphWithText("", false, 22));
            cellInfo.Append(CreateParagraphWithText("1) Họ và tên khai sinh (viết chữ in hoa):.............................. Giới tính:..........", false, 22));
            cellInfo.Append(CreateParagraphWithText("2) Các tên gọi khác: .............................................................................", false, 22));
            cellInfo.Append(CreateParagraphWithText("3) Sinh ngày: ...... tháng ...... năm .........................................................", false, 22));
            cellInfo.Append(CreateParagraphWithText("4) Nơi sinh:.............................................................................................", false, 22));
            cellInfo.Append(CreateParagraphWithText("5) Quê quán (xã, phường): ............ ................tỉnh, TP): ............................", false, 22));
            row.Append(cellInfo);

            table.Append(row);
            body.Append(table);
            AddParagraph(body, "", false, 22);
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
                AddParagraph(body, field, false, 22);
            }
        }

        private void AddSalarySection(Body body)
        {
            AddParagraph(body, "30) Tiền lương", true, 22);
            AddParagraph(body, "30.1) Ngạch/chức danh nghề nghiệp: .................................................. Mã số:...........", false, 22);
            AddParagraph(body, "Ngày bổ nhiệm ngạch/chức danh nghề nghiệp: ......../......../...............................", false, 22);
            AddParagraph(body, "Bậc lương: ...................... Hệ số: ..................... Ngày hưởng: ......../......../.......", false, 22);
            AddParagraph(body, "Phần trăm hưởng:....%; Phụ cấp thâm niên vượt khung:...%; Ngày hưởng PCTNVK:.../.../ ...", false, 22);
            AddParagraph(body, "30.2) Phụ cấp chức vụ: ............ Phụ cấp kiêm nhiệm .................... Phụ cấp khác.........", false, 22);
            AddParagraph(body, "30.3) Vị trí việc làm: ......................................................................... Mã số:.........", false, 22);
            AddParagraph(body, "Bậc lương ........................... Lương theo mức tiền: .....................vnđ. Ngày hưởng: ...../...../.", false, 22);
            AddParagraph(body, "Phần trăm hưởng:...%; Phụ cấp thâm niên vượt khung:..%; Ngày hưởng PCTNVK: ...../..../...", false, 22);
        }

        private void AddHealthSection(Body body)
        {
            AddParagraph(body, "31) Tình trạng sức khoẻ:..................", false, 22);
            AddParagraph(body, "Chiều cao: .......................cm, Cân nặng: .................... kg, Nhóm máu:.......", false, 22);
        }

        private void AddTrainingSection(Body body)
        {
            AddParagraph(body, "32) QUÁ TRÌNH ĐÀO TẠO, BỒI DƯỠNG", true, 22, JustificationValues.Center);

            AddParagraph(body, "32.1- Chuyên môn (từ trung cấp trở lên cả trong nước và nước ngoài)", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Tên cơ sở đào tạo", "Chuyên ngành đào tạo", "Hình thức\nđào tạo", "Văn bằng,\ntrình độ" }, 3);

            AddParagraph(body, "32.2- Lý luận chính trị", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Tên cơ sở đào tạo", "Hình thức\nđào tạo", "Văn bằng được cấp" }, 3);

            AddParagraph(body, "32.3- Bồi dưỡng quản lý nhà nước/ chức danh nghề nghiệp/ nghiệp vụ chuyên ngành", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Tên cơ sở đào tạo", "Chứng chỉ được cấp" }, 3);

            AddParagraph(body, "32.4- Bồi dưỡng kiến thức an ninh, quốc phòng", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Tên cơ sở đào tạo", "Chứng chỉ được cấp" }, 2);

            AddParagraph(body, "32.5- Tin học", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Tên cơ sở đào tạo", "Chứng chỉ được cấp" }, 3);

            AddParagraph(body, "32.6- Ngoại ngữ/ tiếng dân tộc", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Tên cơ sở đào tạo", "Tên ngoại ngữ/\ntiếng dân tộc", "Chứng chỉ\nđược cấp", "Điểm số" }, 3);
        }

        private void AddWorkHistorySection(Body body)
        {
            AddParagraph(body, "33) TÓM TẮT QUÁ TRÌNH CÔNG TÁC", true, 22, JustificationValues.Center);
            AddTableWithHeaders(body, new[] { "Tháng/\nnăm\nTừ", "Tháng/\nnăm\nĐến", "Đơn vị công tác (đảng, chính quyền,\n\nđoàn thể, tổ chức xã hội)", "Chức danh/\nchức vụ" }, 3);
        }

        private void AddPersonalHistorySection(Body body)
        {
            AddParagraph(body, "34) ĐẶC ĐIỂM LỊCH SỬ BẢN THÂN", true, 22, JustificationValues.Center);
            AddParagraph(body, "34.1- Khai rõ: bị bắt, bị tù (từ ngày tháng năm nào đến ngày tháng năm nào, ở đâu?), đã khai báo cho ai, những vấn đề gì?: .....................................................................", false, 22);

            AddParagraph(body, "34.2- Bản thân có làm việc cho chế độ cũ", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/\nnăm\nTừ", "Tháng/\nnăm\nĐến", "Chức danh, chức vụ, đơn vị, địa điểm đã làm việc" }, 3);

            AddParagraph(body, "34.3-Tham gia hoặc có quan hệ với các tổ chức chính trị, kinh tế, xã hội ... ở nước ngoài", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Tên tổ chức, địa chỉ trụ sở, công việc đã làm" }, 3);
        }

        private void AddRewardSection(Body body)
        {
            AddParagraph(body, "35) KHEN THƯỞNG, KỶ LUẬT", true, 22, JustificationValues.Center);

            AddParagraph(body, "35.1- Thành tích thi đua, khen thưởng", false, 22);
            AddTableWithHeaders(body, new[] { "Năm", "Xếp loại chuyên môn", "Xếp loại thi đua", "Hình thức khen\nthưởng" }, 3);

            AddParagraph(body, " 35.2-Kỷ luật Đảng/hành chính", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Hình thức", "Hành vi vi phạm chính", "Cơ quan quyết\nđịnh" }, 3);
        }

        private void AddFamilySection(Body body)
        {
            AddParagraph(body, "36) QUAN HỆ GIA ĐÌNH", true, 22, JustificationValues.Center);

            AddParagraph(body, "36.1- Về bản thân: Cha, Mẹ, Vợ (hoặc chồng), các con, anh chị em ruột", false, 22);
            AddTableWithHeaders(body, new[] { "Mối\nquan\nhệ", "Họ và\ntên", "Năm\nsinh", "Quê quán, nghề nghiệp, chức danh, chức vụ, đơn vị công\ntác, học tập, nơi ở (trong, ngoài nước); thành viên các\ntổ chức chính trị - xã hội (trong, ngoài nước); làm việc\ncho chế độ cũ, tiền án, tiền sự (nếu có)" }, 3);

            AddParagraph(body, "36.2- Cha, Mẹ, anh chị em ruột (bên vợ hoặc chồng)", false, 22);
            AddTableWithHeaders(body, new[] { "Mối quan\nhệ", "Họ và\ntên", "Năm\nsinh", "Quê quán, nghề nghiệp, chức danh, chức vụ, đơn vị công\ntác, học tập, nơi ở (trong, ngoài nước); thành viên các\ntổ chức chính trị - xã hội (trong, ngoài nước); làm việc\ncho chế độ cũ, tiền án, tiền sự (nếu có)" }, 2);
        }

        private void AddEconomicSection(Body body)
        {
            AddParagraph(body, "37) HOÀN CẢNH KINH TẾ GIA ĐÌNH", true, 22, JustificationValues.Center);

            AddParagraph(body, "37.1- Quá trình lương của bản thân", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Mã số", "Bậc lương", "Hệ số lương", "Tiền lương theo vị trí\nviệc làm" }, 2);

            AddParagraph(body, "37.2- Các loại phụ cấp khác", false, 22);
            AddTableWithHeaders(body, new[] { "Tháng/năm\nTừ", "Tháng/năm\nĐến", "Loại phụ cấp", "Phần trăm hưởng", "Hệ số", "Hình thức\nhưởng", "Giá trị\n(đồng)" }, 3);

            AddParagraph(body, "37.3- Nguồn thu nhập chính của gia đình hàng năm", false, 22);
            AddParagraph(body, "- Tiền lương: .................................................................................................", false, 22);
            AddParagraph(body, "- Các nguồn khác: ................................................................................................", false, 22);
            AddParagraph(body, "- Nhà ở:", false, 22);
            AddParagraph(body, "+ Được cấp, được thuê (loại nhà): ................., tổng diện tích sử dụng: ...........m2.", false, 22);
            AddParagraph(body, "Giấy chứng nhận quyền sở hữu:...............................................................", false, 22);
            AddParagraph(body, "+ Nhà tự mua, tự xây (loại nhà): .........., tổng diện tích sử dụng: ...............m2.", false, 22);
            AddParagraph(body, "Giấy chứng nhận quyền sở hữu:....", false, 22);
            AddParagraph(body, "- Đất ở:", false, 22);
            AddParagraph(body, "+ Đất được cấp: ............................. m2.", false, 22);
            AddParagraph(body, "Giấy chứng nhận quyền sử dụng:....", false, 22);
            AddParagraph(body, "+ Đất tự mua: ...................................m2.", false, 22);
            AddParagraph(body, "Giấy chứng nhận quyền sử dụng:...", false, 22);
            AddParagraph(body, "- Đất sản xuất kinh doanh: ..............................................................................................................................................................................................................................................................................................................................................................", false, 22);
        }

        private void AddFooterSection(Body body)
        {
            AddParagraph(body, "38) NHẬN XÉT, ĐÁNH GIÁ CỦA CƠ QUAN, TỔ CHỨC, ĐƠN VỊ SỬ DỤNG", true, 22, JustificationValues.Center);
            AddParagraph(body, ".................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................................", false, 22);
            AddParagraph(body, "", false, 22);

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
            cell1.Append(CreateParagraphWithText("Người khai", true, 22, JustificationValues.Center));
            cell1.Append(CreateParagraphWithText("Tôi xin cam đoan những lời", false, 22, JustificationValues.Center));
            cell1.Append(CreateParagraphWithText("khai trên đây là đúng sự thật", false, 22, JustificationValues.Center));
            cell1.Append(CreateParagraphWithText("", false, 22));
            cell1.Append(CreateParagraphWithText("(Ký tên, ghi rõ họ tên)", false, 22, JustificationValues.Center));
            row.Append(cell1);

            var cell2 = new TableCell();
            cell2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "3000" }));
            cell2.Append(CreateParagraphWithText("............, Ngày.......tháng.........năm 20......", false, 22, JustificationValues.Center));
            cell2.Append(CreateParagraphWithText("", false, 22));
            cell2.Append(CreateParagraphWithText("HIỆU TRƯỞNG", true, 22, JustificationValues.Center));
            cell2.Append(CreateParagraphWithText("", false, 22));
            cell2.Append(CreateParagraphWithText("(Ký tên, đóng dấu)", false, 22, JustificationValues.Center));
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
                    cell.Append(CreateParagraphWithText("", false, 22));
                    dataRow.Append(cell);
                }
                table.Append(dataRow);
            }

            body.Append(table);
            AddParagraph(body, "", false, 22);
        }
    }
}