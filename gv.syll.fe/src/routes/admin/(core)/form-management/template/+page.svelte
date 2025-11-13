<script lang="ts">
	import { flip } from 'svelte/animate';
	import { dndzone } from 'svelte-dnd-action';
	import type { IFormBlockConfig, IFormConfig } from '$lib/models/form-config/form-config.model';
	import Trash from '@tabler/icons-svelte/icons/trash';

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

	function handleDropBlock(
		e: DragEvent & {
			currentTarget: EventTarget & HTMLDivElement;
		}
	) {
		e.preventDefault();
		console.log(e.currentTarget.dataset);
        const index = e.currentTarget.dataset?.index;

		if (e.dataTransfer) {
			const item = JSON.parse(e.dataTransfer.getData('application/json'));

			// Only block allowed at root level
			if (item.type !== 'block') return;

			const newBlock = { type: 'block', id: crypto.randomUUID(), children: [] };
			// page.update((p) => [...p, newBlock]);
			// page.push(newBlock);
			page.splice(Number(index), 0, newBlock)
		}
	}

	function handleDropRow(
		e: DragEvent & {
			currentTarget: EventTarget & HTMLDivElement;
		},
		blockIndex: number
	) {
		e.preventDefault();

		if (e.dataTransfer) {
			const item = JSON.parse(e.dataTransfer.getData('application/json'));

			if (item.type !== 'row') return;

			const newRow = { type: 'row', id: crypto.randomUUID(), children: [] };
			// page.update((p) => [...p, newBlock]);
			page[blockIndex].children.push(newRow);
		}
	}

	function handleDropItem(
		e: DragEvent & {
			currentTarget: EventTarget & HTMLDivElement;
		},
		blockIndex: number,
		rowIndex: number
	) {
		e.preventDefault();

		console.log(e);

		if (e.dataTransfer) {
			const item = JSON.parse(e.dataTransfer.getData('application/json'));

			if (['block', 'row'].includes(item.type)) return;

			const newItem = { type: item.type, id: crypto.randomUUID() };
			// page.update((p) => [...p, newBlock]);
			page[blockIndex].children[rowIndex].children.push(newItem);
		}
	}

	function onRemoveBlock(blockIndex: number) {
		page.splice(blockIndex, 1);
	}

	function onRemoveRow(blockIndex: number, rowIndex: number) {
		page[blockIndex].children.splice(rowIndex, 1);
	}

	function onRemoveItem(blockIndex: number, rowIndex: number, itemIndex: number) {
		page[blockIndex].children[rowIndex].children.splice(itemIndex, 1);
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
	<div class="col-span-3 border w-full p-2" ondragover={allowDrop}>
		<!-- svelte-ignore a11y_no_static_element_interactions -->
		{#each page as block, blockIdx (block.id)}
			<div
				class="w-full h-2.5"
				ondragover={allowDrop}
				data-index={blockIdx}
				ondrop={handleDropBlock}
			></div>
			<div class="block">
				<div class="flex flex-row space-x-2 items-center">
					<p>Block {block.id}</p>
					<Trash class="w-4 h-4 cursor-pointer" onclick={() => onRemoveBlock(blockIdx)} />
				</div>
				<div
					class="border p-2 rounded-sm"
					ondragover={allowDrop}
					ondrop={(e) => handleDropRow(e, blockIdx)}
				>
					{#each block.children as row, rowIndex (row.id)}
						<div>
							<div class="flex flex-row space-x-2 items-center">
								<p>Row</p>
								<Trash
									class="w-4 h-4 cursor-pointer"
									onclick={() => onRemoveRow(blockIdx, rowIndex)}
								/>
							</div>
							<div
								class="border p-2 rounded-sm flex flex-row space-x-2"
								ondragover={allowDrop}
								ondrop={(e) => handleDropItem(e, blockIdx, rowIndex)}
							>
								{#each row.children as item, itemIdx (item.id)}
									<div class="border p-2 rounded-sm w-full flex flex-row space-x-2 items-center">
										<p>{item.type}</p>
										<Trash
											class="w-4 h-4 cursor-pointer"
											onclick={() => onRemoveItem(blockIdx, rowIndex, itemIdx)}
										/>
									</div>
								{/each}
							</div>
						</div>
					{/each}
				</div>
			</div>
		{/each}
		<div
			class="w-full h-2.5"
			ondragover={allowDrop}
			data-index={page.length}
			ondrop={handleDropBlock}
		></div>
	</div>
</div>
