import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import type { IViewNhanVienToChuc, IViewToChuc } from "$lib/models/organization/organization.models";
import type { IBaseResponse, IBaseResponsePaging } from "$lib/models/shared/base-response";
import type { Actions, PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ fetch, cookies, url }) => {
    const page = parseInt(url.searchParams.get('page') || '1');
    const idToChuc = parseInt(url.searchParams.get('idToChuc') || '0');
    const pageSizeToChuc = 12;
    const pageSizeNhanVien = 10;
    

    // Load ToChuc
    const paramsToChuc = new URLSearchParams({
        pageSize: pageSizeToChuc.toString(),
        pageNumber: page.toString()
    });
    const endpointToChuc = `${API_BASE_URL}${ENDPOINTS.getToChucPaging}?` + paramsToChuc;
    console.log('LOAD ToChuc DATA: ', endpointToChuc);

    const resToChuc = await fetch(endpointToChuc, {
        method: 'GET'
    });

    let toChucData: IViewToChuc[] = [];
    let toChucCurrentPage = 1;
    let toChucTotalPages = 1;

    if (resToChuc.ok) {
        const dataToChuc: IBaseResponsePaging<IViewToChuc> = await resToChuc.json();
        if (dataToChuc.status === 1) {
            toChucData = dataToChuc.data.items;
            toChucCurrentPage = page;
            toChucTotalPages = Math.ceil(dataToChuc.data.totalItems / pageSizeToChuc);
        }
    }

    // Load NhanVien
    const paramsNhanVien = new URLSearchParams({
        idToChuc: idToChuc?.toString() || '',
        pageSize: pageSizeNhanVien.toString(),
        pageNumber: page.toString()
    });
    const endpointNhanVien = `${API_BASE_URL}${ENDPOINTS.getPagingNhanVienToChuc}?` + paramsNhanVien;
    console.log('LOAD NhanVienToChuc DATA: ', endpointNhanVien);

    const resNhanVien = await fetch(endpointNhanVien, {
        method: 'GET'
    });

    let nhanVienData: IViewNhanVienToChuc[] = [];
    let nhanVienCurrentPage = 1;
    let nhanVienTotalPages = 1;

    if (resNhanVien.ok) {
        const dataNhanVien: IBaseResponsePaging<IViewNhanVienToChuc> = await resNhanVien.json();
        if (dataNhanVien.status === 1) {
            nhanVienData = dataNhanVien.data.items;
            nhanVienCurrentPage = page;
            nhanVienTotalPages = Math.ceil(dataNhanVien.data.totalItems / pageSizeNhanVien);
        }
    }

    return {
        toChuc: {
            data: toChucData,
            currentPage: toChucCurrentPage,
            totalPages: toChucTotalPages
        },
        nhanVien: {
            data: nhanVienData,
            currentPage: nhanVienCurrentPage,
            totalPages: nhanVienTotalPages
        },
      
    };
};

export const actions: Actions = {
    createToChuc: async ({ request, cookies, fetch }) => {
        const formData = await request.formData();
        console.log(formData);

        const body = {
            tenToChuc: formData.get('tenToChuc'),
            moTa: formData.get('moTa'),
            loaiToChuc: Number(formData.get('loaiToChuc')),
            maSoToChuc: formData.get('maSoToChuc'),
        };

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.createToChuc}`, {
            method: 'POST',
            body: JSON.stringify(body),
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!res.ok) {
            try {
                const errorData: IBaseResponse = await res.json();
                return errorData;
            } catch {
                const failedRes: IBaseResponse = {
                    message: 'Có sự cố xảy ra',
                    code: -1,
                    status: 0
                };
                return failedRes;
            }
        }

        const data: IBaseResponse = await res.json();
        return data;
    },

    deleteToChuc: async ({ request, cookies, fetch }) => {
        const formData = await request.formData();
        console.log(formData);

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.deleteToChuc(Number(formData.get('idToChuc')))}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!res.ok) {
            try {
                const errorData: IBaseResponse = await res.json();
                return errorData;
            } catch {
                const failedRes: IBaseResponse = {
                    message: 'Có sự cố xảy ra',
                    code: -1,
                    status: 0
                };
                return failedRes;
            }
        }

        const data: IBaseResponse = await res.json();
        return data;
    },

    getListDropDownFormLoai: async ({ request, cookies, fetch }) => {
        const formData = await request.formData();
        console.log(formData);

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getListDropDownForm}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!res.ok) {
            try {
                const errorData: IBaseResponse = await res.json();
                return errorData;
            } catch {
                const failedRes: IBaseResponse = {
                    message: 'Có sự cố xảy ra',
                    code: -1,
                    status: 0
                };
                return failedRes;
            }
        }

        const data: IBaseResponse = await res.json();
        return data;
    }
} satisfies Actions;