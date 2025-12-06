export interface IViewChienDich{
    id: number
    tenChienDich: string
    moTa : string
    ngayTao : Date
    thoiGianBatDau: Date
    thoiGianKetThuc: Date
}

export interface IViewChienDichById{
    id: number
    tenChienDich: string
    moTa : string
    //ngayTao : Date
    thoiGianBatDau: Date
    thoiGianKetThuc: Date
    formLoais: IViewFormLoaiChienDich[]
}

export interface IViewFormLoaiChienDich{
    idFormLoai:number
    tenFormLoai: string
}

export interface IViewForm{
    id: number
    tenForm: string
    moTa :string
    tongSoTruong: number
    thoiGianTao?: Date
    thoiGianCapNhatGanNhat? :Date
    thoiGianBatDau?: Date
    thoiGianKetThuc?: Date
}