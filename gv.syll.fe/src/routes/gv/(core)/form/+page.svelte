<script lang="ts">
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import { FormItemsTypes } from '$lib/constants/form.constants.js';

	let { data } = $props();
</script>

<div class="flex flex-col">
	<h3>Form</h3>

	{#each data.data?.items as block (block.id)}
		<div class="mb-5">
			{#each block.items as row (row.id)}
				<div class="mb-2 flex flex-row space-x-2">
					{#each row.items as item (item.id)}
						<div class="flex flex-col">
							{#if item.items}
								{#if item.type === FormItemsTypes.ItemText}
									{item.items[0].tenTruong}
								{:else if item.type === FormItemsTypes.InputText}
									<div class="">
										<Label for="username">{item.items[0].tenTruong}</Label>
										<Input
											id="username"
											name="username"
											type="text"
											required
											value={item.items[0].item.data}
										/>
									</div>
								{:else}
									{item.items[0].tenTruong}
								{/if}
							{/if}
						</div>
					{/each}
				</div>
			{/each}
		</div>
	{/each}
</div>
