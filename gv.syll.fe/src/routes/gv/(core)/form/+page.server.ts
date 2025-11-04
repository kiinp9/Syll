import type { PageServerLoad } from './$types';
import { API_BASE_URL } from '$env/static/private'; // Not exposed to client
import type { IBaseResponseWithData } from '$lib/models/shared/base-response';
import { ENDPOINTS } from '$lib/api/endpoint';
import type { IViewFormLayout } from '$lib/models/form/form-layout.model';

export const load: PageServerLoad = async ({ fetch, cookies }) => {
	const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getGvLayout}/1`, {
        method: 'GET'
    });

    console.log('RES', res);

	if (!res.ok) {
		return {
			data: null
		};
	}

	const data: IBaseResponseWithData<IViewFormLayout> = await res.json();

	if (data.status === 1) {
		return { data: data.data };
	}

	return { data: null };
};
