import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import type { IViewToChuc } from "$lib/models/organization/organization.models";
import type { IBaseResponse, IBaseResponsePaging } from "$lib/models/shared/base-response";
import type { Actions, PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ fetch, cookies,url }) => {
    const page = parseInt(url.searchParams.get('page') || '1');
    const pageSize = 12;

    const params = new URLSearchParams({
        pageSize: pageSize.toString(),
        pageNumber: page.toString()
    });
    const endpoint = `${API_BASE_URL}${ENDPOINTS.getToChucPaging}?` + params;
    console.log('LOAD DATA: ', endpoint);

    const res = await fetch(endpoint, {
        method: 'GET'
    });

    if (!res.ok) {
        return {
            data: []
        };
    }

    const data: IBaseResponsePaging<IViewToChuc> = await res.json();

    if (data.status === 1) {
        const totalPages = Math.ceil(data.data.totalItems / pageSize);
        return { 
            data: data.data.items,
            currentPage: page,
            totalPages: totalPages
        };
    }

    return { 
        data: [],
        currentPage: 1,
        totalPages: 1
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
    }
} satisfies Actions;
