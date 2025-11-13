<template>
  <div
    :draggable="true"
    @dragstart="$emit('drag-start', category)"
    @dragover="handleDragOver"
    @dragleave="$emit('drag-leave')"
    @drop="$emit('drop')"
    @dragend="$emit('drag-end')"
    :class="[
      'category-card p-6 rounded-xl border-2 transition-all cursor-move',
      isDraggedOver ? 'border-primary-500 bg-primary-50' : 'border-neutral-200 bg-white hover:border-primary-300'
    ]"
  >
    <!-- Drag Handle & Category Header -->
    <div class="flex items-start gap-4">
      <!-- Drag Handle -->
      <div class="flex-shrink-0 mt-1 cursor-grab active:cursor-grabbing">
        <svg class="w-6 h-6 text-neutral-400" fill="currentColor" viewBox="0 0 20 20">
          <path d="M7 2a2 2 0 100 4 2 2 0 000-4zM7 9a2 2 0 100 4 2 2 0 000-4zM7 16a2 2 0 100 4 2 2 0 000-4zM13 2a2 2 0 100 4 2 2 0 000-4zM13 9a2 2 0 100 4 2 2 0 000-4zM13 16a2 2 0 100 4 2 2 0 000-4z" />
        </svg>
      </div>

      <!-- Category Content -->
      <div class="flex-1 min-w-0">
        <div class="flex items-center justify-between mb-3">
          <div class="flex items-center gap-3">
            <input
              v-model="localCategory.name"
              @blur="updateCategory"
              class="text-xl font-bold text-neutral-900 bg-transparent border-none focus:outline-none focus:ring-2 focus:ring-primary-500 rounded px-2 -ml-2"
              placeholder="Category Name"
            />
            <span class="px-2 py-1 text-xs font-medium text-neutral-600 bg-neutral-100 rounded">
              {{ category.items.length }} items
            </span>
          </div>

          <!-- Actions -->
          <div class="flex items-center gap-2">
            <button
              @click="$emit('customize')"
              class="p-2 text-neutral-600 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition-all"
              title="Customize Style"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01" />
              </svg>
            </button>

            <button
              @click="$emit('delete')"
              class="p-2 text-neutral-600 hover:text-red-600 hover:bg-red-50 rounded-lg transition-all"
              title="Delete Category"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </div>
        </div>

        <!-- Items List -->
        <div class="space-y-2 mt-4">
          <DraggableMenuItem
            v-for="(item, index) in category.items"
            :key="item.id"
            :item="item"
            :index="index"
            :is-dragged-over="draggedOverItemIndex === index"
            @drag-start="handleItemDragStart"
            @drag-over="handleItemDragOver(index, $event)"
            @drag-leave="draggedOverItemIndex = null"
            @drop="handleItemDrop(index)"
            @drag-end="draggedOverItemIndex = null"
            @update="updateItem"
            @delete="deleteItem(item.id)"
          />

          <!-- Add Item Button -->
          <button
            @click="$emit('add-item')"
            class="w-full py-3 border-2 border-dashed border-neutral-300 rounded-lg hover:border-primary-400 hover:bg-primary-50 text-neutral-600 hover:text-primary-600 transition-all text-sm font-medium"
          >
            + Add Item
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import type { MenuCategory, MenuItem } from '~/stores/restaurant'
import { useDragDrop } from '~/composables/useDragDrop'
import DraggableMenuItem from './DraggableMenuItem.vue'

const props = defineProps<{
  category: MenuCategory
  isDraggedOver: boolean
}>()

const emit = defineEmits<{
  (e: 'drag-start', category: MenuCategory): void
  (e: 'drag-over', event: DragEvent): void
  (e: 'drag-leave'): void
  (e: 'drop'): void
  (e: 'drag-end'): void
  (e: 'update', category: MenuCategory): void
  (e: 'delete'): void
  (e: 'customize'): void
  (e: 'add-item'): void
  (e: 'reorder-items', items: MenuItem[]): void
}>()

const localCategory = reactive({ ...props.category })
const draggedOverItemIndex = ref<number | null>(null)

const {
  handleDragStart: handleItemDragStart,
  handleDragOver: handleItemDragOver,
  handleDrop: handleItemDrop
} = useDragDrop(props.category.items, (reorderedItems) => {
  emit('reorder-items', reorderedItems)
})

const handleDragOver = (event: DragEvent) => {
  event.preventDefault()
  emit('drag-over', event)
}

const updateCategory = () => {
  emit('update', { ...props.category, name: localCategory.name })
}

const updateItem = (item: MenuItem) => {
  const updatedCategory = {
    ...props.category,
    items: props.category.items.map(i => i.id === item.id ? item : i)
  }
  emit('update', updatedCategory)
}

const deleteItem = (itemId: string) => {
  const updatedCategory = {
    ...props.category,
    items: props.category.items.filter(i => i.id !== itemId)
  }
  emit('update', updatedCategory)
}
</script>

<style scoped>
.category-card {
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.category-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}
</style>
