<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import * as Card from '$lib/components/ui/card';
	import logo from '$lib/assets/logo-hucetext-trans.png';
    import {
        IconBrandGoogleFilled
    } from '@tabler/icons-svelte';
	import { goto } from '$app/navigation';

    function onClickLoginGoogle() {
        goto('/gv/syll')
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
                    Đăng nhập bằng Email Google</Button>
			</Card.Footer>
		</Card.Root>
	</div>
</div>
