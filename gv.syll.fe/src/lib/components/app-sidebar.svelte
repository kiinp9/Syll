<script lang="ts" module>
	import NewspaperIcon from "@lucide/svelte/icons/newspaper";
	import BellIcon from "@lucide/svelte/icons/bell";
	import ChartColumnIcon from "@lucide/svelte/icons/chart-column";
	import HouseIcon from "@lucide/svelte/icons/house";

	const data = {
		user: {
			name: "Nguyễn Trọng Nghĩa",
			email: "nghiant@huce.edu.vn",
			avatar: "/avatars/shadcn.jpg",
		},
		navMain: [
			{
				url: "/gv/dashboard",
				title: 'Trang chủ',
				icon: HouseIcon,
				// icon: HouseIcon,
			},
			{
				url: "/gv/form",
				title: 'Quản lý form',
				icon: NewspaperIcon,
			},
			{
				url: "#",
				title: 'Thông báo',
				icon: BellIcon,
			},
			{
				url: "#",
				title: 'Thống kê',
				icon: ChartColumnIcon,
			},
			
		],
		navSecondary: [
		],
		projects: [
			
		],
	};
</script>

<script lang="ts">
	import NavMain from "./nav-main.svelte";
	import NavProjects from "./nav-projects.svelte";
	import NavSecondary from "./nav-secondary.svelte";
	import NavUser from "./nav-user.svelte";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import CommandIcon from "@lucide/svelte/icons/command";
	import type { ComponentProps } from "svelte";

	let { ref = $bindable(null), ...restProps }: ComponentProps<typeof Sidebar.Root> = $props();
</script>

<Sidebar.Root bind:ref variant="inset" {...restProps}>
	<Sidebar.Header>
		<Sidebar.Menu>
			<Sidebar.MenuItem>
				<Sidebar.MenuButton size="lg">
					{#snippet child({ props })}
						<a href="##" {...props}>
							<div
								class="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg"
							>
								<CommandIcon class="size-4" />
							</div>
							<div class="grid flex-1 text-left text-sm leading-tight">
								<span class="truncate font-medium">HR System</span>
								<span class="truncate text-xs">HUCE</span>
							</div>
						</a>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain items={data.navMain} />
		<NavProjects projects={data.projects} />
		<NavSecondary items={data.navSecondary} class="mt-auto" />
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser user={data.user} />
	</Sidebar.Footer>
</Sidebar.Root>
