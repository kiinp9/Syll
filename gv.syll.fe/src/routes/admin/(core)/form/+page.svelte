	<script lang="ts">
		import { Input } from '$lib/components/ui/input';
		import { Label } from '$lib/components/ui/label';
		import { FormItemsTypes } from '$lib/constants/form.constants.js';
		import * as Select from '$lib/components/ui/select/index.js';
		import Button from '$lib/components/ui/button/button.svelte';
		import { page } from '$app/stores';
		import { search } from 'flowbite-svelte';
		import { enhance } from '$app/forms';
		import { toast } from '$lib/utils/toast.utils.js';
		import Toaster from '$lib/components/ui/toaster/toaster.svelte';



		let { data, form } = $props();

		let formLayoutData = $state(data.data);
		let hasExpanded = false;

		function expandBlocksWithMultipleRecords() {
			if (!formLayoutData?.truongCustoms || formLayoutData.truongCustoms.length === 0) {
				return;
			}

			const groupedData = new Map<number, Map<number, any[]>>();

			formLayoutData.truongCustoms.forEach((truong: any) => {
				const blockNum = truong.blockTruongNhanBan;
				const indexRow = truong.item?.indexRowTable ?? 1;

				if (!groupedData.has(blockNum)) {
					groupedData.set(blockNum, new Map());
				}

				const blockMap = groupedData.get(blockNum)!;
				if (!blockMap.has(indexRow)) {
					blockMap.set(indexRow, []);
				}

				blockMap.get(indexRow)!.push(truong);
			});

			formLayoutData?.items?.forEach((block: any) => {
				const blockNums = new Set<number>();
				
				block.items.forEach((row: any) => {
					row.items.forEach((item: any) => {
						item.items?.forEach((truong: any) => {
							if (truong.blockTruongNhanBan > 0) {
								blockNums.add(truong.blockTruongNhanBan);
							}
						});
					});
				});

				blockNums.forEach((blockNum) => {
					const dataByIndex = groupedData.get(blockNum);
					if (!dataByIndex) return;

					const maxIndex = Math.max(...Array.from(dataByIndex.keys()));
					if (maxIndex <= 1) return;

					const blockStartIdx = block.items.findIndex((r: any) =>
						r.items.some((item: any) =>
							item.items?.some((truong: any) => truong.blockTruongNhanBan === blockNum)
						)
					);

					if (blockStartIdx === -1) return;

					const templateRows: any[] = [];
					let currentIdx = blockStartIdx;

					while (currentIdx < block.items.length) {
						const row = block.items[currentIdx];
						const belongsToBlock = row.items.some((item: any) =>
							item.items?.some((truong: any) => truong.blockTruongNhanBan === blockNum)
						);

						if (!belongsToBlock && templateRows.length > 0) break;
						if (belongsToBlock) templateRows.push(row);
						currentIdx++;
					}

					if (templateRows.length === 0) return;

					const firstDataSet = dataByIndex.get(1)!;
					templateRows.forEach((row: any) => {
						row.items.forEach((item: any) => {
							item.items?.forEach((truong: any) => {
								if (truong.blockTruongNhanBan === blockNum) {
									const matchingData = firstDataSet.find((d: any) => d.id === truong.id);
									if (matchingData) {
										truong.item = {
											id: matchingData.item.id,
											data: matchingData.item.data,
											indexRowTable: 1
										};
									}
								}
							});
						});
					});

					const allNewRows: any[] = [];

					for (let idx = 2; idx <= maxIndex; idx++) {
						const dataSet = dataByIndex.get(idx) || [];

						const clonedRows = templateRows.map((row: any) => ({
							...row,
							id: Math.random() * -1000000,
							showNutCustom: row.showNutCustom,
							style: row.style,
							class: row.class,
							items: row.items.map((item: any) => ({
								...item,
								id: Math.random() * -1000000,
								items: item.items?.map((truong: any) => {
									if (truong.blockTruongNhanBan === blockNum) {
										const matchingData = dataSet.find((d: any) => d.id === truong.id);
										return {
											...truong,
											item: matchingData
												? {
														id: matchingData.item.id,
														data: matchingData.item.data,
														indexRowTable: idx
													}
												: {
														id: 0,
														data: '',
														indexRowTable: idx
													}
										};
									}
									return {
										...truong,
										item: truong.item
											? {
													id: truong.item.id,
													data: truong.item.data,
													indexRowTable: truong.item.indexRowTable
												}
											: undefined
									};
								}),
								headers: item.headers
							}))
						}));

						allNewRows.push(...clonedRows);
					}

					const lastTemplateIdx = blockStartIdx + templateRows.length - 1;
					block.items.splice(lastTemplateIdx + 1, 0, ...allNewRows);
				});
			});
		}

		$effect(() => {
			if (formLayoutData && !hasExpanded) {
				hasExpanded = true;
				expandBlocksWithMultipleRecords();
			}
		});

		function handleThemDong(block: any, rowIndex: number) {
			const currentRow = block.items[rowIndex];
			if (!currentRow.showNutCustom)
				return;

			const blockTruongNhanBan = currentRow.items[0]?.items?.[0]?.blockTruongNhanBan;
			if (!blockTruongNhanBan) 
				return;

			const rowsToClone: any[] = [];

			let startIdx = rowIndex;
			while (startIdx > 0) {
				const prevRow = block.items[startIdx - 1];

				if (prevRow.showNutCustom) break;

				const belongsToBlock = prevRow.items.some((item: any) =>
					item.items?.some((truong: any) => truong.blockTruongNhanBan === blockTruongNhanBan)
				);
				if (!belongsToBlock) break;

				startIdx--;
			}

			for (let i = startIdx; i <= rowIndex; i++) {
				rowsToClone.push(block.items[i]);
			}

			const clonedRows = rowsToClone.map((row: any) => ({
				...row,
				id: Math.random() * -1000000,
				showNutCustom: row.showNutCustom,
				items: row.items.map((item: any) => ({
					...item,
					id: Math.random() * -1000000,
				items: item.items?.map((truong: any) => ({
					...truong,
					item: {
						id: 0,
						data: '',
						indexRowTable: null
					}
				})),
				headers: item.headers
				}))
			}));

			block.items.splice(rowIndex + 1, 0, ...clonedRows);
			formLayoutData = formLayoutData;
		}
		async function handleDeleteRow(item: any, rowIndex: number) {
			const startIdx = rowIndex * item.headers!.length;
			const rowCells = item.items!.slice(startIdx, startIdx + item.headers!.length);

			const formData = new FormData();
			rowCells.forEach((cell: any) => {
				if (cell.item?.id && cell.item.id > 0) {
					formData.append(`${cell.id}_${cell.item.id}`, '');
				}
			});

			const response = await fetch('?/deleteRow', {
				method: 'POST',
				body: formData
			});

			if (response.ok) {
				item.items!.splice(startIdx, item.headers!.length);
				item.items = item.items;
			}
		}
		async function handleDeleteBlock(block: any, rowIndex: number) {
			const currentRow = block.items[rowIndex];
			if (!currentRow.showNutCustom) 
				return;

			const blockTruongNhanBan = currentRow.items[0]?.items?.[0]?.blockTruongNhanBan;
			if (!blockTruongNhanBan) 
				return;

			const rowsToDelete: any[] = [];

			let startIdx = rowIndex;
			while (startIdx > 0) {
				const prevRow = block.items[startIdx - 1];
				if (prevRow.showNutCustom) 
				break;

				const belongsToBlock = prevRow.items.some((item: any) =>
					item.items?.some((truong: any) => truong.blockTruongNhanBan === blockTruongNhanBan)
				);
				if (!belongsToBlock) 
				break;

				startIdx--;
			}

			for (let i = startIdx; i <= rowIndex; i++) {
				rowsToDelete.push(block.items[i]);
			}

			const formData = new FormData();
			let hasData = false;

			rowsToDelete.forEach((row: any) => {
				row.items.forEach((item: any) => {
					item.items?.forEach((truong: any) => {
						if (truong.item?.id && truong.item.id > 0) {
							formData.append(`${truong.id}_${truong.item.id}`, '');
							hasData = true;
						} 
					});
				});
			});

			if (hasData) {
			const response = await fetch('?/deleteBlock', {
					method: 'POST',  
					body: formData
				});

			if (!response.ok) {
				console.error('Failed to delete block');
				return;
				}
			}

			block.items.splice(startIdx, rowsToDelete.length);
			formLayoutData = formLayoutData;
		}

		async function handleDownloadForm() {
			try {
				const idFormLoai = $page.url.searchParams.get('idFormLoai') || '0';
                const idDanhBa = $page.url.searchParams.get('idDanhBa') || '0';
        
				const response = await fetch(`/admin/form/download?idFormLoai=${idFormLoai}&idDanhBa=${idDanhBa}`, {
				method: 'POST'
				});
				if (!response.ok) {
					toast.error('Có lỗi xảy ra khi tải file');
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
				toast.error('Có lỗi xảy ra khi tải file');
		}
		}
	</script>
	<Toaster />

	<div>
		
		<form 
			method="POST" 
			action="?/update"
			use:enhance = {({formData}) => {
				formData.append('idFormLoai', $page.url.searchParams.get('idFormLoai') || '0');
				formData.append('idDanhBa', $page.url.searchParams.get('idDanhBa') || '0');

				return async ({result,update}) =>{
					if(result.type === 'success'){
						const data = result.data as any;
					if (data?.status === 1) {
						toast.success("Đã cập nhật thông tin Form thành công!");
					} else {
						toast.error(data?.message || 'Có lỗi xảy ra');
					}
						await update({reset: false});
					}
				}
			}}
			class="mt-5">
			<div class="flex flex-col">
				{#each formLayoutData?.items as block, blockIndex (`block_${block.id}`)}
					<div class="mb-5 pb-5 border-b {block.class}" style={block.style}>
						{#each block.items as row, rowIndex (`row_${row.id}_${rowIndex}`)}
							<div class="mb-2 flex flex-row space-x-2 {row.class}" style={row.style}>
								{#each row.items as item, itemIndex (`item_${item.id}`)}
									<div class="flex flex-col w-full {row.class}" style={row.style}>
										{#if item.items && item.items[0]}
											{#if item.type === FormItemsTypes.ItemText}
												{item.items[0].tenTruong}
											{:else if item.type === FormItemsTypes.InputText}
												<div class="">
													<Label for={item.items[0].id?.toString()} class="mb-2">
														{item.items[0].tenTruong}
													</Label>
													<Input
														id={item.items[0].id?.toString()}
														name={`${item.items[0].id}_${item.items[0].item.id}`}
														type="text"
														value={item.items[0].item.data}
													/>
												</div>
											{:else if item.type === FormItemsTypes.DropDownDate}
												<div class="">
													<Label for={item.items[0].id?.toString()} class="mb-2">
														{item.items[0].tenTruong}
													</Label>
													<Input
														id={item.items[0].id?.toString()}
														name={`${item.items[0].id}_${item.items[0].item.id}`}
														type="date"
														value={item.items[0].item.data}
													/>
												</div>
											{:else if item.type === FormItemsTypes.DropDownText}
												<div class="">
													<Label for={item.items[0].id?.toString()} class="mb-2">
														{item.items[0].tenTruong}
													</Label>
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
														style="grid-template-columns: repeat({item.headers?.length ||
															0}, 1fr) auto;"
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
															    {@const rowIndex = Math.floor(idx / (item.headers?.length || 1))}
					                                            {@const cellIndex = idx % (item.headers?.length || 1)}
																<div class="border border-gray-300 px-2 py-2">
																	<Input
																		type="text"
																		bind:value={cell.item.data}
																		name={`${item.id}_table_${rowIndex}_${cellIndex}_${cell.item?.id ?? 0}`}
																		class="w-full"
																	/>
																</div>
																{#if item.headers && (idx + 1) % item.headers.length === 0}
																	<div
																		class="border border-gray-300 px-4 py-2 flex justify-center items-center"
																	>
																		<Button
																			type="button"
																			class="bg-red-600 hover:bg-red-700 text-white px-3 py-1 text-sm"
																			onclick={() => {
																				const rowIndex = Math.floor(idx / item.headers!.length);
																				handleDeleteRow(item, rowIndex);
																			}}
																		>
																			Xóa
																		</Button>
																	</div>
																{/if}
															{/each}
														{/if}
													</div>
													<div class="mt-2">
														<Button
															type="button"
															class="bg-blue-600 hover:bg-blue-700 text-white"
															onclick={() => {
																if (item.headers && item.items && item.items.length > 0) {
																	const firstRowIds = item.items
																		.slice(0, item.headers.length)
																		.map((cell) => cell.id);

																	const newRow = firstRowIds.map((idTruongData) => ({
																		id: idTruongData,
																		item: {
																			id: 0,
																			data: ''
																		}
																	}));
																	item.items = [...(item.items || []), ...newRow];
																}
															}}
														>
															+ Thêm dòng
														</Button>
													</div>
												</div>
											{:else}
												{item.items[0].tenTruong}
											{/if}
										{/if}
									</div>
								{/each}
							</div>

							{#if row.showNutCustom}
								<div class="flex flex-row space-x-2 mt-2 mb-3">
									<Button
										type="button"
										class="bg-blue-600 hover:bg-blue-700 text-white"
										onclick={() => handleThemDong(block, rowIndex)}
									>
										+ Thêm dòng
									</Button>
									<Button type="button" class="bg-red-600 hover:bg-red-700 text-white"
									onclick={() => handleDeleteBlock(block, rowIndex)}>
										Xóa
									</Button>
								</div>
							{/if}
						{/each}
					</div>
				{/each}
			</div>
			<div class="flex flex-row justify-end space-x-2">
				<Button type="button" class="bg-green-600 hover:bg-green-700" onclick={handleDownloadForm}>Xuất</Button>
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