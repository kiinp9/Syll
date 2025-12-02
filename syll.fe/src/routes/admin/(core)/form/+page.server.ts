import { error, type Actions } from "@sveltejs/kit";

import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import type { IBaseResponse, IBaseResponseWithData } from "$lib/models/shared/base-response";
import type { IViewFormLayout } from "$lib/models/form/form-layout.model";
import type { PageServerLoad } from "./$types";



export const load: PageServerLoad = async ({ fetch, cookies, url }) => {
    const idFormLoai = parseInt(url.searchParams.get('idFormLoai') || '0');
    const idDanhBa = parseInt(url.searchParams.get('idDanhBa') || '0');
    const queryParams = new URLSearchParams({
        idFormLoai: idFormLoai?.toString() || '',
        idDanhBa: idDanhBa?.toString() || '',
    }); 

    /*if (!idForm) {
        throw error(404, 'Không tìm thấy form');
    }*/

    const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getGvLayoutForAdmin}?${queryParams}` , {
        method: 'GET'
    });
    console.log('FETCH FORM LAYOUT FOR ADMIN: ', `${API_BASE_URL}${ENDPOINTS.getGvLayoutForAdmin}?${queryParams}` );

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
    update: async ({ request, fetch, url }) => {
        const formData = await request.formData();
        const idFormLoai = parseInt(formData.get('idFormLoai')?.toString() || '0');
        const idDanhBa = parseInt(formData.get('idDanhBa')?.toString() || '0');
        const queryParams = new URLSearchParams({
            idFormLoai: idFormLoai?.toString() || '',
            idDanhBa: idDanhBa?.toString() || '',
        });



        const truongMap = new Map<number, { datas: { idData: number; data: string }[], tableRows: { idData: number; data: string }[][] }>();

        for (const [key, value] of formData.entries()) {
            if (key === 'idFormLoai' || key === 'idDanhBa') continue;

            const parts = key.split('_');

            if (parts.length === 5 && parts[1] === 'table') {
                const idTruong = Number(parts[0]);
                const rowIndex = Number(parts[2]);
                const cellIndex = Number(parts[3]);
                const idData = Number(parts[4]);

             

                if (!Number.isNaN(idTruong) && !Number.isNaN(rowIndex) && !Number.isNaN(cellIndex) && !Number.isNaN(idData)) {
                    if (!truongMap.has(idTruong)) {
                        truongMap.set(idTruong, { datas: [], tableRows: [] });
                    }

                    const truongData = truongMap.get(idTruong)!;
                    while (truongData.tableRows.length <= rowIndex) {
                        truongData.tableRows.push([]);
                    }
                    const row = truongData.tableRows[rowIndex];
                    while (row.length <= cellIndex) {
                        row.push({ idData: 0, data: '' });
                    }

                    row[cellIndex] = {
                        idData: idData,
                        data: value.toString()
                    };
                }
            }
            else if (parts.length === 2) {
                const idTruong = Number(parts[0]);
                const idData = Number(parts[1]);


                if (!Number.isNaN(idTruong) && !Number.isNaN(idData)) {
                    if (!truongMap.has(idTruong)) {
                        truongMap.set(idTruong, { datas: [], tableRows: [] });
                    }
                    truongMap.get(idTruong)!.datas.push({
                        idData: idData,
                        data: value.toString()
                    });
                }
            }
        }

        const body = {
            truongDatas: Array.from(truongMap.entries()).map(([idTruong, { datas, tableRows }]) => ({
                idTruong: idTruong,
                datas: datas,
                tableRows: tableRows.length > 0 ? tableRows : null
            }))
        };

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.updateFormContentAdmin}?${queryParams}`, {
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
    },



    deleteRow: async ({ request, fetch }) => {
        const formData = await request.formData();

        const truongMap = new Map<number, number[]>();

        for (const [key, value] of formData.entries()) {
            const parts = key.split('_');
            if (parts.length === 2) {
                const idTruong = Number(parts[0]);
                const idData = Number(parts[1]);

                if (!Number.isNaN(idTruong) && !Number.isNaN(idData)) {
                    if (!truongMap.has(idTruong)) {
                        truongMap.set(idTruong, []);
                    }
                    truongMap.get(idTruong)!.push(idData);
                }
            }
        }

        const body = {
            truongs: Array.from(truongMap.entries()).map(([idTruongData, idDatas]) => ({
                idTruongData: idTruongData,
                datas: idDatas.map((idData) => ({ idData }))
            }))
        };

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.deleteRowTableData}`, {
            method: 'DELETE',
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

    deleteBlock: async ({ request, fetch }) => {
        const formData = await request.formData();

        const truongMap = new Map<number, number[]>();

        for (const [key, value] of formData.entries()) {
            const parts = key.split('_');
            if (parts.length === 2) {
                const idTruong = Number(parts[0]);
                const idData = Number(parts[1]);

                if (!Number.isNaN(idTruong) && !Number.isNaN(idData) && idData > 0) {
                    if (!truongMap.has(idTruong)) {
                        truongMap.set(idTruong, []);
                    }
                    truongMap.get(idTruong)!.push(idData);
                }
            }
        }

        const body = {
            truongs: Array.from(truongMap.entries()).map(([idTruongData, idDatas]) => ({
                idTruongData: idTruongData,
                datas: idDatas.map((idData) => ({ idData }))
            }))
        };

        const res = await fetch(`${API_BASE_URL}${ENDPOINTS.deleteRowTableData}`, {
            method: 'DELETE',
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

    
};