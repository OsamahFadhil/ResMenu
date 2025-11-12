<template>
  <div class="min-h-screen" :style="themeStyles">
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

    <div v-else-if="menu" :class="containerClasses" :style="containerStyles">
      <!-- Restaurant Header -->
      <div :class="headerClasses" :style="headerStyles">
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6">
          <div :class="logoSectionClasses">
            <div v-if="menu.logoUrl && displaySettings.showImages" class="flex-shrink-0">
              <img :src="menu.logoUrl" :alt="menu.restaurantLocalizedName" :class="logoClasses" />
            </div>
            <div class="space-y-1">
              <h1 :class="titleClasses">
                {{ menu.restaurantLocalizedName || menu.restaurantName }}
              </h1>
              <p class="text-sm opacity-70 uppercase tracking-wide">
                {{ t('dashboard.analytics') }} · {{ menu.language.toUpperCase() }}
              </p>
            </div>
          </div>

          <div v-if="theme.showRestaurantInfo" class="space-y-3 text-sm">
            <div v-if="menu.contactPhone" class="flex items-center gap-2">
              <span aria-hidden="true">📞</span>
              <span class="font-medium">{{ menu.contactPhone }}</span>
            </div>
            <div v-if="menu.contactEmail" class="flex items-center gap-2">
              <span aria-hidden="true">✉️</span>
              <a :href="`mailto:${menu.contactEmail}`" class="font-medium hover:opacity-80">
                {{ menu.contactEmail }}
              </a>
            </div>
            <div v-if="menu.address" class="flex items-center gap-2">
              <span aria-hidden="true">📍</span>
              <span>{{ menu.address }}</span>
            </div>
          </div>
        </div>
      </div>

      <PublicMenuCategoryTree 
        :categories="menu.categories" 
        :theme="theme"
        :displaySettings="displaySettings"
        :currency="menu.currency"
      />

      <div v-if="menu.categories.length === 0" :class="emptyStateClasses">
        <p class="text-lg font-medium opacity-70">{{ t('menu.comingSoon') }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import PublicMenuCategoryTree from '@/components/PublicMenuCategoryTree.vue';
import type { TemplateTheme } from '~/stores/templates';

const route = useRoute();
const restaurantStore = useRestaurantStore();
const { t, locale } = useI18n();

const menu = ref<any>(null);
const loading = ref(true);
const error = ref('');

// Default theme
const defaultTheme: TemplateTheme = {
  primaryColor: '#dc2626',
  accentColor: '#f59e0b',
  backgroundColor: '#fafaf9',
  surfaceColor: '#ffffff',
  textColor: '#292524',
  fontFamily: 'Inter',
  fontSize: 'medium',
  layout: 'list',
  cardStyle: 'modern',
  borderRadius: 'medium',
  spacing: 'normal',
  showImages: true,
  imageSize: 'medium',
  imageShape: 'rounded',
  logoPosition: 'left',
  showRestaurantInfo: true,
  headerStyle: 'simple'
};

const theme = computed(() => menu.value?.theme || defaultTheme);
const displaySettings = computed(() => menu.value?.displaySettings || {
  showPrices: true,
  showImages: true,
  showDescriptions: true,
  showCategories: true,
  enableSearch: true,
  enableFilters: true,
  availableLanguages: ['en', 'ar']
});

// Dynamic styles based on theme
const themeStyles = computed(() => ({
  '--primary-color': theme.value.primaryColor,
  '--accent-color': theme.value.accentColor,
  '--background-color': theme.value.backgroundColor,
  '--surface-color': theme.value.surfaceColor,
  '--text-color': theme.value.textColor,
  '--font-family': theme.value.fontFamily,
  backgroundColor: theme.value.backgroundColor,
  color: theme.value.textColor,
  fontFamily: theme.value.fontFamily
}));

// Dynamic classes based on theme
const containerClasses = computed(() => {
  const spacing = {
    compact: 'space-y-4',
    normal: 'space-y-6',
    relaxed: 'space-y-8'
  }[theme.value.spacing || 'normal'];

  const maxWidth = theme.value.layout === 'grid' ? 'max-w-7xl' : 'max-w-4xl';

  return `${maxWidth} mx-auto px-4 py-12 ${spacing}`;
});

const headerClasses = computed(() => {
  const borderRadius = {
    none: 'rounded-none',
    small: 'rounded-lg',
    medium: 'rounded-xl',
    large: 'rounded-2xl'
  }[theme.value.borderRadius || 'medium'];

  return `p-8 ${borderRadius} shadow-xl border border-opacity-10`;
});

const logoSectionClasses = computed(() => {
  const position = {
    left: 'flex items-center gap-4',
    center: 'flex flex-col items-center gap-4 text-center',
    right: 'flex items-center gap-4 flex-row-reverse'
  }[theme.value.logoPosition || 'left'];

  return position;
});

const logoClasses = computed(() => {
  const shape = {
    square: 'rounded-none',
    rounded: 'rounded-xl',
    circle: 'rounded-full'
  }[theme.value.imageShape || 'rounded'];

  const size = {
    small: 'h-16 w-16',
    medium: 'h-20 w-20',
    large: 'h-24 w-24'
  }[theme.value.imageSize || 'medium'];

  return `${size} ${shape} border border-opacity-10 object-cover`;
});

const titleClasses = computed(() => {
  const size = {
    small: 'text-2xl',
    medium: 'text-3xl',
    large: 'text-4xl'
  }[theme.value.fontSize || 'medium'];

  return `${size} font-bold tracking-tight`;
});

const emptyStateClasses = computed(() => {
  const borderRadius = {
    none: 'rounded-none',
    small: 'rounded-lg',
    medium: 'rounded-xl',
    large: 'rounded-2xl'
  }[theme.value.borderRadius || 'medium'];

  return `${borderRadius} shadow-lg p-12 text-center border border-dashed border-opacity-20`;
});

// Additional inline styles for full theme control
const containerStyles = computed(() => ({
  color: theme.value.textColor,
  fontFamily: theme.value.fontFamily
}));

const headerStyles = computed(() => ({
  backgroundColor: theme.value.surfaceColor,
  color: theme.value.textColor,
  fontFamily: theme.value.fontFamily
}));

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
    console.log('=== PUBLIC MENU DATA ===');
    console.log('Menu:', menu.value);
    console.log('Theme:', menu.value?.theme);
    console.log('Display Settings:', menu.value?.displaySettings);
    console.log('Applied Theme Styles:', theme.value);
  } catch (err: any) {
    error.value = err.message || t('messages.errorOccurred');
  } finally {
    loading.value = false;
  }
});
</script>
