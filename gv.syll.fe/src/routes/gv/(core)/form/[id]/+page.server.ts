import type { Actions, PageServerLoad } from '../$types';
import { API_BASE_URL } from '$env/static/private'; // Not exposed to client
import type { IBaseResponse, IBaseResponseWithData } from '$lib/models/shared/base-response';
import { ENDPOINTS } from '$lib/api/endpoint';
import type { IViewFormLayout } from '$lib/models/form/form-layout.model';
import { error } from '@sveltejs/kit';

export const load: PageServerLoad = async ({ fetch, cookies, params }) => {

	const formId = (params as any)?.id;

	if (!formId) {
		throw error(404, 'Không tìm thấy form');
	}

	const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getGvLayout}/${formId}`, {
		method: 'GET'
	});

	if (!res.ok) {
		throw error(500, 'Có sự cố xảy ra. Vui lòng thử lại sau');
	}

	const data: IBaseResponseWithData<IViewFormLayout> = await res.json();

	if (data.status === 1) {
		return { data: data.data };
	}

	throw error(400, 'Form chưa được thiết lập. Vui lòng liên hệ admin');
};

export const actions: Actions = {
	default: async ({ request, fetch }) => {
		const formData = await request.formData();

		const truongMap = new Map<number, { idData: number; data: string }[]>();

		for (const [key, value] of formData.entries()) {
			const parts = key.split('_');
			if (parts.length === 2) {
				const idTruong = Number(parts[0]);
				const idData = Number(parts[1]);

				if (!Number.isNaN(idTruong) && !Number.isNaN(idData)) {
					if (!truongMap.has(idTruong)) {
						truongMap.set(idTruong, []);
					}
					truongMap.get(idTruong)!.push({
						idData: idData,
						data: value.toString()
					});
				}
			}
		}

		const body = {
			truongDatas: Array.from(truongMap.entries()).map(([idTruong, datas]) => ({
				idTruong: idTruong,
				datas: datas
			}))
		};

		const res = await fetch(`${API_BASE_URL}${ENDPOINTS.updateFormContent(1)}`, {
			method: 'PUT',
			body: JSON.stringify(body),
			headers: {
				'Content-Type': 'application/json'
			}
		});

		console.log('RES =>', JSON.stringify(body));

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
};
