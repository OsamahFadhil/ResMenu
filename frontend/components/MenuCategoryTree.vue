<template>
  <div v-for="category in categories" :key="category.id" class="border rounded-lg p-4 bg-white">
    <div class="flex items-start justify-between">
      <div>
        <h5 class="text-lg font-semibold text-gray-900">{{ category.localizedName || category.name }}</h5>
        <p class="text-sm text-gray-500">Display Order: {{ category.displayOrder }}</p>
      </div>
      <div class="flex flex-wrap items-center gap-2">
        <button
          @click="onAddSubcategory(category.id)"
          class="inline-flex items-center gap-1 px-3 py-1.5 text-sm text-green-600 hover:bg-green-50 rounded-lg font-medium transition"
        >
          <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          {{ t('menu.addCategory') }}
        </button>
        <button
          @click="onAddItem ? onAddItem(category.id) : onToggleItemForm(category.id)"
          class="inline-flex items-center gap-1 px-3 py-1.5 text-sm text-indigo-600 hover:bg-indigo-50 rounded-lg font-medium transition"
        >
          <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          {{ t('menu.addItem') }}
        </button>
        <button
          @click="onEditCategory(category)"
          class="inline-flex items-center gap-1 px-3 py-1.5 text-sm text-slate-600 hover:bg-slate-100 rounded-lg font-medium transition"
        >
          <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
          </svg>
          {{ t('common.edit') }}
        </button>
        <button
          @click="onDeleteCategory(category)"
          class="inline-flex items-center gap-1 px-3 py-1.5 text-sm text-red-600 hover:bg-red-50 rounded-lg font-medium transition"
        >
          <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
          {{ t('common.delete') }}
        </button>
      </div>
    </div>

    <div v-if="category.items.length" class="mt-3 space-y-2">
      <div
        v-for="item in category.items"
        :key="item.id"
        class="flex items-center justify-between bg-gray-50 rounded-md px-3 py-2 text-sm text-gray-700"
      >
        <div>
          <p class="font-medium">{{ item.localizedName || item.name }}</p>
          <p v-if="item.localizedDescription || item.description" class="text-xs text-gray-500">
            {{ item.localizedDescription || item.description }}
          </p>
        </div>
        <div class="text-right">
          <p class="font-semibold">{{ formatPrice(item.price) }}</p>
          <p class="text-xs text-gray-500">Order: {{ item.displayOrder }}</p>
        </div>
      </div>
    </div>
    <div v-else class="mt-3 text-sm text-gray-500">
      {{ t('menu.emptyCategoryMessage') }}
    </div>

    <div v-if="showItemForms[category.id]" class="mt-4 border-t pt-4">
      <h6 class="font-medium text-gray-900 mb-2">{{ t('menu.addItem') }}</h6>
      <div class="space-y-3">
        <div>
          <label class="block text-sm font-medium text-gray-700">{{ t('menu.itemName') }}</label>
          <input
            v-model="itemForms[category.id].name"
            type="text"
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700">{{ t('menu.description') }}</label>
          <textarea
            v-model="itemForms[category.id].description"
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
            rows="2"
          ></textarea>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
          <div>
            <label class="block text-sm font-medium text-gray-700">{{ t('menu.price') }}</label>
            <input
              v-model.number="itemForms[category.id].price"
              type="number"
              step="0.01"
              min="0"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700">{{ t('menu.sortOrder') }}</label>
            <input
              v-model.number="itemForms[category.id].displayOrder"
              type="number"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
            />
          </div>
          <div class="flex items-center mt-6">
            <input
              v-model="itemForms[category.id].isAvailable"
              type="checkbox"
              :id="'available-' + category.id"
              class="h-4 w-4 text-indigo-600 border-gray-300 rounded"
            />
            <label :for="'available-' + category.id" class="ml-2 text-sm text-gray-700">{{ t('menu.isAvailable') }}</label>
          </div>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700">{{ t('menu.imageUrl') }}</label>
          <input
            v-model="itemForms[category.id].imageUrl"
            type="text"
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>
        <button
          @click="onSaveItem(category.id)"
          class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm"
        >
          {{ t('common.save') }}
        </button>
      </div>
    </div>

    <div v-if="category.children.length" class="mt-4 space-y-3 border-l-2 border-gray-200 pl-4">
      <MenuCategoryTree
        :categories="category.children"
        :showItemForms="showItemForms"
        :itemForms="itemForms"
        :onToggleItemForm="onToggleItemForm"
        :onSaveItem="onSaveItem"
        :onAddSubcategory="onAddSubcategory"
        :onEditCategory="onEditCategory"
        :onDeleteCategory="onDeleteCategory"
        :onAddItem="onAddItem"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { MenuCategory } from '@/stores/restaurant';

defineOptions({
  name: 'MenuCategoryTree',
});

interface MenuItemForm {
  name: string;
  description?: string | null;
  price: number;
  imageUrl?: string | null;
  isAvailable: boolean;
  displayOrder: number;
}

defineProps<{
  categories: MenuCategory[];
  showItemForms: Record<string, boolean>;
  itemForms: Record<string, MenuItemForm>;
  onToggleItemForm: (categoryId: string) => void;
  onSaveItem: (categoryId: string) => Promise<void>;
  onAddSubcategory: (categoryId: string) => void;
  onEditCategory: (category: MenuCategory) => void;
  onDeleteCategory: (category: MenuCategory) => void;
  onAddItem?: (categoryId: string) => void;
}>();

const { t, locale } = useI18n({ useScope: 'global' });
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

