<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { CogSolid } from 'flowbite-svelte-icons';
	import * as Popover from '$lib/components/ui/popover';
	import Edit from '@tabler/icons-svelte/icons/edit';
	import Trash from '@tabler/icons-svelte/icons/trash';
	import type { IViewChienDich } from '$lib/models/campaign/campaign.model';
	import { getContext } from 'svelte';

	let { rowData, rowIndex }: { rowData: IViewChienDich; rowIndex: number } = $props();
	
	let isOpenPopover = $state(false);

	const onUpdateChienDich = getContext<(data: IViewChienDich) => void>('onUpdateChienDich');
	const onDeleteChienDich = getContext<(data: IViewChienDich) => void>('onDeleteChienDich');

	function handleUpdate() {
		isOpenPopover = false;
		if (onUpdateChienDich) onUpdateChienDich(rowData);
	}

	function handleDelete() {
		isOpenPopover = false;
		if (onDeleteChienDich) onDeleteChienDich(rowData);
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
			<button type="button" class="px-4 py-2 text-left text-sm hover:bg-gray-100 cursor-pointer border-b flex items-center gap-2" onclick={handleUpdate}>
				<Edit class="w-4 h-4" />
				Cập nhật 
			</button>
			<button type="button" class="px-4 py-2 text-left text-sm hover:bg-gray-100 cursor-pointer text-red-600 flex items-center gap-2" onclick={handleDelete}>
				<Trash class="w-4 h-4" />
				Xóa
			</button>
		</div>
	</Popover.Content>
</Popover.Root>