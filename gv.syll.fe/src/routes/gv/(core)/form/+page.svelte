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
			{#each formLayoutData?.items as block (`block_${block.id}`)}
				<div class="mb-5 pb-5 border-b {block.class}" style={block.style}>
					{#each block.items as row (`row_${row.id}`)}
						<div class="mb-2 flex flex-row space-x-2 {row.class}" style={row.style}>
							{#each row.items as item, itemIndex (`item_${item.id}`)}
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
													name={`${item.items[0].id}_${item.items[0].item.id}`}
													type="text"
													value={item.items[0].item.data}
												/>
											</div>
										{:else if item.type === FormItemsTypes.DropDownDate}
											<div class="">
												<Label for={item.items[0].id?.toString()} class="mb-2"
													>{item.items[0].tenTruong}</Label
												>
												<Input
													id={item.items[0].id?.toString()}
													name={`${item.items[0].id}_${item.items[0].item.id}`}
													type="date"
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
													name={`${item.items[0].id}_${item.items[0].item.id}`}
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
										{:else if item.type === FormItemsTypes.Table}
											<div>
												<div
													class="grid border-2"
													style="grid-template-columns: repeat({item.headers?.length || 0}, 1fr) auto;"
												>
													{#if item.headers}
														{#each item.headers as col, idx (`tbl_header_${item.id}_${col.id}_${idx}`)}
														<div
															class="border border-gray-300 px-4 py-2 text-center font-bold bg-slate-200"
														>
															{col.data}
														</div>
													{/each}
													<div
														class="border border-gray-300 px-4 py-2 text-center font-bold bg-slate-200"
													>
														Thao tác
													</div>
													{/if}
													{#if item.items}
														{#each item.items as cell, idx (`tbl_body_${item.id}_${cell.id}_${idx}`)}
															<div class="border border-gray-300 px-2 py-2">
																<Input
																	type="text"
																	bind:value={cell.item.data}
																	name={`${cell.id}_${cell.item?.id ?? 0}`}
																	class="w-full"
																/>
															</div>
															{#if item.headers && (idx + 1) % item.headers.length === 0}
																<div class="border border-gray-300 px-4 py-2 flex justify-center items-center">
																	<Button type="button" class="bg-red-600 hover:bg-red-700 text-white px-3 py-1 text-sm" onclick={() => {
																		const rowIndex = Math.floor(idx / item.headers!.length);
																		const startIdx = rowIndex * item.headers!.length;
																		item.items!.splice(startIdx, item.headers!.length);
																		item.items = item.items;
																	}}>Xóa</Button>
																</div>
															{/if}
														{/each}
													{/if}
												</div>
												<div class="mt-2">
													<Button type="button" class="bg-blue-600 hover:bg-blue-700 text-white" onclick={() => {
														if (item.headers && item.items && item.items.length > 0 ) {
															const firstRowIds = item.items
															    .slice(0, item.headers.length)
																.map(cell => cell.id);

															const newRow = firstRowIds.map((idTruongData) => ({
																id: idTruongData,
																item: { 
																	id: 0,
																	data: '' 
																}
															}));
															item.items = [...(item.items || []), ...newRow];
														}
													}}>+ Thêm dòng</Button>
												</div>
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
		<div class="flex flex-row justify-end space-x-2">
			<Button type="button" class="bg-green-600 hover:bg-green-700">Xuất</Button>
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