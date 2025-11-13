<template>
  <NuxtLayout name="dashboard">
    <div class="space-y-6">
      <!-- Header -->
      <div>
        <h1 class="text-2xl font-bold text-neutral-900">Restaurant Settings</h1>
        <p class="mt-1 text-neutral-600">Manage your restaurant's basic information and preferences</p>
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
        <!-- Restaurant Info -->
        <Card>
          <template #header>
            <h2 class="text-lg font-semibold text-neutral-900">Restaurant Information</h2>
          </template>
          <div class="space-y-6">
            <!-- Name -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input
                v-model="form.name"
                label="Restaurant Name"
                required
                placeholder="My Restaurant"
              />
              <Input
                v-model="form.nameAr"
                label="Restaurant Name (Arabic)"
                placeholder="مطعمي"
              />
            </div>

            <!-- Logo Upload -->
            <div>
              <label class="block text-sm font-medium text-neutral-700 mb-2">
                Logo
              </label>
              <div class="flex items-center gap-4">
                <div v-if="form.logoUrl" class="w-20 h-20 rounded-lg overflow-hidden border-2 border-neutral-200">
                  <img :src="form.logoUrl" alt="Restaurant logo" class="w-full h-full object-cover" />
                </div>
                <div v-else class="w-20 h-20 rounded-lg bg-neutral-100 flex items-center justify-center border-2 border-dashed border-neutral-300">
                  <svg class="w-8 h-8 text-neutral-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                </div>
                <div class="flex-1">
                  <Input
                    v-model="form.logoUrl"
                    placeholder="https://example.com/logo.png"
                    label="Logo URL"
                  />
                  <p class="mt-1 text-xs text-neutral-500">Enter a URL to your logo image</p>
                </div>
              </div>
            </div>

            <!-- Contact Information -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input
                v-model="form.contactPhone"
                label="Phone Number"
                type="tel"
                placeholder="+1 234 567 8900"
              />
              <Input
                v-model="form.contactEmail"
                label="Email Address"
                type="email"
                placeholder="info@restaurant.com"
              />
            </div>

            <!-- Address -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-neutral-700 mb-1">
                  Address
                </label>
                <textarea
                  v-model="form.address"
                  rows="3"
                  class="block w-full rounded-lg border-neutral-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm px-4 py-2"
                  placeholder="123 Main St, City, Country"
                ></textarea>
              </div>
              <div>
                <label class="block text-sm font-medium text-neutral-700 mb-1">
                  Address (Arabic)
                </label>
                <textarea
                  v-model="form.addressAr"
                  rows="3"
                  class="block w-full rounded-lg border-neutral-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm px-4 py-2"
                  placeholder="١٢٣ شارع الرئيسي، المدينة، الدولة"
                  dir="rtl"
                ></textarea>
              </div>
            </div>
          </div>
        </Card>

        <!-- Display Settings -->
        <Card>
          <template #header>
            <h2 class="text-lg font-semibold text-neutral-900">Menu Display Preferences</h2>
          </template>
          <div class="space-y-4">
            <p class="text-sm text-neutral-600">
              Control what information is shown on your public menu
            </p>

            <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
              <label class="flex gap-3 items-center p-3 rounded-lg border cursor-pointer border-neutral-200 hover:bg-neutral-50">
                <input
                  type="checkbox"
                  v-model="form.displaySettings.showPrices"
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
                  v-model="form.displaySettings.showImages"
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
                  v-model="form.displaySettings.showDescriptions"
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
                  v-model="form.displaySettings.showCategories"
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
                  v-model="form.displaySettings.enableSearch"
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
                  v-model="form.displaySettings.enableFilters"
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
                v-model="form.currency"
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
                v-model="form.defaultLanguage"
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
        <div class="flex gap-3 justify-end pt-4 border-t">
          <UiButton @click="loadSettings" variant="secondary">
            Cancel
          </UiButton>
          <UiButton @click="saveSettings" :loading="saving" variant="primary">
            Save Changes
          </UiButton>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '~/stores/auth'
import Card from '~/components/ui/Card.vue'
import Input from '~/components/ui/Input.vue'
import UiButton from '~/components/ui/Button.vue'

definePageMeta({
  layout: false,
  middleware: ['owner']
})

const authStore = useAuthStore()
const toast = useToast()
const api = useApi()

const loading = ref(false)
const saving = ref(false)

interface RestaurantForm {
  name: string
  nameAr: string
  logoUrl: string
  contactPhone: string
  contactEmail: string
  address: string
  addressAr: string
  displaySettings: {
    showPrices: boolean
    showImages: boolean
    showDescriptions: boolean
    showCategories: boolean
    enableSearch: boolean
    enableFilters: boolean
  }
  currency: string
  defaultLanguage: string
}

const form = ref<RestaurantForm>({
  name: '',
  nameAr: '',
  logoUrl: '',
  contactPhone: '',
  contactEmail: '',
  address: '',
  addressAr: '',
  displaySettings: {
    showPrices: true,
    showImages: true,
    showDescriptions: true,
    showCategories: true,
    enableSearch: true,
    enableFilters: true
  },
  currency: 'USD',
  defaultLanguage: 'en'
})

const loadSettings = async () => {
  if (!authStore.restaurantId) {
    toast.error('No restaurant ID found')
    return
  }

  loading.value = true
  try {
    // Fetch restaurant details
    const response = await api.get(`/restaurants/${authStore.restaurantId}`)
    const restaurant = response.data

    // Parse translations
    let translations = {}
    if (restaurant.translations) {
      try {
        translations = typeof restaurant.translations === 'string' 
          ? JSON.parse(restaurant.translations)
          : restaurant.translations
      } catch (e) {
        console.error('Failed to parse translations:', e)
      }
    }

    // Parse display settings
    let displaySettings = form.value.displaySettings
    if (restaurant.menuDisplaySettings) {
      try {
        const parsed = typeof restaurant.menuDisplaySettings === 'string'
          ? JSON.parse(restaurant.menuDisplaySettings)
          : restaurant.menuDisplaySettings
        displaySettings = { ...displaySettings, ...parsed }
      } catch (e) {
        console.error('Failed to parse display settings:', e)
      }
    }

    form.value = {
      name: restaurant.name || '',
      nameAr: translations.ar?.name || '',
      logoUrl: restaurant.logoUrl || '',
      contactPhone: restaurant.contactPhone || '',
      contactEmail: restaurant.contactEmail || '',
      address: restaurant.address || '',
      addressAr: translations.ar?.address || '',
      displaySettings,
      currency: restaurant.currency || 'USD',
      defaultLanguage: restaurant.defaultLanguage || 'en'
    }
  } catch (error: any) {
    console.error('Failed to load settings:', error)
    toast.error('Failed to load settings')
  } finally {
    loading.value = false
  }
}

const saveSettings = async () => {
  if (!authStore.restaurantId) return

  saving.value = true
  try {
    // Prepare translations object
    const translations = {
      en: {
        name: form.value.name,
        address: form.value.address
      },
      ar: {
        name: form.value.nameAr || form.value.name,
        address: form.value.addressAr || form.value.address
      }
    }

    // Prepare update payload
    const payload = {
      name: form.value.name,
      logoUrl: form.value.logoUrl || null,
      contactPhone: form.value.contactPhone || null,
      contactEmail: form.value.contactEmail || null,
      address: form.value.address || null,
      translations: JSON.stringify(translations),
      menuDisplaySettings: JSON.stringify(form.value.displaySettings),
      currency: form.value.currency,
      defaultLanguage: form.value.defaultLanguage
    }

    await api.put(`/restaurants/${authStore.restaurantId}`, payload)
    toast.success('Settings saved successfully!')
  } catch (error: any) {
    console.error('Failed to save settings:', error)
    toast.error(error.response?.data?.message || 'Failed to save settings')
  } finally {
    saving.value = false
  }
}

onMounted(async () => {
  if (!authStore.restaurantId) {
    toast.error('No restaurant ID - cannot load settings')
    return
  }

  await loadSettings()
})
</script>
