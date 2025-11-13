<script lang="ts">
	import { flip } from 'svelte/animate';
	import { dndzone } from 'svelte-dnd-action';
	import type { IFormBlockConfig, IFormConfig } from '$lib/models/form-config/form-config.model';

	interface IToolBoxItem {
		type: string;
		name: string;
	}

	let toolBoxItems: IToolBoxItem[] = [
		{
			type: 'block',
			name: 'block'
		},
		{
			type: 'row',
			name: 'row'
		},
		{
			type: 'inputText',
			name: 'inputText'
		},
		{
			type: 'dropdownText',
			name: 'dropdownText'
		},
		{
			type: 'dropdownDate',
			name: 'dropdownDate'
		},
		{
			type: 'checkbox',
			name: 'checkbox'
		},
		{
			type: 'table',
			name: 'table'
		},
		{
			type: 'itemText',
			name: 'itemText'
		}
	];
	let items = [
		{ id: 1, name: 'item1' },
		{ id: 2, name: 'item2' },
		{ id: 3, name: 'item3' },
		{ id: 4, name: 'item4' }
	];
	let page: IFormBlockConfig[] = $state([]);
	const flipDurationMs = 300;

	function handleDragStart(
		e: DragEvent & {
			currentTarget: EventTarget & HTMLDivElement;
		},
		item: IToolBoxItem
	) {
		e.dataTransfer?.setData('application/json', JSON.stringify(item));
	}

	function allowDrop(
		e: DragEvent & {
			currentTarget: EventTarget & HTMLDivElement;
		}
	) {
		e.preventDefault();
	}

	function handleDrop(
		e: DragEvent & {
			currentTarget: EventTarget & HTMLDivElement;
		}
	) {
		e.preventDefault();

		if (e.dataTransfer) {
			const item = JSON.parse(e.dataTransfer.getData('application/json'));

			// Only block allowed at root level
			if (item.type !== 'block') return;

			const newBlock = { type: 'block', id: crypto.randomUUID(), children: [] };
			// page.update((p) => [...p, newBlock]);
			page.push(newBlock);
		}
	}
</script>

<div class="grid grid-cols-4 w-full gap-2">
	<div class="border w-full p-2 flex flex-col space-y-2">
		{#each toolBoxItems as item (item.name)}
			<!-- svelte-ignore a11y_no_static_element_interactions -->
			<div
				class="border rounded-sm p-2"
				draggable="true"
				ondragstart={(e) => handleDragStart(e, item)}
			>
				{item.name}
			</div>
		{/each}
	</div>
	<!-- svelte-ignore a11y_no_static_element_interactions -->
	<div class="col-span-3 border w-full p-2" ondragover={allowDrop} ondrop={handleDrop}>
		<!-- svelte-ignore a11y_no_static_element_interactions -->
		{#each page as block (block.id)}
			<div class="block">
				<div class="block-header">Block</div>
				<!-- <div class="block-body">
                    {#each block.children as row}
                    <Row {row} />
                    {/each}
                </div> -->
			</div>
		{/each}
	</div>
</div>
