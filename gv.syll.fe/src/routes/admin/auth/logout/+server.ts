import { redirect, type RequestHandler } from '@sveltejs/kit';

export const POST: RequestHandler = async ({ cookies, url, request }) => {
	cookies.delete('access_token', { path: '/' });
	cookies.delete('refresh_token', { path: '/' });

  // console.log('LOG OUT', url)

	// if (url.pathname.startsWith('/admin/')) {
	// 	throw redirect(302, '/admin/auth/login');
	// }
	throw redirect(302, '/gv/auth/login');
};
