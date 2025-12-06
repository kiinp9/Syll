<script lang="ts">
	import { goto } from '$app/navigation';
	import { Button } from '$lib/components/ui/button';
	import {
		Card,
		CardContent,
		CardDescription,
		CardFooter,
		CardHeader,
		CardTitle
	} from '$lib/components/ui/card';
	import Download from '@tabler/icons-svelte/icons/download';
	import Edit from '@tabler/icons-svelte/icons/edit';
	import FileText from '@tabler/icons-svelte/icons/file-text';
	import * as Dialog from '$lib/components/ui/dialog';
	import Label from '$lib/components/ui/label/label.svelte';
	import { Input } from '$lib/components/ui/input/index.js';
	import type { IColumn } from '$lib/shared/models/data-table.models.js';
	import { CellViewTypes } from '$lib/shared/constants/data-table.constants.js';
	import DataTable from '$lib/shared/components/data-table/data-table.svelte';
	import Search from '@tabler/icons-svelte/icons/search';
	import Toaster from '$lib/components/ui/toaster/toaster.svelte';
    import TblAction from './tbl-action.svelte';
	import { CalendarDate, DateFormatter, getLocalTimeZone, parseDate, today } from '@internationalized/date';
	import type { SubmitFunction } from '@sveltejs/kit';
	import { toast } from '$lib/utils/toast.utils';
	import { Utils } from '$lib/shared/utils';
	import * as Popover from '$lib/components/ui/popover';
	import CalendarIcon from '@tabler/icons-svelte/icons/calendar';
	import { Calendar } from '$lib/components/ui/calendar';
	import { enhance } from '$app/forms';
	import { page } from '$app/stores';
	import { setContext } from 'svelte';
	import type { IViewForm } from '$lib/models/campaign/campaign.model';

	let { data } = $props();

	let isOpenDialogCreateForm = $state(false);
	let isOpenDialogUpdateForm = $state(false);
	let isOpenDialogDeleteForm = $state(false);
	let searchQuery = $state('');
    let selectedForm = $state<number | null>(null);
	let thoiGianBatDau = $state<CalendarDate | undefined>(undefined);
	let thoiGianKetThuc = $state<CalendarDate | undefined>(undefined);
    let isSubmitting = $state(false);



	// Form fields for update	
	let updateId = $state<number>(0);
	let updateTenForm = $state('');
	let updateMoTa = $state('');
	let updateThoiGianBatDau = $state<CalendarDate | undefined>(undefined);
	let updateThoiGianKetThuc = $state<CalendarDate | undefined>(undefined);

	/*function navigateToDetail(id: number | null | undefined) {
		goto(`/admin/form-management/template`);
	}*/
	let idChienDich = $derived($page.url.searchParams.get('idChienDich') || '0');
    let currentPage = $derived(data.currentPage || 1);
	let totalPages = $derived(data.totalPages || 1);

	setContext('onUpdateForm', (rowData: IViewForm) => {
		openUpdateForm(rowData);
	});
	setContext('onUpdateForm', (rowData: IViewForm) => {
		openUpdateForm(rowData);
	});
	setContext('onViewFormDetail', (idFormLoai: number) => {
	navigateToFormDetail(idFormLoai);
	});
	function navigateToFormDetail(idFormLoai:number){
		goto(`/admin/form?idFormLoai=${idFormLoai}`);
	}

	function openUpdateForm(rowData: IViewForm) {
		updateId = rowData.id;
		updateTenForm = rowData.tenForm || '';
		updateMoTa = rowData.moTa || '';
		updateThoiGianBatDau = rowData.thoiGianBatDau ? Utils.parseCalendarDate(rowData.thoiGianBatDau) : undefined;
		updateThoiGianKetThuc = rowData.thoiGianKetThuc ? Utils.parseCalendarDate(rowData.thoiGianKetThuc) : undefined;

		isOpenDialogUpdateForm = true;

	}

	setContext('onDeleteForm', (rowData: IViewForm) => {
		idChienDich = idChienDich;
		selectedForm = rowData.id;
		isOpenDialogDeleteForm = true;
	});

	function onPageChange(page: number) {
		goto(`?page=${page}&idChienDich=${idChienDich}`);
	}

	function getPageNumbers() {
		const pages = [];
		for (let i = 1; i <= totalPages; i++) {
			pages.push(i);
		}
		return pages;
	}
	function openCreateForm() {
		isOpenDialogCreateForm = true;
		thoiGianBatDau = undefined;
		thoiGianKetThuc = undefined;
	}

	function closeDialog() {
		isOpenDialogCreateForm = false;
	}
	function closeUpdateDialog() {
		isOpenDialogUpdateForm = false;
	}

	const handleSubmit: SubmitFunction = ({ formData }) => {
		isSubmitting = true;
		formData.set('idChienDich', idChienDich);
		formData.set('thoiGianBatDau', Utils.formatDateCallApi(thoiGianBatDau) || '');
		formData.set('thoiGianKetThuc', Utils.formatDateCallApi(thoiGianKetThuc) || '');
	
		return async ({ result, update }) => {
			isSubmitting = false;
			if (result.type === 'success' && result.data) {
				const response = result.data as any;
				if (response.status === 1) {
					toast.success('Tạo form thành công!');
					isOpenDialogCreateForm = false;
					await update();
					goto(`?page=${currentPage}&idChienDich=${idChienDich}`, { invalidateAll: true });
				} else {
					toast.error(response.message || 'Có sự cố xảy ra');
				}
			} else {
				toast.error('Có sự cố xảy ra');
			}
		};
	};
	const handleGetFormById: SubmitFunction = () => {
		return async ({ result }) => {
			if (result.type === 'success' && result.data) {
				const response = result.data as any;
				if (response.status === 1 && response.data) {
					const formLoai = response.data;
				}else {
					toast.error(response.message || 'Có sự cố xảy ra');
				}
			} else {
				toast.error('Có sự cố xảy ra');
			}
		};
	};

	const handleUpdateSubmit: SubmitFunction = ({ formData }) => {
		isSubmitting = true;
		formData.set('idChienDich', idChienDich);
		formData.set('id', updateId.toString());
		formData.set('ten', updateTenForm);
		formData.set('moTa', updateMoTa);
		formData.set('thoiGianBatDau', Utils.formatDateCallApi(updateThoiGianBatDau) || '');
		formData.set('thoiGianKetThuc', Utils.formatDateCallApi(updateThoiGianKetThuc) || '');
	

		return async ({ result, update }) => {
			isSubmitting = false;
			if (result.type === 'success' && result.data) {
				const response = result.data as any;
				if (response.status === 1) {
					toast.success('Cập nhật form thành công!');
					isOpenDialogUpdateForm = false;
					await update();
					goto(`?page=${currentPage}&idChienDich=${idChienDich}`, { invalidateAll: true });
				} else {
					toast.error(response.message || 'Có sự cố xảy ra');
				}
			} else {
				toast.error('Có sự cố xảy ra');
			}
		};
	};
	const handleDeleteForm: SubmitFunction =({}) =>{
        isSubmitting = true;

        return async ({result, update}) =>{
            isSubmitting = false;

            if(result.type === 'success' && result.data){
                const response = result.data as any;
                if(response.status === 1){
                    toast.success('Xóa form thành công!');
                    isOpenDialogDeleteForm = false;
                    selectedForm = null;
                    await update();
                    goto(`?page=${currentPage}&idChienDich=${idChienDich}`, {invalidateAll: true});
                }else{
                toast.error(response.message || 'Có sự cố xảy ra');
                }
            } else {
                toast.error('Có sự cố xảy ra');
            }
        };
        
    }




	const columns:IColumn[] =[
		{ header:'Tên Form', field :'tenForm', headerContainerStyle: 'width:25rem'},
		{ header: 'Mô tả', field: 'moTa', headerContainerStyle: 'width: 20rem' },
		{ header: 'Tổng số trường', field: 'tongSoTruong', headerContainerStyle: 'width: 10rem',cellStyle: 'text-align: center'  },
		{ header: 'Thời gian tạo', field: 'thoiGianTao', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
		{ header: 'Thời gian bắt đầu', field: 'thoiGianBatDau', headerContainerStyle: 'width: 10rem; ', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
		{ header: 'Thời gian kết thúc', field: 'thoiGianKetThuc', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
		{ header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP,customComponent: TblAction, cellStyle: 'text-align: center' }
	]
</script>
<Toaster />

<form method="POST" action="?/getFormById" use:enhance={handleGetFormById} hidden>
	<input type="hidden" name="idChienDich" value="" />
	<input type="hidden" name="id" value="" />
</form>
<div class="mb-6 flex flex-row justify-between items-center">
	<div>
		<h1 class="text-3xl font-bold text-gray-900">Quản lý Form</h1>
		<p class="text-gray-600 mt-1">Thêm, sửa, xóa form tại đây</p>
	</div>
	
</div>

<div class="flex gap-2 items-center">
	<div class="relative w-160">
		<Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
		<Input bind:value={searchQuery} placeholder="Tìm kiếm theo tên form" class="pl-10" />
	</div>
	<div class="flex-1 flex justify-end gap-4">
		<Button class="cursor-pointer">Tìm kiếm</Button>
		<Button class="cursor-pointer" onclick={openCreateForm}>Tạo form mới</Button>
	</div>
</div>

<div class="rounded-md border items-center">
	<DataTable data={data.data} {columns} />
</div>

<div class="flex justify-end items-center gap-2 pt-4">
	<Button variant="outline" size="sm" disabled={currentPage === 1} onclick={() => onPageChange(currentPage - 1)}>Previous</Button>
	{#each getPageNumbers() as page}
		<Button variant={currentPage === page ? 'default' : 'outline'} size="sm" class={currentPage === page ? 'bg-blue-600 text-white' : ''} onclick={() => onPageChange(page)}>{page}</Button>
	{/each}
	<Button variant="outline" size="sm" disabled={currentPage === totalPages} onclick={() => onPageChange(currentPage + 1)}>Next</Button>
</div>

<Dialog.Root open={isOpenDialogCreateForm}>
	<Dialog.Content class="sm:max-w-[425px]">
		<form method="POST" action="?/createForm" use:enhance={handleSubmit}>
			<Dialog.Header>
				<Dialog.Title>Tạo form</Dialog.Title>
				<Dialog.Description>Tạo form. Ấn lưu khi đã nhập xong thông tin</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid items-center gap-2">
					<Label for="tenLoaiForm">Tên form</Label>
					<Input id="tenLoaiForm" name="tenLoaiForm" required />
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
			<Dialog.Footer>
				<Button type="button" variant="outline" onclick={closeDialog} disabled={isSubmitting}>Hủy</Button>
				<Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Đang lưu...' : 'Lưu'}</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>

<Dialog.Root bind:open={isOpenDialogUpdateForm}>
	<Dialog.Content class="sm:max-w-[500px]">
		<form method="POST" action="?/updateForm" use:enhance={handleUpdateSubmit}>
			<Dialog.Header>
				<Dialog.Title>Cập nhật Form</Dialog.Title>
				<Dialog.Description>Cập nhật thông tin form. Ấn lưu khi đã nhập xong thông tin</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid items-center gap-2">
					<Label for="updateTenForm">Tên form</Label>
					<Input id="updateTenForm" bind:value={updateTenForm} required />
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
				
			</div>
			<Dialog.Footer>
				<Button type="button" variant="outline" onclick={closeUpdateDialog} disabled={isSubmitting}>Hủy</Button>
				<Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Đang lưu...' : 'Lưu'}</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>

<Dialog.Root bind:open={isOpenDialogDeleteForm}>
    <Dialog.Content class="sm:max-w-[425px]">
        <form method="POST" action="?/deleteForm" use:enhance={handleDeleteForm}>
            <input type="hidden" name="idChienDich" value={idChienDich || ''} />
			<input type="hidden" name="id" value={selectedForm || ''} />
            <Dialog.Header>
                <Dialog.Title>Xóa form</Dialog.Title>
                <Dialog.Description>Bạn có chắc chắn muốn xóa form này không? Hành động này không thể hoàn tác.</Dialog.Description>
            </Dialog.Header>
            <Dialog.Footer>
                <Button type="button" variant="outline" onclick={() => isOpenDialogDeleteForm = false} disabled={isSubmitting}>Hủy</Button>
                <Button type="submit" variant="destructive" disabled={isSubmitting}>
                    {isSubmitting ? 'Đang xóa...' : 'Xóa'}
                </Button>
            </Dialog.Footer>
        </form>
    </Dialog.Content>
</Dialog.Root>