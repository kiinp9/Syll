<script lang="ts">
	import { goto } from '$app/navigation';
	import { Button } from '$lib/components/ui/button';
	import {
		Card,
		CardContent,
		CardDescription,
		CardHeader,
		CardTitle
	} from '$lib/components/ui/card';
	import Download from '@tabler/icons-svelte/icons/download';
	import Edit from '@tabler/icons-svelte/icons/edit';
	import FileText from '@tabler/icons-svelte/icons/file-text';
	import * as Dialog from '$lib/components/ui/dialog';
	import Label from '$lib/components/ui/label/label.svelte';
	import { Input } from '$lib/components/ui/input/index.js';
	import CardFooter from '$lib/components/ui/card/card-footer.svelte';

	let { data } = $props();

	let isOpenDialogCreateForm = $state(false);

	function navigateToDetail(id: number | null | undefined) {
		goto(`/admin/form-management/template`);
	}

	function openCreateForm() {
		isOpenDialogCreateForm = true;
	}
</script>

<div class="mb-6 flex flex-row justify-between items-center">
	<div>
		<h1 class="text-3xl font-bold text-gray-900">Quản lý form</h1>
		<p class="text-gray-600 mt-1">Thêm, sửa, xóa form tại đây</p>
	</div>
	<div>
		<Button class="cursor-pointer" onclick={openCreateForm}>Tạo form mới</Button>
	</div>
</div>
<div class="grid grid-cols-1 md:grid-cols-3 gap-3">
	{#each data.data as form (form.id)}
		<Card class="border-0 shadow-lg hover:shadow-xl transition-shadow flex flex-col justify-between">
			<div>
				<CardHeader>
					<div class="flex items-start justify-between">
						<div class="flex items-center space-x-3 justify-center">
							<div class="w-12 h-12 bg-blue-100 rounded-xl flex items-center justify-center">
								<FileText class="w-6 h-6 text-[#0b57d0]" />
							</div>
							<div class="flex-1">
								<CardTitle class="text-xl">{form.tenForm}</CardTitle>
								<CardDescription>{form.moTa}</CardDescription>
							</div>
						</div>
					</div>
				</CardHeader>
				<CardContent>
					<div class="space-y-4">
						<div class="text-sm text-gray-600">
							<p><strong>Tổng số trường:</strong> {form.tongSoTruong}</p>
							<p class="mt-1">
								<strong>Lần cập nhật gần nhất:</strong>
						        {form.thoiGianCapNhatGanNhat
						        ? new Date(form.thoiGianCapNhatGanNhat).toLocaleDateString('vi-VN')
						        : 'Chưa cập nhật'
						    }
							</p>
						</div>
					</div>
				</CardContent>
			</div>
			<CardFooter
				><div class="flex gap-3 justify-end w-full">
					<Button variant="outline" class="cursor-pointer">
						<Download class="w-4 h-4 mr-1" />
						Tải file DOCX
					</Button>
					<Button class="cursor-pointer" onclick={() => navigateToDetail(form.id)}>
						<Edit class="w-4 h-4 mr-1" />
						Cập nhật form
					</Button>
				</div></CardFooter
			>
		</Card>
	{/each}
</div>

<Dialog.Root open={isOpenDialogCreateForm}>
	<Dialog.Content class="sm:max-w-[425px]">
		<form method="POST" action="?/createForm">
			<Dialog.Header>
				<Dialog.Title>Tạo form</Dialog.Title>
				<Dialog.Description>Tạo form. Ấn lưu khi đã nhập xong thông tin</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid grid-cols-4 items-center gap-4">
					<Label for="tenLoaiForm" class="text-right">Tên form</Label>
					<Input id="tenLoaiForm" name="tenLoaiForm" required class="col-span-3" />
				</div>
				<div class="grid grid-cols-4 items-center gap-4">
					<Label for="moTa" class="text-right">Mô tả</Label>
					<Input id="moTa" name="moTa" class="col-span-3" />
				</div>
			</div>
			<Dialog.Footer>
				<Button type="submit">Tạo form</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>
