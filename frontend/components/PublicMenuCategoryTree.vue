<template>
  <div
    v-for="category in categories"
    :key="category.id"
    class="mb-12 sm:mb-16"
    :class="designMode ? 'border-2 border-transparent' : ''"
    :style="{
      boxShadow: designMode && selectedCategoryId === category.id ? `0 0 0 3px ${(settings?.primaryColor || '#dc2626')}33` : undefined
    }"
  >
    <!-- Category Header - Menu Style -->
    <div
      v-if="displaySettings?.showCategories !== false"
      class="mb-8 sm:mb-10 text-center"
      :class="designMode ? 'cursor-pointer select-none' : ''"
      @click.stop="handleCategoryClick(category)"
    >
      <h2
        class="text-3xl sm:text-4xl md:text-5xl font-serif font-bold tracking-wide mb-2"
        :style="{
          color: settings?.primaryColor || '#dc2626',
          fontFamily: settings?.fontFamily || 'Playfair Display, serif',
          letterSpacing: '0.05em'
        }"
      >
        {{ category.localizedName || category.name }}
      </h2>
      <div 
        class="mx-auto w-24 h-0.5 sm:w-32"
        :style="{ backgroundColor: settings?.primaryColor || '#dc2626' }"
      ></div>
    </div>

    <!-- Items Container - DYNAMIC LAYOUT -->
    <div 
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
          <div class="space-y-3 h-full">
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
          <!-- List Layout: Classic Menu Style -->
          <div class="menu-item-container">
            <!-- Item Image (if shown) -->
            <div
              v-if="shouldShowImage(category.id) && (item.imageUrl || (item.images && item.images.length > 0))"
              class="menu-item-image"
            >
              <div class="relative overflow-hidden bg-neutral-100" :class="getImageClasses(category.id)">
                <img
                  :src="item.images?.[0] || item.imageUrl"
                  :alt="item.localizedName || item.name"
                  class="w-full h-full object-cover"
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

            <!-- Item Content - Classic Menu Layout -->
            <div class="menu-item-content">
              <!-- Name and Price Row -->
              <div class="menu-item-header">
                <h3
                  class="menu-item-name"
                  :style="{ 
                    color: settings?.textColor || '#1f2937',
                    fontFamily: settings?.fontFamily || 'Playfair Display, serif'
                  }"
                >
                  {{ item.localizedName || item.name }}
                </h3>
                <div class="menu-item-dots"></div>
                <span
                  v-if="shouldShowPrice(category.id)"
                  class="menu-item-price"
                  :style="{ color: settings?.primaryColor || '#dc2626' }"
                >
                  {{ formatPrice(item.price) }}
                </span>
              </div>

              <!-- Description -->
              <p
                v-if="shouldShowDescription(category.id) && (item.localizedDescription || item.description)"
                class="menu-item-description"
                :style="{ 
                  color: settings?.textColor || '#374151',
                  fontFamily: settings?.fontFamily || 'Inter, sans-serif'
                }"
              >
                {{ item.localizedDescription || item.description }}
              </p>
            </div>
          </div>
        </template>

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
  return spacing === 'compact' ? 'space-y-4 sm:space-y-5' : spacing === 'relaxed' ? 'space-y-8 sm:space-y-10' : 'space-y-6 sm:space-y-8'
}

// Get item card classes based on card style
const getItemClasses = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  
  // For list layout, use menu-style classes
  if (!layout || layout.layout === 'list') {
    return 'menu-item'
  }
  
  const borderRadius = layout?.borderRadius || 'medium'
  const baseClasses = [
    'group',
    'relative',
    'p-4',
    'sm:p-6',
    'transition-all',
    'cursor-pointer'
  ]

  // Border radius
  if (borderRadius === 'none') baseClasses.push('rounded-none')
  else if (borderRadius === 'small') baseClasses.push('rounded-lg')
  else if (borderRadius === 'large') baseClasses.push('rounded-2xl')
  else baseClasses.push('rounded-xl')

  // Card style
  const cardStyle = layout?.cardStyle || 'modern'
  
  if (cardStyle === 'modern') {
    baseClasses.push('bg-white', 'shadow-sm', 'hover:shadow-md', 'border', 'border-neutral-100')
  } else if (cardStyle === 'classic') {
    baseClasses.push('bg-white', 'border', 'border-neutral-200')
  } else if (cardStyle === 'minimal') {
    baseClasses.push('bg-transparent', 'border-b', 'border-neutral-200')
  } else {
    baseClasses.push('bg-white', 'border', 'border-neutral-100')
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

/* Classic Menu Styling */
.menu-item {
  position: relative;
  padding: 0;
  margin-bottom: 1.5rem;
  cursor: pointer;
  transition: opacity 0.2s ease;
}

.menu-item:hover {
  opacity: 0.9;
}

.menu-item-container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

@media (min-width: 640px) {
  .menu-item-container {
    flex-direction: row;
    gap: 1.5rem;
  }
}

.menu-item-image {
  flex-shrink: 0;
}

.menu-item-content {
  flex: 1;
  min-width: 0;
}

.menu-item-header {
  display: flex;
  align-items: baseline;
  gap: 0.75rem;
  margin-bottom: 0.5rem;
  flex-wrap: wrap;
}

.menu-item-name {
  font-size: 1.25rem;
  font-weight: 600;
  line-height: 1.4;
  flex: 1;
  min-width: 0;
}

@media (min-width: 640px) {
  .menu-item-name {
    font-size: 1.5rem;
  }
}

.menu-item-dots {
  flex: 1;
  min-width: 0.5rem;
  height: 1px;
  background-image: radial-gradient(circle, currentColor 1px, transparent 1px);
  background-size: 0.5rem 1px;
  background-position: 0 center;
  background-repeat: repeat-x;
  opacity: 0.3;
  margin: 0 0.5rem;
  align-self: flex-end;
  margin-bottom: 0.4rem;
}

.menu-item-price {
  font-size: 1.25rem;
  font-weight: 700;
  white-space: nowrap;
  font-family: 'Inter', sans-serif;
}

@media (min-width: 640px) {
  .menu-item-price {
    font-size: 1.5rem;
  }
}

.menu-item-description {
  font-size: 0.9375rem;
  line-height: 1.6;
  color: #6b7280;
  margin-top: 0.25rem;
  font-weight: 400;
}

@media (min-width: 640px) {
  .menu-item-description {
    font-size: 1rem;
  }
}

</style>
