<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 to-indigo-50">
    <div class="absolute top-4 right-4 z-10">
      <LanguageSwitcher />
    </div>

    <div v-if="loading" class="flex items-center justify-center min-h-screen">
      <div class="text-center space-y-4">
        <div class="inline-flex h-14 w-14 items-center justify-center rounded-full border-4 border-indigo-200 border-t-indigo-600 animate-spin"></div>
        <p class="text-gray-600 font-medium">{{ t('common.loading') }}</p>
      </div>
    </div>

    <div v-else-if="error" class="flex items-center justify-center min-h-screen">
      <div class="bg-white shadow-md rounded-xl px-8 py-6 text-center max-w-md">
        <p class="text-red-600 text-lg font-semibold">{{ error }}</p>
        <p class="mt-2 text-sm text-gray-500">{{ t('messages.errorOccurred') }}</p>
      </div>
    </div>

    <div v-else-if="menu" class="max-w-4xl mx-auto px-4 py-12 space-y-8">
      <!-- Restaurant Header -->
      <div class="bg-white rounded-xl shadow-xl p-8 border border-slate-100">
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6">
          <div class="flex items-center gap-4">
            <div v-if="menu.logoUrl" class="flex-shrink-0">
              <img :src="menu.logoUrl" :alt="menu.restaurantLocalizedName" class="h-20 w-20 rounded-full border border-indigo-100 object-cover" />
            </div>
            <div class="space-y-1">
              <h1 class="text-3xl font-bold text-gray-900 tracking-tight">
                {{ menu.restaurantLocalizedName || menu.restaurantName }}
              </h1>
              <p class="text-sm text-gray-500 uppercase tracking-wide">
                {{ t('dashboard.analytics') }} · {{ menu.language.toUpperCase() }}
              </p>
            </div>
          </div>

          <div class="space-y-3 text-sm text-gray-600">
            <div v-if="menu.contactPhone" class="flex items-center gap-2">
              <span aria-hidden="true" class="text-indigo-500">📞</span>
              <span class="font-medium">{{ menu.contactPhone }}</span>
            </div>
            <div v-if="menu.contactEmail" class="flex items-center gap-2">
              <span aria-hidden="true" class="text-indigo-500">✉️</span>
              <a :href="`mailto:${menu.contactEmail}`" class="font-medium text-indigo-600 hover:text-indigo-500">
                {{ menu.contactEmail }}
              </a>
            </div>
            <div v-if="menu.address" class="flex items-center gap-2">
              <span aria-hidden="true" class="text-indigo-500">📍</span>
              <span>{{ menu.address }}</span>
            </div>
          </div>
        </div>
      </div>

      <PublicMenuCategoryTree :categories="menu.categories" />

      <div v-if="menu.categories.length === 0" class="bg-white rounded-xl shadow-lg p-12 text-center border border-dashed border-indigo-200">
        <p class="text-gray-500 text-lg font-medium">{{ t('menu.comingSoon') }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import PublicMenuCategoryTree from '@/components/PublicMenuCategoryTree.vue';

const route = useRoute();
const restaurantStore = useRestaurantStore();
const { t, locale } = useI18n();

const menu = ref<any>(null);
const loading = ref(true);
const error = ref('');

watch(
  () => locale.value,
  async (newLocale, _oldLocale) => {
    if (!menu.value) {
      return;
    }
    const slug = route.params.slug as string;
    loading.value = true;
    try {
      menu.value = await restaurantStore.fetchPublicMenu(slug, newLocale);
    } catch (err: any) {
      error.value = err.message || t('messages.errorOccurred');
    } finally {
      loading.value = false;
    }
  },
);

onMounted(async () => {
  try {
    const slug = route.params.slug as string;
    menu.value = await restaurantStore.fetchPublicMenu(slug, locale.value);
  } catch (err: any) {
    error.value = err.message || t('messages.errorOccurred');
  } finally {
    loading.value = false;
  }
});
</script>
