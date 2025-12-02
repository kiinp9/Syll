import type { Page } from "$lib/components/ui/breadcrumb";

import { API_BASE_URL } from '$env/static/private';
import type { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from '$lib/models/shared/base-response';
import { ENDPOINTS } from "$lib/api/endpoint";
import { error } from "@sveltejs/kit";
import type { IViewNhanVienToChucReport, IViewTotalNhanVienToChucReport, IViewNhanVienByToChucReport } from "$lib/models/report/report.models";
import type { PageServerLoad } from "../../$types";


export const load: PageServerLoad = async ({ fetch, cookies, url, params }) => {
    const idForm = (params as any)?.id;

    const page = parseInt(url.searchParams.get('page') || '1');
    const pageSizeToChuc = 12;
    const pageSizeNhanVienByToChuc = 10;

    //load report nhân viên tổ chức total data
    const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getReportTotalNhanVienToChuc(Number(idForm))}`, {
        method: 'GET'
    });
    if (!res.ok) {
        throw error(500, 'Có sự cố xảy ra. Vui lòng thử lại sau');
    }
    const data: IBaseResponseWithData<IViewTotalNhanVienToChucReport> = await res.json();

    if (data.status !== 1) {
        throw error(400, 'Không thể tải báo cáo. Vui lòng thử lại sau');
    }

    //load paging to chuc data
    const queryParamsToChuc = new URLSearchParams({
        pageSize: pageSizeToChuc.toString(),
        pageNumber: page.toString()
    });

    const resToChuc = await fetch(`${API_BASE_URL}${ENDPOINTS.getReportNhanVienToChucPaging(Number(idForm))}?` + queryParamsToChuc, {
        method: 'GET'
    });

    let toChucNhanVienData: IViewNhanVienToChucReport[] = [];
    let toChucNhanVienCurrentPage = 1;
    let toChucNhanVienTotalPages = 1;
    if (resToChuc.ok) {
        const dataToChuc: IBaseResponsePaging<IViewNhanVienToChucReport> = await resToChuc.json();
        if (dataToChuc.status === 1) {
            toChucNhanVienData = dataToChuc.data.items;
            toChucNhanVienCurrentPage = page;
            toChucNhanVienTotalPages = Math.ceil(dataToChuc.data.totalItems / pageSizeToChuc);
        }
    }

    //load paging nhan vien by to chuc data
    const idToChuc = parseInt(url.searchParams.get('idToChuc') || '0');
    const keyword = url.searchParams.get('keyword') || '';
    const statusFilter = url.searchParams.get('status') ? parseInt(url.searchParams.get('status')!) : null;

    const queryParamsNhanVienByToChuc = new URLSearchParams({
        idToChuc: idToChuc?.toString() || '',
        pageSize: pageSizeNhanVienByToChuc.toString(),
        pageNumber: page.toString(),
        keyword: keyword || '',
        status: statusFilter?.toString() || ''
    });

    const resNhanVienByToChuc = await fetch(`${API_BASE_URL}${ENDPOINTS.getReportNhanVienByToChucPaging(Number(idForm), idToChuc)}?` + queryParamsNhanVienByToChuc, {
        method: 'GET'
    });

    let nhanVienData: IViewNhanVienByToChucReport[] = [];
    let nhanVienCurrentPage = 1;
    let nhanVienTotalPages = 1;

    if (resNhanVienByToChuc.ok) {
        const dataNhanVien: IBaseResponsePaging<IViewNhanVienByToChucReport> = await resNhanVienByToChuc.json();
        if (dataNhanVien.status === 1) {
            nhanVienData = dataNhanVien.data.items;
            nhanVienCurrentPage = page;
            nhanVienTotalPages = Math.ceil(dataNhanVien.data.totalItems / pageSizeNhanVienByToChuc);
        }
    }

    return {
        totalData: data.data,
        toChuc: {
            data: toChucNhanVienData,
            currentPage: toChucNhanVienCurrentPage,
            totalPages: toChucNhanVienTotalPages
        },
        nhanVien: {
            data: nhanVienData,
            currentPage: nhanVienCurrentPage,
            totalPages: nhanVienTotalPages
        }
    }
}