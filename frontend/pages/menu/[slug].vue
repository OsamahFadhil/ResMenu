<template>
  <div class="min-h-screen bg-gray-50">
    <div v-if="loading" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto"></div>
        <p class="mt-4 text-gray-600">Loading menu...</p>
      </div>
    </div>

    <div v-else-if="error" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <p class="text-red-600 text-xl">{{ error }}</p>
      </div>
    </div>

    <div v-else-if="menu" class="max-w-4xl mx-auto px-4 py-8">
      <!-- Restaurant Header -->
      <div class="bg-white rounded-lg shadow-lg p-6 mb-6">
        <div class="flex items-center space-x-4">
          <div v-if="menu.logoUrl" class="flex-shrink-0">
            <img :src="menu.logoUrl" :alt="menu.restaurantName" class="h-16 w-16 rounded-full object-cover" />
          </div>
          <div>
            <h1 class="text-3xl font-bold text-gray-900">{{ menu.restaurantName }}</h1>
            <div class="mt-2 space-y-1 text-sm text-gray-600">
              <p v-if="menu.contactPhone" class="flex items-center">
                <span class="mr-2">📞</span>{{ menu.contactPhone }}
              </p>
              <p v-if="menu.contactEmail" class="flex items-center">
                <span class="mr-2">✉️</span>{{ menu.contactEmail }}
              </p>
              <p v-if="menu.address" class="flex items-center">
                <span class="mr-2">📍</span>{{ menu.address }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <PublicMenuCategoryTree :categories="menu.categories" />

      <div v-if="menu.categories.length === 0" class="bg-white rounded-lg shadow-lg p-12 text-center">
        <p class="text-gray-500 text-lg">Menu coming soon...</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import PublicMenuCategoryTree from '@/components/PublicMenuCategoryTree.vue';

const route = useRoute();
const restaurantStore = useRestaurantStore();

const menu = ref<any>(null);
const loading = ref(true);
const error = ref('');

onMounted(async () => {
  try {
    const slug = route.params.slug as string;
    menu.value = await restaurantStore.fetchPublicMenu(slug);
  } catch (err: any) {
    error.value = err.message || 'Failed to load menu';
  } finally {
    loading.value = false;
  }
});
</script>
