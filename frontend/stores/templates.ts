import { defineStore } from 'pinia'
import { useApi } from '../composables/useApi'

export interface TemplateTheme {
  primaryColor: string
  accentColor: string
  backgroundColor: string
  surfaceColor: string
  textColor: string
  fontFamily: string
}

export interface TemplateItem {
  name: string
  nameTranslations?: Record<string, string>
  description?: string | null
  descriptionTranslations?: Record<string, string>
  price: number
  imageUrl?: string | null
  isAvailable: boolean
  displayOrder: number
  tags?: string[]
  rating?: number | null
}

export interface TemplateCategory {
  name: string
  translations?: Record<string, string>
  displayOrder: number
  icon?: string | null
  items: TemplateItem[]
}

export interface TemplateStructure {
  categories: TemplateCategory[]
}

export interface MenuTemplate {
  id: string
  name: string
  description?: string | null
  restaurantId?: string | null
  isGlobal: boolean
  isPublished: boolean
  tags: string[]
  theme?: TemplateTheme | null
  structure: TemplateStructure
  createdAt: string
  updatedAt?: string | null
}

export interface UpsertTemplatePayload {
  name: string
  description?: string | null
  restaurantId?: string | null
  isPublished?: boolean
  tags?: string[]
  theme?: TemplateTheme | null
  structure: TemplateStructure
}

export const createDefaultTheme = (): TemplateTheme => ({
  primaryColor: '#f97316',
  accentColor: '#facc15',
  backgroundColor: '#fff7ed',
  surfaceColor: '#ffffff',
  textColor: '#1f2937',
  fontFamily: 'Inter'
})

export const createEmptyStructure = (): TemplateStructure => ({
  categories: [
    {
      name: 'Signature Plates',
      translations: { ar: 'أطباق التوقيع' },
      displayOrder: 1,
      icon: '🥘',
      items: [
        {
          name: 'House Special',
          nameTranslations: { ar: 'طبق البيت الخاص' },
          description: 'A chef-crafted dish to impress every guest.',
          descriptionTranslations: { ar: 'طبق محضّر من الشيف ليبهر ضيوفك.' },
          price: 12.5,
          imageUrl: '',
          isAvailable: true,
          displayOrder: 1,
          tags: ['signature', 'popular'],
          rating: 4.8
        }
      ]
    }
  ]
})

interface TemplatesState {
  templates: MenuTemplate[]
  loading: boolean
  error: string | null
}

export const useTemplateStore = defineStore('templates', {
  state: (): TemplatesState => ({
    templates: [],
    loading: false,
    error: null
  }),

  getters: {
    globalTemplates: (state) => state.templates.filter((template) => template.isGlobal),
    restaurantTemplates: (state) => state.templates.filter((template) => !template.isGlobal)
  },

  actions: {
    createBlankTemplate(): UpsertTemplatePayload {
      return {
        name: 'New Template',
        description: '',
        isPublished: false,
        tags: [],
        theme: createDefaultTheme(),
        structure: createEmptyStructure()
      }
    },

    async fetchTemplates() {
      this.loading = true
      this.error = null

      try {
        const api = useApi()
        const response = await api.get('/menu-templates')

        if (response.data.success) {
          this.templates = response.data.data ?? []
        } else {
          this.error = response.data.message ?? 'Failed to load templates'
        }
      } catch (error: any) {
        this.error = error.message ?? 'Failed to load templates'
      } finally {
        this.loading = false
      }
    },

    async createTemplate(payload: UpsertTemplatePayload) {
      const api = useApi()
      const response = await api.post('/menu-templates', payload)

      if (response.data.success) {
        const created: MenuTemplate = response.data.data
        this.templates = [created, ...this.templates]
        return created
      }

      throw new Error(response.data.message || 'Failed to create template')
    },

    async updateTemplate(id: string, payload: UpsertTemplatePayload) {
      const api = useApi()
      const response = await api.put(`/menu-templates/${id}`, payload)

      if (response.data.success) {
        const updated: MenuTemplate = response.data.data
        this.templates = this.templates.map((template) => (template.id === id ? updated : template))
        return updated
      }

      throw new Error(response.data.message || 'Failed to update template')
    },

    async deleteTemplate(id: string) {
      const api = useApi()
      const response = await api.delete(`/menu-templates/${id}`)

      if (response.data.success) {
        this.templates = this.templates.filter((template) => template.id !== id)
        return true
      }

      throw new Error(response.data.message || 'Failed to delete template')
    }
  }
})

