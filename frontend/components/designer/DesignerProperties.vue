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

        <!-- Advanced Text Properties -->
        <div class="pt-4 border-t border-neutral-700 space-y-3">
          <h5 class="text-xs font-semibold text-neutral-300 uppercase">Advanced Text</h5>
          
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Letter Spacing</label>
            <input
              type="number"
              :value="store.selectedElement.letterSpacing || 0"
              @input="updateElement({ letterSpacing: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
              step="0.1"
            />
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Text Transform</label>
            <select
              :value="store.selectedElement.textTransform || 'none'"
              @change="updateElement({ textTransform: ($event.target as HTMLSelectElement).value as any })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            >
              <option value="none">None</option>
              <option value="uppercase">Uppercase</option>
              <option value="lowercase">Lowercase</option>
              <option value="capitalize">Capitalize</option>
            </select>
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Text Shadow</label>
            <input
              type="text"
              :value="store.selectedElement.textShadow || ''"
              @input="updateElement({ textShadow: ($event.target as HTMLInputElement).value })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none text-xs"
              placeholder="e.g. 2px 2px 4px rgba(0,0,0,0.5)"
            />
          </div>

          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-xs text-neutral-400 mb-1">Text Stroke Color</label>
              <input
                type="color"
                :value="store.selectedElement.textStroke || '#000000'"
                @input="updateElement({ textStroke: ($event.target as HTMLInputElement).value })"
                class="w-full h-10 bg-neutral-700 rounded border border-neutral-600 cursor-pointer"
              />
            </div>
            <div>
              <label class="block text-xs text-neutral-400 mb-1">Stroke Width</label>
              <input
                type="number"
                :value="store.selectedElement.textStrokeWidth || 0"
                @input="updateElement({ textStrokeWidth: Number(($event.target as HTMLInputElement).value) })"
                class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
                min="0"
                max="10"
              />
            </div>
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

        <!-- Advanced Image Properties -->
        <div class="pt-4 border-t border-neutral-700 space-y-3">
          <h5 class="text-xs font-semibold text-neutral-300 uppercase">Advanced Image</h5>
          
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Object Fit</label>
            <select
              :value="store.selectedElement.objectFit || 'cover'"
              @change="updateElement({ objectFit: ($event.target as HTMLSelectElement).value as any })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            >
              <option value="cover">Cover</option>
              <option value="contain">Contain</option>
              <option value="fill">Fill</option>
              <option value="none">None</option>
              <option value="scale-down">Scale Down</option>
            </select>
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Blur</label>
            <div class="flex items-center gap-2">
              <input
                type="range"
                min="0"
                max="20"
                step="0.5"
                :value="store.selectedElement.imageBlur || 0"
                @input="updateElement({ imageBlur: Number(($event.target as HTMLInputElement).value) })"
                class="flex-1"
              />
              <span class="text-white text-sm w-12 text-right">{{ store.selectedElement.imageBlur || 0 }}px</span>
            </div>
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Brightness</label>
            <div class="flex items-center gap-2">
              <input
                type="range"
                min="0"
                max="200"
                step="1"
                :value="(store.selectedElement.imageBrightness || 100)"
                @input="updateElement({ imageBrightness: Number(($event.target as HTMLInputElement).value) / 100 })"
                class="flex-1"
              />
              <span class="text-white text-sm w-12 text-right">{{ Math.round((store.selectedElement.imageBrightness || 1) * 100) }}%</span>
            </div>
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Contrast</label>
            <div class="flex items-center gap-2">
              <input
                type="range"
                min="0"
                max="200"
                step="1"
                :value="(store.selectedElement.imageContrast || 100)"
                @input="updateElement({ imageContrast: Number(($event.target as HTMLInputElement).value) / 100 })"
                class="flex-1"
              />
              <span class="text-white text-sm w-12 text-right">{{ Math.round((store.selectedElement.imageContrast || 1) * 100) }}%</span>
            </div>
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Saturation</label>
            <div class="flex items-center gap-2">
              <input
                type="range"
                min="0"
                max="200"
                step="1"
                :value="(store.selectedElement.imageSaturate || 100)"
                @input="updateElement({ imageSaturate: Number(($event.target as HTMLInputElement).value) / 100 })"
                class="flex-1"
              />
              <span class="text-white text-sm w-12 text-right">{{ Math.round((store.selectedElement.imageSaturate || 1) * 100) }}%</span>
            </div>
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
            <option value="ellipse">Ellipse</option>
            <option value="triangle">Triangle</option>
            <option value="line">Line</option>
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

        <!-- Advanced Shape Properties -->
        <div class="pt-4 border-t border-neutral-700 space-y-3">
          <h5 class="text-xs font-semibold text-neutral-300 uppercase">Advanced Shape</h5>
          
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Background Gradient</label>
            <input
              type="text"
              :value="store.selectedElement.backgroundGradient || ''"
              @input="updateElement({ backgroundGradient: ($event.target as HTMLInputElement).value })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none text-xs"
              placeholder="linear-gradient(90deg, #ff0000, #0000ff)"
            />
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Border Style</label>
            <select
              :value="store.selectedElement.borderStyle || 'solid'"
              @change="updateElement({ borderStyle: ($event.target as HTMLSelectElement).value as any })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            >
              <option value="solid">Solid</option>
              <option value="dashed">Dashed</option>
              <option value="dotted">Dotted</option>
              <option value="double">Double</option>
            </select>
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Box Shadow</label>
            <input
              type="text"
              :value="store.selectedElement.boxShadow || ''"
              @input="updateElement({ boxShadow: ($event.target as HTMLInputElement).value })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none text-xs"
              placeholder="e.g. 0 4px 6px rgba(0,0,0,0.1)"
            />
          </div>
        </div>
      </div>

      <!-- Menu Item Properties -->
      <div v-if="store.selectedElement.type === 'menuItem' || store.selectedElement.type === 'advancedCard'" class="space-y-4">
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

        <!-- Advanced Menu Item Properties -->
        <div v-if="store.selectedElement.type === 'advancedCard'" class="pt-4 border-t border-neutral-700 space-y-3">
          <h5 class="text-xs font-semibold text-neutral-300 uppercase">Advanced Card</h5>
          
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Card Style</label>
            <select
              :value="store.selectedElement.cardStyle || 'modern'"
              @change="updateElement({ cardStyle: ($event.target as HTMLSelectElement).value as any })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
            >
              <option value="modern">Modern</option>
              <option value="elegant">Elegant</option>
              <option value="minimal">Minimal</option>
              <option value="bold">Bold</option>
              <option value="glassmorphism">Glassmorphism</option>
            </select>
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Card Elevation</label>
            <div class="flex items-center gap-2">
              <input
                type="range"
                min="0"
                max="5"
                step="1"
                :value="store.selectedElement.cardElevation || 0"
                @input="updateElement({ cardElevation: Number(($event.target as HTMLInputElement).value) })"
                class="flex-1"
              />
              <span class="text-white text-sm w-12 text-right">{{ store.selectedElement.cardElevation || 0 }}</span>
            </div>
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Badge</label>
            <input
              type="text"
              :value="store.selectedElement.itemBadge || ''"
              @input="updateElement({ itemBadge: ($event.target as HTMLInputElement).value })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
              placeholder="e.g. NEW, POPULAR"
            />
          </div>

          <div>
            <label class="block text-xs text-neutral-400 mb-1">Tag</label>
            <input
              type="text"
              :value="store.selectedElement.itemTag || ''"
              @input="updateElement({ itemTag: ($event.target as HTMLInputElement).value })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
              placeholder="e.g. Vegetarian, Spicy"
            />
          </div>

          <div>
            <label class="flex items-center gap-2">
              <input
                type="checkbox"
                :checked="store.selectedElement.showDivider || false"
                @change="updateElement({ showDivider: ($event.target as HTMLInputElement).checked })"
                class="w-4 h-4 rounded text-primary-600"
              />
              <span class="text-xs text-neutral-400">Show Divider</span>
            </label>
          </div>
        </div>
      </div>

      <!-- Gradient Properties -->
      <div v-if="store.selectedElement.type === 'gradient'" class="space-y-4">
        <h4 class="text-sm font-semibold text-white uppercase tracking-wider">Gradient</h4>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Gradient Type</label>
          <select
            :value="store.selectedElement.gradientType || 'linear'"
            @change="updateElement({ gradientType: ($event.target as HTMLSelectElement).value as any })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
          >
            <option value="linear">Linear</option>
            <option value="radial">Radial</option>
            <option value="conic">Conic</option>
          </select>
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Gradient Angle (for linear/conic)</label>
          <div class="flex items-center gap-2">
            <input
              type="range"
              min="0"
              max="360"
              step="1"
              :value="store.selectedElement.gradientAngle || 90"
              @input="updateElement({ gradientAngle: Number(($event.target as HTMLInputElement).value) })"
              class="flex-1"
            />
            <span class="text-white text-sm w-12 text-right">{{ store.selectedElement.gradientAngle || 90 }}°</span>
          </div>
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Gradient Colors (comma-separated)</label>
          <input
            type="text"
            :value="store.selectedElement.gradientColors?.join(', ') || '#000000, #ffffff'"
            @input="updateElement({ gradientColors: ($event.target as HTMLInputElement).value.split(',').map(c => c.trim()) })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none text-xs"
            placeholder="#ff0000, #0000ff, #00ff00"
          />
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Box Shadow</label>
          <input
            type="text"
            :value="store.selectedElement.boxShadow || ''"
            @input="updateElement({ boxShadow: ($event.target as HTMLInputElement).value })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none text-xs"
            placeholder="e.g. 0 4px 6px rgba(0,0,0,0.1)"
          />
        </div>
      </div>

      <!-- Global Advanced Effects (for all elements) -->
      <div class="pt-4 border-t border-neutral-700 space-y-4">
        <h4 class="text-sm font-semibold text-white uppercase tracking-wider">Advanced Effects</h4>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Box Shadow</label>
          <input
            type="text"
            :value="store.selectedElement.boxShadow || ''"
            @input="updateElement({ boxShadow: ($event.target as HTMLInputElement).value })"
            class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none text-xs"
            placeholder="e.g. 0 4px 6px rgba(0,0,0,0.1)"
          />
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Scale</label>
          <div class="flex items-center gap-2">
            <input
              type="range"
              min="0.5"
              max="2"
              step="0.1"
              :value="store.selectedElement.scale || 1"
              @input="updateElement({ scale: Number(($event.target as HTMLInputElement).value) })"
              class="flex-1"
            />
            <span class="text-white text-sm w-12 text-right">{{ (store.selectedElement.scale || 1).toFixed(1) }}x</span>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Skew X</label>
            <input
              type="number"
              :value="store.selectedElement.skewX || 0"
              @input="updateElement({ skewX: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
              step="1"
            />
          </div>
          <div>
            <label class="block text-xs text-neutral-400 mb-1">Skew Y</label>
            <input
              type="number"
              :value="store.selectedElement.skewY || 0"
              @input="updateElement({ skewY: Number(($event.target as HTMLInputElement).value) })"
              class="w-full px-3 py-2 bg-neutral-700 text-white rounded border border-neutral-600 focus:border-blue-500 focus:outline-none"
              step="1"
            />
          </div>
        </div>

        <div>
          <label class="block text-xs text-neutral-400 mb-1">Blur Filter</label>
          <div class="flex items-center gap-2">
            <input
              type="range"
              min="0"
              max="20"
              step="0.5"
              :value="store.selectedElement.blur || 0"
              @input="updateElement({ blur: Number(($event.target as HTMLInputElement).value) })"
              class="flex-1"
            />
            <span class="text-white text-sm w-12 text-right">{{ store.selectedElement.blur || 0 }}px</span>
          </div>
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
