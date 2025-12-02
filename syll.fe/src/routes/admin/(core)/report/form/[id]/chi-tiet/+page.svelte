<script lang="ts">
	import { goto } from "$app/navigation";
	import { Card, CardContent } from "$lib/components/ui/card";
	import { Users, CheckCircle, Clock, AlertTriangle, Building2, Filter, FileDown } from "lucide-svelte";
	import { Button } from "$lib/components/ui/button";
	import { Input } from "$lib/components/ui/input";
	import { Label } from "$lib/components/ui/label";
	import { toast } from "$lib/utils/toast.utils.js";
	import Toaster from "$lib/components/ui/toaster/toaster.svelte";
	import { ReportConstants } from "$lib/constants/report.constants.js";

	
	let { data } = $props();

    let searchQuery = $state('');
    let statusFilter = $state<number | null>(null);
	let selectedToChuc = $state<number | null>(null);
	let nhanVienSection: HTMLDivElement;
    
    let currentPageToChuc = $derived(data.toChuc.currentPage || 1);
    let totalPagesToChuc = $derived(data.toChuc.totalPages || 1);
    
    let currentPageNhanVien = $derived(data.nhanVien.currentPage || 1);
    let totalPagesNhanVien = $derived(data.nhanVien.totalPages || 1);
    
    function onPageChange(page: number) {
        goto(`?page=${page}`, { 
            keepFocus: true, 
            noScroll: true,
            invalidateAll: true 
        });
    }

	const filterOptions = [
		{ value: ReportConstants.TatCa, label: 'Tất cả' },
		{ value: ReportConstants.DaCheck, label: 'Đã check' },
		{ value: ReportConstants.ChuaCheck, label: 'Chưa check' },
		{ value: ReportConstants.ChuaImportData, label: 'Chưa import data' },
	];

	function getFilterLabel(value?: number) {
		if (!value) return 'Không xác định';
        return filterOptions.find(opt => opt.value === value)?.label || 'Không xác định';
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

    function exportToCSV() {
        toast.success("Tính năng đang được phát triển!");
    }

    function selectDepartment(deptId: number) {
        selectedToChuc = deptId;
        goto(`?page=1&idToChuc=${deptId}`, { invalidateAll: true }).then(() => {
            setTimeout(() => {
                nhanVienSection?.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }, 100);
        });
    }

    function onNhanVienPageChange(page: number) {
        goto(`?page=${page}&idToChuc=${selectedToChuc}&keyword=${searchQuery}&status=${statusFilter || ''}`, { 
            keepFocus: true, 
            noScroll: true,
            invalidateAll: false
        });
    }

    function handleSearchChange() {
        goto(`?page=1&idToChuc=${selectedToChuc}&keyword=${searchQuery}&status=${statusFilter || ''}`, { 
            keepFocus: true,
			noScroll: true,
            invalidateAll: false
        });
    }

    function handleStatusFilterChange() {
        goto(`?page=1&idToChuc=${selectedToChuc}&keyword=${searchQuery}&status=${statusFilter || ''}`, { 
            keepFocus: true,
			noScroll: true,
            invalidateAll: false
        });
    }

    function getStatusLabel(status?: number): string {
        switch (status) {
            case ReportConstants.DaCheck:
                return 'Đã check';
            case ReportConstants.ChuaCheck:
                return 'Chưa check';
            case ReportConstants.ChuaImportData:
                return 'Chưa import data';
            default:
                return 'Không xác định';
        }
    }

    function getStatusBadgeClass(status?: number): string {
        switch (status) {
            case ReportConstants.DaCheck:
                return "bg-green-100 text-green-700";
            case ReportConstants.ChuaCheck:
                return "bg-amber-100 text-amber-700";
            case ReportConstants.ChuaImportData:
                return "bg-red-100 text-red-700";
            default:
                return "bg-gray-100 text-gray-700";
        }
    }

    function formatDate(date?: Date | string): string {
        if (!date) return 'N/A';
        const d = new Date(date);
        return d.toLocaleDateString('vi-VN', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' });
    }

</script>

<Toaster />

<div class="mb-6 flex flex-row justify-between items-center">
	<div>
		<h1 class="text-3xl font-bold text-gray-900">Thống kê chi tiết thông tin form</h1>
		<p class="text-gray-600 mt-1">Xem chi tiết thông kê thông tin form tại đây</p>
	</div>
</div>

<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
    
	<Card class="border-t-4 border-t-white-400 shadow-xl">
		<CardContent class="p-6">
			<div class="flex items-center justify-between mb-4">
				<span class="text-gray-800 text-lg font-semibold">Tổng số nhân viên</span>
				<div class="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
					<Users class="w-6 h-6 text-blue-600" />
				</div>
			</div>
			<div class="text-4xl font-bold text-gray-900 mb-1">{data.totalData?.totalNhanVien || 0}</div>
			<div class="text-sm text-gray-500">Nhân viên được đăng kí</div>
		</CardContent>
	</Card>

	<Card class="border-t-4 border-t-white-400 shadow-xl">
		<CardContent class="p-6">
			<div class="flex items-center justify-between mb-4">
				<span class="text-gray-800 text-lg font-semibold">Đã kiểm tra form</span>
				<div class="w-10 h-10 bg-green-100 rounded-full flex items-center justify-center">
					<CheckCircle class="w-6 h-6 text-green-600" />
				</div>
			</div>
			<div class="text-4xl font-bold text-green-600 mb-1">{data.totalData?.totalNhanVienCheckForm || 0}</div>
            <div class="text-sm text-gray-500">
	            {data.totalData?.totalNhanVien && data.totalData?.totalNhanVienCheckForm 
		        ? ((data.totalData.totalNhanVienCheckForm / data.totalData.totalNhanVien) * 100).toFixed(1)
		        : '0'}% đã hoàn thành
            </div>
		</CardContent>
	</Card>

	<Card class="border-t-4 border-t-white-400 shadow-xl">
		<CardContent class="p-6">
			<div class="flex items-center justify-between mb-4">
				<span class="text-gray-800 text-lg font-semibold">Chưa kiểm tra form</span>
				<div class="w-10 h-10 bg-orange-100 rounded-full flex items-center justify-center">
					<Clock class="w-6 h-6 text-orange-600" />
				</div>
			</div>
			<div class="text-4xl font-bold text-orange-600 mb-1">{data.totalData?.totalNhanVienChuaCheckForm || 0}</div>
			<div class="text-sm text-gray-500">Nhân viên chưa kiểm tra form</div>
		</CardContent>
	</Card>

	<Card class="border-t-4 border-t-white-400 shadow-xl">
		<CardContent class="p-6">
			<div class="flex items-center justify-between mb-4">
				<span class="text-gray-800 text-lg font-semibold">Chưa import data form</span>
				<div class="w-10 h-10 bg-amber-100 rounded-full flex items-center justify-center">
					<AlertTriangle class="w-6 h-6 text-amber-600" />
				</div>
			</div>
			<div class="text-4xl font-bold text-amber-600 mb-1">{data.totalData?.totalNhanVienChuaImportData || 0}</div>
			<div class="text-sm text-gray-500">Nhân viên chưa được import data</div>
		</CardContent>
	</Card>
</div>

<Card class="shadow-xl mb-6">
	<CardContent class="p-6">
		<div class="mb-6">
			<h2 class="text-2xl font-bold text-gray-900">Phòng ban</h2>
			<p class="text-gray-500 text-sm">Thống kê theo từng phòng ban</p>
		</div>

		<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
			{#each data.toChuc?.data || [] as dept}
				<Card 
					class="shadow-sm cursor-pointer hover:shadow-md transition-shadow"
					onclick={() => selectDepartment(dept.idToChuc!)}
				>
					<CardContent class="p-3">
						<div class="space-y-3">
							<div class="flex items-start gap-3">
								<div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center flex-shrink-0">
									<Building2 class="w-5 h-5 text-blue-600" />
								</div>
								<div class="flex-1 min-w-0">
									<h3 class="font-semibold text-gray-900 text-sm leading-tight">{dept.tenToChuc}</h3>
									<p class="text-gray-500 text-xs">{dept.totalNhanVienToChuc} members</p>
								</div>
							</div>

							<div class="space-y-2">
								<div class="flex items-center justify-between">
									<span class="text-xs text-gray-600">Completion</span>
									<span class="text-sm font-semibold text-blue-600">
										{dept.progress?.toFixed(1) || 0}%
									</span>
								</div>
								
								<div class="flex h-2 w-full rounded-full bg-gray-200 overflow-hidden">
									<div 
										class="bg-blue-600 transition-all duration-300"
										style="width: {dept.progress || 0}%"
									></div>
									<div 
										class="bg-gray-300"
										style="width: {100 - (dept.progress || 0)}%"
									></div>
								</div>
								
								<div class="flex items-center justify-between text-xs">
									<span class="text-green-600 flex items-center gap-1">
										<CheckCircle class="w-3 h-3" />
										{dept.totalNhanVienToChucCheckForm}
									</span>
									<span class="text-orange-600 flex items-center gap-1">
										<Clock class="w-3 h-3" />
										{dept.totalNhanVienToChucChuaCheckForm}
									</span>
								</div>
							</div>
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

<Card class="shadow-xl mb-6">
	<CardContent class="p-6">
		<div class="flex items-center justify-between mb-6">
			<div class="flex items-center gap-2">
				<Filter class="w-5 h-5 text-gray-700" />
				<h2 class="text-lg font-semibold text-gray-900">Filters & Export</h2>
			</div>
			<Button class="bg-green-600 hover:bg-green-700 text-white gap-2" onclick={exportToCSV}>
				<FileDown class="w-4 h-4" />
				Export to CSV
			</Button>
		</div>

		<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
			<div>
				<Label for="search-input" class="text-sm font-medium text-gray-700 mb-2">Search</Label>
				<Input
					id="search-input"
					bind:value={searchQuery}
					onkeydown={(e) => e.key === 'Enter' && handleSearchChange()}
					placeholder="Search by name or email..."
					class="w-full"
				/>
			</div>
			<div>
				<Label for="status-filter" class="text-sm font-medium text-gray-700 mb-2">Status Filter</Label>
				<select 
					id="status-filter"
					bind:value={statusFilter}
					onchange={handleStatusFilterChange}
					class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
				>
					<option value={null}>Chọn filter</option>
					{#each filterOptions as option}
						<option value={option.value}>{option.label}</option>
					{/each}
				</select>
			</div>
		</div>
	</CardContent>
</Card>

<div bind:this={nhanVienSection}>
	<Card class="shadow-xl">
		<CardContent class="p-6">
			<div class="mb-6">
				<h2 class="text-2xl font-bold text-gray-900">Staff Status & Activity</h2>
				<p class="text-gray-500 text-sm">
					{#if selectedToChuc && data.nhanVien?.data.length > 0}
						Showing {data.nhanVien.data.length} entries
					{:else}
						Showing 0 entries
					{/if}
				</p>
			</div>

			<div class="overflow-x-auto">
				<table class="w-full">
					<thead class="border-b bg-gray-50">
						<tr>
							<th class="text-left p-4 font-semibold text-gray-700">Name</th>
							<th class="text-left p-4 font-semibold text-gray-700">Email</th>
							<th class="text-left p-4 font-semibold text-gray-700">Department</th>
							<th class="text-left p-4 font-semibold text-gray-700">Status</th>
							<th class="text-left p-4 font-semibold text-gray-700">Last Modified</th>
						</tr>
					</thead>
					<tbody>
						{#if selectedToChuc && data.nhanVien?.data.length > 0}
							{#each data.nhanVien.data as staff}
								<tr class="border-b hover:bg-gray-50 transition-colors">
									<td class="p-4 text-sm font-medium text-gray-900">{staff.hoVaTen}</td>
									<td class="p-4 text-sm text-gray-600">{staff.email}</td>
									<td class="p-4 text-sm text-gray-600">{staff.toChuc?.tenToChuc || 'N/A'}</td>
									<td class="p-4 text-sm">
										<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium {getStatusBadgeClass(staff.status)}">
											{getStatusLabel(staff.status)}
										</span>
									</td>
									<td class="p-4 text-sm text-gray-600">
										{formatDate(staff.lastModified)}
									</td>
								</tr>
							{/each}
						{:else}
							<tr>
								<td colspan="5" class="p-4 text-center text-gray-500">
									Chọn một tổ chức để xem danh sách nhân viên
								</td>
							</tr>
						{/if}
					</tbody>
				</table>
			</div>

			{#if selectedToChuc && data.nhanVien?.data.length > 0}
				<div class="flex items-center justify-between pt-4 text-sm text-gray-600">
					<span>Page {currentPageNhanVien} of {totalPagesNhanVien}</span>
					<div class="flex gap-2">
						<Button 
							variant="outline" 
							size="sm"
							disabled={currentPageNhanVien === 1}
							onclick={() => onNhanVienPageChange(currentPageNhanVien - 1)}
						>
							Previous
						</Button>
						
						{#each getPageNhanVienNumbers() as page}
							<Button 
								variant={currentPageNhanVien === page ? "default" : "outline"}
								size="sm"
								class={currentPageNhanVien === page ? "bg-blue-600 text-white" : ""}
								onclick={() => onNhanVienPageChange(page)}
							>
								{page}
							</Button>
						{/each}

						<Button 
							variant="outline" 
							size="sm"
							disabled={currentPageNhanVien === totalPagesNhanVien}
							onclick={() => onNhanVienPageChange(currentPageNhanVien + 1)}
						>
							Next
						</Button>
					</div>
				</div>
			{/if}
		</CardContent>
	</Card>
</div>