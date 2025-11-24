<script lang="ts">
	import { goto } from "$app/navigation";
	import { Card, CardContent } from "$lib/components/ui/card";
	import { Progressbar } from "flowbite-svelte";
	import { Users, CheckCircle, Clock, AlertTriangle, Building2, Filter, FileDown } from "lucide-svelte";
	import { Button } from "$lib/components/ui/button";
	import { Input } from "$lib/components/ui/input";
	import { Label } from "$lib/components/ui/label";
	import { toast } from "svelte-sonner";
	import Toaster from "$lib/components/ui/toaster/toaster.svelte";

	
	let { data } = $props();

    let searchQuery = $state('');
    let statusFilter = $state('all');
    
    let currentPageToChucNhanVien = $derived(data.toChucNhanVien.currentPage || 1);
    let totalPagesToChucNhanVien = $derived(data.toChucNhanVien.totalPages || 1);
     function onPageChange(page: number) {
        goto(`?page=${page}`, { 
            keepFocus: true, 
            noScroll: true,
            invalidateAll: true 
        });
    }

    function getPageToChucNhanVienNumbers() {
        const pages = [];
        for (let i = 1; i <= totalPagesToChucNhanVien; i++) {
            pages.push(i);
        }
        return pages;
    }

    function exportToCSV() {
        toast.success('Chức năng đang được phát triển!');
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

		<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
			{#each data.toChucNhanVien?.data || [] as dept}
				<Card class="shadow-sm">
					<CardContent class="p-4">
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
								<Progressbar progress={dept.progress || 0} size="h-2" color="blue" />
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
				disabled={currentPageToChucNhanVien === 1}
				onclick={() => onPageChange(currentPageToChucNhanVien - 1)}
			>
				Previous
			</Button>
			
			{#each getPageToChucNhanVienNumbers() as page}
				<Button 
					variant={currentPageToChucNhanVien === page ? "default" : "outline"}
					size="sm"
					class={currentPageToChucNhanVien === page ? "bg-blue-600 text-white" : ""}
					onclick={() => onPageChange(page)}
				>
					{page}
				</Button>
			{/each}

			<Button 
				variant="outline" 
				size="sm"
				disabled={currentPageToChucNhanVien === totalPagesToChucNhanVien}
				onclick={() => onPageChange(currentPageToChucNhanVien + 1)}
			>
				Next
			</Button>
		</div>
	</CardContent>
</Card>

<Card class="shadow-xl">
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
					placeholder="Search by name or email..."
					class="w-full"
				/>
			</div>
			<div>
				<Label for="status-filter" class="text-sm font-medium text-gray-700 mb-2">Status Filter</Label>
				<select 
					id="status-filter"
					bind:value={statusFilter}
					class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
				>
					<option value="all">Tất cả</option>
					<option value="completed">Đã kiểm tra</option>
					<option value="pending">Chưa kiểm tra</option>
					<option value="not-imported">Chưa được import data</option>
				</select>
			</div>
		</div>
	</CardContent>
</Card>