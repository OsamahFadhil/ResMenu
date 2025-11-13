<template>
  <div
    class="fixed inset-0 bg-black/70 backdrop-blur-sm flex items-center justify-center z-50 p-4"
    @click.self="emit('close')"
  >
    <div class="bg-neutral-800 rounded-2xl shadow-2xl max-w-6xl w-full max-h-[90vh] overflow-hidden flex flex-col">
      <!-- Header -->
      <div class="px-6 py-5 border-b border-neutral-700 flex items-center justify-between">
        <div>
          <h2 class="text-2xl font-bold text-white">Template Library</h2>
          <p class="text-sm text-neutral-400 mt-1">Choose a template to start your design</p>
        </div>
        <button
          @click="emit('close')"
          class="p-2 rounded-lg text-neutral-400 hover:text-white hover:bg-neutral-700 transition-colors"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Category Filters -->
      <div class="px-6 py-4 border-b border-neutral-700">
        <div class="flex items-center gap-2 overflow-x-auto">
          <button
            v-for="category in categories"
            :key="category.id"
            @click="selectedCategory = category.id"
            :class="[
              'px-4 py-2 rounded-lg whitespace-nowrap transition-colors',
              selectedCategory === category.id
                ? 'bg-blue-600 text-white'
                : 'bg-neutral-700 text-neutral-300 hover:bg-neutral-600'
            ]"
          >
            {{ category.label }}
          </button>
        </div>
      </div>

      <!-- Templates Grid -->
      <div class="flex-1 overflow-y-auto p-6">
        <!-- Blank Template -->
        <div class="mb-6">
          <h3 class="text-sm font-semibold text-white uppercase tracking-wider mb-3">Start from Scratch</h3>
          <button
            @click="selectBlank"
            class="group relative w-full h-64 rounded-xl border-2 border-dashed border-neutral-600 hover:border-blue-500 transition-colors bg-neutral-900 flex flex-col items-center justify-center gap-3"
          >
            <div class="w-16 h-16 rounded-full bg-neutral-800 group-hover:bg-blue-600 transition-colors flex items-center justify-center">
              <svg class="w-8 h-8 text-neutral-400 group-hover:text-white transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
              </svg>
            </div>
            <div>
              <div class="text-lg font-semibold text-white group-hover:text-blue-400 transition-colors">Blank Canvas</div>
              <div class="text-sm text-neutral-400">Start with an empty canvas</div>
            </div>
          </button>
        </div>

        <!-- Pre-designed Templates -->
        <div>
          <h3 class="text-sm font-semibold text-white uppercase tracking-wider mb-3">
            {{ selectedCategory === 'all' ? 'All Templates' : categories.find(c => c.id === selectedCategory)?.label }}
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div
              v-for="template in filteredTemplates"
              :key="template.id"
              @click="selectTemplate(template)"
              class="group cursor-pointer"
            >
              <div class="relative rounded-xl overflow-hidden shadow-lg hover:shadow-2xl transition-shadow bg-neutral-900">
                <!-- Preview -->
                <div
                  class="aspect-[9/16] relative"
                  :style="{
                    backgroundColor: template.backgroundColor,
                    backgroundImage: template.backgroundImage ? `url(${template.backgroundImage})` : 'none',
                    backgroundSize: 'cover',
                    backgroundPosition: 'center'
                  }"
                >
                  <!-- Sample Elements Preview -->
                  <div class="absolute inset-0 p-4 flex flex-col gap-3">
                    <!-- Header -->
                    <div class="text-white text-xl font-bold opacity-70">Restaurant Name</div>

                    <!-- Sample Items -->
                    <div v-for="i in 3" :key="i" class="bg-white/10 backdrop-blur-sm rounded-lg p-3 flex gap-2">
                      <div class="w-12 h-12 bg-white/20 rounded"></div>
                      <div class="flex-1">
                        <div class="h-3 bg-white/30 rounded w-3/4 mb-2"></div>
                        <div class="h-2 bg-white/20 rounded w-full"></div>
                      </div>
                      <div class="text-white font-bold text-sm">$9.99</div>
                    </div>
                  </div>

                  <!-- Hover Overlay -->
                  <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity flex items-end justify-center pb-6">
                    <div class="px-6 py-3 bg-blue-600 rounded-lg text-white font-medium">
                      Use Template
                    </div>
                  </div>

                  <!-- Premium Badge -->
                  <div
                    v-if="template.isPremium"
                    class="absolute top-3 right-3 px-2 py-1 bg-yellow-500 text-yellow-900 text-xs font-bold rounded"
                  >
                    PREMIUM
                  </div>
                </div>

                <!-- Info -->
                <div class="p-4">
                  <h4 class="text-white font-semibold mb-1">{{ template.name }}</h4>
                  <p class="text-neutral-400 text-sm line-clamp-2">{{ template.description }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div
            v-if="filteredTemplates.length === 0"
            class="text-center py-12 text-neutral-500"
          >
            <svg class="w-16 h-16 mx-auto mb-4 opacity-30" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            <p class="text-lg font-medium">No templates found in this category</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useMenuDesignerStore } from '@/stores/menuDesigner'
import type { MenuDesignTemplate } from '@/stores/menuDesigner'

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'select', template: MenuDesignTemplate | null): void
}>()

const store = useMenuDesignerStore()
const selectedCategory = ref('all')

const categories = [
  { id: 'all', label: 'All Templates' },
  { id: 'modern', label: 'Modern' },
  { id: 'classic', label: 'Classic' },
  { id: 'elegant', label: 'Elegant' },
  { id: 'minimal', label: 'Minimal' },
  { id: 'colorful', label: 'Colorful' }
]

const filteredTemplates = computed(() => {
  if (selectedCategory.value === 'all') {
    return store.templates
  }
  return store.templates.filter(t => t.category === selectedCategory.value)
})

const selectBlank = () => {
  emit('select', null)
  emit('close')
}

const selectTemplate = (template: MenuDesignTemplate) => {
  emit('select', template)
  emit('close')
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
