import type { LayoutServerLoad } from './$types';
import { API_BASE_URL } from '$env/static/private'; // Not exposed to client
import type { IBaseResponseWithData } from '$lib/models/shared/base-response';
import type { IViewUserMe } from '$lib/models/user/user.model';
import { redirect } from '@sveltejs/kit';

export const load: LayoutServerLoad = async ({ fetch, cookies }) => {
	const res = await fetch(`${API_BASE_URL}/api/app/users/me`);

	if (!res.ok) {
		console.error('Failed to load user profile');
		cookies.delete('access_token', { path: '/' });
		cookies.delete('refresh_token', { path: '/' });
		throw redirect(302, '/admin/auth/login');
	}

	const data: IBaseResponseWithData<IViewUserMe> = await res.json();

	if (data.status === 1) {
		console.log('RESULT 1', data.data);
		return { user: data.data };
	}

	cookies.delete('access_token', { path: '/' });
	cookies.delete('refresh_token', { path: '/' });
	throw redirect(302, '/admin/auth/login');
};
