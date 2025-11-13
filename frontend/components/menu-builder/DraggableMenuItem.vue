<template>
  <div
    :draggable="true"
    @dragstart="$emit('drag-start', item)"
    @dragover="handleDragOver"
    @dragleave="$emit('drag-leave')"
    @drop="$emit('drop')"
    @dragend="$emit('drag-end')"
    :class="[
      'menu-item flex items-center gap-3 p-3 rounded-lg border transition-all cursor-move',
      isDraggedOver ? 'border-primary-400 bg-primary-50' : 'border-neutral-200 bg-neutral-50 hover:border-neutral-300'
    ]"
  >
    <!-- Drag Handle -->
    <div class="flex-shrink-0 cursor-grab active:cursor-grabbing">
      <svg class="w-4 h-4 text-neutral-400" fill="currentColor" viewBox="0 0 20 20">
        <path d="M7 2a2 2 0 100 4 2 2 0 000-4zM7 9a2 2 0 100 4 2 2 0 000-4zM7 16a2 2 0 100 4 2 2 0 000-4zM13 2a2 2 0 100 4 2 2 0 000-4zM13 9a2 2 0 100 4 2 2 0 000-4zM13 16a2 2 0 100 4 2 2 0 000-4z" />
      </svg>
    </div>

    <!-- Item Image -->
    <div v-if="item.imageUrl" class="flex-shrink-0">
      <img :src="item.imageUrl" :alt="item.name" class="w-12 h-12 rounded object-cover" />
    </div>

    <!-- Item Info -->
    <div class="flex-1 min-w-0">
      <input
        v-model="localItem.name"
        @blur="updateItem"
        class="font-medium text-neutral-900 bg-transparent border-none focus:outline-none focus:ring-1 focus:ring-primary-500 rounded px-1 -ml-1 w-full"
        placeholder="Item name"
      />
      <input
        v-model="localItem.description"
        @blur="updateItem"
        class="text-sm text-neutral-600 bg-transparent border-none focus:outline-none focus:ring-1 focus:ring-primary-500 rounded px-1 -ml-1 w-full mt-1"
        placeholder="Description (optional)"
      />
    </div>

    <!-- Price -->
    <div class="flex-shrink-0">
      <input
        v-model.number="localItem.price"
        @blur="updateItem"
        type="number"
        step="0.01"
        class="w-20 px-2 py-1 text-right font-semibold text-primary-600 bg-transparent border border-neutral-300 rounded focus:outline-none focus:ring-2 focus:ring-primary-500"
        placeholder="0.00"
      />
    </div>

    <!-- Actions -->
    <div class="flex items-center gap-1 flex-shrink-0">
      <button
        @click="toggleAvailability"
        :class="[
          'p-1.5 rounded transition-all',
          item.isAvailable
            ? 'text-green-600 hover:bg-green-50'
            : 'text-neutral-400 hover:bg-neutral-100'
        ]"
        :title="item.isAvailable ? 'Available' : 'Unavailable'"
      >
        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
        </svg>
      </button>

      <button
        @click="$emit('delete')"
        class="p-1.5 text-neutral-400 hover:text-red-600 hover:bg-red-50 rounded transition-all"
        title="Delete Item"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import type { MenuItem } from '~/stores/restaurant'

const props = defineProps<{
  item: MenuItem
  index: number
  isDraggedOver: boolean
}>()

const emit = defineEmits<{
  (e: 'drag-start', item: MenuItem): void
  (e: 'drag-over', event: DragEvent): void
  (e: 'drag-leave'): void
  (e: 'drop'): void
  (e: 'drag-end'): void
  (e: 'update', item: MenuItem): void
  (e: 'delete'): void
}>()

const localItem = reactive({ ...props.item })

const handleDragOver = (event: DragEvent) => {
  event.preventDefault()
  event.stopPropagation()
  emit('drag-over', event)
}

const updateItem = () => {
  emit('update', { ...props.item, ...localItem })
}

const toggleAvailability = () => {
  localItem.isAvailable = !localItem.isAvailable
  updateItem()
}
</script>
