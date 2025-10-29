// src/routes/auth/callback/+page.server.js
import { redirect } from '@sveltejs/kit';

import type { PageServerLoad } from './$types';
import { AuthConstants } from '$lib/constants/auth.constants';
import { API_BASE_URL, AUTH_CLIENT_ID, AUTH_CLIENT_SECRET } from '$env/static/private';
import { ENDPOINTS } from '$lib/api/endpoint';

export const load: PageServerLoad = async ({ url, cookies }) => {
	const code = url.searchParams.get('code');

	if (!code) {
		return {
			success: false,
			msg: 'No authorization code found'
		};
	}

	// const verifier = sessionStorage.getItem(AuthConstants.SESSION_PKCE_CODE_VERIFIER);
	const verifier = cookies.get(AuthConstants.SESSION_PKCE_CODE_VERIFIER);
	console.log('CODE VERIFIER => ', verifier);
	// Exchange code for tokens
	const params = new URLSearchParams();
	params.append('code', code);
	params.append('code_verifier', verifier || '');
	params.append('grant_type', 'authorization_code');
	params.append('client_id', AUTH_CLIENT_ID);
	params.append('client_secret', AUTH_CLIENT_SECRET);
	params.append('redirect_uri', `http://localhost:5173/gv/auth/sso/google`);

	const endpoint = `${API_BASE_URL}${ENDPOINTS.connectToken}`;

	const res = await fetch(`${endpoint}`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
		body: params.toString()
	});

	const result = await res.json();

    console.log('TOKEN RESULT => ', result);
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
		throw redirect(302, '/gv/dashboard'); // or '/'
	}

	return {
		success: false,
		msg: result.error_description || 'Có sự cố khi đăng nhập. Vui lòng thử lại sau'
	};
};
