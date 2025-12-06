<script lang="ts">
	import { goto } from '$app/navigation';
	import { Button } from '$lib/components/ui/button';
	import type { IColumn } from '$lib/shared/models/data-table.models';
	import { CellViewTypes } from '$lib/shared/constants/data-table.constants';

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

	let searchQuery = $state('');
    let selectedChienDich = $state<number | null>(null);


	const df = new DateFormatter('vi-VN', { dateStyle: 'long' });



	function parseCalendarDate(dateStr: string | Date | null | undefined): CalendarDate | undefined {
		if (!dateStr) return undefined;
		const date = typeof dateStr === 'string' ? new Date(dateStr) : dateStr;
		return new CalendarDate(date.getFullYear(), date.getMonth() + 1, date.getDate());
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
		
	];

	function onCellClick(col: IColumn, rowData: any) {
		if (col.field === 'tenChienDich') {
			goto(`/gv/campaign/form?idChienDich=${rowData.id}`);
		}
	}

	
</script>

<Toaster />


<div class="mb-6 flex flex-row justify-between items-center">
	<div>
		<h1 class="text-3xl font-bold text-gray-900">Xem chiến dịch</h1>
		<p class="text-gray-600 mt-1">Xem các chiến dịch tại đây</p>
	</div>
</div>

<div class="flex gap-2 items-center">
	<div class="relative w-160">
		<Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
		<Input bind:value={searchQuery} placeholder="Tìm kiếm theo tên chiến dịch" class="pl-10" />
	</div>
	<div class="flex-1 flex justify-end gap-4">
		<Button class="cursor-pointer">Tìm kiếm</Button>
		
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

