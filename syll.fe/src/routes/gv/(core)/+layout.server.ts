import type { LayoutServerLoad } from './$types';
import { API_BASE_URL } from '$env/static/private'; // Not exposed to client
import type { IBaseResponseWithData } from '$lib/models/shared/base-response';
import type { IViewUserMe } from '$lib/models/user/user.model';
import { redirect } from '@sveltejs/kit';
import { ENDPOINTS } from '$lib/api/endpoint';

export const load: LayoutServerLoad = async ({ fetch, cookies }) => {
	const res = await fetch(`${API_BASE_URL}${ENDPOINTS.getMe}`);

	if (!res.ok) {
		console.error('Failed to load user profile');
		cookies.delete('access_token', { path: '/' });
		cookies.delete('refresh_token', { path: '/' });
		throw redirect(302, '/gv/auth/login');
	}

	const data: IBaseResponseWithData<IViewUserMe> = await res.json();

	if (data.status === 1) {
		console.log('RESULT 1', data.data);
		return { user: data.data };
	}

	cookies.delete('access_token', { path: '/' });
	cookies.delete('refresh_token', { path: '/' });
	throw redirect(302, '/gv/auth/login');
};
