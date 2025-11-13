<template>
  <Modal :show="show" @close="$emit('close')" size="medium">
    <template #header>
      <div class="flex items-center gap-3">
        <svg class="w-6 h-6 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
        </svg>
        <h2 class="text-xl font-bold">Export Menu</h2>
      </div>
    </template>

    <div class="space-y-6">
      <!-- Export Format -->
      <div>
        <label class="block text-sm font-medium text-neutral-700 mb-3">Export Format</label>
        <div class="grid grid-cols-3 gap-3">
          <button
            v-for="format in exportFormats"
            :key="format.value"
            @click="selectedFormat = format.value"
            :class="[
              'p-4 border-2 rounded-lg transition-all text-center',
              selectedFormat === format.value
                ? 'border-primary-600 bg-primary-50'
                : 'border-neutral-200 hover:border-primary-300'
            ]"
          >
            <div class="mb-2 flex justify-center" v-html="format.icon"></div>
            <span class="text-sm font-medium">{{ format.label }}</span>
          </button>
        </div>
      </div>

      <!-- PDF Options (only show for PDF format) -->
      <div v-if="selectedFormat === 'pdf'" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Paper Size</label>
          <select v-model="exportOptions.paperSize" class="w-full input">
            <option value="a4">A4 (210 × 297 mm)</option>
            <option value="letter">Letter (8.5 × 11 in)</option>
            <option value="legal">Legal (8.5 × 14 in)</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Orientation</label>
          <div class="grid grid-cols-2 gap-3">
            <button
              @click="exportOptions.orientation = 'portrait'"
              :class="[
                'p-3 border-2 rounded-lg transition-all',
                exportOptions.orientation === 'portrait'
                  ? 'border-primary-600 bg-primary-50'
                  : 'border-neutral-200 hover:border-primary-300'
              ]"
            >
              <svg class="w-8 h-8 mx-auto mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <rect x="7" y="2" width="10" height="20" stroke-width="2" rx="2" />
              </svg>
              <span class="text-sm font-medium">Portrait</span>
            </button>
            <button
              @click="exportOptions.orientation = 'landscape'"
              :class="[
                'p-3 border-2 rounded-lg transition-all',
                exportOptions.orientation === 'landscape'
                  ? 'border-primary-600 bg-primary-50'
                  : 'border-neutral-200 hover:border-primary-300'
              ]"
            >
              <svg class="w-8 h-8 mx-auto mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <rect x="2" y="7" width="20" height="10" stroke-width="2" rx="2" />
              </svg>
              <span class="text-sm font-medium">Landscape</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Content Options -->
      <div>
        <label class="block text-sm font-medium text-neutral-700 mb-3">Include in Export</label>
        <div class="space-y-2">
          <label class="flex items-center gap-3 p-3 border rounded-lg hover:bg-neutral-50 cursor-pointer">
            <input
              v-model="exportOptions.includeImages"
              type="checkbox"
              class="w-4 h-4 text-primary-600 rounded focus:ring-primary-500"
            />
            <div class="flex-1">
              <div class="font-medium text-neutral-900">Images</div>
              <div class="text-sm text-neutral-600">Include menu item images</div>
            </div>
          </label>

          <label class="flex items-center gap-3 p-3 border rounded-lg hover:bg-neutral-50 cursor-pointer">
            <input
              v-model="exportOptions.includePrices"
              type="checkbox"
              class="w-4 h-4 text-primary-600 rounded focus:ring-primary-500"
            />
            <div class="flex-1">
              <div class="font-medium text-neutral-900">Prices</div>
              <div class="text-sm text-neutral-600">Display item prices</div>
            </div>
          </label>

          <label class="flex items-center gap-3 p-3 border rounded-lg hover:bg-neutral-50 cursor-pointer">
            <input
              v-model="exportOptions.includeDescriptions"
              type="checkbox"
              class="w-4 h-4 text-primary-600 rounded focus:ring-primary-500"
            />
            <div class="flex-1">
              <div class="font-medium text-neutral-900">Descriptions</div>
              <div class="text-sm text-neutral-600">Include item descriptions</div>
            </div>
          </label>
        </div>
      </div>

      <!-- File Name -->
      <div>
        <label class="block text-sm font-medium text-neutral-700 mb-2">File Name</label>
        <div class="flex gap-2">
          <input
            v-model="fileName"
            type="text"
            class="flex-1 input"
            placeholder="my_restaurant_menu"
          />
          <span class="px-3 py-2 bg-neutral-100 text-neutral-700 rounded-lg border border-neutral-300">
            .{{ selectedFormat }}
          </span>
        </div>
      </div>

      <!-- Export Stats -->
      <div class="p-4 bg-blue-50 rounded-lg border border-blue-200">
        <div class="flex items-start gap-3">
          <svg class="w-5 h-5 text-blue-600 flex-shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
          </svg>
          <div class="text-sm text-blue-900">
            <div class="font-medium mb-1">Export Summary</div>
            <div>
              {{ categories.length }} categories, {{ totalItems }} items
              {{ exportOptions.includeImages ? '(with images)' : '' }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="flex justify-end gap-3">
        <UiButton @click="$emit('close')" variant="secondary">Cancel</UiButton>
        <UiButton @click="handleExport" variant="primary" :disabled="exporting">
          <svg v-if="exporting" class="w-4 h-4 animate-spin -ml-1 mr-2" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          {{ exporting ? 'Exporting...' : 'Export Menu' }}
        </UiButton>
      </div>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import type { MenuCategory } from '~/stores/restaurant'
import type { TemplateTheme } from '~/stores/templates'
import type { ExportOptions } from '~/composables/useMenuExport'
import Modal from '~/components/ui/Modal.vue'
import UiButton from '~/components/ui/Button.vue'

const props = defineProps<{
  show: boolean
  categories: MenuCategory[]
  theme: TemplateTheme
  restaurantName: string
  logoUrl?: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'exported'): void
}>()

const { exportToPDF, exportToHTML, printMenu } = useMenuExport()

const selectedFormat = ref<'pdf' | 'html' | 'print'>('pdf')
const exporting = ref(false)
const fileName = ref(props.restaurantName.replace(/[^a-z0-9]/gi, '_').toLowerCase() + '_menu')

const exportOptions = ref<Partial<ExportOptions>>({
  paperSize: 'a4',
  orientation: 'portrait',
  includeImages: true,
  includePrices: true,
  includeDescriptions: true
})

const exportFormats = [
  {
    value: 'pdf',
    label: 'PDF',
    icon: '<svg class="w-8 h-8 text-red-600" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M4 4a2 2 0 012-2h4.586A2 2 0 0112 2.586L15.414 6A2 2 0 0116 7.414V16a2 2 0 01-2 2H6a2 2 0 01-2-2V4z" clip-rule="evenodd" /></svg>'
  },
  {
    value: 'html',
    label: 'HTML',
    icon: '<svg class="w-8 h-8 text-blue-600" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M3 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z" clip-rule="evenodd" /></svg>'
  },
  {
    value: 'print',
    label: 'Print',
    icon: '<svg class="w-8 h-8 text-neutral-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z" /></svg>'
  }
]

const totalItems = computed(() => {
  return props.categories.reduce((sum, cat) => sum + cat.items.filter(i => i.isAvailable).length, 0)
})

const handleExport = async () => {
  exporting.value = true

  try {
    const fullFileName = `${fileName.value}.${selectedFormat.value}`

    if (selectedFormat.value === 'pdf') {
      await exportToPDF(
        props.categories,
        props.theme,
        props.restaurantName,
        props.logoUrl,
        { ...exportOptions.value, fileName: fullFileName }
      )
    } else if (selectedFormat.value === 'html') {
      exportToHTML(
        props.categories,
        props.theme,
        props.restaurantName,
        props.logoUrl,
        { ...exportOptions.value, fileName: fullFileName }
      )
    } else if (selectedFormat.value === 'print') {
      printMenu(
        props.categories,
        props.theme,
        props.restaurantName,
        props.logoUrl,
        exportOptions.value
      )
    }

    emit('exported')
    emit('close')
  } catch (error) {
    console.error('Export failed:', error)
    alert('Failed to export menu. Please try again.')
  } finally {
    exporting.value = false
  }
}
</script>

<style scoped>
.input {
  @apply px-3 py-2 border border-neutral-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500;
}
</style>
