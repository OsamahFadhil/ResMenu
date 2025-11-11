<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Navigation -->
    <nav class="bg-white shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <h1 class="text-xl font-bold text-indigo-600">Menufy Dashboard</h1>
          </div>
          <div class="flex items-center space-x-4">
            <span class="text-gray-700">{{ authStore.user?.name }}</span>
            <button
              @click="handleLogout"
              class="text-gray-700 hover:text-indigo-600 px-3 py-2 rounded-md text-sm font-medium"
            >
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="mb-8">
        <h2 class="text-2xl font-bold text-gray-900">Welcome, {{ authStore.user?.name }}!</h2>
        <p class="mt-2 text-gray-600">Manage your restaurant and menu from here.</p>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div class="bg-white rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm">Total Categories</p>
              <p class="text-3xl font-bold text-gray-900">0</p>
            </div>
            <div class="text-4xl">📂</div>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm">Total Menu Items</p>
              <p class="text-3xl font-bold text-gray-900">0</p>
            </div>
            <div class="text-4xl">🍽️</div>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm">QR Code</p>
              <button
                @click="generateQR"
                class="mt-2 text-sm text-indigo-600 hover:text-indigo-700 font-medium"
              >
                Generate QR
              </button>
            </div>
            <div class="text-4xl">📱</div>
          </div>
        </div>
      </div>

      <!-- QR Code Display -->
      <div v-if="qrCode" class="bg-white rounded-lg shadow p-6 mb-8">
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Your QR Code</h3>
        <div class="flex items-center space-x-6">
          <img :src="qrCode.imageUrl" alt="QR Code" class="h-48 w-48 border rounded" />
          <div>
            <p class="text-sm text-gray-600 mb-2">Menu Link:</p>
            <a :href="qrCode.link" target="_blank" class="text-indigo-600 hover:text-indigo-700 underline">
              {{ qrCode.link }}
            </a>
            <p class="mt-4 text-sm text-gray-600">Print this QR code and place it on your tables for customers to scan.</p>
          </div>
        </div>
      </div>

      <!-- Menu Management Section -->
      <div class="bg-white rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Menu Management</h3>
        <p class="text-gray-600 mb-4">Create categories and add menu items to build your digital menu.</p>

        <div class="space-y-4">
          <button
            @click="showCategoryForm = !showCategoryForm"
            class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-md text-sm font-medium"
          >
            Add Category
          </button>

          <!-- Category Form -->
          <div v-if="showCategoryForm" class="border rounded-lg p-4 bg-gray-50">
            <h4 class="font-medium text-gray-900 mb-3">New Category</h4>
            <div class="space-y-3">
              <div>
                <label class="block text-sm font-medium text-gray-700">Category Name</label>
                <input
                  v-model="categoryForm.name"
                  type="text"
                  class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Display Order</label>
                <input
                  v-model.number="categoryForm.displayOrder"
                  type="number"
                  class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Parent Category</label>
                <select
                  v-model="categoryForm.parentCategoryId"
                  class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md"
                >
                  <option :value="null">Top Level</option>
                  <option
                    v-for="option in categoryOptions"
                    :key="option.id"
                    :value="option.id"
                  >
                    {{ option.label }}
                  </option>
                </select>
              </div>
              <button
                @click="createCategory"
                class="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md text-sm"
              >
                Save Category
              </button>
            </div>
          </div>

          <!-- Existing Categories -->
          <div class="mt-6 space-y-4">
            <h4 class="font-medium text-gray-900 mb-3">Existing Categories</h4>
            <div v-if="categories.length === 0" class="text-sm text-gray-500">
              No categories yet. Create your first category to start building your menu.
            </div>
            <MenuCategoryTree
              v-else
              :categories="categories"
              :showItemForms="showItemForms"
              :itemForms="itemForms"
              :onToggleItemForm="toggleItemForm"
              :onSaveItem="createMenuItemForCategory"
              :onAddSubcategory="startAddSubcategory"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import MenuCategoryTree from '@/components/MenuCategoryTree.vue';
import type { MenuCategory } from '@/stores/restaurant';

const authStore = useAuthStore();
const restaurantStore = useRestaurantStore();
const router = useRouter();

const qrCode = ref<any>(null);
const showCategoryForm = ref(false);
const categoryForm = ref({
  name: '',
  displayOrder: 0,
  parentCategoryId: null as string | null,
});
const showItemForms = ref<Record<string, boolean>>({});
const itemForms = ref<
  Record<
    string,
    {
      name: string;
      description?: string;
      price: number;
      imageUrl?: string;
      isAvailable: boolean;
      displayOrder: number;
    }
  >
>({});
const categories = computed(() => restaurantStore.categories);
const categoryOptions = computed(() => {
  const options: { id: string; label: string }[] = [];

  const buildOptions = (cats: MenuCategory[], prefix = '') => {
    for (const category of cats) {
      options.push({
        id: category.id,
        label: `${prefix}${category.name}`,
      });
      if (category.children.length) {
        buildOptions(category.children, `${prefix}- `);
      }
    }
  };

  buildOptions(restaurantStore.categories);
  return options;
});

onMounted(() => {
  authStore.loadFromStorage();
  if (!authStore.isAuthenticated) {
    router.push('/login');
  }
});

const loadCategories = async (restaurantId: string) => {
  try {
    await restaurantStore.fetchCategories(restaurantId);
  } catch (err: any) {
    console.error(err);
  }
};

watch(
  () => authStore.restaurantId,
  (restaurantId) => {
    if (restaurantId) {
      loadCategories(restaurantId);
    }
  },
  { immediate: true }
);

const handleLogout = () => {
  authStore.logout();
  router.push('/');
};

const generateQR = async () => {
  try {
    const restaurantId = authStore.restaurantId;
    if (!restaurantId) {
      throw new Error('Restaurant not found for current user');
    }
    qrCode.value = await restaurantStore.generateQRCode(restaurantId);
  } catch (err: any) {
    alert(err.message || 'Failed to generate QR code');
  }
};

const createCategory = async () => {
  try {
    const restaurantId = authStore.restaurantId;
    if (!restaurantId) {
      throw new Error('Restaurant not found for current user');
    }
    await restaurantStore.createCategory(restaurantId, categoryForm.value);
    alert('Category created successfully!');
    showCategoryForm.value = false;
    categoryForm.value = { name: '', displayOrder: 0, parentCategoryId: null };
  } catch (err: any) {
    alert(err.message || 'Failed to create category');
  }
};

const ensureItemForm = (categoryId: string) => {
  if (!itemForms.value[categoryId]) {
    itemForms.value[categoryId] = {
      name: '',
      description: '',
      price: 0,
      imageUrl: '',
      isAvailable: true,
      displayOrder: 0,
    };
  }
};

const toggleItemForm = (categoryId: string) => {
  showItemForms.value[categoryId] = !showItemForms.value[categoryId];
  if (showItemForms.value[categoryId]) {
    ensureItemForm(categoryId);
  }
};

const resetItemForm = (categoryId: string) => {
  itemForms.value[categoryId] = {
    name: '',
    description: '',
    price: 0,
    imageUrl: '',
    isAvailable: true,
    displayOrder: 0,
  };
};

const createMenuItemForCategory = async (categoryId: string) => {
  try {
    const form = itemForms.value[categoryId];
    if (!form) {
      return;
    }
    await restaurantStore.createMenuItem(categoryId, form);
    alert('Menu item created successfully!');
    resetItemForm(categoryId);
    showItemForms.value[categoryId] = false;
  } catch (err: any) {
    alert(err.message || 'Failed to create menu item');
  }
};

const startAddSubcategory = (categoryId: string) => {
  showCategoryForm.value = true;
  categoryForm.value = {
    name: '',
    displayOrder: 0,
    parentCategoryId: categoryId,
  };
};
</script>
