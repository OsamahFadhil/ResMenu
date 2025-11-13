<template>
  <div class="min-h-screen" :style="rootStyles">
    <!-- Language Switcher -->
    <div class="absolute top-4 right-4 z-10">
      <LanguageSwitcher />
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex items-center justify-center min-h-screen">
      <div class="text-center space-y-4">
        <div class="inline-flex h-16 w-16 items-center justify-center rounded-full border-4 animate-spin"
          :style="{ borderColor: `${settings.primaryColor}40`, borderTopColor: settings.primaryColor }">
        </div>
        <p class="text-lg font-semibold" :style="{ color: settings.textColor }">
          {{ t('common.loading') }}
        </p>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="flex items-center justify-center min-h-screen px-4">
      <div class="bg-white shadow-2xl rounded-2xl px-8 py-6 text-center max-w-md">
        <svg class="w-16 h-16 mx-auto mb-4 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
        </svg>
        <p class="text-red-600 text-xl font-bold mb-2">{{ error }}</p>
        <p class="text-neutral-600">{{ t('messages.errorOccurred') }}</p>
      </div>
    </div>

    <!-- Menu Content -->
    <div v-else-if="menu" class="pb-20">
      <!-- Restaurant Header -->
      <header class="relative overflow-hidden"
        :style="{ backgroundColor: settings.surfaceColor }">
        <div class="absolute inset-0 opacity-5"
          :style="{ background: `linear-gradient(135deg, ${settings.primaryColor} 0%, ${settings.accentColor} 100%)` }">
        </div>

        <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 sm:py-16">
          <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-8">
            <!-- Restaurant Info -->
            <div class="flex items-start gap-6">
              <div v-if="menu.logoUrl" class="flex-shrink-0">
                <img
                  :src="menu.logoUrl"
                  :alt="menu.restaurantLocalizedName"
                  class="w-24 h-24 sm:w-32 sm:h-32 rounded-2xl object-cover shadow-xl ring-4 ring-white/50"
                />
              </div>
              <div class="space-y-3">
                <h1 class="text-4xl sm:text-5xl font-bold tracking-tight"
                  :style="{ color: settings.textColor }">
                  {{ menu.restaurantLocalizedName || menu.restaurantName }}
                </h1>
                <div class="flex flex-wrap items-center gap-3 text-sm font-medium"
                  :style="{ color: settings.textColor, opacity: 0.7 }">
                  <span class="px-3 py-1 rounded-full"
                    :style="{ backgroundColor: `${settings.primaryColor}20`, color: settings.primaryColor }">
                    {{ menu.language.toUpperCase() }}
                  </span>
                  <span>•</span>
                  <span>{{ totalItems }} {{ totalItems === 1 ? 'Item' : 'Items' }}</span>
                </div>
              </div>
            </div>

            <!-- Contact Info -->
            <div v-if="menu.contactPhone || menu.contactEmail || menu.address"
              class="space-y-3 text-sm lg:text-right">
              <div v-if="menu.contactPhone" class="flex items-center gap-2 lg:justify-end">
                <svg class="w-5 h-5" :style="{ color: settings.primaryColor }" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M2 3a1 1 0 011-1h2.153a1 1 0 01.986.836l.74 4.435a1 1 0 01-.54 1.06l-1.548.773a11.037 11.037 0 006.105 6.105l.774-1.548a1 1 0 011.059-.54l4.435.74a1 1 0 01.836.986V17a1 1 0 01-1 1h-2C7.82 18 2 12.18 2 5V3z" />
                </svg>
                <span class="font-semibold" :style="{ color: settings.textColor }">{{ menu.contactPhone }}</span>
              </div>
              <div v-if="menu.contactEmail" class="flex items-center gap-2 lg:justify-end">
                <svg class="w-5 h-5" :style="{ color: settings.primaryColor }" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M2.003 5.884L10 9.882l7.997-3.998A2 2 0 0016 4H4a2 2 0 00-1.997 1.884z" />
                  <path d="M18 8.118l-8 4-8-4V14a2 2 0 002 2h12a2 2 0 002-2V8.118z" />
                </svg>
                <a :href="`mailto:${menu.contactEmail}`" class="font-semibold hover:underline"
                  :style="{ color: settings.textColor }">
                  {{ menu.contactEmail }}
                </a>
              </div>
              <div v-if="menu.address" class="flex items-center gap-2 lg:justify-end">
                <svg class="w-5 h-5" :style="{ color: settings.primaryColor }" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clip-rule="evenodd" />
                </svg>
                <span :style="{ color: settings.textColor }">{{ menu.address }}</span>
              </div>
            </div>
          </div>
        </div>
      </header>

      <!-- Search and Filter Bar -->
      <div v-if="displaySettings.enableSearch || displaySettings.enableFilters"
        class="sticky top-0 z-30 backdrop-blur-xl shadow-lg"
        :style="{ backgroundColor: `${settings.surfaceColor}f5` }">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div class="space-y-4">
            <!-- Search Bar -->
            <div v-if="displaySettings.enableSearch" class="relative max-w-2xl">
              <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                <svg class="h-6 w-6" :style="{ color: `${settings.textColor}60` }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
              </div>
              <input
                v-model="searchQuery"
                type="text"
                :placeholder="t('menu.searchPlaceholder') || 'Search menu items...'"
                class="w-full pl-14 pr-12 py-4 text-lg rounded-2xl border-2 transition-all focus:outline-none focus:ring-4 font-medium"
                :style="{
                  borderColor: searchQuery ? settings.primaryColor : '#e5e7eb',
                  color: settings.textColor,
                  backgroundColor: settings.surfaceColor,
                  '--tw-ring-color': `${settings.primaryColor}40`
                }"
              />
              <button
                v-if="searchQuery"
                @click="searchQuery = ''"
                class="absolute inset-y-0 right-0 pr-4 flex items-center hover:scale-110 transition-transform"
              >
                <svg class="h-6 w-6 text-neutral-400 hover:text-neutral-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>

            <!-- Category Filters -->
            <div v-if="displaySettings.enableFilters && allCategories.length > 0" class="flex items-center gap-3 overflow-x-auto pb-2 hide-scrollbar">
              <button
                @click="selectedCategoryId = null"
                :class="[
                  'px-5 py-2.5 rounded-full text-sm font-bold whitespace-nowrap transition-all flex items-center gap-2',
                  selectedCategoryId === null ? 'shadow-lg scale-105' : 'hover:scale-105'
                ]"
                :style="selectedCategoryId === null
                  ? { backgroundColor: settings.primaryColor, color: '#ffffff' }
                  : { backgroundColor: `${settings.primaryColor}20`, color: settings.primaryColor }
                "
              >
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z" />
                  <path fill-rule="evenodd" d="M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm3 4a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3zm-3 4a1 1 0 100 2h.01a1 1 0 100-2H7zm3 0a1 1 0 100 2h3a1 1 0 100-2h-3z" clip-rule="evenodd" />
                </svg>
                All
                <span class="opacity-75">({{ totalItems }})</span>
              </button>

              <button
                v-for="category in allCategories"
                :key="category.id"
                @click="scrollToCategory(category.id)"
                :class="[
                  'px-5 py-2.5 rounded-full text-sm font-bold whitespace-nowrap transition-all',
                  selectedCategoryId === category.id ? 'shadow-lg scale-105' : 'hover:scale-105'
                ]"
                :style="selectedCategoryId === category.id
                  ? { backgroundColor: settings.primaryColor, color: '#ffffff' }
                  : { backgroundColor: `${settings.primaryColor}20`, color: settings.primaryColor }
                "
              >
                {{ category.localizedName || category.name }}
                <span class="ml-1.5 opacity-75">({{ category.items.length }})</span>
              </button>
            </div>

            <!-- Filter Info -->
            <div v-if="searchQuery || selectedCategoryId" class="flex items-center justify-between text-sm font-medium">
              <span :style="{ color: settings.textColor }">
                <span class="font-bold text-lg" :style="{ color: settings.primaryColor }">{{ filteredItemsCount }}</span>
                {{ filteredItemsCount === 1 ? 'item' : 'items' }} found
              </span>
              <button
                @click="clearFilters"
                class="font-bold hover:underline transition-all hover:scale-105"
                :style="{ color: settings.primaryColor }"
              >
                Clear filters ✕
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Menu Categories -->
      <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <PublicMenuCategoryTree
          :categories="filteredCategories"
          :settings="settings"
          :displaySettings="displaySettings"
          :currency="menu.currency"
          @item-click="openItemDetail"
        />

        <!-- No Results -->
        <div v-if="filteredCategories.length === 0 && (searchQuery || selectedCategoryId)"
          class="text-center py-20">
          <svg class="w-24 h-24 mx-auto mb-6 opacity-30" :style="{ color: settings.primaryColor }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <h3 class="text-2xl font-bold mb-2" :style="{ color: settings.textColor }">
            {{ t('menu.noResults') || 'No items found' }}
          </h3>
          <p class="text-lg opacity-60" :style="{ color: settings.textColor }">
            {{ t('menu.tryDifferentSearch') || 'Try a different search or filter' }}
          </p>
        </div>

        <!-- Empty Menu -->
        <div v-else-if="menu.categories.length === 0" class="text-center py-20">
          <svg class="w-24 h-24 mx-auto mb-6 opacity-30" :style="{ color: settings.primaryColor }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <h3 class="text-2xl font-bold" :style="{ color: settings.textColor }">
            {{ t('menu.comingSoon') || 'Menu coming soon' }}
          </h3>
        </div>
      </main>

      <!-- Scroll to Top Button -->
      <Transition name="fade-scale">
        <button
          v-if="showScrollTop"
          @click="scrollToTop"
          class="fixed bottom-8 right-8 p-4 rounded-full shadow-2xl transition-all hover:scale-110 z-40"
          :style="{ backgroundColor: settings.primaryColor, color: '#ffffff' }"
          aria-label="Scroll to top"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 10l7-7m0 0l7 7m-7-7v18" />
          </svg>
        </button>
      </Transition>
    </div>

    <!-- Item Detail Modal -->
    <ItemDetailModal
      v-if="selectedItem"
      :show="showItemDetail"
      :item="selectedItem"
      :category-name="selectedItemCategory?.localizedName || selectedItemCategory?.name"
      :settings="settings"
      :display-price="displaySettings.showPrices"
      :currency="menu?.currency"
      @close="closeItemDetail"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, onBeforeUnmount } from 'vue'
import PublicMenuCategoryTree from '@/components/PublicMenuCategoryTree.vue'
import ItemDetailModal from '@/components/menu/ItemDetailModal.vue'
import type { MenuCategory, MenuItem } from '~/stores/restaurant'

const route = useRoute()
const restaurantStore = useRestaurantStore()
const { t, locale } = useI18n()

const menu = ref<any>(null)
const loading = ref(true)
const error = ref('')

// Search and filter state
const searchQuery = ref('')
const selectedCategoryId = ref<string | null>(null)
const showScrollTop = ref(false)

// Modal state
const showItemDetail = ref(false)
const selectedItem = ref<MenuItem | null>(null)
const selectedItemCategory = ref<MenuCategory | null>(null)

// Settings from API (replacing theme)
const settings = computed(() => ({
  primaryColor: menu.value?.theme?.primaryColor || '#dc2626',
  accentColor: menu.value?.theme?.accentColor || '#f59e0b',
  backgroundColor: menu.value?.theme?.backgroundColor || '#fafaf9',
  surfaceColor: menu.value?.theme?.surfaceColor || '#ffffff',
  textColor: menu.value?.theme?.textColor || '#1f2937',
  fontFamily: menu.value?.theme?.fontFamily || 'Inter'
}))

const displaySettings = computed(() => menu.value?.displaySettings || {
  showPrices: true,
  showImages: true,
  showDescriptions: true,
  showCategories: true,
  enableSearch: true,
  enableFilters: true
})

// Flatten categories
const allCategories = computed(() => {
  const flatten = (categories: MenuCategory[]): MenuCategory[] => {
    let result: MenuCategory[] = []
    for (const category of categories) {
      result.push(category)
      if (category.children?.length > 0) {
        result = result.concat(flatten(category.children))
      }
    }
    return result
  }
  return menu.value?.categories ? flatten(menu.value.categories) : []
})

// Filter categories
const filteredCategories = computed(() => {
  if (!menu.value?.categories) return []

  let categories = [...menu.value.categories]

  if (selectedCategoryId.value) {
    const selected = allCategories.value.find(c => c.id === selectedCategoryId.value)
    if (selected) categories = [selected]
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    categories = categories
      .map(category => filterCategoryBySearch(category, query))
      .filter(c => c !== null) as MenuCategory[]
  }

  return categories
})

const filterCategoryBySearch = (category: MenuCategory, query: string): MenuCategory | null => {
  const matchingItems = category.items.filter(item =>
    (item.localizedName || item.name).toLowerCase().includes(query) ||
    (item.localizedDescription || item.description || '').toLowerCase().includes(query)
  )

  const matchingChildren = category.children
    .map(child => filterCategoryBySearch(child, query))
    .filter(c => c !== null) as MenuCategory[]

  if (matchingItems.length > 0 || matchingChildren.length > 0 ||
      (category.localizedName || category.name).toLowerCase().includes(query)) {
    return { ...category, items: matchingItems, children: matchingChildren }
  }

  return null
}

const filteredItemsCount = computed(() => {
  const countItems = (categories: MenuCategory[]): number => {
    return categories.reduce((sum, category) => {
      return sum + category.items.length + countItems(category.children)
    }, 0)
  }
  return countItems(filteredCategories.value)
})

const totalItems = computed(() => {
  return allCategories.value.reduce((sum, category) => sum + category.items.length, 0)
})

// Modal handlers
const openItemDetail = (item: MenuItem, category: MenuCategory) => {
  selectedItem.value = item
  selectedItemCategory.value = category
  showItemDetail.value = true
  document.body.style.overflow = 'hidden'
}

const closeItemDetail = () => {
  showItemDetail.value = false
  selectedItem.value = null
  selectedItemCategory.value = null
  document.body.style.overflow = ''
}

// Filter handlers
const clearFilters = () => {
  searchQuery.value = ''
  selectedCategoryId.value = null
}

const scrollToCategory = (categoryId: string) => {
  selectedCategoryId.value = categoryId
  setTimeout(() => window.scrollTo({ top: 0, behavior: 'smooth' }), 100)
}

const scrollToTop = () => {
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

const handleScroll = () => {
  showScrollTop.value = window.scrollY > 400
}

onMounted(async () => {
  try {
    const slug = route.params.slug as string
    menu.value = await restaurantStore.fetchPublicMenu(slug, locale.value)
    window.addEventListener('scroll', handleScroll)
  } catch (err: any) {
    error.value = err.message || t('messages.errorOccurred')
  } finally {
    loading.value = false
  }
})

onBeforeUnmount(() => {
  window.removeEventListener('scroll', handleScroll)
  document.body.style.overflow = ''
})

watch(() => locale.value, async (newLocale) => {
  if (!menu.value) return
  const slug = route.params.slug as string
  loading.value = true
  try {
    menu.value = await restaurantStore.fetchPublicMenu(slug, newLocale)
  } catch (err: any) {
    error.value = err.message || t('messages.errorOccurred')
  } finally {
    loading.value = false
  }
})

const rootStyles = computed(() => ({
  '--primary-color': settings.value.primaryColor,
  '--accent-color': settings.value.accentColor,
  backgroundColor: settings.value.backgroundColor,
  color: settings.value.textColor,
  fontFamily: settings.value.fontFamily,
  minHeight: '100vh'
}))
</script>

<style scoped>
.fade-scale-enter-active,
.fade-scale-leave-active {
  transition: opacity 0.3s ease, transform 0.3s ease;
}

.fade-scale-enter-from,
.fade-scale-leave-to {
  opacity: 0;
  transform: scale(0.9) translateY(20px);
}

.hide-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

.hide-scrollbar::-webkit-scrollbar {
  display: none;
}
</style>
