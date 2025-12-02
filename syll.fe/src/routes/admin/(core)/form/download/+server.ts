import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import { error, type RequestHandler } from "@sveltejs/kit";

export const POST: RequestHandler = async ({ fetch, url }) => {
    const idFormLoai = parseInt(url.searchParams.get('idFormLoai') || '0');
    const idDanhBa = parseInt(url.searchParams.get('idDanhBa') || '0');
    const queryParams = new URLSearchParams({
        idFormLoai: idFormLoai?.toString() || '',
        idDanhBa: idDanhBa?.toString() || '',
    });

    const res = await fetch(`${API_BASE_URL}${ENDPOINTS.downloadFormAdmin}?${queryParams}`, {
        method: 'POST'
    });

    /*if (!res.ok) {
        throw error(500, 'Có sự cố xảy ra khi tải file');
    }*/

    const contentType = res.headers.get('Content-Type');
    
    if (contentType?.includes('application/json')) {
        const errorData = await res.json();
        throw error( 500, errorData.message);
    }

    const blob = await res.blob();
    const fileName = `SoYeuLyLich_${new Date().toISOString().slice(0, 19).replace(/[:-]/g, '')}.docx`;

    return new Response(blob, {
        headers: {
            'Content-Type': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
            'Content-Disposition': `attachment; filename="${fileName}"`
        }
    });
};