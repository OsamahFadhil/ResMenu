<template>
  <div class="h-full flex flex-col">
    <div class="px-6 py-4 border-b border-neutral-700">
      <h3 class="text-lg font-bold text-white">Properties</h3>
    </div>

    <div v-if="!store.selectedElement" class="flex-1 flex items-center justify-center p-6">
      <div class="text-center text-neutral-500">
        <svg class="w-16 h-16 mx-auto mb-4 opacity-30" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 15l-2 5L9 9l11 4-5 2zm0 0l5 5M7.188 2.239l.777 2.897M5.136 7.965l-2.898-.777M13.95 4.05l-2.122 2.122m-5.657 5.656l-2.12 2.122" />
        </svg>
        <p class="text-sm">Select an element to edit its properties</p>
      </div>
    </div>

    <div v-else class="flex-1 overflow-y-auto p-6 space-y-6">
      <!-- Common Properties -->
      <div class="space-y-4">
        <h4 class="text-sm font-semibold text-white uppercase tracking-wider">Transform</h4>

        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-xs text-neutral-400 mb-1">X</label>
            <input
              type="number"
              :value="Math.round(store.selectedElement.x)"
              @input="updateElement({ x: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            />
          </div>
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Y</label>
            <input
              type="number"
              :value="Math.round(store.selectedElement.y)"
              @input="updateElement({ y: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            />
          </div>
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Width</label>
            <input
              type="number"
              :value="Math.round(store.selectedElement.width)"
              @input="updateElement({ width: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            />
          </div>
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Height</label>
            <input
              type="number"
              :value="Math.round(store.selectedElement.height)"
              @input="updateElement({ height: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            />
          </div>
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Rotation</label>
          <div class="flex items-center gap-2">
            <input
              type="range"
              min="0"
              max="360"
              :value="store.selectedElement.rotation"
              @input="updateElement({ rotation: Number(($event.target as HTMLInputElement).value) })"
              class="flex-1"
            />
            <span class="text-white text-sm w-12 text-right">{{ Math.round(store.selectedElement.rotation) }}°</span>
          </div>
        </div>
      </div>

      <!-- Text Properties -->
      <div v-if="store.selectedElement.type === 'text'" class="space-y-4">
        <h4 class="text-sm font-semibold text-white uppercase tracking-wider">Text</h4>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Content</label>
          <textarea
            :value="store.selectedElement.text"
            @input="updateElement({ text: ($event.target as HTMLTextAreaElement).value })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none resize-none"
            rows="3"
          ></textarea>
        </div>

        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Font Size</label>
            <input
              type="number"
              :value="store.selectedElement.fontSize"
              @input="updateElement({ fontSize: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            />
          </div>
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Color</label>
            <input
              type="color"
              :value="store.selectedElement.color"
              @input="updateElement({ color: ($event.target as HTMLInputElement).value })"
              class="w-full h-10 bg-neutral-700 rounded border border-neutral-600 cursor-pointer"
            />
          </div>
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Font Family</label>
          <select
            :value="store.selectedElement.fontFamily"
            @change="updateElement({ fontFamily: ($event.target as HTMLSelectElement).value })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
          >
            <option value="Inter">Inter</option>
            <option value="Arial">Arial</option>
            <option value="Helvetica">Helvetica</option>
            <option value="Times New Roman">Times New Roman</option>
            <option value="Georgia">Georgia</option>
            <option value="Courier New">Courier New</option>
            <option value="Playfair Display">Playfair Display</option>
            <option value="Roboto">Roboto</option>
            <option value="Open Sans">Open Sans</option>
          </select>
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Font Weight</label>
          <select
            :value="store.selectedElement.fontWeight"
            @change="updateElement({ fontWeight: ($event.target as HTMLSelectElement).value })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
          >
            <option value="normal">Normal</option>
            <option value="bold">Bold</option>
            <option value="lighter">Lighter</option>
            <option value="bolder">Bolder</option>
          </select>
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Text Align</label>
          <div class="flex gap-2">
            <button
              v-for="align in ['left', 'center', 'right']"
              :key="align"
              @click="updateElement({ textAlign: align as any })"
              :class="[
                'flex-1 px-3 py-2 rounded border transition-colors',
                store.selectedElement.textAlign === align
                  ? 'bg-blue-600 border-blue-500 text-white'
                  : 'bg-neutral-700 border-neutral-600 text-neutral-300 hover:border-neutral-500'
              ]"
            >
              {{ align }}
            </button>
          </div>
        </div>
      </div>

      <!-- Image Properties -->
      <div v-if="store.selectedElement.type === 'image'" class="space-y-4">
        <h4 class="text-sm font-semibold text-white uppercase tracking-wider">Image</h4>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Image URL</label>
          <input
            type="text"
            :value="store.selectedElement.imageUrl"
            @input="updateElement({ imageUrl: ($event.target as HTMLInputElement).value })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            placeholder="https://example.com/image.jpg"
          />
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Opacity</label>
          <div class="flex items-center gap-2">
            <input
              type="range"
              min="0"
              max="1"
              step="0.1"
              :value="store.selectedElement.imageOpacity"
              @input="updateElement({ imageOpacity: Number(($event.target as HTMLInputElement).value) })"
              class="flex-1"
            />
            <span class="text-white text-sm w-12 text-right">{{ Math.round((store.selectedElement.imageOpacity || 1) * 100) }}%</span>
          </div>
        </div>
      </div>

      <!-- Shape Properties -->
      <div v-if="store.selectedElement.type === 'shape'" class="space-y-4">
        <h4 class="text-sm font-semibold text-white uppercase tracking-wider">Shape</h4>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Shape Type</label>
          <select
            :value="store.selectedElement.shapeType"
            @change="updateElement({ shapeType: ($event.target as HTMLSelectElement).value as any })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
          >
            <option value="rectangle">Rectangle</option>
            <option value="circle">Circle</option>
          </select>
        </div>

        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Fill Color</label>
            <input
              type="color"
              :value="store.selectedElement.backgroundColor"
              @input="updateElement({ backgroundColor: ($event.target as HTMLInputElement).value })"
              class="w-full h-10 bg-neutral-700 rounded border border-neutral-600 cursor-pointer"
            />
          </div>
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Border Color</label>
            <input
              type="color"
              :value="store.selectedElement.borderColor"
              @input="updateElement({ borderColor: ($event.target as HTMLInputElement).value })"
              class="w-full h-10 bg-neutral-700 rounded border border-neutral-600 cursor-pointer"
            />
          </div>
        </div>

        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Border Width</label>
            <input
              type="number"
              :value="store.selectedElement.borderWidth"
              @input="updateElement({ borderWidth: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            />
          </div>
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Border Radius</label>
            <input
              type="number"
              :value="store.selectedElement.borderRadius"
              @input="updateElement({ borderRadius: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            />
          </div>
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Opacity</label>
          <div class="flex items-center gap-2">
            <input
              type="range"
              min="0"
              max="1"
              step="0.1"
              :value="store.selectedElement.opacity"
              @input="updateElement({ opacity: Number(($event.target as HTMLInputElement).value) })"
              class="flex-1"
            />
            <span class="text-white text-sm w-12 text-right">{{ Math.round((store.selectedElement.opacity || 1) * 100) }}%</span>
          </div>
        </div>
      </div>

      <!-- Menu Item Properties -->
      <div v-if="store.selectedElement.type === 'menuItem'" class="space-y-4">
        <h4 class="text-sm font-semibold text-white uppercase tracking-wider">Menu Item</h4>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Item Name</label>
          <input
            type="text"
            :value="store.selectedElement.itemName"
            @input="updateElement({ itemName: ($event.target as HTMLInputElement).value })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
          />
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Description</label>
          <input
            type="text"
            :value="store.selectedElement.itemDescription"
            @input="updateElement({ itemDescription: ($event.target as HTMLInputElement).value })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
          />
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Price</label>
          <input
            type="number"
            step="0.01"
            :value="store.selectedElement.itemPrice"
            @input="updateElement({ itemPrice: Number(($event.target as HTMLInputElement).value) })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
          />
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Image URL</label>
          <input
            type="text"
            :value="store.selectedElement.itemImage"
            @input="updateElement({ itemImage: ($event.target as HTMLInputElement).value })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            placeholder="https://example.com/image.jpg"
          />
        </div>

        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Background</label>
            <input
              type="color"
              :value="store.selectedElement.backgroundColor"
              @input="updateElement({ backgroundColor: ($event.target as HTMLInputElement).value })"
              class="w-full h-10 bg-neutral-700 rounded border border-neutral-600 cursor-pointer"
            />
          </div>
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Text Color</label>
            <input
              type="color"
              :value="store.selectedElement.color"
              @input="updateElement({ color: ($event.target as HTMLInputElement).value })"
              class="w-full h-10 bg-neutral-700 rounded border border-neutral-600 cursor-pointer"
            />
          </div>
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Font Size</label>
          <input
            type="number"
            :value="store.selectedElement.fontSize"
            @input="updateElement({ fontSize: Number(($event.target as HTMLInputElement).value) })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
          />
        </div>
      </div>

      <!-- Actions -->
      <div class="space-y-2 pt-4 border-t border-neutral-700">
        <button
          @click="duplicateElement"
          class="w-full px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded transition-colors flex items-center justify-center gap-2"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
          </svg>
          Duplicate
        </button>

        <button
          @click="deleteElement"
          class="w-full px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded transition-colors flex items-center justify-center gap-2"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
          Delete
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useMenuDesignerStore } from '@/stores/menuDesigner'
import type { MenuDesignElement } from '@/stores/menuDesigner'

const store = useMenuDesignerStore()

const updateElement = (updates: Partial<MenuDesignElement>) => {
  if (!store.selectedElement) return
  store.updateElement(store.selectedElement.id, updates)
  store.saveHistory()
}

const duplicateElement = () => {
  if (!store.selectedElement) return
  store.duplicateElement(store.selectedElement.id)
}

const deleteElement = () => {
  if (!store.selectedElement) return
  store.deleteElement(store.selectedElement.id)
}
</script>
