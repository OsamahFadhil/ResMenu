<template>
  <NuxtLayout name="dashboard">
    <div class="space-y-6">
      <!-- Header -->
      <div>
        <h1 class="text-2xl font-bold text-neutral-900">Restaurant Settings</h1>
        <p class="mt-1 text-neutral-600">Manage your menu appearance and preferences</p>
      </div>

      <!-- Error State -->
      <div v-if="!authStore.restaurantId" class="p-6 text-center bg-red-50 rounded-xl border border-red-200">
        <p class="font-semibold text-red-700">No restaurant found</p>
        <p class="mt-2 text-sm text-red-600">Please make sure you're logged in as a restaurant owner</p>
      </div>

      <!-- Loading State -->
      <div v-else-if="loading" class="flex justify-center py-12">
        <div class="inline-flex h-12 w-12 items-center justify-center rounded-full border-4 border-primary-200 border-t-primary-600 animate-spin"></div>
      </div>

      <!-- Settings Content -->
      <div v-else class="space-y-6">
        <!-- Active Template -->
        <Card>
          <template #header>
            <div class="flex justify-between items-center">
              <h2 class="text-lg font-semibold text-neutral-900">Active Template</h2>
              <Badge v-if="settings.activeTemplateId" variant="primary">Active</Badge>
            </div>
          </template>
            <div class="space-y-4">
              <p class="text-sm text-neutral-600">
                Select a template to use as the base for your menu design
              </p>
              
              <!-- No Templates State -->
              <div v-if="templates.length === 0" class="text-center py-8 text-neutral-500">
                <p>No templates available. Create a template first in the Templates page.</p>
              </div>

              <!-- Templates Grid -->
              <div v-else class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
                <div
                  v-for="template in templates"
                  :key="template.id"
                  @click="selectTemplate(template.id)"
                  :class="[
                    'relative p-4 border-2 rounded-xl cursor-pointer transition-all',
                    settings.activeTemplateId === template.id
                      ? 'border-primary-600 bg-primary-50'
                      : 'border-neutral-200 hover:border-neutral-300 hover:shadow-md'
                  ]"
                >
                  <div class="flex justify-between items-start">
                    <div>
                      <h3 class="font-semibold text-neutral-900">{{ template.name }}</h3>
                      <p class="mt-1 text-sm text-neutral-600">{{ template.description || 'No description' }}</p>
                      <div class="flex gap-2 items-center mt-2 text-xs text-neutral-500">
                        <span>{{ template.structure.categories.length }} categories</span>
                      </div>
                    </div>
                    <div v-if="settings.activeTemplateId === template.id" class="flex-shrink-0">
                      <svg class="w-6 h-6 text-primary-600" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                        </svg>
                    </div>
                  </div>
                </div>
              </div>

              <div v-if="settings.activeTemplateId" class="pt-4 border-t">
                <UiButton @click="applyCurrentTemplate" :loading="applying" variant="primary">
                  Apply Template to Menu
                </UiButton>
              </div>
            </div>
        </Card>

        <!-- Custom Theme -->
        <Card>
          <template #header>
            <div class="flex justify-between items-center">
              <h2 class="text-lg font-semibold text-neutral-900">Custom Theme</h2>
              <button
                @click="resetTheme"
                class="text-sm text-neutral-600 hover:text-neutral-900"
              >
                Reset to Default
              </button>
            </div>
          </template>
            <div class="space-y-6">
              <p class="text-sm text-neutral-600">
                Customize the appearance of your public menu
              </p>
              
              <ThemeCustomizer v-if="settings.customTheme" v-model="settings.customTheme" />
              <LayoutCustomizer v-if="settings.customTheme" v-model="settings.customTheme" />
            </div>
        </Card>

        <!-- Display Settings -->
        <Card>
          <template #header>
            <h2 class="text-lg font-semibold text-neutral-900">Display Settings</h2>
          </template>
            <div class="space-y-4">
              <p class="mb-4 text-sm text-neutral-600">
                Control what information is shown on your public menu
              </p>

              <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
                <label class="flex gap-3 items-center p-3 rounded-lg border cursor-pointer border-neutral-200 hover:bg-neutral-50">
                  <input
                    type="checkbox"
                    v-model="settings.displaySettings.showPrices"
                    class="w-4 h-4 rounded text-primary-600 border-neutral-300 focus:ring-primary-500"
                  />
                  <div>
                    <div class="font-medium text-neutral-900">Show Prices</div>
                    <div class="text-xs text-neutral-600">Display item prices on the menu</div>
                  </div>
                </label>

                <label class="flex gap-3 items-center p-3 rounded-lg border cursor-pointer border-neutral-200 hover:bg-neutral-50">
                  <input
                    type="checkbox"
                    v-model="settings.displaySettings.showImages"
                    class="w-4 h-4 rounded text-primary-600 border-neutral-300 focus:ring-primary-500"
                  />
                  <div>
                    <div class="font-medium text-neutral-900">Show Images</div>
                    <div class="text-xs text-neutral-600">Display item images</div>
                  </div>
                </label>

                <label class="flex gap-3 items-center p-3 rounded-lg border cursor-pointer border-neutral-200 hover:bg-neutral-50">
                  <input
                    type="checkbox"
                    v-model="settings.displaySettings.showDescriptions"
                    class="w-4 h-4 rounded text-primary-600 border-neutral-300 focus:ring-primary-500"
                  />
                  <div>
                    <div class="font-medium text-neutral-900">Show Descriptions</div>
                    <div class="text-xs text-neutral-600">Display item descriptions</div>
                  </div>
                </label>

                <label class="flex gap-3 items-center p-3 rounded-lg border cursor-pointer border-neutral-200 hover:bg-neutral-50">
                  <input
                    type="checkbox"
                    v-model="settings.displaySettings.showCategories"
                    class="w-4 h-4 rounded text-primary-600 border-neutral-300 focus:ring-primary-500"
                  />
                  <div>
                    <div class="font-medium text-neutral-900">Show Categories</div>
                    <div class="text-xs text-neutral-600">Display category sections</div>
                  </div>
                </label>

                <label class="flex gap-3 items-center p-3 rounded-lg border cursor-pointer border-neutral-200 hover:bg-neutral-50">
                  <input
                    type="checkbox"
                    v-model="settings.displaySettings.enableSearch"
                    class="w-4 h-4 rounded text-primary-600 border-neutral-300 focus:ring-primary-500"
                  />
                  <div>
                    <div class="font-medium text-neutral-900">Enable Search</div>
                    <div class="text-xs text-neutral-600">Allow customers to search items</div>
                  </div>
                </label>

                <label class="flex gap-3 items-center p-3 rounded-lg border cursor-pointer border-neutral-200 hover:bg-neutral-50">
                  <input
                    type="checkbox"
                    v-model="settings.displaySettings.enableFilters"
                    class="w-4 h-4 rounded text-primary-600 border-neutral-300 focus:ring-primary-500"
                  />
                  <div>
                    <div class="font-medium text-neutral-900">Enable Filters</div>
                    <div class="text-xs text-neutral-600">Allow filtering by tags</div>
                  </div>
                </label>
              </div>
            </div>
        </Card>

        <!-- Localization -->
        <Card>
          <template #header>
            <h2 class="text-lg font-semibold text-neutral-900">Localization</h2>
          </template>
            <div class="grid grid-cols-1 gap-6 md:grid-cols-2">
              <div>
                <label class="block mb-2 text-sm font-medium text-neutral-700">
                  Currency
                </label>
                <select
                  v-model="settings.currency"
                  class="block px-4 py-2.5 w-full rounded-lg border shadow-sm transition-all border-neutral-300 focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="USD">USD ($)</option>
                  <option value="EUR">EUR (€)</option>
                  <option value="GBP">GBP (£)</option>
                  <option value="SAR">SAR (﷼)</option>
                  <option value="AED">AED (د.إ)</option>
                  <option value="EGP">EGP (E£)</option>
                </select>
              </div>

              <div>
                <label class="block mb-2 text-sm font-medium text-neutral-700">
                  Default Language
                </label>
                <select
                  v-model="settings.defaultLanguage"
                  class="block px-4 py-2.5 w-full rounded-lg border shadow-sm transition-all border-neutral-300 focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="en">English</option>
                  <option value="ar">Arabic</option>
                  <option value="fr">French</option>
                  <option value="es">Spanish</option>
                </select>
              </div>
            </div>
        </Card>

        <!-- Save Button -->
        <div class="flex gap-3 justify-end pt-4">
          <UiButton @click="loadSettings" variant="secondary">
            Cancel
          </UiButton>
          <UiButton @click="saveSettings" :loading="saving" variant="primary">
            Save Settings
          </UiButton>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '~/stores/auth'
import { useTemplateStore } from '~/stores/templates'
import { useRestaurantSettings, type RestaurantSettings } from '~/composables/useRestaurantSettings'
import { createDefaultTheme } from '~/stores/templates'
import ThemeCustomizer from '~/components/menu/ThemeCustomizer.vue'
import LayoutCustomizer from '~/components/menu/LayoutCustomizer.vue'
import Card from '~/components/ui/Card.vue'
import Badge from '~/components/ui/Badge.vue'
import UiButton from '~/components/ui/Button.vue'

definePageMeta({
  layout: false,
  middleware: ['owner']
})

const authStore = useAuthStore()
const templateStore = useTemplateStore()
const { loading, error, getSettings, updateSettings, applyTemplate } = useRestaurantSettings()

const settings = ref<RestaurantSettings>({
  restaurantId: authStore.restaurantId || '',
  activeTemplateId: null,
  customTheme: createDefaultTheme(),
  displaySettings: {
    showPrices: true,
    showImages: true,
    showDescriptions: true,
    showCategories: true,
    enableSearch: true,
    enableFilters: true,
    availableLanguages: ['en', 'ar']
  },
  currency: 'USD',
  defaultLanguage: 'en'
})

const templates = computed(() => templateStore.templates)
const saving = ref(false)
const applying = ref(false)

const loadSettings = async () => {
  if (!authStore.restaurantId) {
    console.error('No restaurant ID found in auth store')
    return
  }

  console.log('Loading settings for restaurant:', authStore.restaurantId)
  const result = await getSettings(authStore.restaurantId)
  if (result) {
    console.log('Settings loaded:', result)
    settings.value = {
      ...result,
      customTheme: result.customTheme || createDefaultTheme()
    }
  } else {
    console.error('Failed to load settings:', error.value)
    // Initialize with defaults if loading fails
    settings.value.customTheme = createDefaultTheme()
  }
}

const saveSettings = async () => {
  if (!authStore.restaurantId) return

  saving.value = true
  try {
    const result = await updateSettings(authStore.restaurantId, {
      activeTemplateId: settings.value.activeTemplateId,
      customTheme: settings.value.customTheme,
      displaySettings: settings.value.displaySettings,
      currency: settings.value.currency,
      defaultLanguage: settings.value.defaultLanguage
    })

    if (result) {
      alert('Settings saved successfully!')
    } else if (error.value) {
      alert(error.value)
    }
  } finally {
    saving.value = false
  }
}

const selectTemplate = (templateId: string) => {
  settings.value.activeTemplateId = templateId

  // Auto-load the template's theme into custom theme
  const selectedTemplate = templates.value.find(t => t.id === templateId)
  if (selectedTemplate?.theme) {
    settings.value.customTheme = { ...selectedTemplate.theme }
    console.log('Loaded template theme:', selectedTemplate.theme)
  }
}

const applyCurrentTemplate = async () => {
  if (!authStore.restaurantId || !settings.value.activeTemplateId) return

  // Check if user wants to overwrite existing data
  const hasExistingData = true // Assume user has data if they're seeing this error
  
  let overwrite = false
  if (hasExistingData) {
    const choice = confirm(
      'You already have menu data.\n\n' +
      'Click OK to REPLACE all existing categories and items with the template.\n' +
      'Click Cancel to keep your existing data and only apply the theme/settings.'
    )
    
    if (choice) {
      // User wants to overwrite
      const doubleCheck = confirm(
        '⚠️ WARNING: This will DELETE all your existing categories and items!\n\n' +
        'Are you absolutely sure you want to continue?'
      )
      if (!doubleCheck) {
        return
      }
      overwrite = true
    } else {
      // User wants to keep data, just apply theme
      alert('Template theme and settings will be applied without changing your menu items.')
      await saveSettings()
      return
    }
  }

  applying.value = true
  try {
    // First save the current settings (including the loaded theme)
    await saveSettings()

    // Then apply the template
    const result = await applyTemplate(authStore.restaurantId, {
      templateId: settings.value.activeTemplateId,
      overwriteExisting: overwrite
    })

    if (result) {
      alert(`Template applied successfully! Created ${result.categoriesCreated} categories and ${result.itemsCreated} items.\n\nThe template's theme has been applied to your public menu.`)
      // Reload settings to get updated data
      await loadSettings()
    } else if (error.value) {
      alert(error.value)
    }
  } finally {
    applying.value = false
  }
}

const resetTheme = () => {
  if (confirm('Reset theme to default settings?')) {
    settings.value.customTheme = createDefaultTheme()
  }
}

onMounted(async () => {
  console.log('Settings page mounted')
  console.log('Auth store:', authStore.user)
  console.log('Restaurant ID:', authStore.restaurantId)
  
  if (!authStore.restaurantId) {
    console.error('No restaurant ID - cannot load settings')
    return
  }
  
  try {
    await Promise.all([
      loadSettings(),
      templateStore.fetchTemplates()
    ])
    console.log('Templates loaded:', templateStore.templates)
  } catch (err) {
    console.error('Error loading data:', err)
  }
})
</script>

