<template>
  <div v-for="category in categories" :key="category.id" class="bg-white rounded-lg shadow-lg p-6 mb-6">
    <h2 class="text-2xl font-bold text-gray-900 mb-4 border-b pb-2">{{ category.name }}</h2>

    <div class="space-y-4">
      <div
        v-for="item in category.items"
        :key="item.id"
        class="flex items-start space-x-4 p-4 hover:bg-gray-50 rounded-lg transition"
      >
        <div v-if="item.imageUrl" class="flex-shrink-0">
          <img :src="item.imageUrl" :alt="item.name" class="h-20 w-20 rounded-lg object-cover" />
        </div>
        <div class="flex-1">
          <div class="flex justify-between items-start">
            <h3 class="text-lg font-semibold text-gray-900">{{ item.name }}</h3>
            <span class="text-lg font-bold text-indigo-600">${{ item.price.toFixed(2) }}</span>
          </div>
          <p v-if="item.description" class="mt-1 text-sm text-gray-600">{{ item.description }}</p>
        </div>
      </div>

      <div v-if="category.items.length === 0" class="text-center text-gray-500 py-4">
        No items in this category yet
      </div>
    </div>

    <div v-if="category.children.length" class="mt-6 space-y-4">
      <PublicMenuCategoryTree :categories="category.children" />
    </div>
  </div>
</template>

<script setup lang="ts">
import type { MenuCategory } from '@/stores/restaurant';

defineOptions({
  name: 'PublicMenuCategoryTree',
});

defineProps<{
  categories: MenuCategory[];
}>();
</script>

