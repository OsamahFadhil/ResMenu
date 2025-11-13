<template>
  <NuxtLayout name="dashboard">
    <div class="space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-slate-800">{{ $t('templates.title') }}</h1>
          <p class="mt-1 text-sm text-gray-600">Create reusable menu templates for quick setup</p>
        </div>
        <UiButton @click="openCreateModal" variant="primary">
          <svg class="mr-2 w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          {{ $t('templates.create') }}
        </UiButton>
      </div>

      <!-- Loading State -->
      <div v-if="templateStore.loading" class="flex justify-center py-12">
        <LoadingSpinner size="lg" />
      </div>

      <!-- Templates Grid -->
      <div v-else-if="templateStore.templates.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <Card
          v-for="template in templateStore.templates"
          :key="template.id"
          class="hover:shadow-lg transition-shadow"
        >
          <div class="space-y-4">
            <!-- Header -->
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <h3 class="text-lg font-semibold text-gray-900">{{ template.name }}</h3>
                <p class="mt-1 text-sm text-gray-500 line-clamp-2">{{ template.description || 'No description' }}</p>
              </div>
              <span
                :class="[
                  'px-2 py-1 text-xs font-medium rounded-full',
                  template.isPublished ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'
                ]"
              >
                {{ template.isPublished ? 'Published' : 'Draft' }}
              </span>
            </div>

            <!-- Stats -->
            <div class="flex items-center gap-4 text-sm text-gray-600">
              <div class="flex items-center gap-1">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
                </svg>
                <span>{{ template.structure.categories.length }} categories</span>
              </div>
              <div class="flex items-center gap-1">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <span>{{ countItems(template) }} items</span>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex gap-2 pt-4 border-t">
              <UiButton @click="openEditModal(template)" variant="secondary" size="sm" class="flex-1">
                {{ $t('common.edit') }}
              </UiButton>
              <UiButton @click="generateFromTemplate(template.id)" variant="primary" size="sm" class="flex-1">
                Generate
              </UiButton>
              <button
                @click="deleteTemplate(template.id)"
                class="px-3 py-2 text-red-600 hover:bg-red-50 rounded-lg transition"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
              </button>
            </div>
          </div>
        </Card>
      </div>

      <!-- Empty State -->
      <EmptyState
        v-else
        title="No templates yet"
        description="Create your first template to quickly generate menus"
      >
        <template #action>
          <UiButton @click="openCreateModal" variant="primary">
            {{ $t('templates.create') }}
          </UiButton>
        </template>
      </EmptyState>
    </div>

    <!-- Create/Edit Modal -->
    <Modal
      v-model="showModal"
      :title="editingTemplate ? $t('templates.edit') : $t('templates.create')"
      size="xl"
    >
      <div class="space-y-6 max-h-[70vh] overflow-y-auto pr-2">
        <!-- Basic Info -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Input
            v-model="form.name"
            :label="$t('templates.name')"
            required
            placeholder="e.g., Italian Restaurant Menu"
          />
          <div class="flex items-center gap-2">
            <input
              v-model="form.isPublished"
              type="checkbox"
              id="published"
              class="w-4 h-4 text-indigo-600 rounded border-gray-300 focus:ring-indigo-500"
            />
            <label for="published" class="text-sm text-gray-700">
              Publish template
            </label>
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('templates.description') }}
          </label>
          <textarea
            v-model="form.description"
            rows="2"
            class="block w-full rounded-lg border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm px-4 py-2"
            placeholder="Describe this template..."
          ></textarea>
        </div>

        <!-- Theme Customization -->
        <div class="space-y-6 border-t pt-6">
          <h3 class="text-lg font-semibold text-neutral-900">Theme Customization</h3>
          
          <!-- Theme Customizer Component -->
          <ThemeCustomizer v-model="form.theme" />
          
          <!-- Layout Customizer Component -->
          <LayoutCustomizer v-model="form.theme" />
        </div>

        <!-- Categories -->
        <div>
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-sm font-semibold text-gray-900">Categories & Items</h3>
            <UiButton @click="addCategory" variant="secondary" size="sm">
              + Add Category
            </UiButton>
          </div>

          <div v-if="form.structure.categories.length === 0" class="text-center py-8 text-gray-500">
            No categories yet. Click "Add Category" to start.
          </div>

          <div v-else class="space-y-4">
            <div
              v-for="(category, catIndex) in form.structure.categories"
              :key="catIndex"
              class="border border-gray-200 rounded-lg p-4 bg-gray-50"
            >
              <!-- Category Header -->
              <div class="flex items-start gap-3 mb-3">
                <input
                  v-model="category.icon"
                  type="text"
                  maxlength="2"
                  class="w-12 h-12 text-center text-xl border border-gray-300 rounded-lg"
                  placeholder="🍕"
                />
                <div class="flex-1 grid grid-cols-2 gap-3">
                  <Input
                    v-model="category.name"
                    label="Category Name"
                    placeholder="e.g., Appetizers"
                  />
                  <Input
                    v-model="category.translations.ar"
                    label="Arabic Name"
                    placeholder="المقبلات"
                  />
                </div>
                <button
                  @click="removeCategory(catIndex)"
                  class="mt-6 p-2 text-red-600 hover:bg-red-50 rounded-lg"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                </button>
              </div>

              <!-- Items -->
              <div class="ml-14 space-y-3">
                <div class="flex items-center justify-between">
                  <span class="text-xs font-medium text-gray-600">Items ({{ category.items.length }})</span>
                  <button
                    @click="addItem(catIndex)"
                    class="text-xs text-indigo-600 hover:text-indigo-700 font-medium"
                  >
                    + Add Item
                  </button>
                </div>

                <div
                  v-for="(item, itemIndex) in category.items"
                  :key="itemIndex"
                  class="border border-gray-200 rounded-lg p-3 bg-white space-y-3"
                >
                  <div class="grid grid-cols-2 gap-3">
                    <Input
                      v-model="item.name"
                      label="Item Name"
                      placeholder="e.g., Margherita Pizza"
                    />
                    <Input
                      v-model.number="item.price"
                      label="Price"
                      type="number"
                      step="0.01"
                      placeholder="12.99"
                    />
                  </div>
                  <div class="grid grid-cols-2 gap-3">
                    <Input
                      v-model="item.nameTranslations.ar"
                      label="Arabic Name"
                      placeholder="بيتزا مارغريتا"
                    />
                    <Input
                      v-model="item.description"
                      label="Description"
                      placeholder="Fresh mozzarella..."
                    />
                  </div>
                  <div class="flex items-center justify-between">
                    <div class="flex items-center gap-2">
                      <input
                        v-model="item.isAvailable"
                        type="checkbox"
                        :id="`available-${catIndex}-${itemIndex}`"
                        class="w-4 h-4 text-indigo-600 rounded border-gray-300"
                      />
                      <label :for="`available-${catIndex}-${itemIndex}`" class="text-sm text-gray-700">
                        Available
                      </label>
                    </div>
                    <button
                      @click="removeItem(catIndex, itemIndex)"
                      class="text-sm text-red-600 hover:text-red-700"
                    >
                      Remove
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <template #footer>
        <UiButton @click="showModal = false" variant="secondary">
          {{ $t('common.cancel') }}
        </UiButton>
        <UiButton @click="saveTemplate" variant="primary" :loading="saving">
          {{ $t('common.save') }}
        </UiButton>
      </template>
    </Modal>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { useTemplateStore, type MenuTemplate, type UpsertTemplatePayload, createDefaultTheme } from '@/stores/templates'
import ThemeCustomizer from '~/components/menu/ThemeCustomizer.vue'
import LayoutCustomizer from '~/components/menu/LayoutCustomizer.vue'

definePageMeta({
  layout: false,
  middleware: ['owner']
})

const { t } = useI18n({ useScope: 'global' })
const templateStore = useTemplateStore()
const restaurantStore = useRestaurantStore()
const authStore = useAuthStore()
const toast = useToast()

const showModal = ref(false)
const saving = ref(false)
const editingTemplate = ref<MenuTemplate | null>(null)

const form = ref<UpsertTemplatePayload>(createBlankForm())

function createBlankForm(): UpsertTemplatePayload {
  return {
    name: '',
    description: '',
    isPublished: false,
    tags: [],
    theme: createDefaultTheme(),
    structure: {
      categories: []
    }
  }
}

const countItems = (template: MenuTemplate) => {
  return template.structure.categories.reduce((total, cat) => total + cat.items.length, 0)
}

const openCreateModal = () => {
  console.log('=== OPENING CREATE MODAL ===')
  editingTemplate.value = null
  form.value = createBlankForm()
  console.log('Initial form:', form.value)
  showModal.value = true
}

const openEditModal = (template: MenuTemplate) => {
  editingTemplate.value = template
  form.value = {
    name: template.name,
    description: template.description || '',
    isPublished: template.isPublished,
    tags: [...(template.tags || [])],
    theme: template.theme ? { ...template.theme } : createDefaultTheme(),
    structure: JSON.parse(JSON.stringify(template.structure))
  }
  showModal.value = true
}

const addCategory = () => {
  form.value.structure.categories.push({
    name: '',
    translations: {},
    displayOrder: form.value.structure.categories.length + 1,
    icon: '🍽️',
    items: []
  })
}

const removeCategory = (index: number) => {
  form.value.structure.categories.splice(index, 1)
}

const addItem = (categoryIndex: number) => {
  form.value.structure.categories[categoryIndex].items.push({
    name: '',
    nameTranslations: {},
    description: '',
    descriptionTranslations: {},
    price: 0,
    imageUrl: '',
    isAvailable: true,
    displayOrder: form.value.structure.categories[categoryIndex].items.length + 1,
    tags: []
  })
}

const removeItem = (categoryIndex: number, itemIndex: number) => {
  form.value.structure.categories[categoryIndex].items.splice(itemIndex, 1)
}

const saveTemplate = async () => {
  console.log('=== SAVE TEMPLATE CALLED ===')
  console.log('Form data:', JSON.stringify(form.value, null, 2))
  
  if (!form.value.name.trim()) {
    toast.error('Please enter a template name')
    return
  }

  if (form.value.structure.categories.length === 0) {
    toast.error('Please add at least one category')
    return
  }

  saving.value = true
  console.log('Starting save process...')
  
  try {
    if (editingTemplate.value) {
      console.log('Updating template:', editingTemplate.value.id)
      await templateStore.updateTemplate(editingTemplate.value.id, form.value)
      toast.success('Template updated successfully!')
    } else {
      console.log('Creating new template...')
      const result = await templateStore.createTemplate(form.value)
      console.log('Template created:', result)
      toast.success('Template created successfully!')
    }
    showModal.value = false
    form.value = createBlankForm()
  } catch (error: any) {
    console.error('Failed to save template:', error)
    console.error('Error details:', error.response?.data || error.message)
    toast.error(error.message || 'Failed to save template')
  } finally {
    saving.value = false
    console.log('=== SAVE TEMPLATE FINISHED ===')
  }
}

const deleteTemplate = async (id: string) => {
  if (!confirm(t('messages.confirmDelete'))) {
    return
  }

  try {
    await templateStore.deleteTemplate(id)
    toast.success('Template deleted successfully!')
  } catch (error: any) {
    console.error('Failed to delete template:', error)
    toast.error(error.message || 'Failed to delete template')
  }
}

const generateFromTemplate = async (templateId: string) => {
  const restaurantId = authStore.restaurantId
  if (!restaurantId) {
    toast.error('Restaurant not found')
    return
  }

  if (!confirm('This will generate a menu from this template. Continue?')) {
    return
  }

  try {
    await restaurantStore.generateMenu(restaurantId, {
      templateId,
      overwriteExisting: false
    })
    toast.success('Menu generated successfully!')
  } catch (error: any) {
    console.error('Failed to generate menu:', error)
    toast.error(error.message || 'Failed to generate menu')
  }
}

// Load templates on mount
onMounted(async () => {
  await templateStore.fetchTemplates()
})
</script>
