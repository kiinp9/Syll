export interface IViewToChuc{
    id?: number;
    tenToChuc? : string;
    moTa?: string;
    soNhanVien?: number;
    loaiToChuc?: number;
    maSoToChuc? : string;
}


export interface IGetListToChucDropDown{
    id?: number;
    tenToChuc?: string;
}


export interface ICreateToChuc {
    tenToChuc ?: string;
    moTa?: string;
    loaiToChuc?: number;
    maSoToChuc? : string;
}


export interface IViewNhanVienToChuc{
    id?: number;
    hoVaTen?: string;
    email?: string;
    role?: IViewRoleNhanVien;
    toChuc?: IViewToChucNhanVien;
}

export interface IViewToChucNhanVien {
    id?: number;
    tenToChuc? : string;
}

export interface IViewRoleNhanVien{
    id?: string;
    name?: string;
}


export interface IGetListDropDownForm{
    id?: number;
    tenFormLoai?: string;
}