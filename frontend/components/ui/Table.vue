<template>
  <div class="overflow-hidden shadow ring-1 ring-black ring-opacity-5 rounded-lg">
    <table class="min-w-full divide-y divide-gray-300">
      <thead class="bg-gray-50">
        <tr>
          <th
            v-for="column in columns"
            :key="column.key"
            scope="col"
            class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900"
            :class="column.headerClass"
          >
            {{ column.label }}
          </th>
          <th v-if="$slots.actions" scope="col" class="relative py-3.5 pl-3 pr-4 sm:pr-6">
            <span class="sr-only">Actions</span>
          </th>
        </tr>
      </thead>
      <tbody class="divide-y divide-gray-200 bg-white">
        <tr v-if="loading">
          <td :colspan="columns.length + ($slots.actions ? 1 : 0)" class="px-3 py-4 text-center">
            <LoadingSpinner />
          </td>
        </tr>
        <tr v-else-if="!data || data.length === 0">
          <td :colspan="columns.length + ($slots.actions ? 1 : 0)" class="px-3 py-4 text-center text-sm text-gray-500">
            <slot name="empty">
              {{ $t('messages.noData') }}
            </slot>
          </td>
        </tr>
        <tr v-else v-for="(row, index) in data" :key="index" class="hover:bg-gray-50">
          <td
            v-for="column in columns"
            :key="column.key"
            class="whitespace-nowrap px-3 py-4 text-sm text-gray-900"
            :class="column.cellClass"
          >
            <slot :name="`cell-${column.key}`" :row="row" :value="row[column.key]">
              {{ row[column.key] }}
            </slot>
          </td>
          <td v-if="$slots.actions" class="relative whitespace-nowrap py-4 pl-3 pr-4 text-right text-sm font-medium sm:pr-6">
            <slot name="actions" :row="row"></slot>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
interface Column {
  key: string
  label: string
  headerClass?: string
  cellClass?: string
}

interface Props {
  columns: Column[]
  data: any[]
  loading?: boolean
}

defineProps<Props>()
</script>
