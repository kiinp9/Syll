<script lang="ts">
	import NewspaperIcon from '@lucide/svelte/icons/newspaper';
	import BellIcon from '@lucide/svelte/icons/bell';
	import ChartColumnIcon from '@lucide/svelte/icons/chart-column';
	import HouseIcon from '@lucide/svelte/icons/house';
	import AppSidebar from '$lib/components/app-sidebar.svelte';
	import { Separator } from '$lib/components/ui/separator';
	import * as Sidebar from '$lib/components/ui/sidebar';

	let { children, data } = $props();
</script>

<Sidebar.Provider>
	<AppSidebar
		navMain={[
			{
				url: '/gv/dashboard',
				title: 'Trang chủ',
				icon: HouseIcon
				// icon: HouseIcon,
			},
			{
				url: '/gv/form',
				title: 'Quản lý form',
				icon: NewspaperIcon
			},
			{
				url: '#',
				title: 'Thông báo',
				icon: BellIcon
			},
			{
				url: '#',
				title: 'Thống kê',
				icon: ChartColumnIcon
			}
		]}
		user={data.user ?? {}}
	/>
	<Sidebar.Inset>
		<header class="flex h-16 shrink-0 items-center gap-2">
			<div class="flex items-center gap-2 px-4">
				<Sidebar.Trigger class="-ml-1" />
				<Separator orientation="vertical" class="mr-2 data-[orientation=vertical]:h-4" />
			</div>
		</header>
		<div class="flex flex-1 flex-col gap-4 p-4 pt-0">
			{@render children?.()}
		</div>
	</Sidebar.Inset>
</Sidebar.Provider>
