// src/hooks.server.ts
import type { Handle } from '@sveltejs/kit';
import { redirect } from '@sveltejs/kit';
import {
	API_BASE_URL,
	AUTH_CLIENT_ID,
	AUTH_CLIENT_SECRET,
} from '$env/static/private'; // Not exposed to client

export const handle: Handle = async ({ event, resolve }) => {
	// Wrap the default fetch with your interceptor
	const originalFetch = event.fetch;

	event.fetch = async (input, init = {}) => {
		// Attach access token to all outgoing requests
		const accessToken = event.cookies.get('access_token');
		const refreshToken = event.cookies.get('refresh_token');

		if (accessToken) {
			init.headers = {
				...(init.headers || {}),
				Authorization: `Bearer ${accessToken}`
			};
		}

        console.log('Fetching:', input);

		// Make the original request
		let response = await originalFetch(input, init);

		// If 401, try refresh token flow
		if (response.status === 401 && refreshToken) {
			const endpointRefresh = `${API_BASE_URL}/connect/token`;

			const refreshParams = new URLSearchParams();
			refreshParams.append('grant_type', 'refresh_token');
			refreshParams.append('client_id', AUTH_CLIENT_ID);
			refreshParams.append('client_secret', AUTH_CLIENT_SECRET);
			refreshParams.append('refresh_token', refreshToken);

			const refreshRes = await originalFetch(endpointRefresh, {
				method: 'POST',
				headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
				body: refreshParams.toString()
			});

			if (refreshRes.ok) {
				const refreshResult = await refreshRes.json();

				// Save the new token in cookies
				event.cookies.set('access_token', refreshResult.access_token, {
					path: '/',
					httpOnly: true,
					secure: true,
					maxAge: refreshResult.expires_in // in seconds
				});
				event.cookies.set('refresh_token', refreshResult.refresh_token, {
					path: '/',
					httpOnly: true,
					secure: true,
					maxAge: 60 * 60 * 24 * 365 // 1 day
				});

				// Retry original request with new token
				init.headers = {
					...(init.headers || {}),
					Authorization: `Bearer ${refreshResult.access_token}`
				};

				response = await originalFetch(input, init);
			} else {
				// Refresh failed — clear cookies and redirect to login
				event.cookies.delete('access_token', { path: '/' });
				event.cookies.delete('refresh_token', { path: '/' });
				throw redirect(302, '/login');
			}
		}

		return response;
	};

	// Continue normal flow
	return resolve(event);
};
