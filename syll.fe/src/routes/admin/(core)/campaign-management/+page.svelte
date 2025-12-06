<script lang="ts">
	import { goto } from '$app/navigation';
	import { Button } from '$lib/components/ui/button';
	import type { IColumn } from '$lib/shared/models/data-table.models';
	import { CellViewTypes } from '$lib/shared/constants/data-table.constants';
	import TblAction from './tbl-action.svelte';
	import DataTable from '$lib/shared/components/data-table/data-table.svelte';
	import type { SubmitFunction } from '@sveltejs/kit';
	import { toast } from '$lib/utils/toast.utils';
	import Toaster from '$lib/components/ui/toaster/toaster.svelte';
	import { enhance } from '$app/forms';
	import Search from '@tabler/icons-svelte/icons/search';
	import Input from '$lib/components/ui/input/input.svelte';
	import * as Dialog from '$lib/components/ui/dialog';
	import * as Popover from '$lib/components/ui/popover';
	import { Label } from '$lib/components/ui/label';
	import { Calendar } from '$lib/components/ui/calendar';
	import { CalendarDate, DateFormatter, getLocalTimeZone, parseDate, today } from '@internationalized/date';
	import CalendarIcon from '@tabler/icons-svelte/icons/calendar';
	import X from '@tabler/icons-svelte/icons/x';
	import { cn } from '$lib/utils';
	import { setContext } from 'svelte';
	import type { IViewChienDich } from '$lib/models/campaign/campaign.model';
	import { Utils } from '$lib/shared/utils';

	let { data } = $props();

	let isOpenDialogCreateChienDich = $state(false);
	let isOpenDialogUpdateChienDich = $state(false);
    let isOpenDialogDeleteChienDich = $state(false);
	let isSubmitting = $state(false);
	let isLoadingUpdate = $state(false);
	let searchQuery = $state('');
    let selectedChienDich = $state<number | null>(null);

		// Form fields for create
	let thoiGianBatDau = $state<CalendarDate | undefined>(undefined);
	let thoiGianKetThuc = $state<CalendarDate | undefined>(undefined);
	let selectedFormLoais = $state<{ id: number; tenFormLoai: string }[]>([]);
	let dropDownFormLoaiList = $state<{ id: number; tenFormLoai: string }[]>([]);
	let isOpenFormLoaiPopover = $state(false);

	// Form fields for update
	let updateId = $state<number>(0);
	let updateTenChienDich = $state('');
	let updateMoTa = $state('');
	let updateThoiGianBatDau = $state<CalendarDate | undefined>(undefined);
	let updateThoiGianKetThuc = $state<CalendarDate | undefined>(undefined);
	let updateSelectedFormLoais = $state<{ id: number; tenFormLoai: string }[]>([]);
	let isOpenUpdateFormLoaiPopover = $state(false);

	const df = new DateFormatter('vi-VN', { dateStyle: 'long' });

	setContext('onUpdateChienDich', (rowData: IViewChienDich) => {
		openUpdateChienDich(rowData);
	});

	setContext('onDeleteChienDich', (rowData: IViewChienDich) => {
		selectedChienDich = rowData.id;
		isOpenDialogDeleteChienDich = true;
	});

	function openUpdateChienDich(rowData: IViewChienDich) {
		updateId = rowData.id;
		updateTenChienDich = rowData.tenChienDich || '';
		updateMoTa = rowData.moTa || '';
		updateThoiGianBatDau = rowData.thoiGianBatDau ? parseCalendarDate(rowData.thoiGianBatDau) : undefined;
		updateThoiGianKetThuc = rowData.thoiGianKetThuc ? parseCalendarDate(rowData.thoiGianKetThuc) : undefined;
		updateSelectedFormLoais = [];
		isOpenDialogUpdateChienDich = true;
		// Get dropdown list
		const formDropdown = document.querySelector('form[action="?/getListDropDownFormLoai"]') as HTMLFormElement;
		if (formDropdown) formDropdown.requestSubmit();
		// Get formLoais of this chienDich
		const formGetById = document.querySelector('form[action="?/getChienDichById"]') as HTMLFormElement;
		if (formGetById) {
			const hiddenInput = formGetById.querySelector('input[name="idChienDich"]') as HTMLInputElement;
			if (hiddenInput) hiddenInput.value = rowData.id.toString();
			formGetById.requestSubmit();
		}
	}

	function parseCalendarDate(dateStr: string | Date | null | undefined): CalendarDate | undefined {
		if (!dateStr) return undefined;
		const date = typeof dateStr === 'string' ? new Date(dateStr) : dateStr;
		return new CalendarDate(date.getFullYear(), date.getMonth() + 1, date.getDate());
	}

	function closeUpdateDialog() {
		isOpenDialogUpdateChienDich = false;
	}

	function addUpdateFormLoai(item: { id: number; tenFormLoai: string }) {
		if (!updateSelectedFormLoais.find((f) => f.id === item.id)) {
			updateSelectedFormLoais = [...updateSelectedFormLoais, item];
		}
		isOpenUpdateFormLoaiPopover = false;
	}

	function removeUpdateFormLoai(id: number) {
		updateSelectedFormLoais = updateSelectedFormLoais.filter((f) => f.id !== id);
	}

	function openCreateChienDich() {
		isOpenDialogCreateChienDich = true;
		// Reset form
		thoiGianBatDau = undefined;
		thoiGianKetThuc = undefined;
		selectedFormLoais = [];
		// Trigger get dropdown
		const form = document.querySelector('form[action="?/getListDropDownFormLoai"]') as HTMLFormElement;
		if (form) form.requestSubmit();
	}

	function closeDialog() {
		isOpenDialogCreateChienDich = false;
	}

	function addFormLoai(item: { id: number; tenFormLoai: string }) {
		if (!selectedFormLoais.find((f) => f.id === item.id)) {
			selectedFormLoais = [...selectedFormLoais, item];
		}
		isOpenFormLoaiPopover = false;
	}

	function removeFormLoai(id: number) {
		selectedFormLoais = selectedFormLoais.filter((f) => f.id !== id);
	}

	function formatDateForApi(date: CalendarDate | undefined): string | null {
		if (!date) return null;
		return `${date.year}-${String(date.month).padStart(2, '0')}-${String(date.day).padStart(2, '0')}T00:00:00`;
	}

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

	const columns: IColumn[] = [
		{ header: 'Tên chiến dịch', field: 'tenChienDich', headerContainerStyle: 'width:25rem', clickable: true },
		{ header: 'Mô tả', field: 'moTa', headerContainerStyle: 'width: 20rem' },
		{ header: 'Thời gian tạo', field: 'ngayTao', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
		{ header: 'Thời gian bắt đầu', field: 'thoiGianBatDau', headerContainerStyle: 'width: 10rem; ', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
		{ header: 'Thời gian kết thúc', field: 'thoiGianKetThuc', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
		{ header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction, cellStyle: 'text-align: center' }
	];

	function onCellClick(col: IColumn, rowData: any) {
		if (col.field === 'tenChienDich') {
			goto(`/admin/campaign-management/form?idChienDich=${rowData.id}`);
		}
	}

	const handleSubmit: SubmitFunction = ({ formData }) => {
		isSubmitting = true;
		formData.set('thoiGianBatDau', Utils.formatDateCallApi(thoiGianBatDau) || '');
		formData.set('thoiGianKetThuc', Utils.formatDateCallApi(thoiGianKetThuc) || '');
		formData.set('formLoais', JSON.stringify(selectedFormLoais.map((f) => f.id)));

		return async ({ result, update }) => {
			isSubmitting = false;
			if (result.type === 'success' && result.data) {
				const response = result.data as any;
				if (response.status === 1) {
					toast.success('Tạo chiến dịch thành công!');
					isOpenDialogCreateChienDich = false;
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

	const handleGetDropDownFormLoai: SubmitFunction = () => {
		return async ({ result }) => {
			if (result.type === 'success' && result.data) {
				const response = result.data as any;
				if (response.status === 1 && response.data) {
					dropDownFormLoaiList = response.data;
				}
			}
		};
	};

	const handleGetChienDichById: SubmitFunction = () => {
		return async ({ result }) => {
			if (result.type === 'success' && result.data) {
				const response = result.data as any;
				if (response.status === 1 && response.data) {
					const chienDich = response.data;
					if (chienDich.formLoais && Array.isArray(chienDich.formLoais)) {
						updateSelectedFormLoais = chienDich.formLoais.map((f: { idFormLoai: number; tenFormLoai: string }) => ({
							id: f.idFormLoai,
							tenFormLoai: f.tenFormLoai
						}));
					}
				}
			}
		};
	};

	const handleUpdateSubmit: SubmitFunction = ({ formData }) => {
		isSubmitting = true;
		formData.set('idChienDich', updateId.toString());
		formData.set('tenChienDich', updateTenChienDich);
		formData.set('moTa', updateMoTa);
		formData.set('thoiGianBatDau', formatDateForApi(updateThoiGianBatDau) || '');
		formData.set('thoiGianKetThuc', formatDateForApi(updateThoiGianKetThuc) || '');
		formData.set('formLoais', JSON.stringify(updateSelectedFormLoais.map((f) => f.id)));

		return async ({ result, update }) => {
			isSubmitting = false;
			if (result.type === 'success' && result.data) {
				const response = result.data as any;
				if (response.status === 1) {
					toast.success('Cập nhật chiến dịch thành công!');
					isOpenDialogUpdateChienDich = false;
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
     const handleDeleteChienDich: SubmitFunction =({}) =>{
        isSubmitting = true;

        return async ({result, update}) =>{
            isSubmitting = false;

            if(result.type === 'success' && result.data){
                const response = result.data as any;
                if(response.status === 1){
                    toast.success('Xóa chiến dịch thành công!');
                    isOpenDialogDeleteChienDich = false;
                    selectedChienDich = null;
                    await update();
                    goto(`?page=${currentPage}`, {invalidateAll: true});
                }else{
                toast.error(response.message || 'Có sự cố xảy ra');
                }
            } else {
                toast.error('Có sự cố xảy ra');
            }
        };
        
    }
</script>

<Toaster />
<form method="POST" action="?/getListDropDownFormLoai" use:enhance={handleGetDropDownFormLoai} hidden></form>
<form method="POST" action="?/getChienDichById" use:enhance={handleGetChienDichById} hidden>
	<input type="hidden" name="idChienDich" value="" />
</form>

<div class="mb-6 flex flex-row justify-between items-center">
	<div>
		<h1 class="text-3xl font-bold text-gray-900">Quản lý chiến dịch</h1>
		<p class="text-gray-600 mt-1">Thêm, sửa, xóa chiến dịch tại đây</p>
	</div>
</div>

<div class="flex gap-2 items-center">
	<div class="relative w-160">
		<Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
		<Input bind:value={searchQuery} placeholder="Tìm kiếm theo tên chiến dịch" class="pl-10" />
	</div>
	<div class="flex-1 flex justify-end gap-4">
		<Button class="cursor-pointer">Tìm kiếm</Button>
		<Button class="cursor-pointer" onclick={openCreateChienDich}>Tạo chiến dịch mới</Button>
	</div>
</div>

<div class="rounded-md border items-center">
	<DataTable data={data.data} {columns} {onCellClick} />
</div>

<div class="flex justify-end items-center gap-2 pt-4">
	<Button variant="outline" size="sm" disabled={currentPage === 1} onclick={() => onPageChange(currentPage - 1)}>Previous</Button>
	{#each getPageNumbers() as page}
		<Button variant={currentPage === page ? 'default' : 'outline'} size="sm" class={currentPage === page ? 'bg-blue-600 text-white' : ''} onclick={() => onPageChange(page)}>{page}</Button>
	{/each}
	<Button variant="outline" size="sm" disabled={currentPage === totalPages} onclick={() => onPageChange(currentPage + 1)}>Next</Button>
</div>

<Dialog.Root bind:open={isOpenDialogCreateChienDich}>
	<Dialog.Content class="sm:max-w-[500px]">
		<form method="POST" action="?/createChienDich" use:enhance={handleSubmit}>
			<Dialog.Header>
				<Dialog.Title>Tạo mới chiến dịch</Dialog.Title>
				<Dialog.Description>Tạo mới chiến dịch. Ấn lưu khi đã nhập xong thông tin</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid items-center gap-2">
					<Label for="tenChienDich">Tên chiến dịch</Label>
					<Input id="tenChienDich" name="tenChienDich" required />
				</div>
				<div class="grid items-center gap-2">
					<Label for="moTa">Mô tả</Label>
					<Input id="moTa" name="moTa" />
				</div>
				<div class="grid grid-cols-2 gap-4">
					<div class="grid items-center gap-2">
						<Label>Ngày bắt đầu:</Label>
						<Popover.Root>
							<Popover.Trigger>
								{#snippet child({ props })}
									<div class="relative">
										<CalendarIcon class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
										<Input value={thoiGianBatDau ? `${String(thoiGianBatDau.day).padStart(2, '0')}/${String(thoiGianBatDau.month).padStart(2, '0')}/${thoiGianBatDau.year}` : ''} placeholder="dd/MM/yyyy" readonly class="pl-10 cursor-pointer" {...props} />
									</div>
								{/snippet}
							</Popover.Trigger>
							<Popover.Content class="w-auto p-0">
								<Calendar type="single" bind:value={thoiGianBatDau} />
							</Popover.Content>
						</Popover.Root>
					</div>
					<div class="grid items-center gap-2">
						<Label>Ngày kết thúc:</Label>
						<Popover.Root>
							<Popover.Trigger>
								{#snippet child({ props })}
									<div class="relative">
										<CalendarIcon class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
										<Input value={thoiGianKetThuc ? `${String(thoiGianKetThuc.day).padStart(2, '0')}/${String(thoiGianKetThuc.month).padStart(2, '0')}/${thoiGianKetThuc.year}` : ''} placeholder="dd/MM/yyyy" readonly class="pl-10 cursor-pointer" {...props} />
									</div>
								{/snippet}
							</Popover.Trigger>
							<Popover.Content class="w-auto p-0">
								<Calendar type="single" bind:value={thoiGianKetThuc} />
							</Popover.Content>
						</Popover.Root>
					</div>
				</div>
				<div class="grid items-start gap-2">
					<Label>Form</Label>
					<Popover.Root bind:open={isOpenFormLoaiPopover}>
						<div class="relative">
							<div class="min-h-[40px] w-full border border-gray-300 rounded-md px-3 py-2 pr-12 flex flex-wrap gap-2 items-center bg-white">
								{#if selectedFormLoais.length > 0}
									{#each selectedFormLoais as formLoai}
										<span class="inline-flex items-center gap-1 px-3 py-1 bg-blue-100 text-blue-700 text-sm rounded-full">
											{formLoai.tenFormLoai}
											<button type="button" class="hover:bg-blue-200 rounded-full cursor-pointer" onclick={() => removeFormLoai(formLoai.id)}><X class="w-3 h-3" /></button>
										</span>
									{/each}
								{/if}
							</div>
							<div class="absolute right-1 top-1/2 -translate-y-1/2 flex items-center gap-1.5">
								<div class="h-5 w-px bg-gray-300"></div>
								<Popover.Trigger>
									{#snippet child({ props })}
										<button type="button" class="px-2 hover:bg-gray-50 cursor-pointer rounded" {...props}>
											<svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
												<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
											</svg>
										</button>
									{/snippet}
								</Popover.Trigger>
							</div>
						</div>
						<Popover.Content class="w-[300px] p-0">
							<div class="max-h-[200px] overflow-y-auto">
								{#each dropDownFormLoaiList as item}
									<button type="button" class="w-full px-4 py-2 text-left hover:bg-gray-100 cursor-pointer" onclick={() => addFormLoai(item)}>{item.tenFormLoai}</button>
								{/each}
							</div>
						</Popover.Content>
					</Popover.Root>
				</div>
			</div>
			<Dialog.Footer>
				<Button type="button" variant="outline" onclick={closeDialog} disabled={isSubmitting}>Hủy</Button>
				<Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Đang lưu...' : 'Lưu'}</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>

<Dialog.Root bind:open={isOpenDialogUpdateChienDich}>
	<Dialog.Content class="sm:max-w-[500px]">
		<form method="POST" action="?/updateChienDich" use:enhance={handleUpdateSubmit}>
			<Dialog.Header>
				<Dialog.Title>Cập nhật chiến dịch</Dialog.Title>
				<Dialog.Description>Cập nhật thông tin chiến dịch. Ấn lưu khi đã nhập xong thông tin</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid items-center gap-2">
					<Label for="updateTenChienDich">Tên chiến dịch</Label>
					<Input id="updateTenChienDich" bind:value={updateTenChienDich} required />
				</div>
				<div class="grid items-center gap-2">
					<Label for="updateMoTa">Mô tả</Label>
					<Input id="updateMoTa" bind:value={updateMoTa} />
				</div>
				<div class="grid grid-cols-2 gap-4">
					<div class="grid items-center gap-2">
						<Label>Ngày bắt đầu:</Label>
						<Popover.Root>
							<Popover.Trigger>
								{#snippet child({ props })}
									<div class="relative">
										<CalendarIcon class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
										<Input value={updateThoiGianBatDau ? `${String(updateThoiGianBatDau.day).padStart(2, '0')}/${String(updateThoiGianBatDau.month).padStart(2, '0')}/${updateThoiGianBatDau.year}` : ''} placeholder="dd/MM/yyyy" readonly class="pl-10 cursor-pointer" {...props} />
									</div>
								{/snippet}
							</Popover.Trigger>
							<Popover.Content class="w-auto p-0">
								<Calendar type="single" bind:value={updateThoiGianBatDau} />
							</Popover.Content>
						</Popover.Root>
					</div>
					<div class="grid items-center gap-2">
						<Label>Ngày kết thúc:</Label>
						<Popover.Root>
							<Popover.Trigger>
								{#snippet child({ props })}
									<div class="relative">
										<CalendarIcon class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
										<Input value={updateThoiGianKetThuc ? `${String(updateThoiGianKetThuc.day).padStart(2, '0')}/${String(updateThoiGianKetThuc.month).padStart(2, '0')}/${updateThoiGianKetThuc.year}` : ''} placeholder="dd/MM/yyyy" readonly class="pl-10 cursor-pointer" {...props} />
									</div>
								{/snippet}
							</Popover.Trigger>
							<Popover.Content class="w-auto p-0">
								<Calendar type="single" bind:value={updateThoiGianKetThuc} />
							</Popover.Content>
						</Popover.Root>
					</div>
				</div>
				<div class="grid items-start gap-2">
					<Label>Form</Label>
					<Popover.Root bind:open={isOpenUpdateFormLoaiPopover}>
						<div class="relative">
							<div class="min-h-[40px] w-full border border-gray-300 rounded-md px-3 py-2 pr-12 flex flex-wrap gap-2 items-center bg-white">
								{#if updateSelectedFormLoais.length > 0}
									{#each updateSelectedFormLoais as formLoai}
										<span class="inline-flex items-center gap-1 px-3 py-1 bg-blue-100 text-blue-700 text-sm rounded-full">
											{formLoai.tenFormLoai}
											<button type="button" class="hover:bg-blue-200 rounded-full cursor-pointer" onclick={() => removeUpdateFormLoai(formLoai.id)}><X class="w-3 h-3" /></button>
										</span>
									{/each}
								{/if}
							</div>
							<div class="absolute right-1 top-1/2 -translate-y-1/2 flex items-center gap-1.5">
								<div class="h-5 w-px bg-gray-300"></div>
								<Popover.Trigger>
									{#snippet child({ props })}
										<button type="button" class="px-2 hover:bg-gray-50 cursor-pointer rounded" {...props}>
											<svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
												<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
											</svg>
										</button>
									{/snippet}
								</Popover.Trigger>
							</div>
						</div>
						<Popover.Content class="w-[300px] p-0">
							<div class="max-h-[200px] overflow-y-auto">
								{#each dropDownFormLoaiList as item}
									<button type="button" class="w-full px-4 py-2 text-left hover:bg-gray-100 cursor-pointer" onclick={() => addUpdateFormLoai(item)}>{item.tenFormLoai}</button>
								{/each}
							</div>
						</Popover.Content>
					</Popover.Root>
				</div>
			</div>
			<Dialog.Footer>
				<Button type="button" variant="outline" onclick={closeUpdateDialog} disabled={isSubmitting}>Hủy</Button>
				<Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Đang lưu...' : 'Lưu'}</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>

<Dialog.Root bind:open={isOpenDialogDeleteChienDich}>
    <Dialog.Content class="sm:max-w-[425px]">
        <form method="POST" action="?/deleteChienDich" use:enhance={handleDeleteChienDich}>
            <input type="hidden" name="idChienDich" value={selectedChienDich || ''} />
            <Dialog.Header>
                <Dialog.Title>Xóa chiến dịch</Dialog.Title>
                <Dialog.Description>Bạn có chắc chắn muốn xóa chiến dịch này không? Hành động này không thể hoàn tác.</Dialog.Description>
            </Dialog.Header>
            <Dialog.Footer>
                <Button type="button" variant="outline" onclick={() => isOpenDialogDeleteChienDich = false} disabled={isSubmitting}>Hủy</Button>
                <Button type="submit" variant="destructive" disabled={isSubmitting}>
                    {isSubmitting ? 'Đang xóa...' : 'Xóa'}
                </Button>
            </Dialog.Footer>
        </form>
    </Dialog.Content>
</Dialog.Root>