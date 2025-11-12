import { ref } from 'vue'
import { useApi } from './useApi'
import type { TemplateTheme } from '~/stores/templates'

export interface MenuDisplaySettings {
  showPrices: boolean
  showImages: boolean
  showDescriptions: boolean
  showCategories: boolean
  enableSearch: boolean
  enableFilters: boolean
  availableLanguages: string[]
}

export interface RestaurantSettings {
  restaurantId: string
  activeTemplateId?: string | null
  customTheme?: TemplateTheme | null
  displaySettings: MenuDisplaySettings
  currency: string
  defaultLanguage: string
}

export interface UpdateRestaurantSettingsPayload {
  activeTemplateId?: string | null
  customTheme?: TemplateTheme | null
  displaySettings?: MenuDisplaySettings
  currency?: string
  defaultLanguage?: string
}

export interface ApplyTemplatePayload {
  templateId: string
  overwriteExisting?: boolean
}

export interface ApplyTemplateResult {
  restaurantId: string
  templateId: string
  categoriesCreated: number
  itemsCreated: number
  appliedAt: string
}

export const useRestaurantSettings = () => {
  const api = useApi()
  const loading = ref(false)
  const error = ref<string | null>(null)

  const getSettings = async (restaurantId: string): Promise<RestaurantSettings | null> => {
    loading.value = true
    error.value = null

    try {
      const response = await api.get(`/restaurants/${restaurantId}/settings`)

      if (response.data.success) {
        return response.data.data
      } else {
        error.value = response.data.message || 'Failed to load settings'
        return null
      }
    } catch (err: any) {
      error.value = err.message || 'Failed to load settings'
      return null
    } finally {
      loading.value = false
    }
  }

  const updateSettings = async (
    restaurantId: string,
    payload: UpdateRestaurantSettingsPayload
  ): Promise<RestaurantSettings | null> => {
    loading.value = true
    error.value = null

    try {
      const response = await api.put(`/restaurants/${restaurantId}/settings`, payload)

      if (response.data.success) {
        return response.data.data
      } else {
        error.value = response.data.message || 'Failed to update settings'
        return null
      }
    } catch (err: any) {
      error.value = err.message || 'Failed to update settings'
      return null
    } finally {
      loading.value = false
    }
  }

  const applyTemplate = async (
    restaurantId: string,
    payload: ApplyTemplatePayload
  ): Promise<ApplyTemplateResult | null> => {
    loading.value = true
    error.value = null

    try {
      const response = await api.post(`/restaurants/${restaurantId}/apply-template`, payload)

      if (response.data.success) {
        return response.data.data
      } else {
        error.value = response.data.message || 'Failed to apply template'
        return null
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to apply template'
      console.error('Apply template error:', err.response?.data || err)
      return null
    } finally {
      loading.value = false
    }
  }

  return {
    loading,
    error,
    getSettings,
    updateSettings,
    applyTemplate
  }
}

