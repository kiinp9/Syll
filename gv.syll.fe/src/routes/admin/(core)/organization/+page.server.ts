import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import type { IViewToChuc } from "$lib/models/form/organization.models";
import type { IBaseResponsePaging } from "$lib/models/shared/base-response";
import type { PageServerLoad } from "./$types";

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
}