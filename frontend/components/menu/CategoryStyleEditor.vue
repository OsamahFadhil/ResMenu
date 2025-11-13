<template>
  <Modal :show="show" @close="$emit('close')" size="large">
    <template #header>
      <h2 class="text-xl font-bold">Customize: {{ category.name }}</h2>
    </template>

    <Tabs v-model="activeTab">
      <!-- Background Tab -->
      <Tab name="background" title="Background">
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-2">Background Type</label>
            <select v-model="styleData.backgroundType" class="w-full input">
              <option value="color">Solid Color</option>
              <option value="gradient">Gradient</option>
            </select>
          </div>

          <div v-if="styleData.backgroundType === 'color'">
            <label class="block text-sm font-medium text-neutral-700 mb-2">Background Color</label>
            <div class="flex gap-3">
              <input
                v-model="styleData.backgroundColor"
                type="color"
                class="h-12 w-20 rounded-lg border-2 border-neutral-300 cursor-pointer"
              />
              <input
                v-model="styleData.backgroundColor"
                type="text"
                class="flex-1 input"
                placeholder="#ffffff"
              />
            </div>
          </div>

          <div v-else-if="styleData.backgroundType === 'gradient'">
            <label class="block text-sm font-medium text-neutral-700 mb-2">Gradient Colors</label>
            <div class="space-y-3">
              <div class="flex gap-3">
                <input
                  v-model="styleData.gradientFrom"
                  type="color"
                  class="h-12 w-20 rounded-lg border-2 border-neutral-300 cursor-pointer"
                />
                <input
                  v-model="styleData.gradientFrom"
                  type="text"
                  class="flex-1 input"
                  placeholder="#667eea"
                />
              </div>
              <div class="flex gap-3">
                <input
                  v-model="styleData.gradientTo"
                  type="color"
                  class="h-12 w-20 rounded-lg border-2 border-neutral-300 cursor-pointer"
                />
                <input
                  v-model="styleData.gradientTo"
                  type="text"
                  class="flex-1 input"
                  placeholder="#764ba2"
                />
              </div>
            </div>

            <!-- Gradient Preview -->
            <div
              class="mt-4 h-24 rounded-lg"
              :style="{ background: `linear-gradient(135deg, ${styleData.gradientFrom} 0%, ${styleData.gradientTo} 100%)` }"
            ></div>
          </div>
        </div>
      </Tab>

      <!-- Layout Tab -->
      <Tab name="layout" title="Layout">
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-3">Layout Style</label>
            <div class="grid grid-cols-3 gap-3">
              <button
                v-for="layout in layouts"
                :key="layout.value"
                @click="styleData.layout = layout.value"
                :class="[
                  'p-4 border-2 rounded-lg transition-all text-center',
                  styleData.layout === layout.value
                    ? 'border-primary-600 bg-primary-50'
                    : 'border-neutral-200 hover:border-primary-300'
                ]"
              >
                <div class="mb-2 flex justify-center" v-html="layout.icon"></div>
                <span class="text-sm font-medium">{{ layout.label }}</span>
              </button>
            </div>
          </div>

          <div v-if="styleData.layout === 'grid'">
            <label class="block text-sm font-medium text-neutral-700 mb-2">
              Grid Columns: {{ styleData.gridColumns }}
            </label>
            <input
              v-model.number="styleData.gridColumns"
              type="range"
              min="2"
              max="4"
              class="w-full"
            />
          </div>
        </div>
      </Tab>

      <!-- Typography Tab -->
      <Tab name="typography" title="Typography">
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-2">Font Family</label>
            <select v-model="styleData.fontFamily" class="w-full input">
              <option value="inherit">Use Global Font</option>
              <option value="Inter">Inter</option>
              <option value="Roboto">Roboto</option>
              <option value="Playfair Display">Playfair Display</option>
              <option value="Georgia">Georgia</option>
              <option value="Arial">Arial</option>
            </select>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-neutral-700 mb-2">Title Color</label>
              <div class="flex gap-3">
                <input
                  v-model="styleData.titleColor"
                  type="color"
                  class="h-12 w-20 rounded-lg border-2 border-neutral-300 cursor-pointer"
                />
                <input
                  v-model="styleData.titleColor"
                  type="text"
                  class="flex-1 input"
                  placeholder="#1f2937"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-neutral-700 mb-2">Text Color</label>
              <div class="flex gap-3">
                <input
                  v-model="styleData.textColor"
                  type="color"
                  class="h-12 w-20 rounded-lg border-2 border-neutral-300 cursor-pointer"
                />
                <input
                  v-model="styleData.textColor"
                  type="text"
                  class="flex-1 input"
                  placeholder="#4b5563"
                />
              </div>
            </div>
          </div>
        </div>
      </Tab>

      <!-- Spacing Tab -->
      <Tab name="spacing" title="Spacing & Effects">
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-2">
              Padding: {{ styleData.padding }}px
            </label>
            <input
              v-model.number="styleData.padding"
              type="range"
              min="0"
              max="64"
              step="4"
              class="w-full"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-2">Border Radius</label>
            <select v-model="styleData.borderRadius" class="w-full input">
              <option value="0">None</option>
              <option value="8px">Small (8px)</option>
              <option value="16px">Medium (16px)</option>
              <option value="24px">Large (24px)</option>
            </select>
          </div>

          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-2">Shadow</label>
            <select v-model="styleData.shadow" class="w-full input">
              <option value="none">None</option>
              <option value="sm">Small</option>
              <option value="md">Medium</option>
              <option value="lg">Large</option>
              <option value="xl">Extra Large</option>
            </select>
          </div>
        </div>
      </Tab>
    </Tabs>

    <template #footer>
      <div class="flex justify-between">
        <button
          @click="resetToDefault"
          class="text-neutral-600 hover:text-neutral-900 font-medium"
        >
          Reset to Default
        </button>
        <div class="flex gap-3">
          <UiButton @click="$emit('close')" variant="secondary">Cancel</UiButton>
          <UiButton @click="saveStyle" variant="primary">Save Style</UiButton>
        </div>
      </div>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import type { MenuCategory } from '~/stores/restaurant'
import Modal from '~/components/ui/Modal.vue'
import Tabs from '~/components/ui/Tabs.vue'
import Tab from '~/components/ui/Tabs.vue'
import UiButton from '~/components/ui/Button.vue'

const props = defineProps<{
  show: boolean
  category: MenuCategory
  restaurantId: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'save', style: any): void
}>()

const activeTab = ref('background')

const styleData = reactive({
  backgroundType: 'color',
  backgroundColor: '#ffffff',
  gradientFrom: '#667eea',
  gradientTo: '#764ba2',
  layout: 'list',
  gridColumns: 2,
  fontFamily: 'inherit',
  titleColor: '#1f2937',
  textColor: '#4b5563',
  padding: 24,
  borderRadius: '16px',
  shadow: 'md'
})

const layouts = [
  {
    value: 'list',
    label: 'List',
    icon: '<svg class="w-8 h-8 text-neutral-600" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" /></svg>'
  },
  {
    value: 'grid',
    label: 'Grid',
    icon: '<svg class="w-8 h-8 text-neutral-600" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 5a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM14 5a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1h-4a1 1 0 01-1-1V5zM4 15a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1H5a1 1 0 01-1-1v-4zM14 15a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1h-4a1 1 0 01-1-1v-4z" /></svg>'
  },
  {
    value: 'cards',
    label: 'Cards',
    icon: '<svg class="w-8 h-8 text-neutral-600" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" /></svg>'
  }
]

const saveStyle = async () => {
  const api = useApi()
  await api.put(`/categories/${props.category.id}`, {
    name: props.category.name,
    displayOrder: props.category.displayOrder,
    customTheme: JSON.stringify(styleData),
    customLayout: styleData.layout
  })

  emit('save', styleData)
  emit('close')
}

const resetToDefault = () => {
  Object.assign(styleData, {
    backgroundType: 'color',
    backgroundColor: '#ffffff',
    layout: 'list',
    fontFamily: 'inherit',
    titleColor: '#1f2937',
    textColor: '#4b5563',
    padding: 24,
    borderRadius: '16px',
    shadow: 'md'
  })
}
</script>
