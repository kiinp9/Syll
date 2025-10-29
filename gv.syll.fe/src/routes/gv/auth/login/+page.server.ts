import { API_BASE_URL, APP_URL, AUTH_CLIENT_ID } from '$env/static/private';
import type { PageServerLoad } from '../sso/google/$types';

export const load: PageServerLoad = async ({ cookies }) => {
	return {
		apiBaseUrl: API_BASE_URL,
		authClientId: AUTH_CLIENT_ID,
		appUrl: APP_URL,
	};
};
