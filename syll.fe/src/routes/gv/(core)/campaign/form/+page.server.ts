import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import type { IViewForm } from "$lib/models/form/manage-form.model";
import type { IBaseResponsePaging } from "$lib/models/shared/base-response";
import type { PageServerLoad } from "../$types";

export const load: PageServerLoad = async ({ fetch, cookies,url }) => {
    const page = parseInt(url.searchParams.get('page') || '1');
    const idChienDich = parseInt(url.searchParams.get('idChienDich') || '0');
    const pageSize = 8; 

    const params = new URLSearchParams({
        pageSize: pageSize.toString(),
        pageNumber: page.toString(),
        idChienDich :idChienDich.toString()
    });
    const endpoint = `${API_BASE_URL}${ENDPOINTS.getPagingFormLoaiChienDich}?` + params;
    console.log('LOAD DATA: ', endpoint);

    const res = await fetch(endpoint, {
        method: 'GET'
    });

    let formData :IViewForm[] = [];
    let currentPage = 1;
    let totalPages = 1;

    if (!res.ok) {
        return {
            data: [],
            currentPage: currentPage,
            totalPages: totalPages,
        };
    }

    const data: IBaseResponsePaging<IViewForm> = await res.json();

    if (data.status === 1) {
        formData = data.data.items;
        currentPage = page;
        totalPages = Math.ceil(data.data.totalItems / pageSize);
    }

    return { 
        data: formData,
        currentPage: currentPage,
        totalPages: totalPages
     };
};
