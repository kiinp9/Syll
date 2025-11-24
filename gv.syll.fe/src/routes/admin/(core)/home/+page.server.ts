
import type { PageServerLoad } from "./$types";
import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import type { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from '$lib/models/shared/base-response';
import type { IViewDashBoard } from "$lib/models/dashboard/dashboard.models";
import { error } from "@sveltejs/kit";
export const load: PageServerLoad = async ({ fetch, cookies, url, params }) => {

      const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getDashboardData}`, {
            method: 'GET'
        });
        if (!res.ok) {
            throw error(500, 'Có sự cố xảy ra. Vui lòng thử lại sau');
        }
        const data: IBaseResponseWithData<IViewDashBoard> = await res.json();
    
        if (data.status !== 1) {
            throw error(400, 'Không thể tải báo cáo. Vui lòng thử lại sau');
        }

    return {
        totalData: data.data,
    }
    
}
