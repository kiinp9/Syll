import { API_BASE_URL } from "$env/static/private";
import { ENDPOINTS } from "$lib/api/endpoint";
import { error, type RequestHandler } from "@sveltejs/kit";

export const POST: RequestHandler = async ({ fetch, params }) => {
	const idForm = params.id;

	const res = await fetch(`${API_BASE_URL}${ENDPOINTS.downloadForm(Number(idForm))}`, {
		method: 'POST'
	});

	if (!res.ok) {
		throw error(500, 'Có sự cố xảy ra khi tải file');
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