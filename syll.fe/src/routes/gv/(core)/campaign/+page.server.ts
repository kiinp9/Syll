import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import type { IViewChienDich } from "$lib/models/campaign/campaign.model";
import type { IBaseResponsePaging } from "$lib/models/shared/base-response";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ fetch, cookies, url }) => {
    const page = parseInt(url.searchParams.get('page')  || '1');
    const pageSize = 10;
    const params = new URLSearchParams({
        pageSize: pageSize.toString(),
        pageNumber :page.toString(),
    }) 
    const endpoint = `${API_BASE_URL}${ENDPOINTS.getChienDichPaging}?` + params;
    console.log('LOAD DATA: ', endpoint);

    const res = await fetch(endpoint, {
        method: 'GET'
    });
    let chienDichData : IViewChienDich[] = [];
    let currentPage = 1;
    let totalPages = 1;

    if (!res.ok) {
        return {
            data: [],
            currentPage: currentPage,
            totalPages: totalPages,
        };
    }

    const data: IBaseResponsePaging<IViewChienDich> = await res.json();
    if (data.status === 1) {
        chienDichData = data.data.items;
        currentPage = page;
        totalPages =Math.ceil(data.data.totalItems /pageSize);

    }

    return { 
        data: chienDichData,
        currentPage: currentPage,
        totalPages: totalPages
    };
};
