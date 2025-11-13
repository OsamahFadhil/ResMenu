<template>
  <NuxtLayout name="dashboard">
    <div class="space-y-10">
      <section class="grid gap-6 lg:grid-cols-[2fr,1fr]">
        <div class="overflow-hidden rounded-3xl bg-white shadow-xl ring-1 ring-orange-100">
          <div class="flex flex-col gap-6 p-8 sm:flex-row sm:items-center sm:justify-between bg-gradient-to-r from-amber-50 via-orange-50 to-rose-50">
            <div>
              <p class="text-xs uppercase tracking-[0.4em] text-orange-400">{{ $t('dashboard.welcome') }}</p>
              <h1 class="mt-2 text-3xl font-semibold text-slate-900">{{ authStore.user?.name }}</h1>
              <p class="mt-3 max-w-xl text-sm text-slate-600">
                {{ $t('dashboard.analytics') }} · {{ restaurantStore.categories.length }} {{ $t('menu.categories').toLowerCase() }}
                · {{ countTotalItems }} {{ $t('menu.items').toLowerCase() }}
              </p>
            </div>
            <div class="rounded-3xl border border-orange-200 bg-white/70 px-6 py-4 text-sm shadow">
              <p class="text-xs uppercase text-orange-400">{{ $t('menu.generateSampleMenu') }}</p>
              <p class="mt-2 text-lg font-semibold text-slate-900">{{ generatorSummary || 'Launch-ready in minutes' }}</p>
              <div class="mt-4 flex flex-wrap gap-2">
                <UiButton variant="primary" size="sm" @click="$router.push('/dashboard/templates')">
                  {{ $t('menu.generateSampleMenu') }}
                </UiButton>
                <UiButton variant="secondary" size="sm" @click="handleQuickGenerate">
                  {{ $t('menu.regenerateSampleMenu') }}
                </UiButton>
              </div>
            </div>
          </div>
          <div class="grid gap-4 bg-white/90 px-6 py-5 sm:grid-cols-3">
            <div class="rounded-2xl border border-orange-100 bg-orange-50/70 p-4">
              <p class="text-xs uppercase tracking-wide text-orange-400">{{ $t('menu.items') }}</p>
              <p class="mt-2 text-2xl font-semibold text-slate-900">{{ stats.totalItems }}</p>
              <p class="text-xs text-slate-500">{{ $t('menu.createSuccess') }}</p>
            </div>
            <div class="rounded-2xl border border-emerald-100 bg-emerald-50/70 p-4">
              <p class="text-xs uppercase tracking-wide text-emerald-500">{{ $t('menu.categories') }}</p>
              <p class="mt-2 text-2xl font-semibold text-slate-900">{{ stats.totalCategories }}</p>
              <p class="text-xs text-slate-500">{{ $t('menu.addCategory') }}</p>
            </div>
            <div class="rounded-2xl border border-sky-100 bg-sky-50/70 p-4">
              <p class="text-xs uppercase tracking-wide text-sky-500">{{ $t('qr.scanCount') }}</p>
              <p class="mt-2 text-2xl font-semibold text-slate-900">{{ stats.qrScans }}</p>
              <p class="text-xs text-slate-500">{{ $t('qr.generate') }}</p>
            </div>
          </div>
        </div>

        <div class="flex flex-col gap-4">
          <div class="rounded-3xl border border-orange-100 bg-white p-6 shadow-sm">
            <h3 class="text-sm font-semibold text-slate-900">{{ $t('common.actions') }}</h3>
            <p class="mt-2 text-xs text-slate-500">{{ $t('dashboard.recentActivity') }}</p>
            <div class="mt-4 space-y-3 text-sm">
              <button class="flex w-full items-center justify-between rounded-2xl border border-orange-100 bg-orange-50 px-4 py-3 text-orange-500 hover:bg-orange-100" @click="$router.push('/dashboard/menu')">
                <span>{{ $t('menu.addItem') }}</span>
                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" /></svg>
              </button>
              <button class="flex w-full items-center justify-between rounded-2xl border border-slate-200 bg-white px-4 py-3 text-slate-600 hover:bg-slate-50" @click="$router.push('/dashboard/categories')">
                <span>{{ $t('menu.addCategory') }}</span>
                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" /></svg>
              </button>
              <button class="flex w-full items-center justify-between rounded-2xl border border-slate-200 bg-white px-4 py-3 text-slate-600 hover:bg-slate-50" @click="$router.push('/dashboard/qrcodes')">
                <span>{{ $t('qr.generate') }}</span>
                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" /></svg>
              </button>
              <button class="flex w-full items-center justify-between rounded-2xl border border-slate-200 bg-white px-4 py-3 text-slate-600 hover:bg-slate-50" @click="$router.push('/dashboard/settings')">
                <span>{{ $t('navigation.settings') }}</span>
                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" /></svg>
              </button>
            </div>
          </div>

          <div class="rounded-3xl border border-orange-100 bg-white p-6 shadow-sm">
            <h3 class="text-sm font-semibold text-slate-900">{{ $t('dashboard.recentActivity') }}</h3>
            <EmptyState
              :title="$t('messages.noData')"
              :description="$t('messages.noResults')"
            />
          </div>
        </div>
      </section>

      <section class="grid gap-6 lg:grid-cols-2">
        <div class="rounded-3xl border border-orange-100 bg-white p-6 shadow-sm">
          <h3 class="text-sm font-semibold text-slate-900">{{ $t('menu.generatorSuccess') }}</h3>
          <p class="mt-2 text-sm text-slate-600">
            {{ $t('menu.generatorDescription') }}
          </p>
          <div class="mt-4 flex flex-wrap gap-3">
            <UiButton variant="primary" @click="$router.push('/dashboard/templates')">
              {{ $t('navigation.templates') }}
            </UiButton>
            <UiButton variant="secondary" :loading="generatorLoading" @click="handleQuickGenerate">
              {{ $t('menu.regenerateSampleMenu') }}
            </UiButton>
          </div>
        </div>

        <div class="rounded-3xl border border-orange-100 bg-white p-6 shadow-sm">
          <h3 class="text-sm font-semibold text-slate-900">{{ $t('dashboard.analytics') }}</h3>
          <ul class="mt-4 space-y-3 text-sm text-slate-600">
            <li class="flex items-center justify-between rounded-2xl border border-slate-100 bg-slate-50 px-4 py-3">
              <span>{{ countTotalItems }} menu items live</span>
              <span class="text-slate-400">{{ stats.totalCategories }} categories</span>
            </li>
            <li class="flex items-center justify-between rounded-2xl border border-slate-100 bg-slate-50 px-4 py-3">
              <span>{{ stats.qrScans }} QR scans recorded</span>
              <button class="text-xs text-orange-500 hover:underline" @click="$router.push('/dashboard/qrcodes')">
                {{ $t('qr.generate') }}
              </button>
            </li>
          </ul>
        </div>
      </section>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import type { MenuCategory } from '@/stores/restaurant';

definePageMeta({
  layout: false,
  middleware: ['owner']
});

const authStore = useAuthStore();
const restaurantStore = useRestaurantStore();
const { t, locale } = useI18n({ useScope: 'global' });
const toast = useToast();

const generatorLoading = ref(false);

const countCategories = (categories: MenuCategory[]): number =>
  categories.reduce((total, category) => total + 1 + countCategories(category.children), 0);

const countItems = (categories: MenuCategory[]): number =>
  categories.reduce(
    (total, category) => total + category.items.length + countItems(category.children),
    0,
  );

const stats = computed(() => {
  const categories = restaurantStore.categories;
  return {
    totalItems: countItems(categories),
    totalCategories: countCategories(categories),
    qrScans: restaurantStore.qrCode?.scanCount ?? 0,
  };
});

const generatorSummary = computed(() => {
  const result = restaurantStore.lastGeneratedMenu;
  if (!result) {
    return '';
  }

  return t('menu.generatorSummary', {
    categories: result.categoriesCreated,
    items: result.itemsCreated,
  });
});

const countTotalItems = computed(() => stats.value.totalItems);

const handleQuickGenerate = async () => {
  const restaurantId = authStore.restaurantId;
  if (!restaurantId) {
    toast.error(t('messages.errorOccurred'));
    return;
  }

  generatorLoading.value = true;

  try {
    await restaurantStore.generateMenu(restaurantId, {
      overwriteExisting: Boolean(restaurantStore.lastGeneratedMenu),
      templateKey: 'default',
      languageCodes: [locale.value],
    });
    toast.success(t('menu.generatorSuccess'));
  } catch (error: any) {
    toast.error(error.message || t('messages.errorOccurred'));
  } finally {
    generatorLoading.value = false;
  }
};

onMounted(async () => {
  const restaurantId = authStore.restaurantId;
  if (restaurantId) {
    await restaurantStore.fetchCategories(restaurantId, locale.value);
  }
});

watch(
  () => locale.value,
  async (newLocale, oldLocale) => {
    if (newLocale === oldLocale) {
      return;
    }
    const restaurantId = authStore.restaurantId;
    if (restaurantId) {
      await restaurantStore.fetchCategories(restaurantId, newLocale);
    }
  },
);
</script>
