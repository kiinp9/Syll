<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent } from '$lib/components/ui/card';
	import { Input } from '$lib/components/ui/input';

	import Building from '@tabler/icons-svelte/icons/building';
	import Search from '@tabler/icons-svelte/icons/search';
	import Trash from '@tabler/icons-svelte/icons/trash';
	import Edit from '@tabler/icons-svelte/icons/edit';
	import { goto } from '$app/navigation';

	let { data } = $props();

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
</script>

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
				<Button class="gap-2">
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
								<Button variant="ghost" size="icon" class="text-red-600 hover:text-red-700 hover:bg-red-50">
									<Trash class="w-4 h-4" />
								</Button>
							</div>
							<p class="text-sm text-gray-600">{organization.moTa}</p>
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