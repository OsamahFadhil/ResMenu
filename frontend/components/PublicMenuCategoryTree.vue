<template>
  <div v-for="category in categories" :key="category.id" class="bg-white rounded-lg shadow-lg p-6 mb-6">
    <h2 class="text-2xl font-bold text-gray-900 mb-4 border-b pb-2">{{ category.localizedName || category.name }}</h2>

    <div class="space-y-4">
      <div
        v-for="item in category.items"
        :key="item.id"
        class="flex items-start gap-4 p-4 hover:bg-gray-50 rounded-lg transition"
      >
        <div v-if="item.imageUrl" class="flex-shrink-0">
          <img :src="item.imageUrl" :alt="item.localizedName || item.name" class="h-20 w-20 rounded-lg object-cover" />
        </div>
        <div class="flex-1 space-y-2">
          <div class="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-2">
            <h3 class="text-lg font-semibold text-gray-900">{{ item.localizedName || item.name }}</h3>
            <span class="text-lg font-bold text-indigo-600 whitespace-nowrap">
              {{ formatPrice(item.price) }}
            </span>
          </div>
          <p v-if="item.localizedDescription || item.description" class="text-sm text-gray-600 leading-relaxed">
            {{ item.localizedDescription || item.description }}
          </p>
        </div>
      </div>

      <div v-if="category.items.length === 0" class="text-center text-gray-500 py-4 italic">
        {{ t('menu.emptyCategoryMessage') }}
      </div>
    </div>

    <div v-if="category.children.length" class="mt-6 space-y-4">
      <PublicMenuCategoryTree :categories="category.children" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { MenuCategory } from '@/stores/restaurant';

defineOptions({
  name: 'PublicMenuCategoryTree',
});

defineProps<{
  categories: MenuCategory[];
}>();

const { locale, t } = useI18n({ useScope: 'global' });
const runtimeConfig = useRuntimeConfig();
const defaultCurrency = computed(() => (runtimeConfig.public.defaultCurrency as string) || 'USD');

const formatPrice = (price: number) => {
  try {
    return new Intl.NumberFormat(locale.value, {
      style: 'currency',
      currency: defaultCurrency.value,
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(price);
  } catch (error) {
    return price.toFixed(2);
  }
};
</script>
