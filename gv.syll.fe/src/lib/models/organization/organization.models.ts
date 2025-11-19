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