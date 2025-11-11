<template>
  <nav class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6 rounded-lg">
    <div class="hidden sm:block">
      <p class="text-sm text-gray-700">
        {{ $t('pagination.showing') }}
        <span class="font-medium">{{ from }}</span>
        {{ $t('pagination.to') }}
        <span class="font-medium">{{ to }}</span>
        {{ $t('pagination.of') }}
        <span class="font-medium">{{ total }}</span>
        {{ $t('pagination.results') }}
      </p>
    </div>
    <div class="flex flex-1 justify-between sm:justify-end gap-2">
      <button
        type="button"
        :disabled="currentPage === 1"
        class="relative inline-flex items-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        @click="goToPage(currentPage - 1)"
      >
        {{ $t('pagination.previousPage') }}
      </button>
      <button
        type="button"
        :disabled="currentPage === totalPages"
        class="relative inline-flex items-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        @click="goToPage(currentPage + 1)"
      >
        {{ $t('pagination.nextPage') }}
      </button>
    </div>
  </nav>
</template>

<script setup lang="ts">
interface Props {
  currentPage: number
  totalPages: number
  total: number
  perPage: number
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'update:currentPage', value: number): void
  (e: 'pageChange', value: number): void
}>()

const from = computed(() => {
  return (props.currentPage - 1) * props.perPage + 1
})

const to = computed(() => {
  return Math.min(props.currentPage * props.perPage, props.total)
})

const goToPage = (page: number) => {
  if (page >= 1 && page <= props.totalPages) {
    emit('update:currentPage', page)
    emit('pageChange', page)
  }
}
</script>
