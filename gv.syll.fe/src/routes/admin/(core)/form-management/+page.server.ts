import { API_BASE_URL } from '$env/static/private';
import { ENDPOINTS } from '$lib/api/endpoint';
import type { IViewForm } from '$lib/models/form/manage-form.model';
import type { IBaseResponsePaging } from '$lib/models/shared/base-response';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch, cookies }) => {
	const params = new URLSearchParams({
		pageSize: '10',
        pageNumber: '1',
	});
	const endpoint = `${API_BASE_URL}${ENDPOINTS.getFormPaging}?` + params;
    console.log('LOAD DATA: ', endpoint);
    
	const res = await fetch(endpoint, {
        method: 'GET'
	});

	if (!res.ok) {
		return {
			data: []
		};
	}

	const data: IBaseResponsePaging<IViewForm> = await res.json();

	if (data.status === 1) {
		return { data: data.data.items };
	}

	return { data: [] };
};
