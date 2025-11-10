<script lang="ts">
	import { goto } from '$app/navigation';
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '$lib/components/ui/card';
	import Download from '@tabler/icons-svelte/icons/download';
	import Edit from '@tabler/icons-svelte/icons/edit';
	import FileText from '@tabler/icons-svelte/icons/file-text';

    let { data } = $props();

    function navigateToDetail(id: number | null | undefined) {
		goto(`/gv/form/${id}`);
    }
</script>

<div class="mb-6">
	<h1 class="text-3xl font-bold text-gray-900">Quản lý form</h1>
	<p class="text-gray-600 mt-1">Chọn form muốn cập nhật</p>
</div>
<div class="grid grid-cols-1 md:grid-cols-3 gap-3">
    {#each data.data as form (form.id) }
        <Card class="border-0 shadow-lg hover:shadow-xl transition-shadow">
		<CardHeader>
			<div class="flex items-start justify-between">
				<div class="flex items-center space-x-3 justify-center">
					<div class="w-12 h-12 bg-blue-100 rounded-xl flex items-center justify-center">
						<FileText class="w-6 h-6 text-[#0b57d0]" />
					</div>
					<div class="flex-1">
						<CardTitle class="text-xl">{form.tenForm}</CardTitle>
						<CardDescription>{form.moTa}</CardDescription>
					</div>
				</div>
			</div>
		</CardHeader>
		<CardContent>
			<div class="space-y-4">
				<div class="text-sm text-gray-600">
					<p><strong>Tổng số trường:</strong> 1</p>
					<p class="mt-1">
						<strong>Lần cập nhật gần nhất:</strong>
						{new Date().toLocaleDateString('en-GB')}
					</p>
				</div>

				<div class="flex gap-3 justify-end">
					<Button variant="outline" class="cursor-pointer">
						<Download class="w-4 h-4 mr-1" />
						Tải file DOCX
					</Button>
					<Button class="cursor-pointer" onclick={() => navigateToDetail(form.id)}>
						<Edit class="w-4 h-4 mr-1" />
						Cập nhật form
					</Button>
				</div>
			</div>
		</CardContent>
	</Card>
    {/each}
	
</div>
