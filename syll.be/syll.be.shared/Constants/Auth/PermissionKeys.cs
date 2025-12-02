using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.shared.Constants.Auth
{
    public static class PermissionKeys
    {
        public const string Menu = "Menu.";
        public const string Function = "Function.";

        public const string MenuUserManagement = Menu + "UserManagement";
        public const string MenuUserManagementUser = MenuUserManagement + "_User";
        public const string MenuUserManagementRole = MenuUserManagement + "_User";


        public const string CategoryUser = "QL User";
        public const string UserAdd = Function + "UserAdd";
        public const string UserUpdate = Function + "UserUpdate";
        public const string UserDelete = Function + "UserDelete";
        public const string UserView = Function + "UserView";
        public const string UserSetRoles = Function + "UserSetRoles";

        public const string CategoryToChuc = "QL Tổ chức";
        public const string ToChucAdd = Function + "ToChucAdd";
        public const string ToChucUpdate = Function + "ToChucUpdate";
        public const string ToChucDelete = Function + "ToChucDelete";
        public const string ToChucView = Function + "ToChucView";

        public const string CategoryDanhBa = "QL Danh bạ";
        public const string DanhBaAdd = Function + "DanhBaAdd";
        public const string DanhBaUpdate = Function + "DanhBaUpdate";
        public const string DanhBaDelete = Function + "DanhBaDelete";
        public const string DanhBaView = Function + "DanhBaView";
        public const string DanhBaImport = Function + "DanhBaImport";


        public const string CategoryForm = "OL Form";
        public const string FormAdd = Function + "FormAdd";
        public const string FormUpdate = Function + "FormUpdate";
        public const string FormDelete = Function + "FormDelete";
        public const string FormView = Function + "FormView";
        public const string FormImport = Function + "FormImport";



        public const string CategoryChienDich = "OL Chiến dịch";
        public const string ChienDichAdd = Function + "ChienDichAdd";
        public const string ChienDichUpdate = Function + "ChienDichUpdate";
        public const string ChienDichDelete = Function + "ChienDichDelete";
        public const string ChienDichView = Function + "ChienDichView";






        public const string CategoryReport = "OL Thống kê";
        public const string ReportView = Function + "ReportView";

        public static readonly (string Key, string Name, string Category)[] All =
        {
            (MenuUserManagement, "Menu Quản lý User", "Menu"),
            (MenuUserManagementUser, "Menu Quản lý User - User", "Menu"),
            (MenuUserManagementRole, "Menu Quản lý User - Role", "Menu"),

            (UserAdd, "Thêm user", CategoryUser),
            (UserUpdate, "Cập nhật User" , CategoryUser),
            (UserDelete, "Xoá User" , CategoryUser),
            (UserView, "Xem User" , CategoryUser),
            (UserSetRoles, "Gán role cho User" , CategoryUser),

            (ToChucAdd, "Thêm Tổ chức ", CategoryToChuc),
            (ToChucUpdate, "Cập nhật Tổ chức", CategoryToChuc),
            (ToChucDelete, "Xoá Tổ chức", CategoryToChuc),
            (ToChucView, "Xem Tổ chức", CategoryToChuc),

            (DanhBaAdd, "Thêm Danh bạ ", CategoryDanhBa),
            (DanhBaUpdate, "Cập nhật Danh bạ", CategoryDanhBa),
            (DanhBaDelete, "Xoá Danh bạ", CategoryDanhBa),
            (DanhBaView, "Xem Danh bạ", CategoryDanhBa),
            (DanhBaImport,"Import Danh Bạ",DanhBaImport),


            (FormAdd, "Thêm Form ", CategoryForm),
            (FormUpdate, "Cập nhật Form", CategoryForm),
            (FormDelete, "Xoá Form", CategoryForm),
            (FormView, "Xem Form", CategoryForm),
            (FormImport,"Import Form", CategoryForm),

            (ChienDichAdd, "Thêm Chiến dịch ", CategoryChienDich),
            (ChienDichUpdate, "Cập nhật Chiến dịch", CategoryChienDich),
            (ChienDichDelete, "Xoá Chiến dịch", CategoryChienDich),
            (ChienDichView, "Xem Chiến dịch", CategoryChienDich),

            (ReportView, "Xem Thống kê", CategoryReport),

        };
    }
}
