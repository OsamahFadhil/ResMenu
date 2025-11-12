<template>
  <Card title="Theme Customization" class="space-y-6">
    <!-- Color Scheme -->
    <div>
      <h3 class="text-lg font-semibold text-neutral-900 mb-4">Color Scheme</h3>
      <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Primary Color</label>
          <input
            v-model="localTheme.primaryColor"
            type="color"
            class="w-full h-12 rounded-lg cursor-pointer border-2 border-neutral-300"
            @input="updateTheme"
          />
          <input
            v-model="localTheme.primaryColor"
            type="text"
            class="mt-2 w-full px-3 py-2 text-sm border border-neutral-300 rounded-lg"
            placeholder="#dc2626"
            @input="updateTheme"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Accent Color</label>
          <input
            v-model="localTheme.accentColor"
            type="color"
            class="w-full h-12 rounded-lg cursor-pointer border-2 border-neutral-300"
            @input="updateTheme"
          />
          <input
            v-model="localTheme.accentColor"
            type="text"
            class="mt-2 w-full px-3 py-2 text-sm border border-neutral-300 rounded-lg"
            placeholder="#f59e0b"
            @input="updateTheme"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Background</label>
          <input
            v-model="localTheme.backgroundColor"
            type="color"
            class="w-full h-12 rounded-lg cursor-pointer border-2 border-neutral-300"
            @input="updateTheme"
          />
          <input
            v-model="localTheme.backgroundColor"
            type="text"
            class="mt-2 w-full px-3 py-2 text-sm border border-neutral-300 rounded-lg"
            placeholder="#fafaf9"
            @input="updateTheme"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Surface Color</label>
          <input
            v-model="localTheme.surfaceColor"
            type="color"
            class="w-full h-12 rounded-lg cursor-pointer border-2 border-neutral-300"
            @input="updateTheme"
          />
          <input
            v-model="localTheme.surfaceColor"
            type="text"
            class="mt-2 w-full px-3 py-2 text-sm border border-neutral-300 rounded-lg"
            placeholder="#ffffff"
            @input="updateTheme"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Text Color</label>
          <input
            v-model="localTheme.textColor"
            type="color"
            class="w-full h-12 rounded-lg cursor-pointer border-2 border-neutral-300"
            @input="updateTheme"
          />
          <input
            v-model="localTheme.textColor"
            type="text"
            class="mt-2 w-full px-3 py-2 text-sm border border-neutral-300 rounded-lg"
            placeholder="#292524"
            @input="updateTheme"
          />
        </div>
      </div>
    </div>

    <!-- Typography -->
    <div>
      <h3 class="text-lg font-semibold text-neutral-900 mb-4">Typography</h3>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Font Family</label>
          <select
            v-model="localTheme.fontFamily"
            class="w-full px-4 py-2.5 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
            @change="updateTheme"
          >
            <option value="Inter">Inter</option>
            <option value="Roboto">Roboto</option>
            <option value="Open Sans">Open Sans</option>
            <option value="Lato">Lato</option>
            <option value="Montserrat">Montserrat</option>
            <option value="Playfair Display">Playfair Display</option>
            <option value="Georgia">Georgia</option>
            <option value="Arial">Arial</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Font Size</label>
          <select
            v-model="localTheme.fontSize"
            class="w-full px-4 py-2.5 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
            @change="updateTheme"
          >
            <option value="small">Small</option>
            <option value="medium">Medium</option>
            <option value="large">Large</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Quick Presets -->
    <div>
      <h3 class="text-lg font-semibold text-neutral-900 mb-4">Quick Presets</h3>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
        <button
          v-for="preset in presets"
          :key="preset.name"
          @click="applyPreset(preset)"
          class="p-4 rounded-lg border-2 hover:border-primary-500 transition-all"
          :style="{ borderColor: preset.theme.primaryColor }"
        >
          <div class="flex items-center gap-2 mb-2">
            <div class="w-6 h-6 rounded" :style="{ backgroundColor: preset.theme.primaryColor }"></div>
            <div class="w-6 h-6 rounded" :style="{ backgroundColor: preset.theme.accentColor }"></div>
          </div>
          <span class="text-sm font-medium">{{ preset.name }}</span>
        </button>
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

const presets = [
  {
    name: 'Classic Red',
    theme: {
      primaryColor: '#dc2626',
      accentColor: '#f59e0b',
      backgroundColor: '#fafaf9',
      surfaceColor: '#ffffff',
      textColor: '#292524',
      fontFamily: 'Inter',
      fontSize: 'medium' as const,
    }
  },
  {
    name: 'Ocean Blue',
    theme: {
      primaryColor: '#0284c7',
      accentColor: '#06b6d4',
      backgroundColor: '#f0f9ff',
      surfaceColor: '#ffffff',
      textColor: '#0c4a6e',
      fontFamily: 'Inter',
      fontSize: 'medium' as const,
    }
  },
  {
    name: 'Forest Green',
    theme: {
      primaryColor: '#059669',
      accentColor: '#10b981',
      backgroundColor: '#f0fdf4',
      surfaceColor: '#ffffff',
      textColor: '#064e3b',
      fontFamily: 'Inter',
      fontSize: 'medium' as const,
    }
  },
  {
    name: 'Elegant Purple',
    theme: {
      primaryColor: '#7c3aed',
      accentColor: '#a78bfa',
      backgroundColor: '#faf5ff',
      surfaceColor: '#ffffff',
      textColor: '#4c1d95',
      fontFamily: 'Playfair Display',
      fontSize: 'medium' as const,
    }
  },
]

const applyPreset = (preset: typeof presets[0]) => {
  localTheme.value = { ...preset.theme }
  updateTheme()
}
</script>

