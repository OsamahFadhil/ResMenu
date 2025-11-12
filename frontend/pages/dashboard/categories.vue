<template>
  <NuxtLayout name="dashboard">
    <div class="space-y-6">
      <!-- Header with Search -->
      <div class="flex flex-col gap-4 justify-between items-start sm:flex-row sm:items-center">
        <h1 class="text-2xl font-bold text-slate-800">{{ $t('menu.categories') }}</h1>
        <div class="flex gap-3 items-center w-full sm:w-auto">
          <div class="relative flex-1 sm:w-64">
            <input
              v-model="searchQuery"
              type="text"
              :placeholder="$t('common.search')"
              class="py-2 pr-4 pl-10 w-full rounded-lg border border-gray-300 focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
            />
            <svg class="absolute top-2.5 left-3 w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </div>
          <button 
            @click.stop.prevent="openCreateModal" 
            type="button"
            class="inline-flex items-center justify-center font-medium rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed bg-indigo-600 text-white hover:bg-indigo-700 focus:ring-indigo-500 px-4 py-2 text-sm"
            :disabled="!restaurantId"
          >
            <svg class="mr-2 w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
            </svg>
            {{ $t('menu.addCategory') }}
          </button>
        </div>
      </div>

      <!-- Categories Tree -->
      <Card>
        <div v-if="loading" class="flex justify-center py-12">
          <LoadingSpinner size="lg" />
        </div>

        <div v-else-if="filteredCategories.length === 0 && !searchQuery">
          <EmptyState
            :title="$t('messages.noData')"
            :description="$t('menu.addCategory')"
          >
            <template #action>
              <UiButton @click="openCreateModal" variant="primary">
                {{ $t('menu.addCategory') }}
              </UiButton>
            </template>
          </EmptyState>
        </div>

        <div v-else-if="filteredCategories.length === 0 && searchQuery">
          <EmptyState
            :title="$t('messages.noResults')"
            description="Try adjusting your search"
          />
        </div>

        <div v-else class="space-y-2">
          <MenuCategoryTree
            :categories="paginatedCategories"
            :showItemForms="showItemForms"
            :itemForms="itemForms"
            :onToggleItemForm="toggleItemForm"
            :onSaveItem="createMenuItemForCategory"
            :onAddSubcategory="startAddSubcategory"
            :onEditCategory="editCategory"
            :onDeleteCategory="deleteCategory"
            :onAddItem="openAddItemModal"
          />
          
          <!-- Pagination -->
          <div v-if="totalPages > 1" class="flex justify-between items-center pt-4 mt-6 border-t">
            <div class="text-sm text-gray-700">
              {{ $t('pagination.showing') }} {{ startIndex + 1 }} {{ $t('pagination.to') }} {{ Math.min(endIndex, filteredCategories.length) }} {{ $t('pagination.of') }} {{ filteredCategories.length }}
            </div>
            <div class="flex gap-2">
              <button
                @click="currentPage--"
                :disabled="currentPage === 1"
                class="px-3 py-1 rounded-md border disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
              >
                {{ $t('pagination.previous') }}
              </button>
              <button
                v-for="page in visiblePages"
                :key="page"
                @click="currentPage = page"
                :class="[
                  'px-3 py-1 border rounded-md',
                  currentPage === page ? 'bg-indigo-600 text-white' : 'hover:bg-gray-50'
                ]"
              >
                {{ page }}
              </button>
              <button
                @click="currentPage++"
                :disabled="currentPage === totalPages"
                class="px-3 py-1 rounded-md border disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
              >
                {{ $t('pagination.next') }}
              </button>
            </div>
          </div>
        </div>
      </Card>

      <!-- Create/Edit Category Modal -->
      <div v-if="showCreateModal" class="fixed inset-0 z-[10000] flex items-center justify-center bg-black bg-opacity-50" @click.self="showCreateModal = false">
        <div class="bg-white rounded-lg shadow-xl w-full max-w-lg mx-4 transform transition-all">
          <!-- Header -->
          <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
            <h3 class="text-lg font-semibold text-gray-900">
              {{ editingCategory ? $t('menu.editCategory') : $t('menu.addCategory') }}
            </h3>
            <button
              @click="showCreateModal = false"
              type="button"
              class="text-gray-400 hover:text-gray-500 focus:outline-none"
            >
              <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <!-- Body -->
          <div class="px-6 py-4 space-y-4">
            <div>
              <label class="block mb-1 text-sm font-medium text-gray-700">
                {{ $t('menu.categoryName') }} <span class="text-red-500">*</span>
              </label>
              <input
                v-model="form.name"
                type="text"
                required
                class="block px-4 py-2 w-full rounded-lg border border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                :placeholder="$t('menu.categoryName')"
              />
            </div>
            <div>
              <label class="block mb-1 text-sm font-medium text-gray-700">
                {{ $t('menu.parentCategory') }}
              </label>
              <div class="relative">
                <select
                  v-model="form.parentId"
                  class="block py-2 pr-10 pl-4 w-full rounded-md border border-gray-300 shadow-sm appearance-none focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                >
                  <option :value="null">{{ $t('menu.noCategory') }}</option>
                  <option
                    v-for="option in categoryOptions"
                    :key="option.id"
                    :value="option.id"
                  >
                    {{ option.label }}
                  </option>
                </select>
                <svg class="absolute inset-y-0 right-3 my-auto w-4 h-4 text-gray-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </div>
            </div>
            <div>
              <label class="block mb-1 text-sm font-medium text-gray-700">
                {{ $t('menu.sortOrder') }}
              </label>
              <input
                v-model.number="form.sortOrder"
                type="number"
                min="0"
                class="block px-4 py-2 w-full rounded-lg border border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                :placeholder="$t('menu.sortOrder')"
              />
            </div>
          </div>

          <!-- Footer -->
          <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex justify-end gap-3">
            <button
              @click="showCreateModal = false"
              type="button"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              {{ $t('common.cancel') }}
            </button>
            <button
              @click="saveCategory"
              type="button"
              :disabled="saving"
              class="px-4 py-2 text-sm font-medium text-white bg-indigo-600 border border-transparent rounded-lg hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="saving">Saving...</span>
              <span v-else>{{ $t('common.save') }}</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Add Item Modal -->
      <div v-if="showAddItemModal" class="fixed inset-0 z-[10000] flex items-center justify-center bg-black bg-opacity-50" @click.self="showAddItemModal = false">
        <div class="bg-white rounded-lg shadow-xl w-full max-w-2xl mx-4 max-h-[90vh] overflow-y-auto transform transition-all">
          <!-- Header -->
          <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between sticky top-0 bg-white">
            <h3 class="text-lg font-semibold text-gray-900">
              {{ $t('menu.addItem') }}
            </h3>
            <button
              @click="showAddItemModal = false"
              type="button"
              class="text-gray-400 hover:text-gray-500 focus:outline-none"
            >
              <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <!-- Body -->
          <div class="px-6 py-4 space-y-4">
            <div>
              <label class="block mb-1 text-sm font-medium text-gray-700">
                {{ $t('menu.itemName') }} <span class="text-red-500">*</span>
              </label>
              <input
                v-model="newItemForm.name"
                type="text"
                required
                class="block px-4 py-2 w-full rounded-lg border border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                :placeholder="$t('menu.itemName')"
              />
            </div>
            <div>
              <label class="block mb-1 text-sm font-medium text-gray-700">
                {{ $t('menu.description') }}
              </label>
              <textarea
                v-model="newItemForm.description"
                rows="3"
                class="block px-4 py-2 w-full rounded-lg border border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                :placeholder="$t('menu.description')"
              ></textarea>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block mb-1 text-sm font-medium text-gray-700">
                  {{ $t('menu.price') }} <span class="text-red-500">*</span>
                </label>
                <input
                  v-model.number="newItemForm.price"
                  type="number"
                  step="0.01"
                  min="0"
                  required
                  class="block px-4 py-2 w-full rounded-lg border border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                  :placeholder="$t('menu.price')"
                />
              </div>
              <div>
                <label class="block mb-1 text-sm font-medium text-gray-700">
                  {{ $t('menu.sortOrder') }}
                </label>
                <input
                  v-model.number="newItemForm.displayOrder"
                  type="number"
                  min="0"
                  class="block px-4 py-2 w-full rounded-lg border border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                  :placeholder="$t('menu.sortOrder')"
                />
              </div>
            </div>
            <div class="w-full">
              <FileUpload
                v-model="newItemForm.imageUrl"
                :label="$t('menu.imageUrl') || 'Image'"
                accept="image/*"
                :max-size="5"
                :disabled="uploadingImage"
                :uploading="uploadingImage"
                @upload="handleImageUpload"
              />
            </div>
            <div class="flex items-center">
              <input
                v-model="newItemForm.isAvailable"
                type="checkbox"
                id="item-available"
                class="w-4 h-4 text-indigo-600 rounded border-gray-300 focus:ring-indigo-500"
              />
              <label for="item-available" class="ml-2 text-sm text-gray-700">
                {{ $t('menu.isAvailable') }}
              </label>
            </div>
          </div>

          <!-- Footer -->
          <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex justify-end gap-3 sticky bottom-0 bg-white">
            <button
              @click="showAddItemModal = false"
              type="button"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              {{ $t('common.cancel') }}
            </button>
            <button
              @click="saveNewItem"
              type="button"
              :disabled="saving"
              class="px-4 py-2 text-sm font-medium text-white bg-indigo-600 border border-transparent rounded-lg hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="saving">Saving...</span>
              <span v-else>{{ $t('common.save') }}</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import type { MenuCategory } from '@/stores/restaurant'
import FileUpload from '@/components/ui/FileUpload.vue'

definePageMeta({
  layout: false,
  middleware: ['owner']
})

const { t, locale } = useI18n({ useScope: 'global' })
const authStore = useAuthStore()
const restaurantStore = useRestaurantStore()

const loading = ref(false)
const saving = ref(false)
const showCreateModal = ref(false)
const showAddItemModal = ref(false)
const selectedCategoryForItem = ref<string | null>(null)
const editingCategory = ref<MenuCategory | null>(null)
const showItemForms = ref<Record<string, boolean>>({})
const searchQuery = ref('')
const currentPage = ref(1)
const itemsPerPage = ref(10)
const itemForms = ref<Record<
  string,
  {
    name: string
    description?: string | null
    price: number
    imageUrl?: string | null
    isAvailable: boolean
    displayOrder: number
  }
>>({})

const form = ref({
  name: '',
  parentId: null as string | null,
  sortOrder: 0,
  translations: {
    en: '',
    ar: ''
  }
})

const newItemForm = ref({
  name: '',
  description: '',
  price: 0,
  imageUrl: '',
  isAvailable: true,
  displayOrder: 0
})

const categories = computed(() => restaurantStore.categories)

// Flatten categories for search
const flattenCategories = (cats: MenuCategory[]): MenuCategory[] => {
  const result: MenuCategory[] = []
  const flatten = (list: MenuCategory[]) => {
    for (const cat of list) {
      result.push(cat)
      if (cat.children.length) {
        flatten(cat.children)
      }
    }
  }
  flatten(cats)
  return result
}

const filteredCategories = computed(() => {
  if (!searchQuery.value) {
    return categories.value
  }
  
  const query = searchQuery.value.toLowerCase()
  const flat = flattenCategories(categories.value)
  return flat.filter(cat => 
    (cat.name?.toLowerCase().includes(query)) ||
    (cat.localizedName?.toLowerCase().includes(query))
  )
})

const totalPages = computed(() => Math.ceil(filteredCategories.value.length / itemsPerPage.value))
const startIndex = computed(() => (currentPage.value - 1) * itemsPerPage.value)
const endIndex = computed(() => startIndex.value + itemsPerPage.value)

const paginatedCategories = computed(() => {
  if (!searchQuery.value) {
    return filteredCategories.value.slice(startIndex.value, endIndex.value)
  }
  return filteredCategories.value.slice(startIndex.value, endIndex.value)
})

const visiblePages = computed(() => {
  const pages = []
  const maxVisible = 5
  let start = Math.max(1, currentPage.value - Math.floor(maxVisible / 2))
  let end = Math.min(totalPages.value, start + maxVisible - 1)
  
  if (end - start < maxVisible - 1) {
    start = Math.max(1, end - maxVisible + 1)
  }
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  return pages
})

const categoryOptions = computed(() => {
  const options: { id: string; label: string }[] = []

  const buildOptions = (list: MenuCategory[], prefix = '') => {
    for (const category of list) {
      options.push({
        id: category.id,
        label: `${prefix}${category.localizedName || category.name}`
      })

      if (category.children.length) {
        buildOptions(category.children, `${prefix}› `)
      }
    }
  }

  buildOptions(categories.value)
  return options
})

const restaurantId = computed(() => authStore.restaurantId || '')

watch(searchQuery, () => {
  currentPage.value = 1
})

const openCreateModal = (event?: Event) => {
  event?.preventDefault()
  event?.stopPropagation()
  
  if (!restaurantId.value) {
    alert(t('messages.errorOccurred') || 'Restaurant ID not found. Please login again.')
    return
  }
  
  resetForm()
  editingCategory.value = null
  showCreateModal.value = true
}

const openAddItemModal = (categoryId: string) => {
  if (!categoryId) {
    alert(t('messages.errorOccurred') || 'Category ID not found.')
    return
  }
  selectedCategoryForItem.value = categoryId
  // Reset form
  newItemForm.value = {
    name: '',
    description: '',
    price: 0,
    imageUrl: '',
    isAvailable: true,
    displayOrder: 0
  }
  showAddItemModal.value = true
}

const resetForm = () => {
  form.value = {
    name: '',
    parentId: null,
    sortOrder: 0,
    translations: {
      en: '',
      ar: ''
    }
  }
  editingCategory.value = null
}

const loadCategories = async () => {
  if (!restaurantId.value) {
    return
  }

  loading.value = true
  try {
    await restaurantStore.fetchCategories(restaurantId.value, locale.value)
  } catch (error: any) {
    console.error('Failed to load categories:', error)
    const errorMessage = error?.response?.data?.message || error?.message || t('messages.errorOccurred') || 'Failed to load categories'
    alert(errorMessage)
  } finally {
    loading.value = false
  }
}

const ensureItemForm = (categoryId: string) => {
  if (!itemForms.value[categoryId]) {
    itemForms.value[categoryId] = {
      name: '',
      description: '',
      price: 0,
      imageUrl: '',
      isAvailable: true,
      displayOrder: 0
    }
  }
}

const toggleItemForm = (categoryId: string) => {
  showItemForms.value[categoryId] = !showItemForms.value[categoryId]
  if (showItemForms.value[categoryId]) {
    ensureItemForm(categoryId)
  }
}

const resetItemForm = (categoryId: string) => {
  itemForms.value[categoryId] = {
    name: '',
    description: '',
    price: 0,
    imageUrl: '',
    isAvailable: true,
    displayOrder: 0
  }
}

const startAddSubcategory = (categoryId: string) => {
  resetForm()
  form.value.parentId = categoryId
  showCreateModal.value = true
}

const editCategory = (category: MenuCategory) => {
  editingCategory.value = category
  form.value = {
    name: category.name,
    parentId: category.parentCategoryId ?? null,
    sortOrder: category.displayOrder ?? 0,
    translations: {
      en: category.translations?.en ?? '',
      ar: category.translations?.ar ?? ''
    }
  }
  showCreateModal.value = true
}

const deleteCategory = async (category: MenuCategory) => {
  if (!restaurantId.value) {
    return
  }
  const confirmation = confirm(t('messages.confirmDelete'))
  if (!confirmation) {
    return
  }

  try {
    await restaurantStore.deleteCategory(category.id, restaurantId.value, locale.value)
    await loadCategories()
    alert(t('messages.success') || 'Category deleted successfully!')
  } catch (error: any) {
    console.error('Failed to delete category:', error)
    const errorMessage = error?.response?.data?.message || error?.message || t('messages.errorOccurred') || 'Failed to delete category'
    alert(errorMessage)
  }
}

const saveCategory = async () => {
  if (!restaurantId.value) {
    return
  }

  const payloadTranslations: Record<string, string> = {}
  if (form.value.translations.en?.trim()) {
    payloadTranslations.en = form.value.translations.en.trim()
  }
  if (form.value.translations.ar?.trim()) {
    payloadTranslations.ar = form.value.translations.ar.trim()
  }

  const payload = {
    name: form.value.name.trim(),
    displayOrder: Number.isFinite(form.value.sortOrder) ? Number(form.value.sortOrder) : 0,
    parentCategoryId: form.value.parentId || null,
    translations: Object.keys(payloadTranslations).length ? payloadTranslations : undefined
  }

  if (!payload.name) {
    alert(t('validation.required'))
    return
  }

  saving.value = true
  try {
    if (editingCategory.value) {
      await restaurantStore.updateCategory(editingCategory.value.id, payload, restaurantId.value, locale.value)
    } else {
      await restaurantStore.createCategory(restaurantId.value, payload, locale.value)
    }
    await loadCategories()
    showCreateModal.value = false
    resetForm()
    alert(t('messages.success') || 'Category saved successfully!')
  } catch (error: any) {
    console.error('Failed to save category:', error)
    const errorMessage = error?.response?.data?.message || error?.message || t('messages.errorOccurred') || 'Failed to save category'
    alert(errorMessage)
  } finally {
    saving.value = false
  }
}

const uploadingImage = ref(false)

const handleImageUpload = async (file: File) => {
  uploadingImage.value = true
  const api = useApi()
  const formData = new FormData()
  formData.append('file', file)

  try {
    const response = await api.post('/Files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })

    // The API returns Result<string> with success and data properties
    if (response.data.success) {
      newItemForm.value.imageUrl = response.data.data
    } else {
      throw new Error(response.data.message || 'Upload failed')
    }
  } catch (error: any) {
    console.error('Failed to upload image:', error)
    const errorMessage = error?.response?.data?.message || error?.response?.data?.error || error?.message || 'Failed to upload image'
    alert(errorMessage)
    newItemForm.value.imageUrl = ''
  } finally {
    uploadingImage.value = false
  }
}

const saveNewItem = async () => {
  if (!selectedCategoryForItem.value) {
    alert(t('messages.errorOccurred') || 'Category not selected.')
    return
  }

  if (!newItemForm.value.name.trim()) {
    alert(t('validation.required') || 'Item name is required')
    return
  }

  const price = Number(newItemForm.value.price)
  if (!price || price <= 0 || !isFinite(price)) {
    alert(t('validation.required') || 'Price must be greater than 0')
    return
  }

  saving.value = true
  try {
    await restaurantStore.createMenuItem(selectedCategoryForItem.value, {
      name: newItemForm.value.name.trim(),
      description: newItemForm.value.description?.trim() || null,
      price: price,
      imageUrl: newItemForm.value.imageUrl?.trim() || null,
      isAvailable: newItemForm.value.isAvailable,
      displayOrder: Number(newItemForm.value.displayOrder) || 0
    })
    
    // Reset form
    newItemForm.value = {
      name: '',
      description: '',
      price: 0,
      imageUrl: '',
      isAvailable: true,
      displayOrder: 0
    }
    
    showAddItemModal.value = false
    selectedCategoryForItem.value = null
    await loadCategories()
    alert(t('messages.success') || 'Menu item created successfully!')
  } catch (error: any) {
    console.error('Failed to create menu item:', error)
    const errorMessage = error?.response?.data?.message || error?.message || t('messages.errorOccurred') || 'Failed to create menu item'
    alert(errorMessage)
  } finally {
    saving.value = false
  }
}

const createMenuItemForCategory = async (categoryId: string) => {
  const formState = itemForms.value[categoryId]
  if (!formState) {
    return
  }

  if (!formState.name.trim()) {
    alert(t('validation.required') || 'Item name is required')
    return
  }

  try {
    await restaurantStore.createMenuItem(categoryId, {
      name: formState.name.trim(),
      description: formState.description?.trim() || null,
      price: Number(formState.price) || 0,
      imageUrl: formState.imageUrl || null,
      isAvailable: formState.isAvailable,
      displayOrder: Number(formState.displayOrder) || 0
    })
    resetItemForm(categoryId)
    showItemForms.value[categoryId] = false
    await loadCategories()
  } catch (error: any) {
    console.error('Failed to create menu item:', error)
    const errorMessage = error?.response?.data?.message || error?.message || t('messages.errorOccurred') || 'Failed to create menu item'
    alert(errorMessage)
  }
}

onMounted(async () => {
  authStore.loadFromStorage()
  if (!authStore.isAuthenticated) {
    navigateTo('/login')
    return
  }
  await loadCategories()
})

watch(
  () => locale.value,
  async () => {
    await loadCategories()
  }
)

watch(
  () => authStore.restaurantId,
  async (newId, oldId) => {
    if (newId && newId !== oldId) {
      await loadCategories()
    }
  }
)
</script>
