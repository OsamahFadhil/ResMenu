<template>
  <div v-for="category in categories" :key="category.id" class="border rounded-lg p-4 bg-white">
    <div class="flex items-start justify-between">
      <div>
        <h5 class="text-lg font-semibold text-gray-900">{{ category.name }}</h5>
        <p class="text-sm text-gray-500">Display Order: {{ category.displayOrder }}</p>
      </div>
      <div class="flex items-center space-x-3">
        <button
          @click="onAddSubcategory(category.id)"
          class="text-sm text-green-600 hover:text-green-700 font-medium"
        >
          Add Subcategory
        </button>
        <button
          @click="() => onToggleItemForm(category.id)"
          class="text-sm text-indigo-600 hover:text-indigo-700 font-medium"
        >
          {{ showItemForms[category.id] ? 'Cancel Item' : 'Add Item' }}
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
          <p class="font-medium">{{ item.name }}</p>
          <p v-if="item.description" class="text-xs text-gray-500">{{ item.description }}</p>
        </div>
        <div class="text-right">
          <p class="font-semibold">${{ item.price.toFixed(2) }}</p>
          <p class="text-xs text-gray-500">Order: {{ item.displayOrder }}</p>
        </div>
      </div>
    </div>
    <div v-else class="mt-3 text-sm text-gray-500">
      No items yet in this category.
    </div>

    <div v-if="showItemForms[category.id]" class="mt-4 border-t pt-4">
      <h6 class="font-medium text-gray-900 mb-2">New Item</h6>
      <div class="space-y-3">
        <div>
          <label class="block text-sm font-medium text-gray-700">Item Name</label>
          <input
            v-model="itemForms[category.id].name"
            type="text"
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700">Description</label>
          <textarea
            v-model="itemForms[category.id].description"
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
            rows="2"
          ></textarea>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
          <div>
            <label class="block text-sm font-medium text-gray-700">Price</label>
            <input
              v-model.number="itemForms[category.id].price"
              type="number"
              step="0.01"
              min="0"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700">Display Order</label>
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
            <label :for="'available-' + category.id" class="ml-2 text-sm text-gray-700">Available</label>
          </div>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700">Image URL</label>
          <input
            v-model="itemForms[category.id].imageUrl"
            type="text"
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>
        <button
          @click="() => onSaveItem(category.id)"
          class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm"
        >
          Save Item
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
      />
    </div>
  </div>
</template>

<script setup lang="ts">
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
}>();
</script>

