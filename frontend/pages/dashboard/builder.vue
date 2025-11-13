<template>
  <NuxtLayout name="dashboard">
    <!-- Header -->
    <div class="flex items-center justify-between mb-6 px-6 pt-6">
      <div>
        <h1 class="text-2xl font-bold text-neutral-900">Menu Builder</h1>
        <p class="mt-1 text-neutral-600">Drag and drop to organize your menu</p>
      </div>

      <!-- Auto-save Indicator & Actions -->
      <div class="flex items-center gap-3">
        <Transition name="fade">
          <div v-if="saving" class="flex items-center gap-2 text-blue-600">
            <svg class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            <span class="text-sm">Saving...</span>
          </div>
          <div v-else-if="saved" class="flex items-center gap-2 text-green-600">
            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
            </svg>
            <span class="text-sm">Saved</span>
          </div>
        </Transition>

        <button
          @click="showPreview = !showPreview"
          class="px-3 py-2 rounded-lg border border-neutral-300 hover:bg-neutral-50 transition-all"
          :title="showPreview ? 'Hide Preview' : 'Show Preview'"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
          </svg>
        </button>

        <button
          @click="showExportDialog = true"
          class="px-3 py-2 rounded-lg border border-neutral-300 hover:bg-neutral-50 transition-all flex items-center gap-2"
          :disabled="categories.length === 0"
          title="Export Menu"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
          </svg>
          <span>Export</span>
        </button>

        <UiButton @click="addCategory" variant="primary">
          + Add Category
        </UiButton>
      </div>
    </div>

    <!-- Split View: Editor + Preview -->
    <div class="flex gap-4 px-6 pb-6" :class="showPreview ? 'h-[calc(100vh-180px)]' : ''">
      <!-- Editor Panel -->
      <div :class="showPreview ? 'w-1/2 overflow-auto' : 'w-full'">
        <div class="space-y-4">

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center py-12">
        <div class="inline-flex h-12 w-12 items-center justify-center rounded-full border-4 border-primary-200 border-t-primary-600 animate-spin"></div>
      </div>

      <!-- Categories -->
      <div v-else-if="categories.length > 0" class="space-y-4">
        <DraggableCategory
          v-for="(category, index) in categories"
          :key="category.id"
          :category="category"
          :is-dragged-over="draggedOverIndex === index"
          @drag-start="handleDragStart"
          @drag-over="handleDragOver(index, $event)"
          @drag-leave="draggedOverIndex = null"
          @drop="handleDrop(index)"
          @drag-end="draggedOverIndex = null"
          @update="updateCategory"
          @delete="deleteCategory(category.id)"
          @customize="openStyleEditor(category)"
          @add-item="openItemDialog(category)"
          @reorder-items="reorderItems(category.id, $event)"
        />
      </div>

      <!-- Empty State -->
      <div v-else class="text-center py-12">
        <div class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-neutral-100 mb-4">
          <svg class="w-8 h-8 text-neutral-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
        </div>
        <h3 class="text-lg font-semibold text-neutral-900 mb-2">No categories yet</h3>
        <p class="text-neutral-600 mb-4">Create your first category to start building your menu</p>
        <UiButton @click="addCategory" variant="primary">
          + Add Category
        </UiButton>
      </div>
        </div>
      </div>

      <!-- Preview Panel -->
      <div v-if="showPreview" class="w-1/2">
        <PreviewPanel
          :categories="categories"
          :theme="currentTheme"
          :display-settings="displaySettings"
          :restaurant="restaurant"
          :language="'en'"
          :currency="'USD'"
        />
      </div>
    </div>

    <!-- Category Style Editor Modal -->
    <CategoryStyleEditor
      v-if="styleEditorCategory"
      :show="showStyleEditor"
      :category="styleEditorCategory"
      :restaurant-id="authStore.restaurantId || ''"
      @close="closeStyleEditor"
      @save="handleStyleSave"
    />

    <!-- Add Item Modal -->
    <Modal :show="showItemDialog" @close="closeItemDialog">
      <template #header>
        <h2 class="text-xl font-bold">Add Item to {{ selectedCategory?.name }}</h2>
      </template>

      <div class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Item Name</label>
          <input
            v-model="newItem.name"
            class="w-full input"
            placeholder="e.g., Grilled Chicken"
            @keyup.enter="saveNewItem"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-neutral-700 mb-2">Description</label>
          <textarea
            v-model="newItem.description"
            class="w-full input"
            rows="3"
            placeholder="Brief description of the item"
          ></textarea>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-2">Price</label>
            <input
              v-model.number="newItem.price"
              type="number"
              step="0.01"
              class="w-full input"
              placeholder="0.00"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-neutral-700 mb-2">Image URL (optional)</label>
            <input
              v-model="newItem.imageUrl"
              class="w-full input"
              placeholder="https://..."
            />
          </div>
        </div>
      </div>

      <template #footer>
        <div class="flex justify-end gap-3">
          <UiButton @click="closeItemDialog" variant="secondary">Cancel</UiButton>
          <UiButton @click="saveNewItem" variant="primary">Add Item</UiButton>
        </div>
      </template>
    </Modal>

    <!-- Export Dialog -->
    <ExportDialog
      v-if="showExportDialog"
      :show="showExportDialog"
      :categories="categories"
      :theme="currentTheme"
      :restaurant-name="restaurant.name"
      :logo-url="restaurant.logoUrl"
      @close="showExportDialog = false"
      @exported="handleExported"
    />
  </NuxtLayout>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue'
import { useAuthStore } from '~/stores/auth'
import { useRestaurantStore } from '~/stores/restaurant'
import { useTemplateStore, createDefaultTheme } from '~/stores/templates'
import { useDragDrop } from '~/composables/useDragDrop'
import type { MenuCategory, MenuItem } from '~/stores/restaurant'
import DraggableCategory from '~/components/menu-builder/DraggableCategory.vue'
import CategoryStyleEditor from '~/components/menu/CategoryStyleEditor.vue'
import PreviewPanel from '~/components/menu-builder/PreviewPanel.vue'
import ExportDialog from '~/components/menu/ExportDialog.vue'
import Modal from '~/components/ui/Modal.vue'
import UiButton from '~/components/ui/Button.vue'

definePageMeta({
  layout: false,
  middleware: ['owner']
})

const authStore = useAuthStore()
const restaurantStore = useRestaurantStore()
const templateStore = useTemplateStore()
const { $api } = useNuxtApp()

const loading = ref(true)
const saving = ref(false)
const saved = ref(false)
const categories = ref<MenuCategory[]>([])
const draggedOverIndex = ref<number | null>(null)

// Preview panel state
const showPreview = ref(false)
const currentTheme = ref(createDefaultTheme())
const displaySettings = ref({
  showImages: true,
  showPrices: true,
  showDescriptions: true
})
const restaurant = computed(() => ({
  name: 'Restaurant Preview',
  logoUrl: null
}))

const showStyleEditor = ref(false)
const styleEditorCategory = ref<MenuCategory | null>(null)

const showItemDialog = ref(false)
const showExportDialog = ref(false)
const selectedCategory = ref<MenuCategory | null>(null)
const newItem = ref({
  name: '',
  description: '',
  price: 0,
  imageUrl: '',
  isAvailable: true,
  displayOrder: 1
})

// Drag & Drop handlers
const {
  handleDragStart,
  handleDragOver,
  handleDrop
} = useDragDrop(categories.value, async (reorderedCategories) => {
  categories.value = reorderedCategories
  await saveReorder()
})

const saveReorder = async () => {
  if (!authStore.restaurantId) return

  saving.value = true
  try {
    const api = useApi()
    await api.post(`/restaurants/${authStore.restaurantId}/categories/reorder`,
      categories.value.map((c, i) => ({ id: c.id, displayOrder: i + 1 }))
    )

    showSavedFeedback()
  } catch (error) {
    console.error('Failed to reorder categories:', error)
  } finally {
    saving.value = false
  }
}

const reorderItems = async (categoryId: string, reorderedItems: MenuItem[]) => {
  saving.value = true
  try {
    const api = useApi()
    await api.post(`/categories/${categoryId}/items/reorder`,
      reorderedItems.map((item, i) => ({ id: item.id, displayOrder: i + 1 }))
    )

    showSavedFeedback()
  } catch (error) {
    console.error('Failed to reorder items:', error)
  } finally {
    saving.value = false
  }
}

const addCategory = async () => {
  if (!authStore.restaurantId) return

  try {
    const newCategory = await restaurantStore.createCategory(
      authStore.restaurantId,
      {
        name: 'New Category',
        displayOrder: categories.value.length + 1,
        translations: {}
      }
    )

    await loadCategories()
  } catch (error) {
    console.error('Failed to create category:', error)
  }
}

const updateCategory = async (category: MenuCategory) => {
  saving.value = true
  try {
    await restaurantStore.updateCategory(
      category.id,
      {
        name: category.name,
        displayOrder: category.displayOrder,
        parentCategoryId: category.parentCategoryId,
        translations: category.translations
      },
      authStore.restaurantId || ''
    )

    showSavedFeedback()
  } catch (error) {
    console.error('Failed to update category:', error)
  } finally {
    saving.value = false
  }
}

const deleteCategory = async (categoryId: string) => {
  if (!confirm('Are you sure you want to delete this category? All items will be deleted.')) {
    return
  }

  try {
    await restaurantStore.deleteCategory(categoryId, authStore.restaurantId || '')
    await loadCategories()
  } catch (error) {
    console.error('Failed to delete category:', error)
  }
}

const openStyleEditor = (category: MenuCategory) => {
  styleEditorCategory.value = category
  showStyleEditor.value = true
}

const closeStyleEditor = () => {
  showStyleEditor.value = false
  styleEditorCategory.value = null
}

const handleStyleSave = async (style: any) => {
  // Style is already saved by the editor component
  showSavedFeedback()
  await loadCategories()
}

const openItemDialog = (category: MenuCategory) => {
  selectedCategory.value = category
  newItem.value = {
    name: '',
    description: '',
    price: 0,
    imageUrl: '',
    isAvailable: true,
    displayOrder: (category.items?.length || 0) + 1
  }
  showItemDialog.value = true
}

const closeItemDialog = () => {
  showItemDialog.value = false
  selectedCategory.value = null
}

const saveNewItem = async () => {
  if (!selectedCategory.value || !newItem.value.name) return

  try {
    await restaurantStore.createMenuItem(selectedCategory.value.id, {
      name: newItem.value.name,
      description: newItem.value.description || null,
      price: newItem.value.price,
      imageUrl: newItem.value.imageUrl || null,
      isAvailable: newItem.value.isAvailable,
      displayOrder: newItem.value.displayOrder,
      translations: {}
    })

    await loadCategories()
    closeItemDialog()
  } catch (error) {
    console.error('Failed to create item:', error)
  }
}

const handleExported = () => {
  showSavedFeedback()
}

const loadCategories = async () => {
  if (!authStore.restaurantId) return

  loading.value = true
  try {
    const loadedCategories = await restaurantStore.fetchCategories(authStore.restaurantId)
    categories.value = loadedCategories
  } catch (error) {
    console.error('Failed to load categories:', error)
  } finally {
    loading.value = false
  }
}

const showSavedFeedback = () => {
  saved.value = true
  setTimeout(() => {
    saved.value = false
  }, 2000)
}

onMounted(async () => {
  await loadCategories()
})
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
