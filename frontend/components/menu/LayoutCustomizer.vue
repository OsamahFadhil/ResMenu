<template>
  <Card title="Layout & Display" class="space-y-6">
    <!-- Layout Style -->
    <div>
      <h3 class="text-lg font-semibold text-neutral-900 mb-4">Layout Style</h3>
      <div class="grid grid-cols-3 gap-4">
        <button
          v-for="layout in layouts"
          :key="layout.value"
          @click="selectLayout(layout.value)"
          :class="[
            'p-4 rounded-lg border-2 transition-all',
            localTheme.layout === layout.value
              ? 'border-primary-600 bg-primary-50'
              : 'border-neutral-300 hover:border-primary-400'
          ]"
        >
          <div class="mb-3">
            <component :is="layout.icon" class="w-full h-16 text-neutral-400" />
          </div>
          <span class="text-sm font-medium">{{ layout.label }}</span>
        </button>
      </div>
    </div>

    <!-- Card Style -->
    <div>
      <h3 class="text-lg font-semibold text-neutral-900 mb-4">Card Style</h3>
      <div class="grid grid-cols-3 gap-4">
        <button
          v-for="style in cardStyles"
          :key="style.value"
          @click="selectCardStyle(style.value)"
          :class="[
            'p-4 rounded-lg border-2 transition-all',
            localTheme.cardStyle === style.value
              ? 'border-primary-600 bg-primary-50'
              : 'border-neutral-300 hover:border-primary-400'
          ]"
        >
          <div class="mb-2 h-20 rounded" :class="style.preview"></div>
          <span class="text-sm font-medium">{{ style.label }}</span>
        </button>
      </div>
    </div>

    <!-- Border Radius -->
    <div>
      <h3 class="text-lg font-semibold text-neutral-900 mb-4">Border Radius</h3>
      <div class="grid grid-cols-4 gap-3">
        <button
          v-for="radius in borderRadiusOptions"
          :key="radius.value"
          @click="selectBorderRadius(radius.value)"
          :class="[
            'p-3 border-2 transition-all',
            localTheme.borderRadius === radius.value
              ? 'border-primary-600 bg-primary-50'
              : 'border-neutral-300 hover:border-primary-400'
          ]"
          :style="{ borderRadius: radius.style }"
        >
          <span class="text-sm font-medium">{{ radius.label }}</span>
        </button>
      </div>
    </div>

    <!-- Spacing -->
    <div>
      <h3 class="text-lg font-semibold text-neutral-900 mb-4">Spacing</h3>
      <div class="grid grid-cols-3 gap-4">
        <button
          v-for="space in spacingOptions"
          :key="space.value"
          @click="selectSpacing(space.value)"
          :class="[
            'p-4 rounded-lg border-2 transition-all',
            localTheme.spacing === space.value
              ? 'border-primary-600 bg-primary-50'
              : 'border-neutral-300 hover:border-primary-400'
          ]"
        >
          <div class="mb-2">
            <div class="space-y-1">
              <div v-for="i in 3" :key="i" class="h-2 bg-neutral-300 rounded" :class="space.preview"></div>
            </div>
          </div>
          <span class="text-sm font-medium">{{ space.label }}</span>
        </button>
      </div>
    </div>

    <!-- Image Settings -->
    <div>
      <h3 class="text-lg font-semibold text-neutral-900 mb-4">Image Settings</h3>
      <div class="space-y-4">
        <div class="flex items-center gap-3">
          <input
            v-model="localTheme.showImages"
            type="checkbox"
            id="show-images"
            class="w-5 h-5 text-primary-600 rounded border-neutral-300 focus:ring-primary-500"
            @change="updateTheme"
          />
          <label for="show-images" class="text-sm font-medium text-neutral-700">
            Show Item Images
          </label>
        </div>

        <div v-if="localTheme.showImages" class="grid grid-cols-2 gap-4 ml-8">
          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-2">Image Size</label>
            <select
              v-model="localTheme.imageSize"
              class="w-full px-4 py-2.5 border border-neutral-300 rounded-lg"
              @change="updateTheme"
            >
              <option value="small">Small</option>
              <option value="medium">Medium</option>
              <option value="large">Large</option>
            </select>
          </div>

          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-2">Image Shape</label>
            <select
              v-model="localTheme.imageShape"
              class="w-full px-4 py-2.5 border border-neutral-300 rounded-lg"
              @change="updateTheme"
            >
              <option value="square">Square</option>
              <option value="rounded">Rounded</option>
              <option value="circle">Circle</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Header Settings -->
    <div>
      <h3 class="text-lg font-semibold text-neutral-900 mb-4">Header Settings</h3>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Logo Position</label>
          <select
            v-model="localTheme.logoPosition"
            class="w-full px-4 py-2.5 border border-neutral-300 rounded-lg"
            @change="updateTheme"
          >
            <option value="left">Left</option>
            <option value="center">Center</option>
            <option value="right">Right</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Header Style</label>
          <select
            v-model="localTheme.headerStyle"
            class="w-full px-4 py-2.5 border border-neutral-300 rounded-lg"
            @change="updateTheme"
          >
            <option value="simple">Simple</option>
            <option value="banner">Banner</option>
            <option value="overlay">Overlay</option>
          </select>
        </div>
      </div>

      <div class="mt-4 flex items-center gap-3">
        <input
          v-model="localTheme.showRestaurantInfo"
          type="checkbox"
          id="show-info"
          class="w-5 h-5 text-primary-600 rounded border-neutral-300 focus:ring-primary-500"
          @change="updateTheme"
        />
        <label for="show-info" class="text-sm font-medium text-neutral-700">
          Show Restaurant Contact Information
        </label>
      </div>
    </div>
  </Card>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import type { TemplateTheme } from '@/stores/templates'
import Card from '~/components/ui/Card.vue'

const props = defineProps<{
  modelValue: TemplateTheme
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: TemplateTheme): void
}>()

const localTheme = ref<TemplateTheme>({ ...props.modelValue })

watch(() => props.modelValue, (newValue) => {
  localTheme.value = { ...newValue }
}, { deep: true })

const updateTheme = () => {
  emit('update:modelValue', { ...localTheme.value })
}

const layouts = [
  { value: 'list', label: 'List', icon: 'IconList' },
  { value: 'grid', label: 'Grid', icon: 'IconGrid' },
  { value: 'cards', label: 'Cards', icon: 'IconCards' },
]

const cardStyles = [
  { value: 'modern', label: 'Modern', preview: 'bg-gradient-to-br from-primary-100 to-accent-100 shadow-lg' },
  { value: 'classic', label: 'Classic', preview: 'bg-white border-2 border-neutral-300' },
  { value: 'minimal', label: 'Minimal', preview: 'bg-neutral-50 border border-neutral-200' },
]

const borderRadiusOptions = [
  { value: 'none', label: 'None', style: '0' },
  { value: 'small', label: 'Small', style: '0.375rem' },
  { value: 'medium', label: 'Medium', style: '0.75rem' },
  { value: 'large', label: 'Large', style: '1.5rem' },
]

const spacingOptions = [
  { value: 'compact', label: 'Compact', preview: 'w-3/4' },
  { value: 'normal', label: 'Normal', preview: 'w-full' },
  { value: 'relaxed', label: 'Relaxed', preview: 'w-full' },
]

const selectLayout = (value: string) => {
  localTheme.value.layout = value as any
  updateTheme()
}

const selectCardStyle = (value: string) => {
  localTheme.value.cardStyle = value as any
  updateTheme()
}

const selectBorderRadius = (value: string) => {
  localTheme.value.borderRadius = value as any
  updateTheme()
}

const selectSpacing = (value: string) => {
  localTheme.value.spacing = value as any
  updateTheme()
}
</script>

