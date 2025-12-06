import { API_BASE_URL } from '$env/static/private';
import { ENDPOINTS } from '$lib/api/endpoint';
import type { IViewForm } from '$lib/models/campaign/campaign.model';
import type { IViewFormById } from '$lib/models/form/form.models';

import type { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from '$lib/models/shared/base-response';
import type { Actions, PageServerLoad } from '../../form/$types';


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

export const actions: Actions = {
	createForm: async ({ request, cookies, fetch }) => {
		const formData = await request.formData();
		console.log(formData);

		const body = {
			idChienDich: Number(formData.get('idChienDich')),
			tenLoaiForm: formData.get('tenLoaiForm'),
			moTa: formData.get('moTa'),
			thoiGianBatDau: formData.get('thoiGianBatDau') || null,
            thoiGianKetThuc: formData.get('thoiGianKetThuc') || null,
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
	},
	updateForm : async ({request,cookies, fetch}) => {
        const formData = await request.formData();
        console.log(formData);
        const body ={
            idChienDich: Number(formData.get('idChienDich')),
			id :Number(formData.get('id')),
            ten: formData.get('ten'),
            moTa: formData.get('moTa'),
            thoiGianBatDau: formData.get('thoiGianBatDau') || null,
            thoiGianKetThuc: formData.get('thoiGianKetThuc') || null,
            
        };

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.updateForm}`, {
            method: 'PUT',
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
    },
	getFormById: async ({request, cookies, fetch, params}) => {
			const formData = await request.formData();
	        const idChienDich = Number(formData.get('idChienDich'));
			const id = Number(formData.get('id'));
	
			const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getFormById(idChienDich, id)}`, {
				method: 'GET',
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
	
			const data: IBaseResponseWithData<IViewFormById> = await res.json();
			if (data.status === 1 && data.data) {
				return {
					status: data.status,
					message: data.message,
					code: data.code,
					data: data.data
				};
			}
	
			return data;
		},
	deleteForm: async ({ request, cookies, fetch }) => {
        const formData = await request.formData();
		const idChienDich = Number(formData.get('idChienDich'));
		const id = Number(formData.get('id'));
		console.log(formData);

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.deleteForm(idChienDich, id)}`, {
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

} satisfies Actions;
