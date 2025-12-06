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

	let searchQuery = $state('');

	/*function navigateToDetail(id: number | null | undefined) {
		goto(`/admin/form-management/template`);
	}*/
	let idChienDich = $derived($page.url.searchParams.get('idChienDich') || '0');
    let currentPage = $derived(data.currentPage || 1);
	let totalPages = $derived(data.totalPages || 1);

	setContext('onViewFormDetail', (idFormLoai: number) => {
	navigateToFormDetail(idFormLoai);
	});
	function navigateToFormDetail(idFormLoai:number){
		goto(`/gv/campaign/form/${idFormLoai}`);
	}



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




	const columns:IColumn[] =[
		{ header:'Tên Form', field :'tenForm', headerContainerStyle: 'width:25rem'},
		{ header: 'Mô tả', field: 'moTa', headerContainerStyle: 'width: 20rem' },
		{ header: 'Tổng số trường', field: 'tongSoTruong', headerContainerStyle: 'width: 10rem',cellStyle: 'text-align: center'  },
		{ header: 'Thời gian tạo', field: 'thoiGianTao', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
        { header: 'Thời gian cập nhật gần nhất', field: 'thoiGianCapNhatGanNhat', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
		{ header: 'Thời gian bắt đầu', field: 'thoiGianBatDau', headerContainerStyle: 'width: 10rem; ', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
		{ header: 'Thời gian kết thúc', field: 'thoiGianKetThuc', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, cellStyle: 'text-align: center' },
		{ header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP,customComponent: TblAction, cellStyle: 'text-align: center' }
	]
</script>
<Toaster />


<div class="mb-6 flex flex-row justify-between items-center">
	<div>
		<h1 class="text-3xl font-bold text-gray-900">Xem Form</h1>
		<p class="text-gray-600 mt-1">Xem chi tiết form tại đây</p>
	</div>
	
</div>

<div class="flex gap-2 items-center">
	<div class="relative w-160">
		<Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
		<Input bind:value={searchQuery} placeholder="Tìm kiếm theo tên form" class="pl-10" />
	</div>
	<div class="flex-1 flex justify-end gap-4">
		<Button class="cursor-pointer">Tìm kiếm</Button>
		
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

