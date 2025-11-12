import { defineStore } from 'pinia';
import { useApi } from '../composables/useApi';

export interface MenuItemTranslations {
  [locale: string]: {
    name?: string;
    description?: string;
  };
}

export interface MenuCategoryTranslations {
  [locale: string]: string;
}

export interface MenuItem {
  id: string;
  name: string;
  localizedName: string;
  description?: string | null;
  localizedDescription?: string | null;
  price: number;
  imageUrl?: string | null;
  isAvailable: boolean;
  displayOrder: number;
  translations?: MenuItemTranslations;
}

export interface MenuCategory {
  id: string;
  name: string;
  localizedName: string;
  displayOrder: number;
  parentCategoryId?: string | null;
  translations?: MenuCategoryTranslations;
  items: MenuItem[];
  children: MenuCategory[];
}

export interface CreateCategoryPayload {
  name: string;
  displayOrder: number;
  parentCategoryId?: string | null;
  translations?: MenuCategoryTranslations;
}

export interface UpdateCategoryPayload extends CreateCategoryPayload {}

export interface CreateMenuItemPayload {
  name: string;
  description?: string | null;
  price: number;
  imageUrl?: string | null;
  isAvailable?: boolean;
  displayOrder: number;
  translations?: MenuItemTranslations;
}

interface Restaurant {
  id: string;
  name: string;
  slug: string;
  logoUrl?: string;
  contactPhone?: string;
  contactEmail?: string;
  address?: string;
}

interface PublicMenu {
  restaurantId: string;
  restaurantName: string;
  restaurantLocalizedName: string;
  logoUrl?: string;
  contactPhone?: string;
  contactEmail?: string;
  address?: string;
  language: string;
  categories: MenuCategory[];
}

export interface MenuGenerationResult {
  restaurantId: string;
  categoriesCreated: number;
  itemsCreated: number;
  overwroteExisting: boolean;
  templateKey: string;
  generatedAt: string;
}

const normalizeMenuItem = (item: any): MenuItem => ({
  id: item.id,
  name: item.name,
  localizedName: item.localizedName ?? item.name,
  description: item.description ?? null,
  localizedDescription: item.localizedDescription ?? item.description ?? null,
  price: Number(item.price ?? 0),
  imageUrl: item.imageUrl ?? null,
  isAvailable: item.isAvailable ?? true,
  displayOrder: item.displayOrder ?? 0,
  translations: item.translations ?? undefined,
});

const normalizeCategory = (category: any): MenuCategory => ({
  id: category.id,
  name: category.name,
  localizedName: category.localizedName ?? category.name,
  displayOrder: category.displayOrder ?? 0,
  parentCategoryId: category.parentCategoryId ?? null,
  translations: category.translations ?? undefined,
  items: (category.items ?? []).map(normalizeMenuItem),
  children: normalizeCategoryTree(category.children ?? []),
});

const normalizeCategoryTree = (categories: any[]): MenuCategory[] =>
  categories.map((category) => normalizeCategory(category));

const insertSorted = (list: MenuCategory[], category: MenuCategory) => {
  let index = list.length;
  for (let i = 0; i < list.length; i += 1) {
    if (list[i].displayOrder > category.displayOrder) {
      index = i;
      break;
    }
  }

  list.splice(index, 0, category);
};

const insertMenuItemSorted = (items: MenuItem[], item: MenuItem) => {
  let index = items.length;
  for (let i = 0; i < items.length; i += 1) {
    if (items[i].displayOrder > item.displayOrder) {
      index = i;
      break;
    }
  }

  items.splice(index, 0, item);
};

const findCategoryById = (categories: MenuCategory[], categoryId: string): MenuCategory | null => {
  for (const category of categories) {
    if (category.id === categoryId) {
      return category;
    }

    const found = findCategoryById(category.children, categoryId);
    if (found) {
      return found;
    }
  }

  return null;
};

const removeCategoryFromTree = (categories: MenuCategory[], categoryId: string): boolean => {
  const index = categories.findIndex((category) => category.id === categoryId);

  if (index !== -1) {
    categories.splice(index, 1);
    return true;
  }

  for (const category of categories) {
    if (removeCategoryFromTree(category.children, categoryId)) {
      return true;
    }
  }

  return false;
};

export const useRestaurantStore = defineStore('restaurant', {
  state: () => ({
    currentRestaurant: null as Restaurant | null,
    publicMenu: null as PublicMenu | null,
    qrCode: null as any,
    categories: [] as MenuCategory[],
    lastGeneratedMenu: null as MenuGenerationResult | null,
  }),

  actions: {
    async fetchPublicMenu(slug: string, language?: string) {
      const api = useApi();
      const response = await api.get(`/menu/${slug}`, {
        params: language ? { lang: language } : undefined,
      });

      if (response.data.success) {
        const normalizedMenu = {
          ...response.data.data,
          restaurantLocalizedName:
            response.data.data?.restaurantLocalizedName ?? response.data.data?.restaurantName,
          language: response.data.data?.language ?? language ?? 'en',
          categories: normalizeCategoryTree(response.data.data?.categories ?? []),
        } as PublicMenu;
        this.publicMenu = normalizedMenu;
        return normalizedMenu;
      }

      throw new Error(response.data.message || 'Failed to fetch menu');
    },

    async fetchCategories(restaurantId: string, language?: string) {
      const api = useApi();
      const response = await api.get(`/restaurants/${restaurantId}/categories`, {
        params: language ? { lang: language } : undefined,
      });

      if (response.data.success) {
        const normalized = normalizeCategoryTree(response.data.data ?? []);
        this.categories = normalized;
        return normalized;
      }

      throw new Error(response.data.message || 'Failed to fetch categories');
    },

    async createCategory(restaurantId: string, categoryData: CreateCategoryPayload, language?: string) {
      const api = useApi();
      try {
        const response = await api.post(`/restaurants/${restaurantId}/categories`, categoryData);

        if (response.data.success) {
          const category = normalizeCategory(response.data.data);
          await this.fetchCategories(restaurantId, language);
          return category;
        }

        throw new Error(response.data.message || 'Failed to create category');
      } catch (error: any) {
        const errorMessage = error?.response?.data?.message || error?.response?.data?.error || error?.message || 'Failed to create category';
        throw new Error(errorMessage);
      }
    },

    async updateCategory(
      categoryId: string,
      categoryData: UpdateCategoryPayload,
      restaurantId: string,
      language?: string,
    ) {
      const api = useApi();
      try {
        const response = await api.put(`/categories/${categoryId}`, categoryData);

        if (response.data.success) {
          await this.fetchCategories(restaurantId, language);
          return true;
        }

        throw new Error(response.data.message || 'Failed to update category');
      } catch (error: any) {
        const errorMessage = error?.response?.data?.message || error?.response?.data?.error || error?.message || 'Failed to update category';
        throw new Error(errorMessage);
      }
    },

    async deleteCategory(categoryId: string, restaurantId: string, language?: string) {
      const api = useApi();
      const response = await api.delete(`/categories/${categoryId}`);

      if (response.data.success) {
        removeCategoryFromTree(this.categories, categoryId);
        if (restaurantId) {
          await this.fetchCategories(restaurantId, language);
        }
        return true;
      }

      throw new Error(response.data.message || 'Failed to delete category');
    },

    async createMenuItem(categoryId: string, itemData: CreateMenuItemPayload) {
      const api = useApi();
      try {
        const response = await api.post(`/categories/${categoryId}/items`, itemData);

        if (response.data.success) {
          const item = normalizeMenuItem(response.data.data);
          const category = findCategoryById(this.categories, categoryId);
          if (category) {
            insertMenuItemSorted(category.items, item);
          }
          return item;
        }

        throw new Error(response.data.message || 'Failed to create menu item');
      } catch (error: any) {
        const errorMessage = error?.response?.data?.message || error?.response?.data?.error || error?.message || 'Failed to create menu item';
        throw new Error(errorMessage);
      }
    },

    async generateMenu(
      restaurantId: string,
      options: {
        overwriteExisting?: boolean
        templateKey?: string
        templateId?: string
        languageCodes?: string[]
        targetCategories?: number
        targetItemsPerCategory?: number
      } = {},
    ) {
      const api = useApi();
      const response = await api.post(`/restaurants/${restaurantId}/menu/generate`, {
        overwriteExisting: options.overwriteExisting ?? false,
        templateKey: options.templateKey ?? 'default',
        templateId: options.templateId ?? null,
        languageCodes: options.languageCodes ?? undefined,
        targetCategories: options.targetCategories ?? undefined,
        targetItemsPerCategory: options.targetItemsPerCategory ?? undefined,
      });

      if (response.data.success) {
        this.lastGeneratedMenu = response.data.data as MenuGenerationResult;
        await this.fetchCategories(restaurantId, options.languageCodes ? options.languageCodes[0] : undefined);
        return this.lastGeneratedMenu;
      }

      throw new Error(response.data.message || 'Failed to generate menu');
    },

    async generateQRCode(restaurantId: string) {
      const api = useApi();
      const response = await api.get(`/qrcode/${restaurantId}`);

      if (response.data.success) {
        this.qrCode = response.data.data;
        return response.data.data;
      }

      throw new Error(response.data.message || 'Failed to generate QR code');
    },
  },
});
