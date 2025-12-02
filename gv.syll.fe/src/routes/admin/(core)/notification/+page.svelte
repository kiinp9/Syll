<script lang="ts">
	import { Construction } from 'lucide-svelte';
</script>

<div class="flex items-center justify-center min-h-[calc(100vh-200px)] p-6">
	<div class="max-w-md w-full">
		<div class="bg-white rounded-2xl shadow-lg border border-gray-100 p-8 text-center">
			<!-- Icon -->
			<div class="inline-flex items-center justify-center w-20 h-20 rounded-full bg-amber-50 mb-6">
				<Construction class="w-10 h-10 text-amber-500" />
			</div>

			<!-- Heading -->
			<h2 class="text-2xl font-bold text-gray-900 mb-3">
				Tính năng đang phát triển
			</h2>

			<!-- Description -->
			<p class="text-gray-600 leading-relaxed mb-6">
				Chúng tôi đang nỗ lực hoàn thiện tính năng này để mang đến trải nghiệm tốt nhất cho bạn.
			</p>

			<!-- Divider -->
			<div class="relative my-6">
				<div class="absolute inset-0 flex items-center">
					<div class="w-full border-t border-gray-200"></div>
				</div>
				<div class="relative flex justify-center text-xs">
					<span class="px-3 bg-white text-gray-500">Cần hỗ trợ?</span>
				</div>
			</div>

			<!-- Contact Info -->
			<p class="text-sm text-gray-700 mb-4">
				Vui lòng liên hệ với <span class="font-semibold text-gray-900">Admin</span> để biết thêm chi tiết.
			</p>

			<!-- Action Button -->
			<button class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-3 px-6 rounded-lg transition-colors duration-200 shadow-sm hover:shadow-md">
				Liên hệ Admin
			</button>
		</div>

		<!-- Additional Info -->
		<p class="text-center text-xs text-gray-500 mt-6">
			Cảm ơn bạn đã kiên nhẫn chờ đợi
		</p>
	</div>
</div>