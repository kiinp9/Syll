
<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { CogSolid } from 'flowbite-svelte-icons';
	import * as Popover from '$lib/components/ui/popover';
	import Edit from '@tabler/icons-svelte/icons/edit';
	import Trash from '@tabler/icons-svelte/icons/trash';
	import type { IViewChienDich, IViewForm } from '$lib/models/campaign/campaign.model';
	import FileText from '@tabler/icons-svelte/icons/file-text';
	import { getContext } from 'svelte';

	let { rowData, rowIndex }: { rowData: IViewForm; rowIndex: number } = $props();
	
	let isOpenPopover = $state(false);

   
    const onViewFormDetail = getContext<(idFormLoai: number) => void>('onViewFormDetail');


    function handleView() {
		// TODO: Xử lý logic delete

		isOpenPopover = false;
        if (onViewFormDetail) onViewFormDetail(rowData.id);
	}
</script>

<Popover.Root bind:open={isOpenPopover}>
	<Popover.Trigger>
		<Button variant="ghost" size="icon">
			<CogSolid class="w-4 h-4 text-blue-600" />
		</Button>
	</Popover.Trigger>
	<Popover.Content class="w-40 p-0" align="end">
		<div class="flex flex-col">
            <button 
				type="button" 
				class="px-4 py-2 text-left text-sm hover:bg-gray-100 cursor-pointer border-b flex items-center gap-2" 
				onclick={handleView}
			>
				<FileText class="w-4 h-4" />
				Chi tiết
			</button>
			
		</div>
	</Popover.Content>
</Popover.Root>