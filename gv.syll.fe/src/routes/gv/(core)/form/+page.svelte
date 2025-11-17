<script lang="ts">
	import { goto } from '$app/navigation';
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '$lib/components/ui/card';
	import Download from '@tabler/icons-svelte/icons/download';
	import Edit from '@tabler/icons-svelte/icons/edit';
	import FileText from '@tabler/icons-svelte/icons/file-text';


	import DialogContent from '$lib/components/ui/dialog/dialog-content.svelte';
	import DialogHeader from '$lib/components/ui/dialog/dialog-header.svelte';
	
	import DialogFooter from '$lib/components/ui/dialog/dialog-footer.svelte';
	import DialogTitle from '$lib/components/ui/dialog/dialog-title.svelte';
	import DialogDescription from '$lib/components/ui/dialog/dialog-description.svelte';
	import { Dialog } from '$lib/components/ui/dialog/index.js';

    let { data } = $props();
	let errorDialogOpen = $state(false);
	let errorMessage = $state('');

    function navigateToDetail(id: number | null | undefined) {
		goto(`/gv/form/${id}`);
    }
	async function handleDownloadForm(id: number | null | undefined) {
        try {
            const response = await fetch(`/gv/form/${id}/download`, {
            method: 'POST'
            });
			/*if (!response.ok) {
			    alert('Có lỗi xảy ra khi tải file');
			    return;
		    }*/
			if (!response.ok) {
				const errorText = await response.text();
				try {
					const errorData = JSON.parse(errorText);
					errorMessage = errorData.message || 'Có lỗi xảy ra khi tải file';
				} catch {
					errorMessage = 'Có lỗi xảy ra khi tải file';
				}
				errorDialogOpen = true;
			    return;
		    }

			const blob = await response.blob();
        
            //DEBUG
            console.log('Blob size:', blob.size);
            console.log('Blob type:', blob.type);
			const contentDisposition = response.headers.get('Content-Disposition');
			const fileNameMatch = contentDisposition?.match(/filename="(.+)"/);
			const fileName = fileNameMatch?.[1] || `SoYeuLyLich_${new Date().toISOString().slice(0, 19).replace(/[:-]/g, '')}.docx`;



            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = fileName;
			document.body.appendChild(a);
			a.click();
			document.body.removeChild(a);
			URL.revokeObjectURL(url);
           
        } catch (error) {
            console.error('Error downloading file:', error);
            errorMessage = 'Có lỗi xảy ra khi tải file';
			errorDialogOpen = true;
       }
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
					<p><strong>Tổng số trường:</strong> {form.tongSoTruong}</p>
					<p class="mt-1">
						<strong>Lần cập nhật gần nhất:</strong>
						{form.thoiGianCapNhatGanNhat
						? new Date(form.thoiGianCapNhatGanNhat).toLocaleDateString('vi-VN')
						: 'Chưa cập nhật'
						}
					</p>
				</div>

				<div class="flex gap-3 justify-end">
					<Button variant="outline" class="cursor-pointer" onclick={() => handleDownloadForm(form.id)}>
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
<Dialog bind:open={errorDialogOpen}>
	<DialogContent>
		<DialogHeader>
			<DialogTitle>Lỗi</DialogTitle>
			<DialogDescription>
				{errorMessage}
			</DialogDescription>
		</DialogHeader>
		<DialogFooter>
			<Button onclick={() => errorDialogOpen = false}>OK</Button>
		</DialogFooter>
	</DialogContent>
</Dialog>