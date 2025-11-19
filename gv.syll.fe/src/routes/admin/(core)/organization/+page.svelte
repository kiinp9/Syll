<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent } from '$lib/components/ui/card';
	import { Input } from '$lib/components/ui/input';

	import Building from '@tabler/icons-svelte/icons/building';
	import Search from '@tabler/icons-svelte/icons/search';
	import Trash from '@tabler/icons-svelte/icons/trash';
	import Edit from '@tabler/icons-svelte/icons/edit';
	import { goto } from '$app/navigation';
	import * as Dialog from '$lib/components/ui/dialog';
	import Label from '$lib/components/ui/label/label.svelte';
	import { ToChucConstants } from '$lib/constants/organization.constants.js';
	import type { SubmitFunction } from '@sveltejs/kit';

	import { enhance } from '$app/forms';
	import Toaster from '$lib/components/ui/toaster/toaster.svelte';
	import { toast } from '$lib/utils/toast.utils.js';


	let { data } = $props();


	let isOpenDialogCreateToChuc = $state(false);
	let isOpenDialogDeleteToChuc = $state(false);
	let isSubmitting = $state(false);
	let selectedToChuc = $state<number | null>(null);


	function openCreateToChuc() {
		isOpenDialogCreateToChuc = true;
	}

	
	function openDeleteToChuc( id: number ) {
		selectedToChuc = id;
		isOpenDialogDeleteToChuc = true;
	}

	function closeDialog() {
		isOpenDialogCreateToChuc = false;
	}

	const staff = [
		{ name: 'Test Admin User', email: 'test.admin.1761371844057@example.com', department: 'Unassigned', role: 'Admin' },
		{ name: 'Test Staff User', email: 'test.staff.1761371844079@example.com', department: 'Unassigned', role: 'Staff' },
		{ name: 'Admin User', email: 'admin@huce.edu.vn', department: 'Unassigned', role: 'Admin' },
		{ name: 'Nguyễn Văn Minh', email: 'staff@huce.edu.vn', department: 'Khoa Xây dựng Dân dụng và Công nghiệp', role: 'Staff' },
		{ name: 'Dương Thị Tú', email: 'user1@huce.edu.vn', department: 'Khoa Xây dựng Dân dụng và Công nghiệp', role: 'Staff' },
		{ name: 'Đỗ Minh Dũng', email: 'user2@huce.edu.vn', department: 'Phòng Đào tạo', role: 'Staff' },
		{ name: 'Phạm Quốc Tuấn', email: 'user3@huce.edu.vn', department: 'Phòng Hành chính - Tổng hợp', role: 'Staff' },
		{ name: 'Phan Đức Chi', email: 'user4@huce.edu.vn', department: 'Phòng Đào tạo', role: 'Staff' }
	];
	const loaiToChucOptions = [
		{ value: ToChucConstants.DaiHocCongLap, label: "Đại học công lập" },
		{ value: ToChucConstants.KhoaDaoTao, label: "Khoa đào tạo" },
		{ value: ToChucConstants.PhongBan, label: "Phòng ban" }
	];
	function getLoaiToChucLabel(value?: number): string {
	if (!value) return 'Không xác định';
	return loaiToChucOptions.find(opt => opt.value === value)?.label || 'Không xác định';
	}

	let searchQuery = $state('');
	
	let currentPage = $derived(data.currentPage || 1);
	let totalPages = $derived(data.totalPages || 1);

	function onPageChange(page: number) {
		goto(`?page=${page}`);
	}

	function getPageNumbers() {
		const pages = [];
		for (let i = 1; i <= totalPages; i++) {
			pages.push(i);
		}
		return pages;
	}

	const handleSubmit: SubmitFunction = () => {
		isSubmitting = true;
		
		return async ({ result, update }) => {
			isSubmitting = false;
			
			if (result.type === 'success' && result.data) {
				const response = result.data as any;
				
				if (response.status === 1) {
					toast.success('Tạo tổ chức thành công!');
					isOpenDialogCreateToChuc = false;
					await update();
					goto(`?page=${currentPage}`, { invalidateAll: true });
				} else {
					toast.error(response.message || 'Có sự cố xảy ra');
				}
			} else {
				toast.error('Có sự cố xảy ra');
			}
		};
	};


	const handleDeleteToChuc: SubmitFunction = () => {
		isSubmitting = true;
		
		return async ({ result, update }) => {
			isSubmitting = false;
			
			if (result.type === 'success' && result.data) {
				const response = result.data as any;
				
				if (response.status === 1) {
					toast.success('Xóa tổ chức thành công!');
					isOpenDialogDeleteToChuc = false;
					selectedToChuc = null;
					await update();
					goto(`?page=${currentPage}`, { invalidateAll: true });
				} else {
					toast.error(response.message || 'Có sự cố xảy ra');
				}
			} else {
				toast.error('Có sự cố xảy ra');
			}
		};
	};
</script>
<Toaster />
<div class="p-6 space-y-8">
	<div>
		<h1 class="text-3xl font-bold text-gray-900">Quản lý tổ chức</h1>
		<p class="text-gray-600 mt-1">Quản lý phòng ban, cán bộ, giảng viên, công nhân viên chức</p>
	</div>

	<Card class="border-t-4 border-t-white-400 shadow-xl">
		<CardContent class="p-6 space-y-4">
			<div class="flex items-center justify-between">
				<div>
					<h2 class="text-xl font-semibold">Phòng ban</h2>
					<p class="text-sm text-gray-600">Quản lý các phòng ban trong tổ chức</p>
				</div>
				<Button class="gap-2" onclick={openCreateToChuc}>
					<span class="text-lg">+</span>
					Thêm phòng ban
				</Button>
			</div>

			<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
				{#each data.data as organization}
					<Card class="hover:shadow-md transition-shadow">
						<CardContent class="p-4">
							<div class="flex items-start justify-between mb-3">
								<div class="flex items-center gap-3">
									<div class="p-2 bg-blue-50 rounded-lg">
										<Building class="w-5 h-5 text-blue-600" />
									</div>
									<div>
										<h3 class="font-semibold text-gray-900">{organization.tenToChuc}</h3>
										<p class="text-sm text-gray-600">{organization.soNhanVien} members</p>
									</div>
								</div>
								<Button variant="ghost" size="icon" class="text-red-600 hover:text-red-700 hover:bg-red-50 h-10 w-10" onclick={() => openDeleteToChuc(organization.id!)}>
	                                <Trash class="w-6 h-6" />
                                </Button>
							</div>
							<p class="text-sm text-gray-600 mb-1"><strong>Loại tổ chức:</strong> {getLoaiToChucLabel(organization.loaiToChuc)}</p>
							<p class="text-sm text-gray-600 mb-1"><strong>Mô tả:</strong> {organization.moTa}</p>
							<p class="text-sm text-gray-600"><strong>Mã số tổ chức:</strong> {organization.maSoToChuc || 'Chưa có'}</p>
						</CardContent>
					</Card>
				{/each}
			</div>

			<div class="flex justify-end items-center gap-2 pt-4">
				<Button 
					variant="outline" 
					size="sm"
					disabled={currentPage === 1}
					onclick={() => onPageChange(currentPage - 1)}
				>
					Previous
				</Button>
				
				{#each getPageNumbers() as page}
					<Button 
						variant={currentPage === page ? "default" : "outline"}
						size="sm"
						class={currentPage === page ? "bg-blue-600 text-white" : ""}
						onclick={() => onPageChange(page)}
					>
						{page}
					</Button>
				{/each}

				<Button 
					variant="outline" 
					size="sm"
					disabled={currentPage === totalPages}
					onclick={() => onPageChange(currentPage + 1)}
				>
					Next
				</Button>
			</div>
		</CardContent>
	</Card>

	<Card class="border-t-4 border-t-white-400 shadow-xl">
		<CardContent class="p-6 space-y-4">
			<div>
				<h2 class="text-xl font-semibold">Staff Management</h2>
				<p class="text-sm text-gray-600">Assign staff to departments and manage roles</p>
			</div>

			<div class="relative">
				<Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
				<Input
					bind:value={searchQuery}
					placeholder="Search by name or email..."
					class="pl-10"
				/>
			</div>

			<Card>
				<CardContent class="p-0">
					<div class="overflow-x-auto">
						<table class="w-full">
							<thead class="border-b bg-gray-50">
								<tr>
									<th class="text-left p-4 font-semibold text-gray-700">Name</th>
									<th class="text-left p-4 font-semibold text-gray-700">Email</th>
									<th class="text-left p-4 font-semibold text-gray-700">Department</th>
									<th class="text-left p-4 font-semibold text-gray-700">Role</th>
									<th class="text-left p-4 font-semibold text-gray-700">Action</th>
								</tr>
							</thead>
							<tbody>
								{#each staff as member}
									<tr class="border-b hover:bg-gray-50 transition-colors">
										<td class="p-4 font-medium">{member.name}</td>
										<td class="p-4 text-gray-600">{member.email}</td>
										<td class="p-4 text-gray-600">{member.department}</td>
										<td class="p-4">
											<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium {member.role === 'Admin' ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-700'}">
												{member.role}
											</span>
										</td>
										<td class="p-4">
											<Button variant="ghost" size="icon" class="text-blue-600 hover:text-blue-700 hover:bg-blue-50">
												<Edit class="w-4 h-4" />
											</Button>
										</td>
									</tr>
								{/each}
							</tbody>
						</table>
					</div>
				</CardContent>
			</Card>
		</CardContent>
	</Card>
</div>


<Dialog.Root bind:open={isOpenDialogCreateToChuc}>
	<Dialog.Content class="sm:max-w-[425px]">
		<form method="POST" action="?/createToChuc" use:enhance={handleSubmit}>
			<Dialog.Header>
				<Dialog.Title>Tạo mới tổ chức</Dialog.Title>
				<Dialog.Description>Tạo tổ chức. Ấn lưu khi đã nhập xong thông tin</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid grid-cols-4 items-center gap-4">
					<Label for="tenToChuc" class="text-right">Tên tổ chức</Label>
					<Input id="tenToChuc" name="tenToChuc" required class="col-span-3" />
				</div>
				<div class="grid grid-cols-4 items-center gap-4">
					<Label for="moTa" class="text-right">Mô tả</Label>
					<Input id="moTa" name="moTa" class="col-span-3" />
				</div>
				<div class="grid grid-cols-4 items-center gap-4">
					<Label for="loaiToChuc" class="text-right">Loại tổ chức</Label>
					<select id="loaiToChuc" name="loaiToChuc" required class="col-span-3 flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50">
						<option value="">Chọn loại tổ chức</option>
						{#each loaiToChucOptions as option}
							<option value={option.value}>{option.label}</option>
						{/each}
					</select>
				</div>
				<div class="grid grid-cols-4 items-center gap-4">
					<Label for="maSoToChuc" class="text-right whitespace-nowrap">Mã số tổ chức</Label>
					<Input id="maSoToChuc" name="maSoToChuc" class="col-span-3" />
				</div>
			</div>
			<Dialog.Footer>
				<Button type="button" variant="outline" onclick={closeDialog} disabled={isSubmitting}>Hủy</Button>
				<Button type="submit" disabled={isSubmitting}>
					{isSubmitting ? 'Đang lưu...' : 'Lưu'}
				</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>


<Dialog.Root bind:open={isOpenDialogDeleteToChuc}>
	<Dialog.Content class="sm:max-w-[425px]">
		<form method="POST" action="?/deleteToChuc" use:enhance={handleDeleteToChuc}>
			<input type="hidden" name="idToChuc" value={selectedToChuc || ''} />
			<Dialog.Header>
				<Dialog.Title>Xóa tổ chức</Dialog.Title>
				<Dialog.Description>Bạn có chắc chắn muốn xóa tổ chức này không? Hành động này không thể hoàn tác.</Dialog.Description>
			</Dialog.Header>
			<Dialog.Footer>
				<Button type="button" variant="outline" onclick={() => isOpenDialogDeleteToChuc = false} disabled={isSubmitting}>Hủy</Button>
				<Button type="submit" variant="destructive" disabled={isSubmitting}>
					{isSubmitting ? 'Đang xóa...' : 'Xóa'}
				</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>