import { redirect, type RequestHandler } from '@sveltejs/kit';

export const POST: RequestHandler  = async ({ cookies }) => {
  // ❌ Remove token
  cookies.delete('access_token', { path: '/' });
  cookies.delete('refresh_token', { path: '/' });

  // ✅ Redirect to login page
  throw redirect(302, '/admin/auth/login');
};
