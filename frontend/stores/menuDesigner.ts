import { defineStore } from 'pinia'

export interface MenuDesignElement {
  id: string
  type: 'text' | 'image' | 'menuItem' | 'shape' | 'icon' | 'gradient' | 'advancedCard'
  x: number
  y: number
  width: number
  height: number
  rotation: number
  zIndex: number
  locked: boolean
  visible: boolean

  // Text properties
  text?: string
  fontSize?: number
  fontFamily?: string
  fontWeight?: string
  color?: string
  textAlign?: 'left' | 'center' | 'right'
  lineHeight?: number
  letterSpacing?: number
  textTransform?: 'none' | 'uppercase' | 'lowercase' | 'capitalize'
  textShadow?: string
  textStroke?: string
  textStrokeWidth?: number

  // Image properties
  imageUrl?: string
  imageOpacity?: number
  imageFilter?: string
  imageBlur?: number
  imageBrightness?: number
  imageContrast?: number
  imageSaturate?: number
  objectFit?: 'cover' | 'contain' | 'fill' | 'none' | 'scale-down'

  // Menu item properties
  itemName?: string
  itemDescription?: string
  itemPrice?: number
  itemImage?: string
  itemBadge?: string
  itemTag?: string
  showDivider?: boolean

  // Shape properties
  shapeType?: 'rectangle' | 'circle' | 'line' | 'triangle' | 'ellipse' | 'polygon'
  backgroundColor?: string
  backgroundGradient?: string
  backgroundImage?: string
  borderColor?: string
  borderWidth?: number
  borderRadius?: number
  borderStyle?: 'solid' | 'dashed' | 'dotted' | 'double'
  opacity?: number

  // Advanced styling properties
  boxShadow?: string
  dropShadow?: string
  backdropFilter?: string
  blur?: number
  brightness?: number
  contrast?: number
  saturate?: number
  hueRotate?: number
  invert?: number
  sepia?: number
  
  // Transform effects
  scale?: number
  skewX?: number
  skewY?: number
  transformOrigin?: string
  
  // Advanced card properties
  cardStyle?: 'modern' | 'elegant' | 'minimal' | 'bold' | 'glassmorphism'
  cardElevation?: number
  cardHover?: boolean
  
  // Gradient properties
  gradientType?: 'linear' | 'radial' | 'conic'
  gradientColors?: string[]
  gradientAngle?: number
  gradientStops?: number[]
}

export interface MenuDesignTemplate {
  id: string
  name: string
  description: string
  thumbnail: string
  category: 'modern' | 'classic' | 'elegant' | 'minimal' | 'colorful'
  isPremium: boolean
  width: number
  height: number
  backgroundColor: string
  backgroundImage?: string
  elements: MenuDesignElement[]
}

export interface MenuDesignProject {
  id: string
  name: string
  templateId?: string
  restaurantId: string
  width: number
  height: number
  backgroundColor: string
  backgroundImage?: string
  elements: MenuDesignElement[]
  createdAt: string
  updatedAt: string
}

export const useMenuDesignerStore = defineStore('menuDesigner', {
  state: () => ({
    currentProject: null as MenuDesignProject | null,
    projects: [] as MenuDesignProject[],
    templates: [] as MenuDesignTemplate[],
    selectedElement: null as MenuDesignElement | null,
    clipboard: null as MenuDesignElement | null,
    history: [] as MenuDesignProject[],
    historyIndex: -1,
    zoom: 100,
    gridEnabled: true,
    snapToGrid: true,
    gridSize: 10
  }),

  getters: {
    canUndo: (state) => state.historyIndex > 0,
    canRedo: (state) => state.historyIndex < state.history.length - 1,
    sortedElements: (state) => {
      if (!state.currentProject) return []
      return [...state.currentProject.elements].sort((a, b) => a.zIndex - b.zIndex)
    }
  },

  actions: {
    // Project management
    createProject(name: string, restaurantId: string, template?: MenuDesignTemplate) {
      const project: MenuDesignProject = {
        id: crypto.randomUUID(),
        name,
        restaurantId,
        templateId: template?.id,
        width: template?.width || 1080,
        height: template?.height || 1920,
        backgroundColor: template?.backgroundColor || '#1a1a1a',
        backgroundImage: template?.backgroundImage,
        elements: template?.elements ? JSON.parse(JSON.stringify(template.elements)) : [],
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }

      this.currentProject = project
      this.projects.push(project)
      this.saveHistory()
      return project
    },

    loadProject(projectId: string) {
      const project = this.projects.find(p => p.id === projectId)
      if (project) {
        this.currentProject = JSON.parse(JSON.stringify(project))
        this.selectedElement = null
        this.history = [JSON.parse(JSON.stringify(project))]
        this.historyIndex = 0
      }
    },

    saveProject() {
      if (!this.currentProject) return

      const index = this.projects.findIndex(p => p.id === this.currentProject!.id)
      if (index !== -1) {
        this.currentProject.updatedAt = new Date().toISOString()
        this.projects[index] = JSON.parse(JSON.stringify(this.currentProject))
      }
    },

    deleteProject(projectId: string) {
      this.projects = this.projects.filter(p => p.id !== projectId)
      if (this.currentProject?.id === projectId) {
        this.currentProject = null
      }
    },

    // Element management
    addElement(element: Omit<MenuDesignElement, 'id' | 'zIndex'>) {
      if (!this.currentProject) return

      const maxZIndex = Math.max(...this.currentProject.elements.map(e => e.zIndex), -1)
      const newElement: MenuDesignElement = {
        ...element,
        id: crypto.randomUUID(),
        zIndex: maxZIndex + 1
      }

      this.currentProject.elements.push(newElement)
      this.selectedElement = newElement
      this.saveHistory()
    },

    updateElement(elementId: string, updates: Partial<MenuDesignElement>) {
      if (!this.currentProject) return

      const element = this.currentProject.elements.find(e => e.id === elementId)
      if (element) {
        Object.assign(element, updates)
        if (this.selectedElement?.id === elementId) {
          this.selectedElement = element
        }
        // Note: saveHistory is called from the canvas after update
      }
    },

    deleteElement(elementId: string) {
      if (!this.currentProject) return

      this.currentProject.elements = this.currentProject.elements.filter(e => e.id !== elementId)
      if (this.selectedElement?.id === elementId) {
        this.selectedElement = null
      }
      this.saveHistory()
    },

    duplicateElement(elementId: string) {
      if (!this.currentProject) return

      const element = this.currentProject.elements.find(e => e.id === elementId)
      if (element) {
        const duplicate = JSON.parse(JSON.stringify(element))
        duplicate.id = crypto.randomUUID()
        duplicate.x += 20
        duplicate.y += 20
        duplicate.zIndex = Math.max(...this.currentProject.elements.map(e => e.zIndex)) + 1

        this.currentProject.elements.push(duplicate)
        this.selectedElement = duplicate
        this.saveHistory()
      }
    },

    moveElement(elementId: string, direction: 'up' | 'down' | 'top' | 'bottom') {
      if (!this.currentProject) return

      const element = this.currentProject.elements.find(e => e.id === elementId)
      if (!element) return

      const elements = this.currentProject.elements

      switch (direction) {
        case 'up':
          element.zIndex = Math.min(element.zIndex + 1, elements.length - 1)
          break
        case 'down':
          element.zIndex = Math.max(element.zIndex - 1, 0)
          break
        case 'top':
          element.zIndex = elements.length - 1
          break
        case 'bottom':
          element.zIndex = 0
          break
      }

      // Reorder z-indexes
      elements.sort((a, b) => a.zIndex - b.zIndex)
      elements.forEach((el, index) => {
        el.zIndex = index
      })
    },

    selectElement(elementId: string | null) {
      if (elementId === null) {
        this.selectedElement = null
        return
      }

      const element = this.currentProject?.elements.find(e => e.id === elementId)
      this.selectedElement = element || null
    },

    // History management
    saveHistory() {
      if (!this.currentProject) return

      // Remove any history after current index
      this.history = this.history.slice(0, this.historyIndex + 1)

      // Add current state
      this.history.push(JSON.parse(JSON.stringify(this.currentProject)))
      this.historyIndex = this.history.length - 1

      // Limit history to 50 states
      if (this.history.length > 50) {
        this.history.shift()
        this.historyIndex--
      }
    },

    undo() {
      if (!this.canUndo) return

      this.historyIndex--
      this.currentProject = JSON.parse(JSON.stringify(this.history[this.historyIndex]))
    },

    redo() {
      if (!this.canRedo) return

      this.historyIndex++
      this.currentProject = JSON.parse(JSON.stringify(this.history[this.historyIndex]))
    },

    // Clipboard
    copyElement(elementId: string) {
      const element = this.currentProject?.elements.find(e => e.id === elementId)
      if (element) {
        this.clipboard = JSON.parse(JSON.stringify(element))
      }
    },

    pasteElement() {
      if (!this.clipboard || !this.currentProject) return

      const duplicate = JSON.parse(JSON.stringify(this.clipboard))
      duplicate.id = crypto.randomUUID()
      duplicate.x += 20
      duplicate.y += 20
      duplicate.zIndex = Math.max(...this.currentProject.elements.map(e => e.zIndex), -1) + 1

      this.currentProject.elements.push(duplicate)
      this.selectedElement = duplicate
      this.saveHistory()
    },

    // View controls
    setZoom(zoom: number) {
      this.zoom = Math.max(10, Math.min(200, zoom))
    },

    toggleGrid() {
      this.gridEnabled = !this.gridEnabled
    },

    toggleSnap() {
      this.snapToGrid = !this.snapToGrid
    },

    // Template management
    loadTemplates() {
      // In a real app, this would fetch from API
      this.templates = getDefaultTemplates()
    }
  }
})

// Default templates
function getDefaultTemplates(): MenuDesignTemplate[] {
  return [
    {
      id: 'dark-elegant',
      name: 'Dark Elegant',
      description: 'Professional dark menu with elegant typography',
      thumbnail: '/templates/dark-elegant.jpg',
      category: 'elegant',
      isPremium: false,
      width: 1080,
      height: 1920,
      backgroundColor: '#1a1a1a',
      elements: [
        // Header with restaurant name
        {
          id: 'header-title',
          type: 'text',
          x: 540,
          y: 100,
          width: 800,
          height: 100,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          text: 'RESTAURANT NAME',
          fontSize: 64,
          fontFamily: 'Playfair Display',
          fontWeight: 'bold',
          color: '#ffffff',
          textAlign: 'center',
          lineHeight: 1.2
        },
        // Decorative line
        {
          id: 'header-line',
          type: 'shape',
          x: 340,
          y: 220,
          width: 400,
          height: 2,
          rotation: 0,
          zIndex: 0,
          locked: false,
          visible: true,
          shapeType: 'rectangle',
          backgroundColor: '#dc2626',
          borderColor: '#dc2626',
          borderWidth: 0,
          borderRadius: 0,
          opacity: 1
        },
        // Category title
        {
          id: 'category-1',
          type: 'text',
          x: 100,
          y: 300,
          width: 880,
          height: 60,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          text: 'APPETIZERS',
          fontSize: 36,
          fontFamily: 'Inter',
          fontWeight: 'bold',
          color: '#dc2626',
          textAlign: 'left',
          lineHeight: 1.2
        },
        // Menu item 1
        {
          id: 'item-1',
          type: 'menuItem',
          x: 100,
          y: 400,
          width: 880,
          height: 120,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Bruschetta',
          itemDescription: 'Toasted bread with tomatoes, garlic, and basil',
          itemPrice: 8.99,
          backgroundColor: '#2a2a2a',
          color: '#ffffff',
          fontSize: 20,
          fontFamily: 'Inter'
        },
        // Menu item 2
        {
          id: 'item-2',
          type: 'menuItem',
          x: 100,
          y: 540,
          width: 880,
          height: 120,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Caesar Salad',
          itemDescription: 'Crisp romaine lettuce with parmesan and croutons',
          itemPrice: 9.99,
          backgroundColor: '#2a2a2a',
          color: '#ffffff',
          fontSize: 20,
          fontFamily: 'Inter'
        },
        // Category title 2
        {
          id: 'category-2',
          type: 'text',
          x: 100,
          y: 720,
          width: 880,
          height: 60,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          text: 'MAIN COURSES',
          fontSize: 36,
          fontFamily: 'Inter',
          fontWeight: 'bold',
          color: '#dc2626',
          textAlign: 'left',
          lineHeight: 1.2
        },
        // Menu item 3
        {
          id: 'item-3',
          type: 'menuItem',
          x: 100,
          y: 820,
          width: 880,
          height: 120,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Grilled Salmon',
          itemDescription: 'Fresh salmon with lemon butter sauce and vegetables',
          itemPrice: 24.99,
          backgroundColor: '#2a2a2a',
          color: '#ffffff',
          fontSize: 20,
          fontFamily: 'Inter'
        },
        // Menu item 4
        {
          id: 'item-4',
          type: 'menuItem',
          x: 100,
          y: 960,
          width: 880,
          height: 120,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Ribeye Steak',
          itemDescription: 'Premium cut with herb butter and roasted potatoes',
          itemPrice: 32.99,
          backgroundColor: '#2a2a2a',
          color: '#ffffff',
          fontSize: 20,
          fontFamily: 'Inter'
        }
      ]
    },
    {
      id: 'modern-colorful',
      name: 'Modern Colorful',
      description: 'Vibrant and modern design with bold colors',
      thumbnail: '/templates/modern-colorful.jpg',
      category: 'modern',
      isPremium: false,
      width: 1080,
      height: 1920,
      backgroundColor: '#1e293b',
      elements: [
        // Colorful header background
        {
          id: 'header-bg',
          type: 'shape',
          x: 0,
          y: 0,
          width: 1080,
          height: 300,
          rotation: 0,
          zIndex: 0,
          locked: false,
          visible: true,
          shapeType: 'rectangle',
          backgroundColor: '#3b82f6',
          borderColor: '#3b82f6',
          borderWidth: 0,
          borderRadius: 0,
          opacity: 1
        },
        // Restaurant name
        {
          id: 'title',
          type: 'text',
          x: 140,
          y: 120,
          width: 800,
          height: 80,
          rotation: 0,
          zIndex: 2,
          locked: false,
          visible: true,
          text: 'Modern Bistro',
          fontSize: 56,
          fontFamily: 'Roboto',
          fontWeight: 'bold',
          color: '#ffffff',
          textAlign: 'center',
          lineHeight: 1.2
        },
        // Section 1
        {
          id: 'section-1',
          type: 'text',
          x: 80,
          y: 350,
          width: 920,
          height: 50,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          text: 'Featured Items',
          fontSize: 32,
          fontFamily: 'Roboto',
          fontWeight: 'bold',
          color: '#f59e0b',
          textAlign: 'left',
          lineHeight: 1.2
        },
        // Item 1
        {
          id: 'item-1',
          type: 'menuItem',
          x: 80,
          y: 430,
          width: 920,
          height: 110,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Signature Burger',
          itemDescription: 'Angus beef, aged cheddar, special sauce',
          itemPrice: 16.99,
          backgroundColor: '#334155',
          color: '#ffffff',
          fontSize: 18,
          fontFamily: 'Roboto'
        },
        // Item 2
        {
          id: 'item-2',
          type: 'menuItem',
          x: 80,
          y: 560,
          width: 920,
          height: 110,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Truffle Pasta',
          itemDescription: 'Handmade pasta with truffle cream sauce',
          itemPrice: 22.99,
          backgroundColor: '#334155',
          color: '#ffffff',
          fontSize: 18,
          fontFamily: 'Roboto'
        }
      ]
    },
    {
      id: 'classic-print',
      name: 'Classic Print',
      description: 'Traditional restaurant menu design',
      thumbnail: '/templates/classic-print.jpg',
      category: 'classic',
      isPremium: false,
      width: 1080,
      height: 1920,
      backgroundColor: '#f7f3e9',
      elements: [
        // Ornamental border
        {
          id: 'border',
          type: 'shape',
          x: 60,
          y: 60,
          width: 960,
          height: 1800,
          rotation: 0,
          zIndex: 0,
          locked: false,
          visible: true,
          shapeType: 'rectangle',
          backgroundColor: 'transparent',
          borderColor: '#8b4513',
          borderWidth: 4,
          borderRadius: 8,
          opacity: 1
        },
        // Restaurant name
        {
          id: 'name',
          type: 'text',
          x: 140,
          y: 140,
          width: 800,
          height: 100,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          text: 'The Classic Restaurant',
          fontSize: 48,
          fontFamily: 'Georgia',
          fontWeight: 'bold',
          color: '#2c1810',
          textAlign: 'center',
          lineHeight: 1.3
        },
        // Decorative line
        {
          id: 'line-1',
          type: 'shape',
          x: 300,
          y: 260,
          width: 480,
          height: 2,
          rotation: 0,
          zIndex: 0,
          locked: false,
          visible: true,
          shapeType: 'rectangle',
          backgroundColor: '#8b4513',
          borderColor: '#8b4513',
          borderWidth: 0,
          borderRadius: 0,
          opacity: 0.5
        },
        // Section
        {
          id: 'section',
          type: 'text',
          x: 140,
          y: 320,
          width: 800,
          height: 50,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          text: 'Chef\'s Specialties',
          fontSize: 32,
          fontFamily: 'Georgia',
          fontWeight: 'bold',
          color: '#8b4513',
          textAlign: 'center',
          lineHeight: 1.2
        },
        // Item 1
        {
          id: 'item-1',
          type: 'menuItem',
          x: 120,
          y: 400,
          width: 840,
          height: 100,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Coq au Vin',
          itemDescription: 'Chicken braised in red wine with mushrooms',
          itemPrice: 28.00,
          backgroundColor: '#ffffff',
          color: '#2c1810',
          fontSize: 18,
          fontFamily: 'Georgia'
        },
        // Item 2
        {
          id: 'item-2',
          type: 'menuItem',
          x: 120,
          y: 520,
          width: 840,
          height: 100,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Beef Wellington',
          itemDescription: 'Tender beef wrapped in puff pastry with mushroom duxelles',
          itemPrice: 38.00,
          backgroundColor: '#ffffff',
          color: '#2c1810',
          fontSize: 18,
          fontFamily: 'Georgia'
        }
      ]
    },
    {
      id: 'minimal-white',
      name: 'Minimal White',
      description: 'Clean and minimalist design with lots of white space',
      thumbnail: '/templates/minimal-white.jpg',
      category: 'minimal',
      isPremium: false,
      width: 1080,
      height: 1920,
      backgroundColor: '#ffffff',
      elements: [
        // Simple header
        {
          id: 'header',
          type: 'text',
          x: 140,
          y: 200,
          width: 800,
          height: 60,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          text: 'MENU',
          fontSize: 72,
          fontFamily: 'Inter',
          fontWeight: 'bold',
          color: '#000000',
          textAlign: 'center',
          lineHeight: 1
        },
        // Thin line
        {
          id: 'line',
          type: 'shape',
          x: 440,
          y: 290,
          width: 200,
          height: 1,
          rotation: 0,
          zIndex: 0,
          locked: false,
          visible: true,
          shapeType: 'rectangle',
          backgroundColor: '#000000',
          borderColor: '#000000',
          borderWidth: 0,
          borderRadius: 0,
          opacity: 0.3
        },
        // Items
        {
          id: 'item-1',
          type: 'menuItem',
          x: 140,
          y: 400,
          width: 800,
          height: 90,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Margherita Pizza',
          itemDescription: 'Fresh mozzarella, tomato, basil',
          itemPrice: 14.00,
          backgroundColor: '#f9fafb',
          color: '#111827',
          fontSize: 20,
          fontFamily: 'Inter'
        },
        {
          id: 'item-2',
          type: 'menuItem',
          x: 140,
          y: 510,
          width: 800,
          height: 90,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Greek Salad',
          itemDescription: 'Feta, olives, cucumber, tomato',
          itemPrice: 12.00,
          backgroundColor: '#f9fafb',
          color: '#111827',
          fontSize: 20,
          fontFamily: 'Inter'
        }
      ]
    },
    {
      id: 'vibrant-gradient',
      name: 'Vibrant Gradient',
      description: 'Eye-catching design with colorful gradients',
      thumbnail: '/templates/vibrant-gradient.jpg',
      category: 'colorful',
      isPremium: true,
      width: 1080,
      height: 1920,
      backgroundColor: '#7c3aed',
      elements: [
        // Gradient overlay (simulated with shapes)
        {
          id: 'gradient-top',
          type: 'shape',
          x: 0,
          y: 0,
          width: 1080,
          height: 400,
          rotation: 0,
          zIndex: 0,
          locked: false,
          visible: true,
          shapeType: 'rectangle',
          backgroundColor: '#ec4899',
          borderColor: 'transparent',
          borderWidth: 0,
          borderRadius: 0,
          opacity: 0.6
        },
        // Title
        {
          id: 'title',
          type: 'text',
          x: 140,
          y: 150,
          width: 800,
          height: 100,
          rotation: 0,
          zIndex: 2,
          locked: false,
          visible: true,
          text: 'Flavor Fusion',
          fontSize: 64,
          fontFamily: 'Open Sans',
          fontWeight: 'bold',
          color: '#ffffff',
          textAlign: 'center',
          lineHeight: 1.2
        },
        // Items with colorful backgrounds
        {
          id: 'item-1',
          type: 'menuItem',
          x: 80,
          y: 450,
          width: 920,
          height: 120,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Rainbow Sushi Roll',
          itemDescription: 'Assorted fresh fish with avocado',
          itemPrice: 18.99,
          backgroundColor: '#f59e0b',
          color: '#ffffff',
          fontSize: 20,
          fontFamily: 'Open Sans'
        },
        {
          id: 'item-2',
          type: 'menuItem',
          x: 80,
          y: 590,
          width: 920,
          height: 120,
          rotation: 0,
          zIndex: 1,
          locked: false,
          visible: true,
          itemName: 'Dragon Fruit Smoothie',
          itemDescription: 'Tropical blend with dragon fruit and mango',
          itemPrice: 8.99,
          backgroundColor: '#10b981',
          color: '#ffffff',
          fontSize: 20,
          fontFamily: 'Open Sans'
        }
      ]
    }
  ]
}
