<template>
  <div class="flex-1 flex flex-col bg-neutral-900 overflow-hidden">
    <!-- Canvas Toolbar -->
    <div class="bg-neutral-800/50 border-b border-neutral-700 px-4 py-2 flex items-center justify-between">
      <div class="text-sm text-neutral-400">
        Canvas: {{ store.currentProject?.width }}px × {{ store.currentProject?.height }}px
      </div>
      <div class="flex items-center gap-2">
        <button
          v-for="mode in previewModes"
          :key="mode.id"
          @click="currentMode = mode.id"
          :class="[
            'px-3 py-1 rounded text-sm transition-colors',
            currentMode === mode.id
              ? 'bg-blue-600 text-white'
              : 'text-neutral-400 hover:text-white hover:bg-neutral-700'
          ]"
        >
          {{ mode.label }}
        </button>
      </div>
    </div>

    <!-- Canvas Container -->
    <div
      ref="containerRef"
      class="flex-1 overflow-auto p-8 flex items-center justify-center"
      @click="handleCanvasClick"
    >
      <div
        ref="canvasRef"
        class="relative shadow-2xl"
        :style="{
          width: `${scaledWidth}px`,
          height: `${scaledHeight}px`,
          backgroundColor: store.currentProject?.backgroundColor || '#1a1a1a',
          backgroundImage: store.currentProject?.backgroundImage
            ? `url(${store.currentProject.backgroundImage})`
            : 'none',
          backgroundSize: 'cover',
          backgroundPosition: 'center'
        }"
      >
        <!-- Grid -->
        <div
          v-if="store.gridEnabled"
          class="absolute inset-0 pointer-events-none"
          :style="{
            backgroundImage: `
              linear-gradient(to right, rgba(255,255,255,0.1) 1px, transparent 1px),
              linear-gradient(to bottom, rgba(255,255,255,0.1) 1px, transparent 1px)
            `,
            backgroundSize: `${store.gridSize * (store.zoom / 100)}px ${store.gridSize * (store.zoom / 100)}px`
          }"
        ></div>

        <!-- Elements -->
        <DesignerElement
          v-for="element in store.sortedElements"
          :key="element.id"
          :element="element"
          :zoom="store.zoom"
          :is-selected="store.selectedElement?.id === element.id"
          @select="store.selectElement(element.id)"
          @update="(updates) => handleElementUpdate(element.id, updates)"
          @delete="store.deleteElement(element.id)"
        />

        <!-- Add Element Overlay -->
        <div
          v-if="selectedTool !== 'select'"
          class="absolute inset-0 cursor-crosshair"
          @click="handleAddElement"
        >
          <div class="absolute inset-0 bg-blue-500/10 border-2 border-blue-500 border-dashed flex items-center justify-center">
            <div class="bg-neutral-900/90 px-4 py-2 rounded-lg text-white text-sm">
              Click to add {{ selectedTool }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Layers Panel (Bottom) -->
    <div class="bg-neutral-800 border-t border-neutral-700 px-4 py-3">
      <div class="flex items-center justify-between mb-2">
        <h3 class="text-sm font-semibold text-white">Layers</h3>
        <div class="flex items-center gap-1">
          <button
            v-if="store.selectedElement"
            @click="store.moveElement(store.selectedElement.id, 'top')"
            class="p-1 rounded text-neutral-400 hover:text-white hover:bg-neutral-700 transition-colors"
            title="Bring to Front"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 10l7-7m0 0l7 7m-7-7v18" />
            </svg>
          </button>
          <button
            v-if="store.selectedElement"
            @click="store.moveElement(store.selectedElement.id, 'up')"
            class="p-1 rounded text-neutral-400 hover:text-white hover:bg-neutral-700 transition-colors"
            title="Bring Forward"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7" />
            </svg>
          </button>
          <button
            v-if="store.selectedElement"
            @click="store.moveElement(store.selectedElement.id, 'down')"
            class="p-1 rounded text-neutral-400 hover:text-white hover:bg-neutral-700 transition-colors"
            title="Send Backward"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
            </svg>
          </button>
          <button
            v-if="store.selectedElement"
            @click="store.moveElement(store.selectedElement.id, 'bottom')"
            class="p-1 rounded text-neutral-400 hover:text-white hover:bg-neutral-700 transition-colors"
            title="Send to Back"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 14l-7 7m0 0l-7-7m7 7V3" />
            </svg>
          </button>
        </div>
      </div>
      <div class="flex items-center gap-2 overflow-x-auto pb-2">
        <div
          v-for="element in store.sortedElements.slice().reverse()"
          :key="element.id"
          @click="store.selectElement(element.id)"
          :class="[
            'flex-shrink-0 px-3 py-2 rounded cursor-pointer transition-colors',
            store.selectedElement?.id === element.id
              ? 'bg-blue-600 text-white'
              : 'bg-neutral-700 text-neutral-300 hover:bg-neutral-600'
          ]"
        >
          <div class="flex items-center gap-2">
            <component :is="getElementIcon(element.type)" class="w-4 h-4" />
            <span class="text-xs font-medium">{{ getElementName(element) }}</span>
          </div>
        </div>
        <div
          v-if="store.sortedElements.length === 0"
          class="text-neutral-500 text-sm"
        >
          No elements yet. Use the toolbar to add elements.
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useMenuDesignerStore } from '@/stores/menuDesigner'
import type { MenuDesignElement } from '@/stores/menuDesigner'
import DesignerElement from './DesignerElement.vue'

const props = defineProps<{
  selectedTool: string
}>()

const emit = defineEmits<{
  (e: 'tool-used'): void
}>()

const store = useMenuDesignerStore()
const containerRef = ref<HTMLElement>()
const canvasRef = ref<HTMLElement>()
const currentMode = ref('desktop')

const previewModes = [
  { id: 'mobile', label: 'Mobile' },
  { id: 'tablet', label: 'Tablet' },
  { id: 'desktop', label: 'Desktop' }
]

const scaledWidth = computed(() => {
  if (!store.currentProject) return 0
  return store.currentProject.width * (store.zoom / 100)
})

const scaledHeight = computed(() => {
  if (!store.currentProject) return 0
  return store.currentProject.height * (store.zoom / 100)
})

const handleCanvasClick = (e: MouseEvent) => {
  if (e.target === canvasRef.value || (e.target as HTMLElement).closest('.absolute.inset-0.pointer-events-none')) {
    store.selectElement(null)
  }
}

const handleAddElement = (e: MouseEvent) => {
  if (!canvasRef.value || !store.currentProject) return

  const rect = canvasRef.value.getBoundingClientRect()
  const x = (e.clientX - rect.left) / (store.zoom / 100)
  const y = (e.clientY - rect.top) / (store.zoom / 100)

  let elementData: Partial<MenuDesignElement> = {
    x: store.snapToGrid ? Math.round(x / store.gridSize) * store.gridSize : x,
    y: store.snapToGrid ? Math.round(y / store.gridSize) * store.gridSize : y,
    rotation: 0,
    locked: false,
    visible: true
  }

  switch (props.selectedTool) {
    case 'text':
      elementData = {
        ...elementData,
        type: 'text',
        width: 200,
        height: 40,
        text: 'New Text',
        fontSize: 16,
        fontFamily: 'Inter',
        fontWeight: 'normal',
        color: '#ffffff',
        textAlign: 'left',
        lineHeight: 1.5
      }
      break

    case 'image':
      elementData = {
        ...elementData,
        type: 'image',
        width: 200,
        height: 200,
        imageUrl: 'https://via.placeholder.com/200',
        imageOpacity: 1
      }
      break

    case 'shape':
      elementData = {
        ...elementData,
        type: 'shape',
        width: 150,
        height: 150,
        shapeType: 'rectangle',
        backgroundColor: '#3b82f6',
        borderColor: '#2563eb',
        borderWidth: 2,
        borderRadius: 8,
        opacity: 1
      }
      break

    case 'menuItem':
      elementData = {
        ...elementData,
        type: 'menuItem',
        width: 300,
        height: 100,
        itemName: 'Menu Item',
        itemDescription: 'Description',
        itemPrice: 9.99,
        backgroundColor: '#1f2937',
        color: '#ffffff',
        fontSize: 16,
        fontFamily: 'Inter'
      }
      break

    default:
      return
  }

  store.addElement(elementData as Omit<MenuDesignElement, 'id' | 'zIndex'>)
  emit('tool-used')
}

const handleElementUpdate = (elementId: string, updates: Partial<MenuDesignElement>) => {
  store.updateElement(elementId, updates)
  store.saveHistory()
}

const getElementIcon = (type: string) => {
  const icons: Record<string, any> = {
    text: defineComponent({
      template: '<svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" /></svg>'
    }),
    image: defineComponent({
      template: '<svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" /></svg>'
    }),
    shape: defineComponent({
      template: '<svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01" /></svg>'
    }),
    menuItem: defineComponent({
      template: '<svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" /></svg>'
    }),
    icon: defineComponent({
      template: '<svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" /></svg>'
    })
  }
  return icons[type] || icons.text
}

const getElementName = (element: MenuDesignElement) => {
  switch (element.type) {
    case 'text':
      return element.text?.substring(0, 20) || 'Text'
    case 'image':
      return 'Image'
    case 'shape':
      return element.shapeType || 'Shape'
    case 'menuItem':
      return element.itemName || 'Menu Item'
    default:
      return element.type
  }
}
</script>
