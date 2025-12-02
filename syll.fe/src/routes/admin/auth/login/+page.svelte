<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import * as Card from '$lib/components/ui/card';
	import logo from '$lib/assets/logo-hucetext-trans.png';
	import { Label } from '$lib/components/ui/label';
	import { Input } from '$lib/components/ui/input';

	export let form;

</script>

<div class="h-screen w-screen flex justify-center items-center">
	<div class="w-full lg:w-1/2 flex-col flex items-center justify-center">
		<form method="POST" action="?/login" class="">
			<Card.Root class="w-full max-w-sm pt-0">
				<div class="bg-[#002a5c] p-5 rounded-tl-md rounded-tr-md">
					<img src={logo} alt="" class="w-full" />
				</div>
				<Card.Header>
					<Card.Title class ="text-center">
						<span class="text-xl"> Đăng nhập hệ thống </span></Card.Title
					>
					<Card.Description class = "text-center">Trang admin quản lý cập nhật sơ yếu lý lịch</Card.Description>
				</Card.Header>
				<Card.Content>
					<div class="flex flex-col gap-6">
						<div class="grid gap-2">
							<Label for="username">Tài khoản</Label>
							<Input id="username" name="username" type="text" required />
						</div>
						<div class="grid gap-2">
							<div class="flex items-center">
								<Label for="password">Mật khẩu</Label>
							</div>
							<Input id="password" name="password" type="password" required />
						</div>
					</div>
					{#if form?.msg}
						<p class="text-sm text-red-600 mt-4">{form.msg}</p>
					{/if}
				</Card.Content>
				<Card.Footer class="flex-col gap-2">
					<Button type="submit" class="w-full cursor-pointer">Đăng nhập</Button>
				</Card.Footer>
			</Card.Root>
		</form>
	</div>
</div>
