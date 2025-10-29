// src/routes/auth/callback/+page.server.js
import { redirect } from '@sveltejs/kit';

import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ url, cookies }) => {
	const accessToken = url.searchParams.get('access_token');
	const refreshToken = url.searchParams.get('refresh_token');
	if (accessToken) {
		cookies.set('access_token', accessToken, {
			httpOnly: true,
			path: '/',
			secure: true,
			sameSite: 'lax',
			maxAge: 60 * 60 * 24
		});
	}
	throw redirect(302, '/dashboard');
};
// export async function load({ url, cookies })  {
//   const token = url.searchParams.get('token');
//   if (token) {
//     cookies.set('jwt', token, {
//       httpOnly: true,
//       path: '/',
//       secure: true,
//       sameSite: 'lax',
//       maxAge: 60 * 60 * 24
//     });
//   }
//   throw redirect(302, '/dashboard');
// }
