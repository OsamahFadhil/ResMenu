<template>
  <div
    v-for="category in categories"
    :key="category.id"
    class="mb-8 rounded-3xl shadow-xl overflow-hidden transition-all hover:shadow-2xl"
    :style="{ backgroundColor: settings?.surfaceColor || '#ffffff' }"
  >
    <!-- Category Header -->
    <div
      v-if="displaySettings?.showCategories !== false"
      class="px-6 sm:px-8 py-6 border-b-4"
      :style="{
        borderColor: settings?.primaryColor || '#dc2626',
        backgroundColor: `${settings?.primaryColor || '#dc2626'}10`
      }"
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

    <!-- Items Container -->
    <div class="p-4 sm:p-6 space-y-4">
      <div
        v-for="item in category.items.filter(i => i.isAvailable)"
        :key="item.id"
        class="group relative rounded-2xl p-4 sm:p-6 border-2 border-neutral-100 hover:border-neutral-300 transition-all cursor-pointer hover:shadow-lg bg-white"
        @click="$emit('item-click', item, category)"
      >
        <div class="flex flex-col sm:flex-row gap-4 sm:gap-6">
          <!-- Item Image -->
          <div
            v-if="displaySettings?.showImages && (item.imageUrl || (item.images && item.images.length > 0))"
            class="flex-shrink-0"
          >
            <div class="relative rounded-xl overflow-hidden w-full sm:w-32 h-32 bg-neutral-100">
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
                v-if="displaySettings?.showPrices"
                class="text-xl sm:text-2xl font-bold whitespace-nowrap"
                :style="{ color: settings?.primaryColor || '#dc2626' }"
              >
                {{ formatPrice(item.price) }}
              </span>
            </div>

            <p
              v-if="displaySettings?.showDescriptions && (item.localizedDescription || item.description)"
              class="text-base sm:text-lg leading-relaxed line-clamp-2"
              :style="{ color: settings?.textColor || '#374151', opacity: 0.8 }"
            >
              {{ item.localizedDescription || item.description }}
            </p>
          </div>

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
      </div>

      <!-- Empty Category -->
      <div
        v-if="category.items.filter(i => i.isAvailable).length === 0"
        class="text-center py-12 text-neutral-500"
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
        :currency="currency"
        @item-click="(item, cat) => $emit('item-click', item, cat)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { MenuCategory, MenuItem } from '@/stores/restaurant'

defineOptions({
  name: 'PublicMenuCategoryTree'
})

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
  currency?: string
}>(), {
  settings: () => ({
    primaryColor: '#dc2626',
    accentColor: '#f59e0b',
    surfaceColor: '#ffffff',
    textColor: '#1f2937',
    fontFamily: 'Inter'
  }),
  displaySettings: null,
  currency: 'USD'
})

defineEmits<{
  (e: 'item-click', item: MenuItem, category: MenuCategory): void
}>()

const { locale, t } = useI18n({ useScope: 'global' })

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
