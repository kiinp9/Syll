import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import type { IViewChienDich, IViewChienDichById } from "$lib/models/campaign/campaign.model";
import type { IGetListDropDownFormLoai } from "$lib/models/organization/organization.models";

import type { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from "$lib/models/shared/base-response";
import type { Actions, PageServerLoad } from "./$types";

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

export const actions: Actions = {
    createChienDich: async ({ request, cookies, fetch }) => {
        const formData = await request.formData();
        console.log(formData);

        const body = {
            tenChienDich: formData.get('tenChienDich'),
            moTa: formData.get('moTa'),
            thoiGianBatDau: formData.get('thoiGianBatDau') || null,
            thoiGianKetThuc: formData.get('thoiGianKetThuc') || null,
            formLoais: JSON.parse(formData.get('formLoais') as string || '[]'),
        };

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.createChienDich}`, {
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

    getListDropDownFormLoai: async ({ request, cookies, fetch }) => {
        const formData = await request.formData();
        console.log(formData);

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getListDropDownForm}`, {
            method: 'GET',
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

        const data: IBaseResponseWithData<IGetListDropDownFormLoai[]> = await res.json();
        if(data.status === 1 && data.data){
            return{
                status:data.status,
                message:data.message,
                code:data.code,
                data:(data.data as IGetListDropDownFormLoai[]).map(item=>({
                    id: item.id,
                    tenFormLoai: item.tenFormLoai
                }))
            };
        }
        return data;
    },

    updateChienDich : async ({request,cookies, fetch}) => {
        const formData = await request.formData();
        console.log(formData);
        const body ={
            idChienDich: parseInt(formData.get('idChienDich') as string),
            tenChienDich: formData.get('tenChienDich'),
            moTa: formData.get('moTa'),
            thoiGianBatDau: formData.get('thoiGianBatDau') || null,
            thoiGianKetThuc: formData.get('thoiGianKetThuc') || null,
            formLoais: JSON.parse(formData.get('formLoais') as string || '[]')
        };

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.updateChienDich}`, {
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

    getChienDichById: async ({request, cookies, fetch}) => {
        const formData = await request.formData();


        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getChienDichById(Number(formData.get('idChienDich')))}`, {
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

        const data: IBaseResponseWithData<IViewChienDichById> = await res.json();
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
    deleteChienDich: async ({ request, cookies, fetch }) => {
        const formData = await request.formData();
        console.log(formData);

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.deleteChienDich(Number(formData.get('idChienDich')))}`, {
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