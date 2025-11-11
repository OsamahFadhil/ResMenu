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
          <UiButton @click="openCreateModal" variant="primary">
            <svg class="mr-2 w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
            </svg>
            {{ $t('menu.addCategory') }}
          </UiButton>
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

      <!-- Create/Edit Modal -->
      <Modal v-model="showCreateModal" :title="editingCategory ? $t('menu.editCategory') : $t('menu.addCategory')">
        <div class="space-y-4">
          <Input
            v-model="form.name"
            :label="$t('menu.categoryName')"
            required
          />
          <div>
            <label class="block mb-1 text-sm font-medium text-gray-700">
              {{ $t('menu.parentCategory') }}
            </label>
            <div class="relative">
              <select
                v-model="form.parentId"
                class="block py-2 pr-10 pl-4 w-full rounded-md border-gray-300 shadow-sm appearance-none focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
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
          <Input
            v-model="form.sortOrder"
            type="number"
            :label="$t('menu.sortOrder')"
          />
        </div>

        <template #footer>
          <UiButton @click="showCreateModal = false" variant="secondary">
            {{ $t('common.cancel') }}
          </UiButton>
          <UiButton @click="saveCategory" variant="primary" :loading="saving">
            {{ $t('common.save') }}
          </UiButton>
        </template>
      </Modal>

      <!-- Add Item Modal -->
      <Modal v-model="showAddItemModal" :title="$t('menu.addItem')">
        <div class="space-y-4">
          <Input
            v-model="newItemForm.name"
            :label="$t('menu.itemName')"
            required
          />
          <div>
            <label class="block mb-1 text-sm font-medium text-gray-700">
              {{ $t('menu.description') }}
            </label>
            <textarea
              v-model="newItemForm.description"
              rows="3"
              class="block px-4 py-2 w-full rounded-lg border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
            ></textarea>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <Input
              v-model="newItemForm.price"
              type="number"
              step="0.01"
              :label="$t('menu.price')"
              required
            />
            <Input
              v-model="newItemForm.displayOrder"
              type="number"
              :label="$t('menu.sortOrder')"
            />
          </div>
          <FileUpload
            v-model="newItemForm.imageUrl"
            :label="$t('menu.imageUrl')"
            @upload="handleImageUpload"
          />
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

        <template #footer>
          <UiButton @click="showAddItemModal = false" variant="secondary">
            {{ $t('common.cancel') }}
          </UiButton>
          <UiButton @click="saveNewItem" variant="primary" :loading="saving">
            {{ $t('common.save') }}
          </UiButton>
        </template>
      </Modal>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import type { MenuCategory } from '@/stores/restaurant'

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

const openCreateModal = () => {
  resetForm()
  showCreateModal.value = true
}

const openAddItemModal = (categoryId: string) => {
  selectedCategoryForItem.value = categoryId
  ensureItemForm(categoryId)
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
  } catch (error) {
    console.error('Failed to load categories:', error)
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
  } catch (error) {
    console.error('Failed to delete category:', error)
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
  } catch (error) {
    console.error('Failed to save category:', error)
  } finally {
    saving.value = false
  }
}

const handleImageUpload = async (file: File) => {
  // For now, convert to base64 data URL
  // In production, you'd upload to a cloud storage service
  const reader = new FileReader()
  reader.onload = (e) => {
    newItemForm.value.imageUrl = e.target?.result as string
  }
  reader.readAsDataURL(file)
}

const saveNewItem = async () => {
  if (!selectedCategoryForItem.value) {
    return
  }

  if (!newItemForm.value.name.trim()) {
    alert(t('validation.required'))
    return
  }

  saving.value = true
  try {
    await restaurantStore.createMenuItem(selectedCategoryForItem.value, {
      name: newItemForm.value.name.trim(),
      description: newItemForm.value.description?.trim() || null,
      price: Number(newItemForm.value.price) || 0,
      imageUrl: newItemForm.value.imageUrl || null,
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
  } catch (error) {
    console.error('Failed to create menu item:', error)
    alert(t('messages.errorOccurred'))
  } finally {
    saving.value = false
  }
}

const createMenuItemForCategory = async (categoryId: string) => {
  const formState = itemForms.value[categoryId]
  if (!formState) {
    return
  }

  try {
    await restaurantStore.createMenuItem(categoryId, {
      name: formState.name,
      description: formState.description,
      price: Number(formState.price) || 0,
      imageUrl: formState.imageUrl,
      isAvailable: formState.isAvailable,
      displayOrder: Number(formState.displayOrder) || 0
    })
    resetItemForm(categoryId)
    showItemForms.value[categoryId] = false
    await loadCategories()
  } catch (error) {
    console.error('Failed to create menu item:', error)
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
