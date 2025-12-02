export interface IViewTotalNhanVienToChucReport {
    totalNhanVien?: number;
    totalNhanVienCheckForm?: number;
    totalNhanVienChuaCheckForm?: number;
    totalNhanVienChuaImportData?: number;

}


export interface IViewNhanVienToChucReport {
    idToChuc?: number;
    tenToChuc?: string;
    totalNhanVienToChuc?: number;
    totalNhanVienToChucCheckForm?: number;
    totalNhanVienToChucChuaCheckForm?: number;
    progress?: number;
}


export interface IViewNhanVienByToChucReport {
    id?: number;
    hoVaTen?: string;
    email?: string;
    status?: number;
    lastModified?: Date;
    toChuc?: IViewToChucNhanVien;
}


export interface IViewToChucNhanVien {
    id?: number;
    tenToChuc? : string;
}