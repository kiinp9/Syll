<script lang="ts">
	import { Toaster as SonnerToaster } from 'svelte-sonner';
</script>

<SonnerToaster 
	position="top-right"
	richColors={false}
	expand={false}
	duration={3000}
	toastOptions={{
		style: 'border-radius: 8px; padding: 16px; font-size: 14px;',
	}}
/>
