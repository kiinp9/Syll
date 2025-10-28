import type { Actions } from './$types';
import {
	API_BASE_URL,
	AUTH_GRANT_TYPE,
	AUTH_CLIENT_ID,
	AUTH_CLIENT_SECRET,
	AUTH_SCOPE
} from '$env/static/private'; // Not exposed to client
import { redirect } from '@sveltejs/kit';

export const actions = {
	login: async ({ request, cookies, fetch }) => {
		const formData = await request.formData();
		const username = formData.get('username');
		const password = formData.get('password');

		// Build URL-encoded form data (application/x-www-form-urlencoded)
		const params = new URLSearchParams();
		params.append('username', username?.toString() || '');
		params.append('password', password?.toString() || '');
		params.append('grant_type', AUTH_GRANT_TYPE);
		params.append('client_id', AUTH_CLIENT_ID);
		params.append('client_secret', AUTH_CLIENT_SECRET);
		params.append('scope', AUTH_SCOPE);

		const endpoint = `${API_BASE_URL}/connect/token`;

		const res = await fetch(`${endpoint}`, {
			method: 'POST',
			headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
			body: params.toString()
		});

		const result = await res.json();

		if (res.ok) {
			// OAuth token endpoint typically returns `access_token`
			const accessToken = result.access_token;
			const refreshToken = result.refresh_token;

			cookies.set('access_token', accessToken, {
				path: '/',
				httpOnly: true,
				secure: true,
				maxAge: result.expires_in // in seconds
			});
			cookies.set('refresh_token', refreshToken, {
				path: '/',
				httpOnly: true,
				secure: true,
				maxAge: 60 * 60 * 24 * 365 // 1 day
			});
			throw redirect(302, '/admin/home'); // or '/'
		}

		return {
			success: false,
			msg: result.error_description || 'Có sự cố khi đăng nhập. Vui lòng thử lại sau'
		};
	}
} satisfies Actions;
