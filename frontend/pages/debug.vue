<template>
  <div class="min-h-screen bg-gray-100 p-8">
    <div class="max-w-4xl mx-auto space-y-6">
      <h1 class="text-3xl font-bold">Debug Information</h1>
      
      <!-- Auth Store -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-semibold mb-4">Auth Store</h2>
        <pre class="bg-gray-50 p-4 rounded overflow-auto">{{ authDebug }}</pre>
      </div>

      <!-- Template Store -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-semibold mb-4">Template Store</h2>
        <button @click="fetchTemplates" class="bg-blue-500 text-white px-4 py-2 rounded mb-4">
          Fetch Templates
        </button>
        <pre class="bg-gray-50 p-4 rounded overflow-auto">{{ templateDebug }}</pre>
      </div>

      <!-- Settings API Test -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-semibold mb-4">Settings API Test</h2>
        <button @click="testSettings" class="bg-green-500 text-white px-4 py-2 rounded mb-4">
          Test Get Settings
        </button>
        <pre class="bg-gray-50 p-4 rounded overflow-auto">{{ settingsDebug }}</pre>
      </div>

      <!-- Public Menu Test -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-semibold mb-4">Public Menu Test</h2>
        <input 
          v-model="testSlug" 
          placeholder="Enter restaurant slug"
          class="border px-4 py-2 rounded mr-2"
        />
        <button @click="testPublicMenu" class="bg-purple-500 text-white px-4 py-2 rounded mb-4">
          Test Public Menu
        </button>
        <pre class="bg-gray-50 p-4 rounded overflow-auto">{{ publicMenuDebug }}</pre>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuthStore } from '~/stores/auth'
import { useTemplateStore } from '~/stores/templates'
import { useRestaurantStore } from '~/stores/restaurant'
import { useRestaurantSettings } from '~/composables/useRestaurantSettings'

const authStore = useAuthStore()
const templateStore = useTemplateStore()
const restaurantStore = useRestaurantStore()
const { getSettings } = useRestaurantSettings()

const testSlug = ref('osamah-8f0959')
const settingsDebug = ref({})
const publicMenuDebug = ref({})

const authDebug = computed(() => ({
  isAuthenticated: authStore.isAuthenticated,
  user: authStore.user,
  restaurantId: authStore.restaurantId,
  token: authStore.token ? 'EXISTS' : 'NULL'
}))

const templateDebug = computed(() => ({
  loading: templateStore.loading,
  error: templateStore.error,
  templatesCount: templateStore.templates.length,
  templates: templateStore.templates
}))

const fetchTemplates = async () => {
  try {
    await templateStore.fetchTemplates()
  } catch (err: any) {
    console.error('Error fetching templates:', err)
  }
}

const testSettings = async () => {
  if (!authStore.restaurantId) {
    settingsDebug.value = { error: 'No restaurant ID' }
    return
  }

  try {
    const result = await getSettings(authStore.restaurantId)
    settingsDebug.value = result || { error: 'No result' }
  } catch (err: any) {
    settingsDebug.value = { error: err.message }
  }
}

const testPublicMenu = async () => {
  try {
    const result = await restaurantStore.fetchPublicMenu(testSlug.value, 'en')
    publicMenuDebug.value = result
  } catch (err: any) {
    publicMenuDebug.value = { error: err.message }
  }
}
</script>

