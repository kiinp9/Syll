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
	let nhanVienSection: HTMLDivElement;


	function openCreateToChuc() {
		isOpenDialogCreateToChuc = true;
	}

	
	function openDeleteToChuc(event: Event, id: number) {
		event.stopPropagation();
		selectedToChuc = id;
		isOpenDialogDeleteToChuc = true;
	}

	function getNhanVienToChuc(id: number) {
		selectedToChuc = id;
		goto(`?page=1&idToChuc=${id}`).then(() => {
			setTimeout(() => {
				nhanVienSection?.scrollIntoView({ behavior: 'smooth', block: 'start' });
			}, 100);
		});
	}

	function closeDialog() {
		isOpenDialogCreateToChuc = false;
	}

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
	
	let currentPageToChuc = $derived(data.toChuc.currentPage || 1);
	let totalPagesToChuc = $derived(data.toChuc.totalPages || 1);


	let currentPageNhanVien = $derived(data.nhanVien.currentPage || 1);
	let totalPagesNhanVien = $derived(data.nhanVien.totalPages || 1);
	
	function onPageChange(page: number) {
		goto(`?page=${page}`);
	}

	function getPageToChucNumbers() {
		const pages = [];
		for (let i = 1; i <= totalPagesToChuc; i++) {
			pages.push(i);
		}
		return pages;
	}
	function getPageNhanVienNumbers() {
		const pages = [];
		for (let i = 1; i <= totalPagesNhanVien; i++) {
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
					goto(`?page=${currentPageToChuc}`, { invalidateAll: true });
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
					goto(`?page=${currentPageToChuc}`, { invalidateAll: true });
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
<div class="p-6 pt-0 space-y-8">
	<div>
		<h1 class="text-3xl font-bold text-gray-900">Quản lý tổ chức</h1>
		<p class="text-gray-600 mt-1">Quản lý phòng ban, cán bộ, giảng viên, công nhân viên chức</p>
	</div>

	<Card class="border-t-4 border-t-white-400 shadow-xl">
		<CardContent class="p-6 pt-1 space-y-4">
			<div class="flex items-center justify-between ">
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
				{#each data.toChuc.data as organization}
					<Card class="hover:shadow-md transition-shadow cursor-pointer" onclick={() => getNhanVienToChuc(organization.id!)}>
						<CardContent class="p-4 flex justify-between gap-3">
							<div class="flex-1 ">
								<div class="flex items-center gap-3">
									<div class="p-2 bg-blue-50 rounded-lg">
										<Building class="w-5 h-5 text-blue-600" />
									</div>
									<div>
										<h3 class="font-semibold text-gray-900">{organization.tenToChuc}</h3>
										<p class="text-sm text-gray-600">{organization.soNhanVien} members</p>
									</div>
								</div>
								<p class="text-sm text-gray-600 mb-1"><strong>Loại tổ chức:</strong> {getLoaiToChucLabel(organization.loaiToChuc)}</p>
							    <p class="text-sm text-gray-600 mb-1"><strong>Mô tả:</strong> {organization.moTa}</p>
							    <p class="text-sm text-gray-600"><strong>Mã số tổ chức:</strong> {organization.maSoToChuc || 'Chưa có'}</p>
								
							</div>
							<div class="flex items-center">
							    <Button variant="ghost" size="icon-sm" class="text-red-600 hover:text-red-700 hover:bg-red-50 self-center" onclick={(e) => openDeleteToChuc(e, organization.id!)}>
	                                <Trash class="size-5" />
                                </Button>
							</div>
						</CardContent>
					</Card>
				{/each}
			</div>

			<div class="flex justify-end items-center gap-2 pt-4">
				<Button 
					variant="outline" 
					size="sm"
					disabled={currentPageToChuc === 1}
					onclick={() => onPageChange(currentPageToChuc - 1)}
				>
					Previous
				</Button>
				
				{#each getPageToChucNumbers() as page}
					<Button 
						variant={currentPageToChuc === page ? "default" : "outline"}
						size="sm"
						class={currentPageToChuc === page ? "bg-blue-600 text-white" : ""}
						onclick={() => onPageChange(page)}
					>
						{page}
					</Button>
				{/each}

				<Button 
					variant="outline" 
					size="sm"
					disabled={currentPageToChuc === totalPagesToChuc}
					onclick={() => onPageChange(currentPageToChuc + 1)}
				>
					Next
				</Button>
			</div>
		</CardContent>
	</Card>

	<div bind:this={nhanVienSection}>
		<Card class="border-t-4 border-t-white-400 shadow-xl">
			<CardContent class="p-6 space-y-4">
				<div>
					<h2 class="text-xl font-semibold">Quản lý nhân viên</h2>
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
										<th class="text-left p-4 font-semibold text-gray-700">Role</th>
										<th class="text-left p-4 font-semibold text-gray-700">Action</th>
									</tr>
								</thead>
								<tbody>
									{#if data.nhanVien.data.length > 0}
										{#each data.nhanVien.data as member}
											<tr class="border-b hover:bg-gray-50 transition-colors">
												<td class="p-4 font-medium">{member.hoVaTen}</td>
												<td class="p-4 text-gray-600">{member.email}</td>
												<td class="p-4">
													<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-700">
														{member.role?.name || 'N/A'}
													</span>
												</td>
												<td class="p-4">
													<Button variant="ghost" size="icon" class="text-blue-600 hover:text-blue-700 hover:bg-blue-50">
														<Edit class="w-4 h-4" />
													</Button>
												</td>
											</tr>
										{/each}
									{:else}
										<tr>
											<td colspan="4" class="p-4 text-center text-gray-500">
												Chọn một tổ chức để xem danh sách nhân viên
											</td>
										</tr>
									{/if}
								</tbody>
							</table>
						</div>
					</CardContent>
				</Card>

				{#if data.nhanVien.data.length > 0}
					<div class="flex justify-end items-center gap-2 pt-4">
						<Button 
							variant="outline" 
							size="sm"
							disabled={currentPageNhanVien === 1}
							onclick={() => goto(`?page=${currentPageNhanVien - 1}&idToChuc=${selectedToChuc}`)}
						>
							Previous
						</Button>
						
						{#each getPageNhanVienNumbers() as page}
							<Button 
								variant={currentPageNhanVien === page ? "default" : "outline"}
								size="sm"
								class={currentPageNhanVien === page ? "bg-blue-600 text-white" : ""}
								onclick={() => goto(`?page=${page}&idToChuc=${selectedToChuc}`)}
							>
								{page}
							</Button>
						{/each}

						<Button 
							variant="outline" 
							size="sm"
							disabled={currentPageNhanVien === totalPagesNhanVien}
							onclick={() => goto(`?page=${currentPageNhanVien + 1}&idToChuc=${selectedToChuc}`)}
						>
							Next
						</Button>
					</div>
				{/if}
			</CardContent>
		</Card>
	</div>
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