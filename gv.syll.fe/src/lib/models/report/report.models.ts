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
