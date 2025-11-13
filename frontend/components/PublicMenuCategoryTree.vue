<template>
  <div
    v-for="category in categories"
    :key="category.id"
    class="mb-8 rounded-3xl shadow-xl overflow-hidden transition-all hover:shadow-2xl"
    :class="designMode ? 'border-2 border-transparent' : ''"
    :style="{
      backgroundColor: settings?.surfaceColor || '#ffffff',
      boxShadow: designMode && selectedCategoryId === category.id ? `0 0 0 3px ${(settings?.primaryColor || '#dc2626')}33` : undefined
    }"
  >
    <!-- Category Header -->
    <div
      v-if="displaySettings?.showCategories !== false"
      class="px-6 sm:px-8 py-6 border-b-4"
      :class="designMode ? 'cursor-pointer select-none' : ''"
      :style="{
        borderColor: settings?.primaryColor || '#dc2626',
        backgroundColor: `${settings?.primaryColor || '#dc2626'}10`
      }"
      @click.stop="handleCategoryClick(category)"
    >
      <h2
        class="text-3xl sm:text-4xl font-bold tracking-tight"
        :style="{
          color: settings?.primaryColor || '#dc2626',
          fontFamily: settings?.fontFamily || 'Inter'
        }"
      >
        {{ category.localizedName || category.name }}
      </h2>
    </div>

    <!-- Items Container - DYNAMIC LAYOUT -->
    <div 
      class="p-4 sm:p-6"
      :class="getContainerClasses(category.id)"
    >
      <div
        v-for="item in category.items.filter(i => i.isAvailable)"
        :key="item.id"
        :class="getItemClasses(category.id)"
        @click="$emit('item-click', item, category)"
      >
        <!-- Layout-specific rendering -->
        <template v-if="getCategoryLayout(category.id)?.layout === 'grid' || getCategoryLayout(category.id)?.layout === 'cards'">
          <!-- Grid/Cards Layout: Image on top -->
          <div class="space-y-3">
            <!-- Item Image -->
            <div
              v-if="shouldShowImage(category.id) && (item.imageUrl || (item.images && item.images.length > 0))"
              class="relative overflow-hidden bg-neutral-100"
              :class="getImageClasses(category.id)"
            >
              <img
                :src="item.images?.[0] || item.imageUrl"
                :alt="item.localizedName || item.name"
                class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
              />
              <!-- Multi-image indicator -->
              <div
                v-if="item.images && item.images.length > 1"
                class="absolute bottom-2 right-2 px-2 py-1 rounded-full bg-black/70 backdrop-blur-sm text-white text-xs font-bold flex items-center gap-1"
              >
                <svg class="w-3 h-3" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z" clip-rule="evenodd" />
                </svg>
                {{ item.images.length }}
              </div>
            </div>

            <!-- Item Content -->
            <div class="space-y-2">
              <div class="space-y-1">
                <h3
                  class="text-lg sm:text-xl font-bold group-hover:underline"
                  :style="{ color: settings?.textColor || '#1f2937' }"
                >
                  {{ item.localizedName || item.name }}
                </h3>
                <span
                  v-if="shouldShowPrice(category.id)"
                  class="block text-lg font-bold"
                  :style="{ color: settings?.primaryColor || '#dc2626' }"
                >
                  {{ formatPrice(item.price) }}
                </span>
              </div>

              <p
                v-if="shouldShowDescription(category.id) && (item.localizedDescription || item.description)"
                class="text-sm leading-relaxed line-clamp-2"
                :style="{ color: settings?.textColor || '#374151', opacity: 0.8 }"
              >
                {{ item.localizedDescription || item.description }}
              </p>
            </div>
          </div>
        </template>

        <template v-else>
          <!-- List Layout: Image on side (default) -->
          <div class="flex flex-col sm:flex-row gap-4 sm:gap-6">
            <!-- Item Image -->
            <div
              v-if="shouldShowImage(category.id) && (item.imageUrl || (item.images && item.images.length > 0))"
              class="flex-shrink-0"
            >
              <div class="relative overflow-hidden bg-neutral-100" :class="getImageClasses(category.id)">
                <img
                  :src="item.images?.[0] || item.imageUrl"
                  :alt="item.localizedName || item.name"
                  class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
                />
                <!-- Multi-image indicator -->
                <div
                  v-if="item.images && item.images.length > 1"
                  class="absolute bottom-2 right-2 px-2 py-1 rounded-full bg-black/70 backdrop-blur-sm text-white text-xs font-bold flex items-center gap-1"
                >
                  <svg class="w-3 h-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z" clip-rule="evenodd" />
                  </svg>
                  {{ item.images.length }}
                </div>
              </div>
            </div>

            <!-- Item Content -->
            <div class="flex-1 space-y-2 min-w-0">
              <div class="flex items-start justify-between gap-4">
                <h3
                  class="text-xl sm:text-2xl font-bold group-hover:underline"
                  :style="{ color: settings?.textColor || '#1f2937' }"
                >
                  {{ item.localizedName || item.name }}
                </h3>
                <span
                  v-if="shouldShowPrice(category.id)"
                  class="text-xl sm:text-2xl font-bold whitespace-nowrap"
                  :style="{ color: settings?.primaryColor || '#dc2626' }"
                >
                  {{ formatPrice(item.price) }}
                </span>
              </div>

              <p
                v-if="shouldShowDescription(category.id) && (item.localizedDescription || item.description)"
                class="text-base sm:text-lg leading-relaxed line-clamp-2"
                :style="{ color: settings?.textColor || '#374151', opacity: 0.8 }"
              >
                {{ item.localizedDescription || item.description }}
              </p>
            </div>
          </div>
        </template>

        <!-- Click Indicator -->
        <div class="absolute top-4 right-4 opacity-0 group-hover:opacity-100 transition-opacity">
          <div
            class="p-2 rounded-full"
            :style="{ backgroundColor: `${settings?.primaryColor || '#dc2626'}20` }"
          >
            <svg
              class="w-5 h-5"
              :style="{ color: settings?.primaryColor || '#dc2626' }"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
          </div>
        </div>
      </div>

      <!-- Empty Category -->
      <div
        v-if="category.items.filter(i => i.isAvailable).length === 0"
        class="text-center py-12 text-neutral-500"
        :class="getCategoryLayout(category.id)?.layout === 'grid' ? 'col-span-full' : ''"
      >
        <svg class="w-16 h-16 mx-auto mb-4 opacity-30" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
        </svg>
        <p class="text-lg font-medium">
          {{ t('menu.emptyCategoryMessage') || 'No items in this category' }}
        </p>
      </div>
    </div>

    <!-- Child Categories -->
    <div v-if="category.children.length" class="px-4 sm:px-6 pb-6 space-y-6">
      <PublicMenuCategoryTree
        :categories="category.children"
        :settings="settings"
        :displaySettings="displaySettings"
        :layoutConfiguration="layoutConfiguration"
        :currency="currency"
        :designMode="designMode"
        :selectedCategoryId="selectedCategoryId"
        @item-click="(item, cat) => $emit('item-click', item, cat)"
        @category-click="cat => emits('category-click', cat)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, toRef } from 'vue'
import type { MenuCategory, MenuItem } from '@/stores/restaurant'

defineOptions({
  name: 'PublicMenuCategoryTree'
})

interface CategoryLayout {
  categoryId: string
  displayOrder: number
  layout: 'list' | 'grid' | 'cards'
  cardStyle: 'modern' | 'classic' | 'minimal'
  columns?: number
  spacing?: 'compact' | 'normal' | 'relaxed'
  borderRadius?: 'none' | 'small' | 'medium' | 'large'
  imageSize?: 'small' | 'medium' | 'large'
  imageShape?: 'square' | 'rounded' | 'circle'
  showImages: boolean
  showPrices: boolean
  showDescriptions: boolean
}

interface LayoutConfiguration {
  categories: CategoryLayout[]
  globalSettings: any
}

const props = withDefaults(defineProps<{
  categories: MenuCategory[]
  settings?: {
    primaryColor?: string
    accentColor?: string
    surfaceColor?: string
    textColor?: string
    fontFamily?: string
  }
  displaySettings?: {
    showPrices?: boolean
    showImages?: boolean
    showDescriptions?: boolean
    showCategories?: boolean
  } | null
  layoutConfiguration?: LayoutConfiguration | null
  currency?: string
  designMode?: boolean
  selectedCategoryId?: string | null
}>(), {
  settings: () => ({
    primaryColor: '#dc2626',
    accentColor: '#f59e0b',
    surfaceColor: '#ffffff',
    textColor: '#1f2937',
    fontFamily: 'Inter'
  }),
  displaySettings: null,
  layoutConfiguration: null,
  currency: 'USD',
  designMode: false,
  selectedCategoryId: null
})

const emits = defineEmits<{
  (e: 'item-click', item: MenuItem, category: MenuCategory): void
  (e: 'category-click', category: MenuCategory): void
}>()

const { locale, t } = useI18n({ useScope: 'global' })
const designMode = toRef(props, 'designMode')
const selectedCategoryId = toRef(props, 'selectedCategoryId')

const handleCategoryClick = (category: MenuCategory) => {
  if (!designMode.value) return
  emits('category-click', category)
}

// Get layout configuration for a specific category
const getCategoryLayout = (categoryId: string): CategoryLayout | null => {
  if (!props.layoutConfiguration) return null
  return props.layoutConfiguration.categories.find(c => c.categoryId === categoryId) || null
}

// Get container classes based on layout type
const getContainerClasses = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  
  if (!layout) {
    return 'space-y-4' // Default list layout
  }

  const spacing = layout.spacing || 'normal'
  const spacingClass = spacing === 'compact' ? 'gap-3' : spacing === 'relaxed' ? 'gap-6' : 'gap-4'

  if (layout.layout === 'grid') {
    const cols = layout.columns || 3
    return `grid ${spacingClass} grid-cols-1 sm:grid-cols-2 lg:grid-cols-${cols}`
  } else if (layout.layout === 'cards') {
    return `grid ${spacingClass} grid-cols-1 sm:grid-cols-2 lg:grid-cols-3`
  }

  // List layout
  return spacing === 'compact' ? 'space-y-3' : spacing === 'relaxed' ? 'space-y-6' : 'space-y-4'
}

// Get item card classes based on card style
const getItemClasses = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  const borderRadius = layout?.borderRadius || 'medium'
  
  const baseClasses = [
    'group',
    'relative',
    'p-4',
    'sm:p-6',
    'transition-all',
    'cursor-pointer',
    'hover:shadow-lg'
  ]

  // Border radius
  if (borderRadius === 'none') baseClasses.push('rounded-none')
  else if (borderRadius === 'small') baseClasses.push('rounded-lg')
  else if (borderRadius === 'large') baseClasses.push('rounded-3xl')
  else baseClasses.push('rounded-2xl')

  // Card style
  const cardStyle = layout?.cardStyle || 'modern'
  
  if (cardStyle === 'modern') {
    baseClasses.push('bg-gradient-to-br', 'from-white', 'to-neutral-50', 'shadow-md', 'hover:shadow-xl', 'border', 'border-neutral-100')
  } else if (cardStyle === 'classic') {
    baseClasses.push('bg-white', 'border-2', 'border-neutral-200', 'hover:border-neutral-400')
  } else if (cardStyle === 'minimal') {
    baseClasses.push('bg-neutral-50', 'hover:bg-neutral-100', 'border', 'border-transparent')
  } else {
    // Fallback
    baseClasses.push('border-2', 'border-neutral-100', 'hover:border-neutral-300', 'bg-white')
  }

  return baseClasses.join(' ')
}

// Get image classes based on size and shape
const getImageClasses = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  const isGridOrCards = layout?.layout === 'grid' || layout?.layout === 'cards'
  
  const classes = []
  
  // Size
  const size = layout?.imageSize || 'medium'
  if (isGridOrCards) {
    // For grid/cards, image is full width
    if (size === 'small') classes.push('h-32')
    else if (size === 'large') classes.push('h-64')
    else classes.push('h-48') // medium
    classes.push('w-full')
  } else {
    // For list, fixed size
    if (size === 'small') classes.push('w-20', 'h-20')
    else if (size === 'large') classes.push('w-40', 'h-40')
    else classes.push('w-32', 'h-32') // medium
    classes.push('sm:w-32', 'sm:h-32') // Responsive
  }

  // Shape
  const shape = layout?.imageShape || 'rounded'
  if (shape === 'circle') classes.push('rounded-full')
  else if (shape === 'square') classes.push('rounded-none')
  else classes.push('rounded-xl') // rounded

  return classes.join(' ')
}

// Visibility helpers - check per-category settings first, then fall back to global
const shouldShowImage = (categoryId: string): boolean => {
  const layout = getCategoryLayout(categoryId)
  if (layout !== null) return layout.showImages
  return props.displaySettings?.showImages ?? true
}

const shouldShowPrice = (categoryId: string): boolean => {
  const layout = getCategoryLayout(categoryId)
  if (layout !== null) return layout.showPrices
  return props.displaySettings?.showPrices ?? true
}

const shouldShowDescription = (categoryId: string): boolean => {
  const layout = getCategoryLayout(categoryId)
  if (layout !== null) return layout.showDescriptions
  return props.displaySettings?.showDescriptions ?? true
}

const formatPrice = (price: number) => {
  try {
    return new Intl.NumberFormat(locale.value, {
      style: 'currency',
      currency: props.currency || 'USD',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(price)
  } catch (error) {
    return `${props.currency} ${price.toFixed(2)}`
  }
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
