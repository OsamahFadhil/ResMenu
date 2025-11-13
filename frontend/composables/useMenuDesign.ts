import type { TemplateTheme } from '@/stores/templates'

export interface CategoryLayout {
  categoryId: string
  displayOrder: number
  layout: 'list' | 'grid' | 'cards'
  cardStyle: 'modern' | 'classic' | 'minimal'
  columns?: number
  spacing?: 'compact' | 'normal' | 'relaxed'
  borderRadius?: 'none' | 'small' | 'medium' | 'large'
  imageSize?: 'small' | 'medium' | 'large'
  imageShape?: 'square' | 'rounded' | 'circle'
  showImages: boolean
  showPrices: boolean
  showDescriptions: boolean
}

export interface GlobalLayoutSettings {
  spacing: 'compact' | 'normal' | 'relaxed'
  borderRadius: 'none' | 'small' | 'medium' | 'large'
  fontFamily: string
  headerStyle: 'simple' | 'banner' | 'overlay'
  logoPosition: 'left' | 'center' | 'right'
  showRestaurantInfo: boolean
  showLogo: boolean
  headerAlignment: 'left' | 'center' | 'right'
  tagline?: string
}

export interface LayoutConfiguration {
  categories: CategoryLayout[]
  globalSettings: GlobalLayoutSettings
}

export interface MenuDesign {
  id: string
  restaurantId: string
  layoutConfiguration: LayoutConfiguration
  globalTheme: TemplateTheme
  version: number
  isActive: boolean
  name?: string
  createdAt: string
  updatedAt: string
}

export interface SaveMenuDesignPayload {
  restaurantId: string
  layoutConfiguration: LayoutConfiguration
  globalTheme: TemplateTheme
  name?: string
  setAsActive: boolean
}

export interface SaveMenuDesignResult {
  id: string
  version: number
  isActive: boolean
  message: string
}

export const useMenuDesign = () => {
  const api = useApi()
  const loading = ref(false)
  const error = ref<string | null>(null)

  const getActiveDesign = async (restaurantId: string): Promise<MenuDesign | null> => {
    loading.value = true
    error.value = null
    try {
      const response = await api.get(`/menu-design/restaurant/${restaurantId}`)
      return response.data
    } catch (err: any) {
      if (err.response?.status === 404) {
        // No design found is not an error, just return null
        return null
      }
      error.value = err.response?.data?.message || 'Failed to fetch menu design'
      console.error('Failed to fetch menu design:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  const getDesignById = async (designId: string, restaurantId: string): Promise<MenuDesign | null> => {
    loading.value = true
    error.value = null
    try {
      const response = await api.get(`/menu-design/${designId}?restaurantId=${restaurantId}`)
      return response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to fetch menu design'
      console.error('Failed to fetch menu design:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  const saveDesign = async (payload: SaveMenuDesignPayload): Promise<SaveMenuDesignResult | null> => {
    loading.value = true
    error.value = null
    try {
      const response = await api.post('/menu-design', payload)
      return response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to save menu design'
      console.error('Failed to save menu design:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  const updateDesign = async (payload: SaveMenuDesignPayload): Promise<SaveMenuDesignResult | null> => {
    // Update is the same as save in this system (creates new version)
    return saveDesign(payload)
  }

  return {
    loading,
    error,
    getActiveDesign,
    getDesignById,
    saveDesign,
    updateDesign
  }
}

