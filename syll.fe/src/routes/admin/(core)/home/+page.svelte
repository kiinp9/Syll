<script lang="ts">
	import { Card, CardContent, CardHeader, CardTitle } from '$lib/components/ui/card';
	import Building from '@tabler/icons-svelte/icons/building';
	import Forms from '@tabler/icons-svelte/icons/forms';
	import Users from '@tabler/icons-svelte/icons/users';


		
	let { data } = $props();
</script>

<div class="mb-6">
	<h1 class="text-3xl font-bold text-gray-900">Dashboard</h1>
	<p class="text-gray-600 mt-1">Tổng quan các thông số</p>
</div>
<div class="grid grid-cols-1 md:grid-cols-3 gap-6">
	<Card class="border-t-4 border-t-white-200 shadow-lg hover:shadow-xl transition-shadow">
		<CardHeader class="flex flex-row items-center justify-between pb-2">
			<CardTitle class="text-sm font-medium text-gray-600">Tổng số cán bộ giảng viên</CardTitle>
			<Users class="w-5 h-5 text-[#0b57d0]" />
		</CardHeader>
		<CardContent>
			<div class="text-4xl font-bold text-gray-900">{data.totalData.totalNhanVien}</div>
			<p class="text-sm text-gray-500 mt-1">Nhân viên được đăng kí</p>
		</CardContent>
	</Card>
	<Card class="border-t-4 border-t-white-200 shadow-lg hover:shadow-xl transition-shadow">
		<CardHeader class="flex flex-row items-center justify-between pb-2">
			<CardTitle class="text-sm font-medium text-gray-600">Tổng số tổ chức</CardTitle>
			<Building class="w-5 h-5 text-[#0b57d0]" />
		</CardHeader>
		<CardContent>
			<div class="text-4xl font-bold text-gray-900">{data.totalData.totalToChuc}</div>
			<p class="text-sm text-gray-500 mt-1">Tổ chức được đăng kí</p>
		</CardContent>
	</Card>
	<Card class="border-t-4 border-t-white-200 shadow-lg hover:shadow-xl transition-shadow">
		<CardHeader class="flex flex-row items-center justify-between pb-2">
			<CardTitle class="text-sm font-medium text-gray-600">Tổng số loại biểu mẫu</CardTitle>
			<Forms class="w-5 h-5 text-[#0b57d0]" />
		</CardHeader>
		<CardContent>
			<div class="text-4xl font-bold text-gray-900">{data.totalData.totalForm}</div>
			<p class="text-sm text-gray-500 mt-1">Loại biểu mẫu được đăng kí</p>
		</CardContent>
	</Card>
</div>
