using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.shared.HttpRequest.Error
{
    public static class ErrorMessages
    {
        private static readonly Dictionary<int, string> _messages = new()
        {
            { ErrorCodes.System, "Lỗi hệ thống" },
            { ErrorCodes.InternalServerError, "Lỗi server" },
            { ErrorCodes.BadRequest, "Request không hợp lệ" },
            { ErrorCodes.NotFound, "Không tìm thấy trong hệ thống" },
            { ErrorCodes.Unauthorized, "Không được phân quyền" },
            { ErrorCodes.AuthErrorUserNotFound, "User không tồn tại" },
            { ErrorCodes.AuthErrorRoleNotFound, "Role không tồn tại" },
            { ErrorCodes.AuthInvalidPassword, "Mật khẩu không đúng" },
            { ErrorCodes.AuthErrorCreateUser, "Lỗi tạo user" },
            { ErrorCodes.AuthErrorCreateRole, "Lỗi tạo role" },
            { ErrorCodes.Found, "Đã tồn tại trong hệ thống" },
            { ErrorCodes.ToChucErrorNotFound, "Tổ chức không tồn tại" },
            { ErrorCodes.ToChucErrorLoaiToChucNotFound,"Loại tổ chức không tồn tại" },
            { ErrorCodes.ServiceAccountErrorNotFound,"Không tìm thấy đường dẫn file service-account" },
            { ErrorCodes.GoogleSheetUrlErrorInvalid, "URL Google Sheet không hợp lệ hoặc không thể truy cập được" },
            { ErrorCodes.DanhBaErrorNotFound, "Danh bạ không tồn tại"},
            { ErrorCodes.DanhBaErrorNotFoundInToChuc, "Danh bạ không tồn tại trong tổ chức"},
            { ErrorCodes.FormLoaiErrorNotFound, "Form chưa được thiết lập. Vui lòng liên hệ admin"},
            { ErrorCodes.FormDanhBaErrorNotFound, "Người dùng trong danh bạ không đăng kí Form này"},
            { ErrorCodes.FormTruongDataErrorNotFound,"Trường dữ liệu không tồn tại"},
            { ErrorCodes.FormLoaiErrorLayoutNotFound, "Layout không tồn tại trong loại form này" },
            { ErrorCodes.FormLoaiErrorBlockNotFound, "Block không tồn tại" },
            { ErrorCodes.FormLoaiErrorBlockOrderInvalid,"Thứ tự của block không hợp lệ" },
            { ErrorCodes.FormLoaiErrorRowNotFound,"Row không tồn tại" },
            { ErrorCodes.FormLoaiErrorRowOrderInvalid,"Thứ tự của row không hợp lệ" },
            { ErrorCodes.FormLoaiErrorItemNotFound, "Item không tồn tại" },
            { ErrorCodes.FormLoaiErrorItemOrderInvalid,"Thứ tự item không hợp lệ" },
            { ErrorCodes.FormLoaiErrorItemTypeInvalid,"Loại item không hợp lệ" },
            { ErrorCodes.FormLoaiErrorLayoutOrderInvalid,"Thứ tự layout không hợp lệ" },
            { ErrorCodes.FormLoaiErrorTruongDataNotFound,"Trường data không tồn tại" },
            { ErrorCodes.TemplateErrorTemplateFormLoaiNotFound,"Template Form chưa được thiết lập. Vui lòng liên hệ admin" },
            { ErrorCodes.ToChucErrorToChucExisted, "Tổ chức đã tồn tại"},
            { ErrorCodes.FormDataErrorNotFound, "Dữ liệu form không tồn tại"},
            { ErrorCodes.FormLoaiErrorTableHeadersNotFound, "Tiêu đề bảng của form không tồn tại"},
            { ErrorCodes.FormLoaiErrorTableCellCountMismatch, "Số lượng ô trong bảng không khớp với số lượng tiêu đề"},

        };

        public static string GetMessage(int code)
        {
            return _messages.TryGetValue(code, out var message) ? message : "Unknown error.";
        }
    }

}

