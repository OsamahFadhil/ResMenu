<template>
  <div class="preview-panel h-full flex flex-col bg-neutral-100">
    <!-- Preview Header -->
    <div class="flex items-center justify-between p-4 bg-white border-b border-neutral-200">
      <div class="flex items-center gap-3">
        <h3 class="text-lg font-semibold text-neutral-900">Live Preview</h3>
        <span class="px-2 py-1 text-xs font-medium text-neutral-600 bg-neutral-100 rounded">
          {{ viewMode }}
        </span>
      </div>

      <!-- View Mode Switcher -->
      <div class="flex items-center gap-2 bg-neutral-100 rounded-lg p-1">
        <button
          v-for="mode in viewModes"
          :key="mode.value"
          @click="viewMode = mode.value"
          :class="[
            'px-3 py-1.5 rounded-md text-sm font-medium transition-all',
            viewMode === mode.value
              ? 'bg-white text-neutral-900 shadow-sm'
              : 'text-neutral-600 hover:text-neutral-900'
          ]"
          :title="mode.label"
        >
          <component :is="mode.icon" class="w-5 h-5" />
        </button>
      </div>
    </div>

    <!-- Preview Container -->
    <div class="flex-1 overflow-auto p-6">
      <div
        :class="[
          'preview-content mx-auto transition-all duration-300 bg-white rounded-lg shadow-lg overflow-hidden',
          viewModeClasses
        ]"
      >
        <!-- Preview Iframe -->
        <div
          class="preview-iframe overflow-auto"
          :style="previewStyles"
        >
          <!-- Restaurant Header Preview -->
          <div
            class="p-8 border-b"
            :style="{
              backgroundColor: theme.surfaceColor,
              color: theme.textColor,
              fontFamily: theme.fontFamily
            }"
          >
            <div class="flex items-center gap-4">
              <div v-if="restaurant?.logoUrl" class="flex-shrink-0">
                <img
                  :src="restaurant.logoUrl"
                  :alt="restaurant.name"
                  class="w-20 h-20 rounded-xl object-cover"
                />
              </div>
              <div>
                <h1
                  class="text-3xl font-bold tracking-tight"
                  :style="{ fontFamily: theme.fontFamily }"
                >
                  {{ restaurant?.name || 'Restaurant Name' }}
                </h1>
                <p class="text-sm opacity-70 mt-1">
                  {{ language.toUpperCase() }} • Preview Mode
                </p>
              </div>
            </div>
          </div>

          <!-- Categories Preview -->
          <div class="p-6 space-y-6" :style="{ backgroundColor: theme.backgroundColor }">
            <div
              v-for="category in categories"
              :key="category.id"
              :class="[
                'category-preview rounded-xl p-6 shadow-lg border border-opacity-10',
                categoryLayoutClass(category)
              ]"
              :style="getCategoryStyle(category)"
            >
              <!-- Category Title -->
              <h2
                class="text-2xl font-bold mb-4 pb-2 border-b border-opacity-20"
                :style="{
                  color: getCategoryTheme(category).titleColor || theme.primaryColor,
                  fontFamily: getCategoryTheme(category).fontFamily || theme.fontFamily,
                  borderColor: getCategoryTheme(category).titleColor || theme.primaryColor
                }"
              >
                {{ category.localizedName || category.name }}
              </h2>

              <!-- Items Grid/List -->
              <div :class="getItemsContainerClass(category)">
                <div
                  v-for="item in category.items.filter(i => i.isAvailable)"
                  :key="item.id"
                  :class="getItemClass(category)"
                  :style="{
                    backgroundColor: getCategoryTheme(category).backgroundColor || theme.surfaceColor,
                    borderColor: theme.primaryColor,
                    fontFamily: theme.fontFamily
                  }"
                >
                  <!-- Item Image -->
                  <div v-if="displaySettings.showImages && item.imageUrl" class="flex-shrink-0">
                    <img
                      :src="item.imageUrl"
                      :alt="item.name"
                      :class="getItemImageClass(category)"
                    />
                  </div>

                  <!-- Item Content -->
                  <div class="flex-1 min-w-0">
                    <div class="flex justify-between items-start gap-3">
                      <h3
                        class="font-semibold"
                        :style="{
                          color: getCategoryTheme(category).textColor || theme.textColor,
                          fontSize: fontSizeMap[theme.fontSize || 'medium']
                        }"
                      >
                        {{ item.localizedName || item.name }}
                      </h3>
                      <span
                        v-if="displaySettings.showPrices"
                        class="font-bold whitespace-nowrap"
                        :style="{ color: theme.primaryColor }"
                      >
                        {{ formatPrice(item.price) }}
                      </span>
                    </div>
                    <p
                      v-if="displaySettings.showDescriptions && item.description"
                      class="text-sm mt-1 opacity-80"
                      :style="{ color: getCategoryTheme(category).textColor || theme.textColor }"
                    >
                      {{ item.localizedDescription || item.description }}
                    </p>
                  </div>
                </div>

                <!-- Empty Category -->
                <div
                  v-if="category.items.filter(i => i.isAvailable).length === 0"
                  class="text-center py-8 text-neutral-500 text-sm italic"
                >
                  No items in this category
                </div>
              </div>
            </div>

            <!-- Empty State -->
            <div v-if="categories.length === 0" class="text-center py-12">
              <p class="text-neutral-500">No categories to preview</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Preview Footer -->
    <div class="p-4 bg-white border-t border-neutral-200">
      <div class="flex items-center justify-between text-sm text-neutral-600">
        <div>
          {{ categories.length }} categories • {{ totalItems }} items
        </div>
        <div>
          Last updated: {{ lastUpdated }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import type { MenuCategory } from '~/stores/restaurant'
import type { TemplateTheme } from '~/stores/templates'

const props = defineProps<{
  categories: MenuCategory[]
  theme: TemplateTheme
  displaySettings: any
  restaurant?: any
  language?: string
  currency?: string
}>()

const viewMode = ref<'desktop' | 'tablet' | 'mobile'>('desktop')

const viewModes = [
  {
    value: 'desktop',
    label: 'Desktop View',
    icon: defineComponent({
      template: '<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" /></svg>'
    })
  },
  {
    value: 'tablet',
    label: 'Tablet View',
    icon: defineComponent({
      template: '<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z" /></svg>'
    })
  },
  {
    value: 'mobile',
    label: 'Mobile View',
    icon: defineComponent({
      template: '<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z" /></svg>'
    })
  }
]

const viewModeClasses = computed(() => {
  switch (viewMode.value) {
    case 'desktop':
      return 'max-w-7xl'
    case 'tablet':
      return 'max-w-3xl'
    case 'mobile':
      return 'max-w-md'
    default:
      return 'max-w-7xl'
  }
})

const previewStyles = computed(() => ({
  backgroundColor: props.theme.backgroundColor,
  color: props.theme.textColor,
  fontFamily: props.theme.fontFamily,
  minHeight: '600px'
}))

const fontSizeMap = {
  small: '0.875rem',
  medium: '1rem',
  large: '1.125rem'
}

const totalItems = computed(() => {
  return props.categories.reduce((sum, cat) => sum + cat.items.filter(i => i.isAvailable).length, 0)
})

const lastUpdated = computed(() => {
  return new Date().toLocaleTimeString()
})

const getCategoryTheme = (category: MenuCategory): any => {
  if (category.customTheme) {
    try {
      return typeof category.customTheme === 'string'
        ? JSON.parse(category.customTheme)
        : category.customTheme
    } catch {
      return {}
    }
  }
  return {}
}

const getCategoryStyle = (category: MenuCategory) => {
  const customTheme = getCategoryTheme(category)

  const style: any = {
    backgroundColor: customTheme.backgroundColor || props.theme.surfaceColor,
    fontFamily: customTheme.fontFamily || props.theme.fontFamily
  }

  if (customTheme.backgroundType === 'gradient') {
    style.background = `linear-gradient(135deg, ${customTheme.gradientFrom || '#667eea'} 0%, ${customTheme.gradientTo || '#764ba2'} 100%)`
  }

  if (customTheme.borderRadius) {
    style.borderRadius = customTheme.borderRadius
  }

  if (customTheme.padding) {
    style.padding = `${customTheme.padding}px`
  }

  return style
}

const categoryLayoutClass = (category: MenuCategory) => {
  const layout = category.customLayout || props.theme.layout || 'list'
  const spacing = props.theme.spacing || 'normal'

  const spacingMap = {
    compact: 'space-y-2',
    normal: 'space-y-4',
    relaxed: 'space-y-6'
  }

  return spacingMap[spacing]
}

const getItemsContainerClass = (category: MenuCategory) => {
  const layout = category.customLayout || props.theme.layout || 'list'

  if (layout === 'grid') {
    const customTheme = getCategoryTheme(category)
    const cols = customTheme.gridColumns || 2
    return `grid grid-cols-1 sm:grid-cols-${cols} gap-4`
  }

  if (layout === 'cards') {
    return 'grid grid-cols-1 md:grid-cols-2 gap-4'
  }

  return 'space-y-3'
}

const getItemClass = (category: MenuCategory) => {
  const layout = category.customLayout || props.theme.layout || 'list'

  if (layout === 'grid') {
    return 'flex flex-col gap-3 p-4 border-2 rounded-lg hover:shadow-xl transition-all'
  }

  if (layout === 'cards') {
    return 'flex gap-4 p-4 border rounded-lg hover:shadow-lg transition-all'
  }

  return 'flex items-start gap-4 p-3 hover:bg-opacity-5 hover:bg-black rounded-lg transition-all'
}

const getItemImageClass = (category: MenuCategory) => {
  const layout = category.customLayout || props.theme.layout || 'list'
  const shape = props.theme.imageShape || 'rounded'

  const shapeMap = {
    square: 'rounded-none',
    rounded: 'rounded-lg',
    circle: 'rounded-full'
  }

  if (layout === 'grid') {
    return `h-40 w-full object-cover ${shapeMap[shape]}`
  }

  return `h-20 w-20 object-cover ${shapeMap[shape]}`
}

const formatPrice = (price: number) => {
  const currency = props.currency || 'USD'
  try {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency,
      minimumFractionDigits: 2
    }).format(price)
  } catch {
    return `${currency} ${price.toFixed(2)}`
  }
}
</script>

<style scoped>
.preview-panel {
  background: linear-gradient(to bottom, #f5f5f5 0%, #e5e5e5 100%);
}

.preview-content {
  transition: max-width 0.3s ease-in-out;
}

.preview-iframe::-webkit-scrollbar {
  width: 8px;
}

.preview-iframe::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 4px;
}

.preview-iframe::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 4px;
}

.preview-iframe::-webkit-scrollbar-thumb:hover {
  background: #555;
}
</style>
