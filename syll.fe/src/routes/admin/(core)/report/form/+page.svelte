<script lang="ts">
	import { goto } from '$app/navigation';
	import Button from '$lib/components/ui/button/button.svelte';
	import CardContent from '$lib/components/ui/card/card-content.svelte';
	import CardDescription from '$lib/components/ui/card/card-description.svelte';
	import CardFooter from '$lib/components/ui/card/card-footer.svelte';
	import CardHeader from '$lib/components/ui/card/card-header.svelte';
	import CardTitle from '$lib/components/ui/card/card-title.svelte';
	import Card from '$lib/components/ui/card/card.svelte';
	import Download from '@tabler/icons-svelte/icons/download';
	import Edit from '@tabler/icons-svelte/icons/edit';
	import FileText from '@tabler/icons-svelte/icons/file-text';
	import { ChartColumnIcon } from 'lucide-svelte';
	

	let { data } = $props();
	function navigateToDetail(idForm:number|null|undefined) {
		goto(`/admin/report/form/${idForm}/chi-tiet`);
	}

</script>


<div class="mb-6 flex flex-row justify-between items-center">
	<div>
		<h1 class="text-3xl font-bold text-gray-900">Thống kê thông tin form</h1>
		<p class="text-gray-600 mt-1">Chọn form để xem thông kê chi tiết thông tin form</p>
	</div>
	
</div>
<div class="grid grid-cols-1 md:grid-cols-3 gap-3">
	{#each data.data as form (form.id)}
		<Card class="border-0 shadow-lg hover:shadow-xl transition-shadow flex flex-col justify-between">
			<div>
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
							
						</div>
					</div>
				</CardContent>
			</div>
			<CardFooter
				><div class="flex gap-3 justify-end w-full">
					<Button class="cursor-pointer"onclick={() => navigateToDetail(form.id)} >
						<ChartColumnIcon class="w-4 h-4 mr-1" />
						Xem thống kê
					</Button>
				</div></CardFooter
			>
		</Card>
	{/each}
</div>