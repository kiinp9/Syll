import { API_BASE_URL } from '$env/static/private';
import { ENDPOINTS } from '$lib/api/endpoint';
import type { IViewForm } from '$lib/models/form/manage-form.model';
import type { IBaseResponse, IBaseResponsePaging } from '$lib/models/shared/base-response';
import type { PageServerLoad, Actions } from './$types';

export const load: PageServerLoad = async ({ fetch, cookies }) => {
	const params = new URLSearchParams({
		pageSize: '10',
		pageNumber: '1'
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

export const actions: Actions = {
	createForm: async ({ request, cookies, fetch }) => {
		const formData = await request.formData();
		console.log(formData);

		const body = {
			tenLoaiForm: formData.get('tenLoaiForm'),
			moTa: formData.get('moTa'),
			thoiGianBatDau: new Date(),
			thoiGianKetThuc: new Date(),
		};

		const res = await fetch(`${API_BASE_URL}${ENDPOINTS.createForm}`, {
			method: 'POST',
			body: JSON.stringify(body),
			headers: {
				'Content-Type': 'application/json'
			}
		});

		if (!res.ok) {
			const failedRes: IBaseResponse = {
				message: 'Có sự cố xảy ra',
				code: -1,
				status: 0
			};
			return failedRes;
		}

		const data: IBaseResponse = await res.json();
		return data;
	}
} satisfies Actions;
