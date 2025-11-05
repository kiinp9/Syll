<script lang="ts">
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import { FormItemsTypes } from '$lib/constants/form.constants.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import Button from '$lib/components/ui/button/button.svelte';

	let { data, form } = $props();

	let formLayoutData = $state(data.data);
	// export let form;
</script>

<div>
	{#if form?.message}
		{#if form.status === 1}
			<div class="p-5 rounded-sm bg-green-200">
				<p class="text-green-500 font-bold">Đã cập nhật form</p>
			</div>
		{:else}
			<div class="p-5 rounded-sm bg-red-200">
				<p class="text-red-500 font-bold">{form?.message}</p>
			</div>
		{/if}
	{/if}
	<form method="POST" class="mt-5">
		<div class="flex flex-col">
			{#each formLayoutData?.items as block (block.id)}
				<div class="mb-5 pb-5 border-b {block.class}" style={block.style}>
					{#each block.items as row (row.id)}
						<div class="mb-2 flex flex-row space-x-2 {row.class}" style={row.style}>
							{#each row.items as item, itemIndex (item.id)}
								<div class="flex flex-col w-full {row.class}" style={row.style}>
									{#if item.items && item.items[0]}
										{#if item.type === FormItemsTypes.ItemText}
											{item.items[0].tenTruong}
										{:else if item.type === FormItemsTypes.InputText}
											<div class="">
												<Label for={item.items[0].id?.toString()} class="mb-2"
													>{item.items[0].tenTruong}</Label
												>
												<Input
													id={item.items[0].id?.toString()}
													name={item.items[0].id?.toString()}
													type="text"
													value={item.items[0].item.data}
												/>
											</div>
										{:else if item.type === FormItemsTypes.DropDownText}
											<div class="">
												<Label for={item.items[0].id?.toString()} class="mb-2"
													>{item.items[0].tenTruong}</Label
												>
												<Select.Root
													type="single"
													bind:value={item.items[0].item.data}
													onValueChange={(val) => {
														row.items[itemIndex].items![0].item.data = val;
													}}
													name={item.items[0].id?.toString()}
												>
													<Select.Trigger class="w-full">
														{item.items[0].item.data || 'Chọn giá trị'}
													</Select.Trigger>
													<Select.Content>
														{#each item.items[0].items as option (option.id)}
															<Select.Item value={option.data} label={option.data} />
														{/each}
													</Select.Content>
												</Select.Root>
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
		<div class="flex flex-row justify-end">
			<Button type="submit">Lưu</Button>
		</div>
	</form>
</div>

<style>
	.f-form-title {
		text-align: center;
		color: #002a5c;
		font-size: 20px;
		font-weight: bold;
	}
	.f-block-title {
		color: #002a5c;
		font-weight: bold;
	}
</style>
