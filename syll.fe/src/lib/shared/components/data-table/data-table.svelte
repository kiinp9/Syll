<script lang="ts" generics="TData extends Record<string, any>">
	import {
		type ColumnDef,
		type SortingState,
		getCoreRowModel,
		getSortedRowModel
	} from '@tanstack/table-core';
	import {
		createSvelteTable,
		FlexRender,
		renderComponent
	} from '$lib/components/ui/data-table/index.js';
	import type { IColumn } from '$lib/shared/models/data-table.models';
	import { CellViewTypes } from '$lib/shared/constants/data-table.constants';
	import { Utils } from '$lib/shared/utils';

	type Props = {
		data: TData[];
		columns: IColumn[];
		class?: string;
		onCellClick?: (col: IColumn, rowData: TData) => void;
	};

	let { data, columns, class: className = '', onCellClick }: Props = $props();

	function buildColumnDefs(): ColumnDef<TData>[] {
		return columns.map((col, index) => {
			const columnDef: ColumnDef<TData> = {
				id: col.field || `col-${index}`,
				accessorKey: col.field,
				header: col.header,
				enableSorting: !!col.field && col.cellViewType !== CellViewTypes.CUSTOM_COMP && col.cellViewType !== CellViewTypes.INDEX,
				meta: { column: col }
			};

			columnDef.cell = ({ row, getValue }) => {
				const rowIndex = row.index;
				const value = col.field ? getValue() : null;

				if (col.cellViewType === CellViewTypes.INDEX) {
					return rowIndex + 1;
				}

				if (col.cellViewType === CellViewTypes.DATE) {
					return Utils.formatDateView(value as string | Date, col.dateFormat || 'DD/MM/YYYY');
				}

				if (col.cellViewType === CellViewTypes.CURRENCY) {
					return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value as number);
				}

				if (col.cellViewType === CellViewTypes.CUSTOM_COMP && col.customComponent) {
					return renderComponent(col.customComponent, { rowData: row.original, rowIndex });
				}

				return value ?? '';
			};

			return columnDef;
		});
	}

	let sorting = $state<SortingState>([]);

	const columnDefs = $derived(buildColumnDefs());

	const table = createSvelteTable({
		get data() {
			return data;
		},
		get columns() {
			return columnDefs;
		},
		state: {
			get sorting() {
				return sorting;
			}
		},
		onSortingChange: (updater) => {
			if (typeof updater === 'function') {
				sorting = updater(sorting);
			} else {
				sorting = updater;
			}
		},
		getCoreRowModel: getCoreRowModel(),
		getSortedRowModel: getSortedRowModel()
	});

	function getSortIcon(isSorted: false | 'asc' | 'desc') {
		if (isSorted === 'asc') return '↑';
		if (isSorted === 'desc') return '↓';
		return '';
	}
</script>

<div class="data-table-wrapper {className}">
	<table class="data-table">
		<thead>
			{#each table.getHeaderGroups() as headerGroup (headerGroup.id)}
				<tr>
					{#each headerGroup.headers as header, headerIndex (header.id)}
						{@const col = columns[headerIndex]}
						{@const isLastColumn = headerIndex === headerGroup.headers.length - 1}
						<th
							class="data-table-th {col.headerContainerClass || ''}"
							style={col.headerContainerStyle || ''}
						>
							<div class="th-content">
								{#if header.column.getCanSort()}
									<button
										type="button"
										class="sort-button"
										onclick={header.column.getToggleSortingHandler()}
									>
										<FlexRender
											content={header.column.columnDef.header}
											context={header.getContext()}
										/>
										<span class="sort-icon">
											{getSortIcon(header.column.getIsSorted())}
										</span>
									</button>
								{:else}
									<FlexRender
										content={header.column.columnDef.header}
										context={header.getContext()}
									/>
								{/if}
							</div>
							{#if !isLastColumn}
								<div class="resize-handle">
									<div class="resize-indicator"></div>
								</div>
							{/if}
						</th>
					{/each}
				</tr>
			{/each}
		</thead>
		<tbody>
			{#each table.getRowModel().rows as row (row.id)}
				<tr class="data-table-row">
					{#each row.getVisibleCells() as cell, cellIndex (cell.id)}
						{@const col = columns[cellIndex]}
						<td
							class="data-table-td {col.cellClass || ''}"
							style={col.cellStyle || ''}
						>
							{#if col.clickable && onCellClick}
								<button
									type="button"
									class="clickable-cell"
									onclick={() => onCellClick(col, row.original)}
								>
									<FlexRender
										content={cell.column.columnDef.cell}
										context={cell.getContext()}
									/>
								</button>
							{:else}
								<FlexRender
									content={cell.column.columnDef.cell}
									context={cell.getContext()}
								/>
							{/if}
						</td>
					{/each}
				</tr>
			{:else}
				<tr>
					<td colspan={columns.length} class="data-table-td data-table-empty">
						Không có dữ liệu
					</td>
				</tr>
			{/each}
		</tbody>
	</table>
</div>

<style>
	.data-table-wrapper {
		width: 100%;
		overflow-x: auto;
	}
	.data-table {
		width: 100%;
		border-collapse: collapse;
		font-size: 0.875rem;
	}
	.data-table-th {
		position: relative;
		padding: 0.75rem 1rem;
		text-align: center;
		vertical-align: middle;
		font-weight: 500;
		color: hsl(var(--muted-foreground));
		border-bottom: 1px solid hsl(var(--border));
	}
	.th-content {
		display: flex;
		justify-content: center;
		align-items: center;
	}
	.data-table-td {
		padding: 0.75rem 1rem;
		vertical-align: middle;
		border-bottom: 1px solid hsl(var(--border));
		word-wrap: break-word;
		overflow-wrap: break-word;
		white-space: normal;
	}
	.data-table-row:hover {
		background: hsl(var(--muted) / 0.5);
	}
	.data-table-empty {
		height: 6rem;
		text-align: center;
	}
	.sort-button {
		display: inline-flex;
		align-items: center;
		gap: 0.25rem;
		background: none;
		border: none;
		cursor: pointer;
		font: inherit;
		color: inherit;
		padding: 0;
	}
	.sort-button:hover {
		opacity: 0.8;
	}
	.sort-icon {
		font-size: 0.75rem;
		opacity: 0.6;
	}
	.resize-handle {
		position: absolute;
		right: 0;
		top: 50%;
		transform: translateY(-50%);
		width: 12px;
		height: 100%;
		display: flex;
		align-items: center;
		justify-content: center;
	}
	.resize-indicator {
		width: 2px;
		height: 16px;
		background-color: #d1d5db;
		border-radius: 1px;
	}
	.clickable-cell {
		background: none;
		border: none;
		padding: 0;
		font: inherit;
		color: inherit;
		text-align: left;
		cursor: pointer;
	}
	.clickable-cell:hover {
		color: #2563eb;
		text-decoration: underline;
	}
</style>