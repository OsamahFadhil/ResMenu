<template>
  <div 
    v-for="category in categories" 
    :key="category.id" 
    :class="categoryClasses"
    :style="categoryStyle"
  >
    <h2 
      v-if="displaySettings?.showCategories !== false"
      :class="categoryTitleClasses" 
      :style="categoryTitleStyle"
    >
      {{ category.localizedName || category.name }}
    </h2>

    <div :class="itemsContainerClasses">
      <div
        v-for="item in category.items"
        :key="item.id"
        :class="itemClasses"
        :style="itemStyle"
      >
        <div v-if="displaySettings?.showImages && item.imageUrl" class="flex-shrink-0">
          <img :src="item.imageUrl" :alt="item.localizedName || item.name" :class="imageClasses" />
        </div>
        <div class="flex-1 space-y-2">
          <div class="flex flex-col gap-2 sm:flex-row sm:items-start sm:justify-between">
            <h3 :class="itemTitleClasses" :style="{ color: theme?.textColor || '#292524' }">
              {{ item.localizedName || item.name }}
            </h3>
            <span 
              v-if="displaySettings?.showPrices" 
              class="text-lg font-bold whitespace-nowrap"
              :style="{ color: theme?.primaryColor || '#dc2626' }"
            >
              {{ formatPrice(item.price) }}
            </span>
          </div>
          <p 
            v-if="displaySettings?.showDescriptions && (item.localizedDescription || item.description)" 
            class="text-sm leading-relaxed"
            :style="{ color: theme?.textColor || '#292524', opacity: 0.8 }"
          >
            {{ item.localizedDescription || item.description }}
          </p>
        </div>
      </div>

      <div v-if="category.items.length === 0" class="py-4 italic text-center opacity-70">
        {{ t('menu.emptyCategoryMessage') }}
      </div>
    </div>

    <div v-if="category.children.length" :class="childrenContainerClasses">
      <PublicMenuCategoryTree 
        :categories="category.children" 
        :theme="theme"
        :displaySettings="displaySettings"
        :currency="currency"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { MenuCategory } from '@/stores/restaurant';
import type { TemplateTheme } from '~/stores/templates';

defineOptions({
  name: 'PublicMenuCategoryTree',
});

const props = withDefaults(defineProps<{
  categories: MenuCategory[];
  theme?: TemplateTheme | null;
  displaySettings?: {
    showPrices?: boolean;
    showImages?: boolean;
    showDescriptions?: boolean;
    showCategories?: boolean;
    enableSearch?: boolean;
    enableFilters?: boolean;
  } | null;
  currency?: string;
}>(), {
  theme: null,
  displaySettings: null,
  currency: 'USD'
});

const { locale, t } = useI18n({ useScope: 'global' });

const formatPrice = (price: number) => {
  try {
    return new Intl.NumberFormat(locale.value, {
      style: 'currency',
      currency: props.currency || 'USD',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(price);
  } catch (error) {
    return `${props.currency} ${price.toFixed(2)}`;
  }
};

// Dynamic classes based on theme
const categoryClasses = computed(() => {
  const borderRadius = {
    none: 'rounded-none',
    small: 'rounded-lg',
    medium: 'rounded-xl',
    large: 'rounded-2xl'
  }[props.theme?.borderRadius || 'medium'];

  const spacing = {
    compact: 'p-4 mb-4',
    normal: 'p-6 mb-6',
    relaxed: 'p-8 mb-8'
  }[props.theme?.spacing || 'normal'];

  return `${borderRadius} ${spacing} shadow-lg border border-opacity-10`;
});

const categoryTitleClasses = computed(() => {
  const size = {
    small: 'text-xl',
    medium: 'text-2xl',
    large: 'text-3xl'
  }[props.theme?.fontSize || 'medium'];

  return `${size} font-bold mb-4 border-b pb-2 border-opacity-20`;
});

const itemsContainerClasses = computed(() => {
  const spacing = {
    compact: 'space-y-2',
    normal: 'space-y-4',
    relaxed: 'space-y-6'
  }[props.theme?.spacing || 'normal'];

  const layout = props.theme?.layout || 'list';
  
  if (layout === 'grid') {
    const gap = {
      compact: 'gap-3',
      normal: 'gap-4',
      relaxed: 'gap-6'
    }[props.theme?.spacing || 'normal'];
    return `grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 ${gap}`;
  }
  
  if (layout === 'cards') {
    const gap = {
      compact: 'gap-3',
      normal: 'gap-4',
      relaxed: 'gap-6'
    }[props.theme?.spacing || 'normal'];
    return `grid grid-cols-1 md:grid-cols-2 ${gap}`;
  }
  
  return spacing;
});

const itemClasses = computed(() => {
  const borderRadius = {
    none: 'rounded-none',
    small: 'rounded',
    medium: 'rounded-lg',
    large: 'rounded-xl'
  }[props.theme?.borderRadius || 'medium'];

  const layout = props.theme?.layout || 'list';
  const cardStyle = props.theme?.cardStyle || 'modern';

  // Grid layout - vertical cards
  if (layout === 'grid') {
    const padding = {
      compact: 'p-3',
      normal: 'p-4',
      relaxed: 'p-6'
    }[props.theme?.spacing || 'normal'];

    if (cardStyle === 'modern') {
      return `flex flex-col gap-3 ${padding} border-2 ${borderRadius} hover:shadow-xl transition-all duration-300 hover:-translate-y-1`;
    } else if (cardStyle === 'minimal') {
      return `flex flex-col gap-3 ${padding} border ${borderRadius} hover:shadow-md transition-all`;
    } else { // classic
      return `flex flex-col gap-3 ${padding} border ${borderRadius} shadow-sm hover:shadow-lg transition-all`;
    }
  }

  // Cards layout - horizontal cards
  if (layout === 'cards') {
    const padding = {
      compact: 'p-3',
      normal: 'p-4',
      relaxed: 'p-5'
    }[props.theme?.spacing || 'normal'];

    return `flex flex-col sm:flex-row gap-4 ${padding} border ${borderRadius} hover:shadow-lg transition-all`;
  }

  // List layout - simple rows
  const padding = {
    compact: 'p-2',
    normal: 'p-4',
    relaxed: 'p-5'
  }[props.theme?.spacing || 'normal'];

  return `flex flex-col sm:flex-row items-start gap-4 ${padding} hover:bg-opacity-5 hover:bg-black ${borderRadius} transition-all`;
});

const imageClasses = computed(() => {
  const shape = {
    square: 'rounded-none',
    rounded: 'rounded-lg',
    circle: 'rounded-full'
  }[props.theme?.imageShape || 'rounded'];

  const layout = props.theme?.layout || 'list';
  
  // Different sizes for different layouts
  let size = '';
  if (layout === 'grid') {
    size = {
      small: 'h-32 w-full',
      medium: 'h-40 w-full',
      large: 'h-48 w-full'
    }[props.theme?.imageSize || 'medium'];
  } else if (layout === 'cards') {
    size = {
      small: 'h-24 w-24',
      medium: 'h-32 w-32',
      large: 'h-40 w-40'
    }[props.theme?.imageSize || 'medium'];
  } else {
    size = {
      small: 'h-16 w-16',
      medium: 'h-20 w-20',
      large: 'h-24 w-24'
    }[props.theme?.imageSize || 'medium'];
  }

  return `${size} ${shape} object-cover`;
});

const itemTitleClasses = computed(() => {
  const size = {
    small: 'text-base',
    medium: 'text-lg',
    large: 'text-xl'
  }[props.theme?.fontSize || 'medium'];

  return `${size} font-semibold`;
});

// Dynamic styles for full customization
const categoryStyle = computed(() => ({
  backgroundColor: props.theme?.surfaceColor || '#ffffff',
  color: props.theme?.textColor || '#292524',
  fontFamily: props.theme?.fontFamily || 'Inter'
}));

const categoryTitleStyle = computed(() => ({
  color: props.theme?.primaryColor || '#dc2626',
  borderColor: props.theme?.primaryColor || '#dc2626',
  fontFamily: props.theme?.headingFont || props.theme?.fontFamily || 'Inter'
}));

const itemStyle = computed(() => {
  const layout = props.theme?.layout || 'list';
  const baseStyle: any = {
    backgroundColor: props.theme?.surfaceColor || '#ffffff',
    fontFamily: props.theme?.bodyFont || props.theme?.fontFamily || 'Inter'
  };

  // For grid/cards layout, add background and border
  if (layout === 'grid' || layout === 'cards') {
    baseStyle.backgroundColor = props.theme?.surfaceColor || '#ffffff';
    baseStyle.borderColor = props.theme?.primaryColor || '#dc2626';
  }

  return baseStyle;
});

const childrenContainerClasses = computed(() => {
  const spacing = {
    compact: 'mt-4 space-y-2',
    normal: 'mt-6 space-y-4',
    relaxed: 'mt-8 space-y-6'
  }[props.theme?.spacing || 'normal'];

  return spacing;
});
</script>
