import { defineStore } from 'pinia';
import { useApi } from '../composables/useApi';

export interface MenuItem {
  id: string;
  name: string;
  description?: string | null;
  price: number;
  imageUrl?: string | null;
  isAvailable: boolean;
  displayOrder: number;
}

export interface MenuCategory {
  id: string;
  name: string;
  displayOrder: number;
  parentCategoryId?: string | null;
  items: MenuItem[];
  children: MenuCategory[];
}

export interface CreateCategoryPayload {
  name: string;
  displayOrder: number;
  parentCategoryId?: string | null;
}

export interface CreateMenuItemPayload {
  name: string;
  description?: string | null;
  price: number;
  imageUrl?: string | null;
  isAvailable?: boolean;
  displayOrder: number;
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
  logoUrl?: string;
  contactPhone?: string;
  contactEmail?: string;
  address?: string;
  categories: MenuCategory[];
}

const normalizeMenuItem = (item: any): MenuItem => ({
  id: item.id,
  name: item.name,
  description: item.description ?? null,
  price: item.price,
  imageUrl: item.imageUrl ?? null,
  isAvailable: item.isAvailable ?? true,
  displayOrder: item.displayOrder ?? 0,
});

const normalizeCategory = (category: any): MenuCategory => ({
  id: category.id,
  name: category.name,
  displayOrder: category.displayOrder ?? 0,
  parentCategoryId: category.parentCategoryId ?? null,
  items: (category.items ?? []).map(normalizeMenuItem),
  children: normalizeCategoryTree(category.children ?? []),
});

const normalizeCategoryTree = (categories: any[]): MenuCategory[] =>
  categories.map((category) => normalizeCategory(category));

const insertCategoryIntoTree = (categories: MenuCategory[], category: MenuCategory): boolean => {
  if (!category.parentCategoryId) {
    insertSorted(categories, category);
    return true;
  }

  for (const existing of categories) {
    if (existing.id === category.parentCategoryId) {
      insertSorted(existing.children, category);
      return true;
    }

    if (insertCategoryIntoTree(existing.children, category)) {
      return true;
    }
  }

  return false;
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

export const useRestaurantStore = defineStore('restaurant', {
  state: () => ({
    currentRestaurant: null as Restaurant | null,
    publicMenu: null as PublicMenu | null,
    qrCode: null as any,
    categories: [] as MenuCategory[],
  }),

  actions: {
    async fetchPublicMenu(slug: string) {
      const api = useApi();
      const response = await api.get(`/menu/${slug}`);

      if (response.data.success) {
        const normalizedMenu = {
          ...response.data.data,
          categories: normalizeCategoryTree(response.data.data?.categories ?? []),
        } as PublicMenu;
        this.publicMenu = normalizedMenu;
        return normalizedMenu;
      }

      throw new Error(response.data.message || 'Failed to fetch menu');
    },

    async fetchCategories(restaurantId: string) {
      const api = useApi();
      const response = await api.get(`/restaurants/${restaurantId}/categories`);

      if (response.data.success) {
        const normalized = normalizeCategoryTree(response.data.data ?? []);
        this.categories = normalized;
        return normalized;
      }

      throw new Error(response.data.message || 'Failed to fetch categories');
    },

    async createCategory(restaurantId: string, categoryData: CreateCategoryPayload) {
      const api = useApi();
      const response = await api.post(`/restaurants/${restaurantId}/categories`, categoryData);

      if (response.data.success) {
        const category = normalizeCategory(response.data.data);
        const inserted = insertCategoryIntoTree(this.categories, category);
        if (!inserted) {
          insertSorted(this.categories, category);
        }
        return category;
      }

      throw new Error(response.data.message || 'Failed to create category');
    },

    async createMenuItem(categoryId: string, itemData: CreateMenuItemPayload) {
      const api = useApi();
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
