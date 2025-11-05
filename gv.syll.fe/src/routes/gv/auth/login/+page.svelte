<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import * as Card from '$lib/components/ui/card';
	import logo from '$lib/assets/logo-hucetext-trans.png';
	import { IconBrandGoogleFilled } from '@tabler/icons-svelte';
	import { CryptoUtils } from '$lib/crypto.utils';
	import { AuthConstants } from '$lib/constants/auth.constants';

	// `data` comes automatically from the server-side load function
	export let data;

	// You can destructure it if you want
	const { apiBaseUrl, authClientId, appUrl } = data;

	async function onClickLoginGoogle() {
		const backendUrl = apiBaseUrl;
		const redirectUri = `${appUrl}/gv/auth/sso/google`;
		const { codeChallenge, codeVerifier } = await CryptoUtils.generatePKCECodes();

		// sessionStorage.setItem(AuthConstants.SESSION_PKCE_CODE_VERIFIER, codeVerifier);
		document.cookie = `${AuthConstants.SESSION_PKCE_CODE_VERIFIER}=${codeVerifier}`;

		const url =
			`${backendUrl}/connect/authorize?` +
			`client_id=${encodeURIComponent(authClientId)}` +
			`&redirect_uri=${encodeURIComponent(redirectUri)}` +
			`&response_type=code` +
			`&scope=openid offline_access` +
			`&prompt=login&code_challenge=${codeChallenge}&code_challenge_method=${AuthConstants.PKCE_CODE_CHALLENGE_METHOD}`;
		console.log(url);
		// window.location.href = url;
	}
</script>

<div class="h-screen w-screen flex justify-center items-center">
	<div class="w-full lg:w-1/2 flex-col flex items-center">
		<Card.Root class="w-full max-w-sm pt-0">
			<div class="bg-[#002a5c] p-5 rounded-tl-md rounded-tr-md">
				<img src={logo} alt="" class="w-full" />
			</div>
			<Card.Header>
				<Card.Title>
					<span class="text-xl"> Đăng nhập hệ thống </span></Card.Title
				>
				<Card.Description>Hỗ trợ cán bộ giảng viên cập nhật sơ yếu lý lịch</Card.Description>
			</Card.Header>
			<Card.Footer class="flex-col gap-2">
				<Button type="button" class="w-full cursor-pointer" onclick={onClickLoginGoogle}>
					<IconBrandGoogleFilled />
					Đăng nhập bằng Email Google</Button
				>
			</Card.Footer>
		</Card.Root>
	</div>
</div>
