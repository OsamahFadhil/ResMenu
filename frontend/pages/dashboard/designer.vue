<template>
  <NuxtLayout name="dashboard">
    <div class="h-[calc(100vh-4rem)] flex flex-col">
      <!-- Header -->
      <div class="flex justify-between items-center px-6 py-4 bg-white border-b border-neutral-200">
        <div>
          <h1 class="text-2xl font-bold text-neutral-900">Menu Designer</h1>
          <p class="mt-1 text-sm text-neutral-600">Drag and drop to design your menu layout</p>
        </div>
        <div class="flex gap-3 items-center">
          <UiButton @click="previewMenu" variant="secondary" size="sm">
            <svg class="mr-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
            </svg>
            Preview
          </UiButton>
          <UiButton @click="() => saveDesign()" :loading="saving" variant="primary" size="sm">
            <svg class="mr-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-3m-1 4l-3 3m0 0l-3-3m3 3V4" />
            </svg>
            Save & Publish
          </UiButton>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="initialLoading" class="flex flex-1 justify-center items-center">
        <div class="text-center">
          <div class="inline-flex justify-center items-center mb-4 w-12 h-12 rounded-full border-4 animate-spin border-primary-200 border-t-primary-600"></div>
          <p class="text-neutral-600">Loading your menu...</p>
        </div>
      </div>

      <!-- Main Content: Three-Panel Layout -->
      <div v-else class="flex overflow-hidden flex-1">
        <!-- Left Panel: Your Menu (Content Source) -->
        <div class="overflow-y-auto w-64 border-r bg-neutral-50 border-neutral-200">
          <div class="p-4">
            <h3 class="flex items-center mb-3 text-sm font-semibold text-neutral-900">
              <svg class="mr-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
              Your Menu
            </h3>

            <!-- Categories List -->
            <div v-if="flattenedCategories.length > 0" class="space-y-2">
              <div
                v-for="entry in flattenedCategories"
                :key="entry.node.id"
                draggable="true"
                @dragstart="onCategoryDragStart($event, entry.node)"
                class="p-3 bg-white rounded-lg border-2 transition-all cursor-move border-neutral-200 hover:border-primary-400 hover:shadow-sm"
              >
                <div class="flex gap-2 items-center">
                  <span class="text-lg">{{ entry.node.icon || '📁' }}</span>
                  <div class="flex-1 min-w-0">
                    <div
                      class="text-sm font-medium truncate text-neutral-900"
                      :style="{ paddingLeft: `${entry.depth * 12}px` }"
                    >
                      {{ entry.node.name }}
                    </div>
                    <div class="text-xs text-neutral-500">
                      {{ entry.node.items?.length || 0 }} items
                    </div>
                    <div v-if="entry.node.children?.length" class="text-xs text-neutral-400">
                      {{ entry.node.children.length }} subcategories
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Empty State -->
            <div v-else class="py-8 text-sm text-center text-neutral-500">
              <p>No categories yet</p>
              <router-link to="/dashboard/categories" class="inline-block mt-2 text-primary-600 hover:underline">
                Create categories
              </router-link>
            </div>
          </div>
        </div>

        <!-- Center Panel: Live Canvas -->
        <div class="overflow-y-auto flex-1 p-6" :style="previewBackgroundStyle">
          <div class="mx-auto max-w-4xl">
            <!-- Restaurant Header Preview -->
            <div class="overflow-hidden relative p-6 mb-6 rounded-xl border shadow-lg border-neutral-200" :style="headerContainerStyle">
              <div :class="headerContentClass">
                <div
                  v-if="globalSettings.showLogo && restaurantInfo.logoUrl"
                  class="overflow-hidden w-16 h-16 rounded-lg border-2 border-white shadow-md backdrop-blur bg-white/80"
                  :class="logoOrderClass"
                >
                  <img :src="restaurantInfo.logoUrl" alt="Logo" class="object-cover w-full h-full" />
                </div>
                <div :class="headerTextAlignClass">
                  <h1 class="text-2xl font-bold tracking-tight sm:text-3xl">
                    {{ restaurantInfo.name }}
                  </h1>
                  <p v-if="globalSettings.tagline" class="text-sm font-medium opacity-80">
                    {{ globalSettings.tagline }}
                  </p>
                  <p
                    v-if="globalSettings.showRestaurantInfo && restaurantInfo.address"
                    class="text-xs opacity-70 sm:text-sm"
                  >
                    {{ restaurantInfo.address }}
                  </p>
                </div>
              </div>
            </div>

            <!-- Drop Zone -->
            <div
              @drop="onDrop"
              @dragover="onDragOver"
              class="p-6 rounded-xl border-2 border-dashed transition-colors min-h-96 border-neutral-300"
              :class="{ 'border-primary-500 bg-primary-50': isDragging }"
            >
              <!-- Empty State -->
              <div v-if="designedCategories.length === 0" class="flex flex-col justify-center items-center py-16 text-center">
                <svg class="mb-4 w-16 h-16 text-neutral-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
                </svg>
                <h3 class="mb-2 text-lg font-semibold text-neutral-900">Drag categories here</h3>
                <p class="max-w-md text-sm text-neutral-600">
                  Start building your menu by dragging categories from the left panel
                </p>
              </div>

              <!-- Designed Categories -->
              <div v-else class="space-y-6">
                <div
                  v-for="(catLayout, index) in designedCategories"
                  :key="catLayout.categoryId"
                  @click="selectCategory(catLayout)"
                  class="p-4 rounded-xl border-2 transition-all cursor-pointer bg-neutral-50"
                  :class="{
                    'border-primary-600 shadow-md': selectedCategory?.categoryId === catLayout.categoryId,
                    'border-neutral-200 hover:border-neutral-300': selectedCategory?.categoryId !== catLayout.categoryId
                  }"
                >
                  <!-- Category Header -->
                  <div class="flex justify-between items-center mb-4">
                    <div class="flex gap-3 items-center">
                      <div class="flex gap-1">
                        <button
                          @click.stop="moveCategoryUp(index)"
                          :disabled="index === 0"
                          class="p-1 rounded hover:bg-neutral-200 disabled:opacity-30 disabled:cursor-not-allowed"
                        >
                          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7" />
                          </svg>
                        </button>
                        <button
                          @click.stop="moveCategoryDown(index)"
                          :disabled="index === designedCategories.length - 1"
                          class="p-1 rounded hover:bg-neutral-200 disabled:opacity-30 disabled:cursor-not-allowed"
                        >
                          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                          </svg>
                        </button>
                      </div>
                      <h3 class="text-lg font-bold" :style="{ color: globalTheme.primaryColor }">
                        {{ getCategoryBreadcrumb(catLayout.categoryId) }}
                      </h3>
                      <span class="px-2 py-1 text-xs font-medium rounded-full bg-primary-100 text-primary-700">
                        {{ catLayout.layout }}
                      </span>
                    </div>
                    <button
                      @click.stop="removeCategory(index)"
                      class="p-2 text-red-600 rounded-lg transition hover:bg-red-50"
                    >
                      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                    </button>
                  </div>

                  <!-- Preview Items (mock) -->
                  <div :class="getPreviewClass(catLayout)">
                    <div
                      v-for="i in Math.min(4, getCategoryItemCount(catLayout.categoryId))"
                      :key="i"
                      :class="getItemPreviewClass(catLayout)"
                    >
                      <div v-if="catLayout.showImages" :class="getImageClass(catLayout)" class="bg-neutral-200"></div>
                      <div class="flex-1 min-w-0">
                        <div class="mb-2 w-3/4 h-3 rounded bg-neutral-300"></div>
                        <div v-if="catLayout.showDescriptions" class="w-full h-2 rounded bg-neutral-200"></div>
                      </div>
                      <div v-if="catLayout.showPrices" class="w-16 h-3 rounded bg-primary-300"></div>
                    </div>
                  </div>
                </div>
              </div>
              <div class="pt-6 mt-8 space-y-4 border-t border-dashed border-neutral-200">
                <div class="flex justify-between items-center">
                  <h4 class="text-base font-semibold text-neutral-800">Live Preview</h4>
                  <span class="text-xs text-neutral-500">Updates instantly as you customize</span>
                </div>
                <div class="overflow-hidden rounded-3xl border border-neutral-200" :style="previewBackgroundStyle">
                  <PublicMenuCategoryTree
                    v-if="previewCategories.length"
                    :categories="previewCategories"
                    :settings="previewTheme"
                    :displaySettings="previewDisplaySettings"
                    :layoutConfiguration="previewLayoutConfiguration"
                    :currency="previewCurrency"
                    designMode
                    :selectedCategoryId="selectedCategory?.categoryId || null"
                    @category-click="handlePreviewCategoryClick"
                  />
                  <div v-else class="p-6 text-sm text-center text-neutral-500">
                    Add categories to see the live preview of your menu.
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Panel: Customization -->
        <div class="overflow-y-auto w-80 border-l bg-neutral-50 border-neutral-200">
          <div class="p-4">
            <!-- Global Theme Tab -->
            <div v-if="!selectedCategory" class="space-y-6">
              <div>
                <h3 class="flex items-center mb-3 text-sm font-semibold text-neutral-900">
                  <svg class="mr-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01" />
                  </svg>
                  Global Theme
                </h3>
                <p class="mb-4 text-xs text-neutral-600">These settings apply to your entire menu</p>
              </div>

              <!-- Primary Color -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Primary Color</label>
                <input
                  v-model="globalTheme.primaryColor"
                  type="color"
                  class="w-full h-10 rounded-lg border-2 cursor-pointer border-neutral-300"
                />
              </div>

              <!-- Accent Color -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Accent Color</label>
                <input
                  v-model="globalTheme.accentColor"
                  type="color"
                  class="w-full h-10 rounded-lg border-2 cursor-pointer border-neutral-300"
                />
              </div>

              <!-- Background Style -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Background Style</label>
                <div class="grid grid-cols-3 gap-2">
                  <button
                    v-for="type in ['color', 'image', 'gradient'] as const"
                    :key="type"
                    @click="globalTheme.backgroundType = type"
                    :class="[
                      'px-3 py-2 text-xs rounded-lg border-2 transition capitalize',
                      globalTheme.backgroundType === type
                        ? 'border-primary-600 bg-primary-50 text-primary-700'
                        : 'border-neutral-300 text-neutral-700 hover:border-neutral-400'
                    ]"
                  >
                    {{ type }}
                  </button>
                </div>
              </div>

              <!-- Background Color -->
              <div v-if="globalTheme.backgroundType !== 'image'">
                <label class="block mb-2 text-xs font-medium text-neutral-700">
                  {{ globalTheme.backgroundType === 'gradient' ? 'Fallback Background Color' : 'Background Color' }}
                </label>
                <input
                  v-model="globalTheme.backgroundColor"
                  type="color"
                  class="w-full h-10 rounded-lg border-2 cursor-pointer border-neutral-300"
                />
              </div>

              <!-- Background Gradient -->
              <div v-if="globalTheme.backgroundType === 'gradient'">
                <label class="block mb-2 text-xs font-medium text-neutral-700">Gradient CSS</label>
                <input
                  v-model="globalTheme.backgroundGradient"
                  type="text"
                  placeholder="e.g. linear-gradient(135deg, #dc2626 0%, #f97316 100%)"
                  class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                />
                <p class="mt-1 text-xs text-neutral-500">
                  Use any valid CSS gradient. The fallback color will show while loading.
                </p>
              </div>

              <!-- Background Image -->
              <div v-if="globalTheme.backgroundType === 'image'" class="space-y-3">
                <div v-if="globalTheme.backgroundImageUrl" class="relative">
                  <img
                    :src="globalTheme.backgroundImageUrl"
                    alt="Background preview"
                    class="object-cover w-full h-32 rounded-lg border border-neutral-300"
                  />
                  <button
                    @click="globalTheme.backgroundImageUrl = null"
                    class="absolute top-2 right-2 p-1.5 text-white bg-red-500 rounded-full shadow-md transition hover:bg-red-600"
                    title="Remove background"
                  >
                    <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>

                <div class="space-y-2">
                  <label class="block cursor-pointer">
                    <input
                      ref="backgroundFileInput"
                      type="file"
                      accept="image/*"
                      @change="handleBackgroundUpload"
                      class="hidden"
                    />
                    <div class="px-4 py-2.5 text-sm font-medium text-center text-white rounded-lg transition bg-primary-600 hover:bg-primary-700">
                      <svg class="inline-block mr-1.5 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                      </svg>
                      {{ globalTheme.backgroundImageUrl ? 'Change Background' : 'Upload Background' }}
                    </div>
                  </label>

                  <div v-if="uploadingBackground" class="flex gap-2 justify-center items-center py-2 text-xs text-neutral-600">
                    <div class="w-3 h-3 rounded-full border-2 animate-spin border-primary-600 border-t-transparent"></div>
                    <span>Uploading background...</span>
                  </div>

                  <div>
                    <label class="block mb-1 text-xs font-medium text-neutral-700">Image URL</label>
                    <input
                      v-model="globalTheme.backgroundImageUrl"
                      type="text"
                      placeholder="https://example.com/background.jpg"
                      class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                    />
                  </div>

                  <div>
                    <label class="block mb-2 text-xs font-medium text-neutral-700">Overlay</label>
                    <div class="grid grid-cols-3 gap-2">
                      <button
                        v-for="overlay in ['none', 'light', 'dark'] as const"
                        :key="overlay"
                        @click="globalTheme.backgroundOverlay = overlay"
                        :class="[
                          'px-3 py-2 text-xs rounded-lg border-2 transition capitalize',
                          globalTheme.backgroundOverlay === overlay
                            ? 'border-primary-600 bg-primary-50 text-primary-700'
                            : 'border-neutral-300 text-neutral-700 hover:border-neutral-400'
                        ]"
                      >
                        {{ overlay }}
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Component Defaults -->
              <div class="px-3 py-3 space-y-3 rounded-lg border bg-white/70">
                <div class="flex justify-between items-center">
                  <label class="text-xs font-semibold tracking-wide uppercase text-neutral-700">Component Styles</label>
                  <button
                    type="button"
                    class="text-xs font-medium text-primary-600 hover:text-primary-700"
                    @click="applyDefaultsToCategories"
                  >
                    Apply to all
                  </button>
                </div>

                <div class="grid grid-cols-1 gap-3 sm:grid-cols-2">
                  <div>
                    <label class="block mb-1 text-xs font-medium text-neutral-700">Default Layout</label>
                    <select
                      v-model="globalTheme.layout"
                      class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                    >
                      <option value="list">List</option>
                      <option value="grid">Grid</option>
                      <option value="cards">Cards</option>
                    </select>
                  </div>
                  <div>
                    <label class="block mb-1 text-xs font-medium text-neutral-700">Card Style</label>
                    <select
                      v-model="globalTheme.cardStyle"
                      class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                    >
                      <option value="modern">Modern</option>
                      <option value="classic">Classic</option>
                      <option value="minimal">Minimal</option>
                    </select>
                  </div>
                  <div>
                    <label class="block mb-1 text-xs font-medium text-neutral-700">Spacing</label>
                    <select
                      v-model="globalTheme.spacing"
                      class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                    >
                      <option value="compact">Compact</option>
                      <option value="normal">Comfortable</option>
                      <option value="relaxed">Relaxed</option>
                    </select>
                  </div>
                  <div>
                    <label class="block mb-1 text-xs font-medium text-neutral-700">Border Radius</label>
                    <select
                      v-model="globalTheme.borderRadius"
                      class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                    >
                      <option value="none">None</option>
                      <option value="small">Small</option>
                      <option value="medium">Medium</option>
                      <option value="large">Large</option>
                    </select>
                  </div>
                  <div>
                    <label class="block mb-1 text-xs font-medium text-neutral-700">Image Size</label>
                    <select
                      v-model="globalTheme.imageSize"
                      class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                    >
                      <option value="small">Small</option>
                      <option value="medium">Medium</option>
                      <option value="large">Large</option>
                    </select>
                  </div>
                  <div>
                    <label class="block mb-1 text-xs font-medium text-neutral-700">Image Shape</label>
                    <select
                      v-model="globalTheme.imageShape"
                      class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                    >
                      <option value="rounded">Rounded</option>
                      <option value="square">Square</option>
                      <option value="circle">Circle</option>
                    </select>
                  </div>
                </div>

                <label class="flex justify-between items-center text-sm text-neutral-700">
                  <span>Show Images by Default</span>
                  <input type="checkbox" v-model="globalTheme.showImages" class="w-4 h-4 rounded text-primary-600" />
                </label>

                <p class="text-xs text-neutral-500">
                  These defaults apply to new categories you drag into the canvas. Use “Apply to all” to update existing
                  sections.
                </p>
              </div>

              <!-- Font Family -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Font Family</label>
                <select
                  v-model="globalTheme.fontFamily"
                  class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                >
                  <option value="Inter">Inter</option>
                  <option value="Roboto">Roboto</option>
                  <option value="Open Sans">Open Sans</option>
                  <option value="Playfair Display">Playfair Display</option>
                </select>
              </div>

              <!-- Surface Color -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Surface Color</label>
                <input
                  v-model="globalTheme.surfaceColor"
                  type="color"
                  class="w-full h-10 rounded-lg border-2 cursor-pointer border-neutral-300"
                />
              </div>

              <!-- Text Color -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Text Color</label>
                <input
                  v-model="globalTheme.textColor"
                  type="color"
                  class="w-full h-10 rounded-lg border-2 cursor-pointer border-neutral-300"
                />
              </div>

              <!-- Header Style -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Header Style</label>
                <select
                  v-model="globalSettings.headerStyle"
                  class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                >
                  <option value="simple">Simple</option>
                  <option value="banner">Banner</option>
                  <option value="overlay">Overlay</option>
                </select>
              </div>

              <!-- Logo Position -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Logo Position</label>
                <div class="grid grid-cols-3 gap-2">
                  <button
                    v-for="pos in ['left', 'center', 'right'] as const"
                    :key="pos"
                    @click="globalSettings.logoPosition = pos as 'left' | 'center' | 'right'"
                    :class="[
                      'px-3 py-2 text-xs rounded-lg border-2 transition',
                      globalSettings.logoPosition === pos
                        ? 'border-primary-600 bg-primary-50 text-primary-700'
                        : 'border-neutral-300 text-neutral-700 hover:border-neutral-400'
                    ]"
                  >
                    {{ pos }}
                  </button>
                </div>
              </div>

              <!-- Header Alignment -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Header Alignment</label>
                <div class="grid grid-cols-3 gap-2">
                  <button
                    v-for="align in ['left', 'center', 'right'] as const"
                    :key="align"
                    @click="globalSettings.headerAlignment = align"
                    :class="[
                      'px-3 py-2 text-xs rounded-lg border-2 transition capitalize',
                      globalSettings.headerAlignment === align
                        ? 'border-primary-600 bg-primary-50 text-primary-700'
                        : 'border-neutral-300 text-neutral-700 hover:border-neutral-400'
                    ]"
                  >
                    {{ align }}
                  </button>
                </div>
              </div>

              <!-- Header Tagline -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Tagline / Subtitle</label>
                <input
                  v-model="globalSettings.tagline"
                  type="text"
                  maxlength="80"
                  placeholder="e.g. Finest Mediterranean Cuisine Since 1987"
                  class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                />
                <p class="mt-1 text-xs text-neutral-500">
                  Optional subtitle that appears below your restaurant name.
                </p>
              </div>

              <!-- Header Visibility -->
              <div class="px-3 py-3 space-y-2 rounded-lg border bg-white/70">
                <label class="block text-xs font-semibold text-neutral-700">Header Elements</label>
                <label class="flex justify-between items-center text-sm text-neutral-700">
                  <span>Show Logo</span>
                  <input type="checkbox" v-model="globalSettings.showLogo" class="w-4 h-4 rounded text-primary-600" />
                </label>
                <label class="flex justify-between items-center text-sm text-neutral-700">
                  <span>Show Restaurant Info</span>
                  <input type="checkbox" v-model="globalSettings.showRestaurantInfo" class="w-4 h-4 rounded text-primary-600" />
                </label>
              </div>

              <!-- Logo Upload -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Restaurant Logo</label>
                
                <!-- Current Logo Preview -->
                <div v-if="restaurantInfo.logoUrl" class="relative mx-auto mb-3 w-full h-32">
                  <img :src="restaurantInfo.logoUrl" alt="Logo" class="object-contain p-2 w-full h-full bg-white rounded-lg border-2 border-neutral-300" />
                  <button
                    @click="restaurantInfo.logoUrl = ''"
                    class="absolute -top-2 -right-2 p-1.5 text-white bg-red-500 rounded-full shadow-md transition hover:bg-red-600"
                    title="Remove logo"
                  >
                    <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
                
                <!-- Upload Button -->
                <div class="space-y-2">
                  <label class="block cursor-pointer">
                    <input
                      ref="logoFileInput"
                      type="file"
                      accept="image/*"
                      @change="handleLogoUpload"
                      class="hidden"
                    />
                    <div class="px-4 py-2.5 text-sm font-medium text-center text-white rounded-lg transition bg-primary-600 hover:bg-primary-700">
                      <svg class="inline-block mr-1.5 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                      </svg>
                      {{ restaurantInfo.logoUrl ? 'Change Logo' : 'Upload Logo' }}
                    </div>
                  </label>
                  
                  <!-- Loading State -->
                  <div v-if="uploadingLogo" class="flex gap-2 justify-center items-center py-2 text-xs text-neutral-600">
                    <div class="w-3 h-3 rounded-full border-2 animate-spin border-primary-600 border-t-transparent"></div>
                    <span>Uploading...</span>
                  </div>

                  <!-- Help Text -->
                  <p class="text-xs text-center text-neutral-500">Max 5MB • PNG, JPG, GIF</p>
                </div>
              </div>

              <!-- Restaurant Name -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Restaurant Name</label>
                <input
                  v-model="restaurantInfo.name"
                  type="text"
                  placeholder="Enter restaurant name"
                  class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300 focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                />
              </div>
            </div>

            <!-- Category Customization -->
            <div v-else class="space-y-4">
              <div>
                <h3 class="mb-1 text-sm font-semibold text-neutral-900">
                  {{ getCategoryName(selectedCategory.categoryId) }}
                </h3>
                <p class="mb-4 text-xs text-neutral-600">Customize how this category appears</p>
              </div>

              <!-- Layout -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Layout Style</label>
                <div class="grid grid-cols-3 gap-2">
                  <button
                    v-for="layout in ['list', 'grid', 'cards'] as const"
                    :key="layout"
                    @click="selectedCategory.layout = layout as 'list' | 'grid' | 'cards'"
                    :class="[
                      'px-3 py-2 text-xs rounded-lg border-2 transition',
                      selectedCategory.layout === layout
                        ? 'border-primary-600 bg-primary-50 text-primary-700'
                        : 'border-neutral-300 text-neutral-700 hover:border-neutral-400'
                    ]"
                  >
                    {{ layout }}
                  </button>
                </div>
              </div>

              <!-- Card Style -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Card Style</label>
                <div class="grid grid-cols-3 gap-2">
                  <button
                    v-for="style in ['modern', 'classic', 'minimal'] as const"
                    :key="style"
                    @click="selectedCategory.cardStyle = style as 'modern' | 'classic' | 'minimal'"
                    :class="[
                      'px-3 py-2 text-xs rounded-lg border-2 transition',
                      selectedCategory.cardStyle === style
                        ? 'border-primary-600 bg-primary-50 text-primary-700'
                        : 'border-neutral-300 text-neutral-700 hover:border-neutral-400'
                    ]"
                  >
                    {{ style }}
                  </button>
                </div>
              </div>

              <!-- Columns (for grid layout) -->
              <div v-if="selectedCategory.layout === 'grid'">
                <label class="block mb-2 text-xs font-medium text-neutral-700">Columns</label>
                <input
                  v-model.number="selectedCategory.columns"
                  type="range"
                  min="2"
                  max="4"
                  class="w-full"
                />
                <div class="mt-1 text-xs text-center text-neutral-600">{{ selectedCategory.columns }} columns</div>
              </div>

              <!-- Image Size -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Image Size</label>
                <select
                  v-model="selectedCategory.imageSize"
                  class="px-3 py-2 w-full text-sm rounded-lg border border-neutral-300"
                >
                  <option value="small">Small</option>
                  <option value="medium">Medium</option>
                  <option value="large">Large</option>
                </select>
              </div>

              <!-- Image Shape -->
              <div>
                <label class="block mb-2 text-xs font-medium text-neutral-700">Image Shape</label>
                <div class="grid grid-cols-3 gap-2">
                  <button
                    v-for="shape in ['square', 'rounded', 'circle'] as const"
                    :key="shape"
                    @click="selectedCategory.imageShape = shape as 'square' | 'rounded' | 'circle'"
                    :class="[
                      'px-3 py-2 text-xs rounded-lg border-2 transition',
                      selectedCategory.imageShape === shape
                        ? 'border-primary-600 bg-primary-50 text-primary-700'
                        : 'border-neutral-300 text-neutral-700 hover:border-neutral-400'
                    ]"
                  >
                    {{ shape }}
                  </button>
                </div>
              </div>

              <!-- Toggles -->
              <div class="pt-4 space-y-2 border-t">
                <label class="flex gap-2 items-center">
                  <input
                    type="checkbox"
                    v-model="selectedCategory.showImages"
                    class="w-4 h-4 rounded text-primary-600"
                  />
                  <span class="text-sm text-neutral-700">Show Images</span>
                </label>
                <label class="flex gap-2 items-center">
                  <input
                    type="checkbox"
                    v-model="selectedCategory.showPrices"
                    class="w-4 h-4 rounded text-primary-600"
                  />
                  <span class="text-sm text-neutral-700">Show Prices</span>
                </label>
                <label class="flex gap-2 items-center">
                  <input
                    type="checkbox"
                    v-model="selectedCategory.showDescriptions"
                    class="w-4 h-4 rounded text-primary-600"
                  />
                  <span class="text-sm text-neutral-700">Show Descriptions</span>
                </label>
              </div>

              <!-- Back Button -->
              <div class="pt-4 border-t">
                <button
                  @click="selectedCategory = null"
                  class="px-4 py-2 w-full text-sm bg-white rounded-lg border text-neutral-700 border-neutral-300 hover:bg-neutral-50"
                >
                  ← Back to Global Theme
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useAuthStore } from '~/stores/auth'
import { useRestaurantStore } from '~/stores/restaurant'
import { useMenuDesign, type CategoryLayout, type GlobalLayoutSettings } from '~/composables/useMenuDesign'
import { useApi } from '~/composables/useApi'
import { createDefaultTheme, type TemplateTheme } from '~/stores/templates'
import UiButton from '~/components/ui/Button.vue'
import PublicMenuCategoryTree from '~/components/PublicMenuCategoryTree.vue'

definePageMeta({
  layout: false,
  middleware: ['owner']
})

const authStore = useAuthStore()
const restaurantStore = useRestaurantStore()
const { getActiveDesign, saveDesign: saveDesignApi } = useMenuDesign()
const api = useApi()
const toast = useToast()
const router = useRouter()

// State
const initialLoading = ref(true)
const saving = ref(false)
const isDragging = ref(false)
const selectedCategory = ref<CategoryLayout | null>(null)
const isInitialLoad = ref(true) // Track if we're still in initial data load phase

const availableCategories = ref<any[]>([])
const flattenCategories = (categories: any[], depth = 0): Array<{ node: any; depth: number }> => {
  const result: Array<{ node: any; depth: number }> = []
  categories.forEach((category) => {
    result.push({ node: category, depth })
    if (category.children && category.children.length > 0) {
      result.push(...flattenCategories(category.children, depth + 1))
    }
  })
  return result
}
const flattenedCategories = computed(() => flattenCategories(availableCategories.value))
const designedCategories = ref<CategoryLayout[]>([])

const globalTheme = ref<TemplateTheme>(createDefaultTheme())
const globalSettings = ref<GlobalLayoutSettings>({
  spacing: 'normal',
  borderRadius: 'medium',
  fontFamily: 'Inter',
  headerStyle: 'simple',
  logoPosition: 'center',
  showRestaurantInfo: true,
  showLogo: true,
  headerAlignment: 'center',
  tagline: ''
})

const restaurantInfo = ref({
  name: '',
  logoUrl: '',
  address: ''
})

const logoFileInput = ref<HTMLInputElement | null>(null)
const uploadingLogo = ref(false)
const backgroundFileInput = ref<HTMLInputElement | null>(null)
const uploadingBackground = ref(false)
const restaurantCurrency = ref('USD')

const cloneDeep = <T>(value: T): T => {
  if (value === null || value === undefined) {
    return value
  }
  
  try {
    // Use JSON serialization as primary method - it's safer and strips non-serializable properties
    // This prevents issues with functions, DOM elements, circular refs, and Vue reactive proxies
    return JSON.parse(JSON.stringify(value)) as T
  } catch (error) {
    console.warn('cloneDeep failed, returning original value:', error)
    // If cloning fails completely, return a shallow copy or original
    if (Array.isArray(value)) {
      return [...value] as T
    }
    if (typeof value === 'object') {
      return { ...value } as T
    }
    return value
  }
}

// Computed
const headerContainerStyle = computed(() => {
  const primary = globalTheme.value.primaryColor || '#dc2626'
  const accent = globalTheme.value.accentColor || '#f59e0b'
  const surface = globalTheme.value.surfaceColor || '#ffffff'
  const text = globalTheme.value.textColor || '#1f2937'

  if (globalSettings.value.headerStyle === 'banner') {
    // Banner style: use gradient background
    return {
      backgroundImage: `linear-gradient(135deg, ${primary}, ${accent})`,
      backgroundColor: primary, // Fallback
      color: '#ffffff'
    }
  }

  if (globalSettings.value.headerStyle === 'overlay') {
    // Overlay style: dark overlay for readability
    return {
      backgroundColor: 'rgba(0, 0, 0, 0.4)',
      backdropFilter: 'blur(10px)',
      color: '#ffffff'
    }
  }

  // Simple style: transparent or semi-transparent to show page background
  const hasBackgroundImage = globalTheme.value.backgroundType === 'image' && globalTheme.value.backgroundImageUrl
  const hasBackgroundGradient = globalTheme.value.backgroundType === 'gradient' && globalTheme.value.backgroundGradient
  
  if (hasBackgroundImage || hasBackgroundGradient) {
    // Transparent header to show page background
    return {
      backgroundColor: 'rgba(255, 255, 255, 0.1)',
      backdropFilter: 'blur(8px)',
      color: text
    }
  }

  // Use surface color when no background image/gradient
  return {
    backgroundColor: surface,
    color: text
  }
})

const headerContentClass = computed(() => {
  const classes = ['flex', 'w-full', 'gap-4']
  const alignment = globalSettings.value.headerAlignment || 'center'

  if (alignment === 'left') {
    classes.push('flex-col sm:flex-row', 'items-start', 'sm:items-center', 'text-left')
  } else if (alignment === 'right') {
    classes.push('flex-col', 'items-end', 'text-right')
  } else {
    classes.push('flex-col', 'items-center', 'text-center')
  }

  return classes.join(' ')
})

const headerTextAlignClass = computed(() => {
  const alignment = globalSettings.value.headerAlignment || 'center'
  if (alignment === 'left') return 'flex flex-col items-start text-left space-y-1'
  if (alignment === 'right') return 'flex flex-col items-end text-right space-y-1'
  return 'flex flex-col items-center text-center space-y-1'
})

const logoOrderClass = computed(() => {
  const position = globalSettings.value.logoPosition || 'center'
  if (position === 'left') return 'order-0'
  if (position === 'right') return 'order-2 sm:order-last'
  return 'order-0'
})

const previewCategories = computed(() => {
  if (!designedCategories.value.length || !availableCategories.value.length) return []
  
  return designedCategories.value
    .map((layout) => {
      try {
        const category = findCategoryById(availableCategories.value, layout.categoryId)
        if (!category) return null
        // Only clone the properties we need for preview
        return {
          id: category.id,
          name: category.name,
          localizedName: category.localizedName,
          items: category.items?.map((item: any) => ({
            id: item.id,
            name: item.name,
            localizedName: item.localizedName,
            description: item.description,
            localizedDescription: item.localizedDescription,
            price: item.price,
            imageUrl: item.imageUrl,
            images: item.images,
            isAvailable: item.isAvailable
          })) || [],
          children: category.children || []
        }
      } catch (error) {
        console.warn('Failed to process category for preview:', error)
        return null
      }
    })
    .filter((category): category is any => !!category)
})

const previewLayoutConfiguration = computed(() => {
  try {
    return {
      categories: cloneDeep(designedCategories.value) || [],
      globalSettings: cloneDeep(globalSettings.value) || {
        spacing: 'normal',
        borderRadius: 'medium',
        fontFamily: 'Inter',
        headerStyle: 'simple',
        logoPosition: 'center',
        showRestaurantInfo: true,
        showLogo: true,
        headerAlignment: 'center',
        tagline: ''
      }
    }
  } catch (error) {
    console.warn('Failed to clone layout configuration:', error)
    return {
      categories: [],
      globalSettings: {
        spacing: 'normal',
        borderRadius: 'medium',
        fontFamily: 'Inter',
        headerStyle: 'simple',
        logoPosition: 'center',
        showRestaurantInfo: true,
        showLogo: true,
        headerAlignment: 'center',
        tagline: ''
      }
    }
  }
})

const previewTheme = computed(() => ({
  primaryColor: globalTheme.value.primaryColor,
  accentColor: globalTheme.value.accentColor,
  surfaceColor: globalTheme.value.surfaceColor,
  textColor: globalTheme.value.textColor,
  fontFamily: globalTheme.value.fontFamily
}))

const previewDisplaySettings = computed(() => ({
  showPrices: true,
  showImages: true,
  showDescriptions: true,
  showCategories: true
}))

const previewCurrency = computed(() => restaurantCurrency.value || 'USD')

const previewBackgroundStyle = computed(() => {
  const style: Record<string, string> = {
    backgroundColor: globalTheme.value.backgroundColor || '#fafaf9',
    minHeight: '100%' // Ensure background covers entire area
  }

  if (globalTheme.value.backgroundType === 'image' && globalTheme.value.backgroundImageUrl) {
    const overlay = globalTheme.value.backgroundOverlay || 'none'
    let overlayGradient = ''
    if (overlay === 'light') {
      overlayGradient = 'linear-gradient(rgba(255,255,255,0.55), rgba(255,255,255,0.55))'
    } else if (overlay === 'dark') {
      overlayGradient = 'linear-gradient(rgba(0,0,0,0.45), rgba(0,0,0,0.45))'
    }

    style.backgroundImage = overlayGradient
      ? `${overlayGradient}, url("${globalTheme.value.backgroundImageUrl}")`
      : `url("${globalTheme.value.backgroundImageUrl}")`
    style.backgroundSize = 'cover'
    style.backgroundPosition = 'center'
    style.backgroundRepeat = 'no-repeat'
    style.backgroundAttachment = 'fixed' // Fixed background for full page effect
  } else if (globalTheme.value.backgroundType === 'gradient' && globalTheme.value.backgroundGradient) {
    style.backgroundImage = globalTheme.value.backgroundGradient
  } else {
    style.backgroundImage = 'none'
  }

  return style
})

// Drag & Drop
const onCategoryDragStart = (event: DragEvent, category: any) => {
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'copy'
    event.dataTransfer.setData('categoryId', category.id)
  }
}

const onDragOver = (event: DragEvent) => {
  event.preventDefault()
  isDragging.value = true
  if (event.dataTransfer) {
    event.dataTransfer.dropEffect = 'copy'
  }
}

const onDrop = (event: DragEvent) => {
  event.preventDefault()
  isDragging.value = false
  
  const categoryId = event.dataTransfer?.getData('categoryId')
  if (!categoryId) return

  // Check if already added
  if (designedCategories.value.some(c => c.categoryId === categoryId)) {
    toast.warning('Category already added')
    return
  }

  // Add to designed categories
  const defaultLayout = globalTheme.value.layout || 'list'
  const newLayout: CategoryLayout = {
    categoryId,
    displayOrder: designedCategories.value.length + 1,
    layout: defaultLayout,
    cardStyle: globalTheme.value.cardStyle || 'modern',
    columns: defaultLayout === 'grid' ? 3 : undefined,
    spacing: globalTheme.value.spacing || 'normal',
    borderRadius: globalTheme.value.borderRadius || 'medium',
    imageSize: globalTheme.value.imageSize || 'medium',
    imageShape: globalTheme.value.imageShape || 'rounded',
    showImages: globalTheme.value.showImages ?? true,
    showPrices: true,
    showDescriptions: true
  }

  designedCategories.value.push(newLayout)
  toast.success('Category added!')
  // Auto-save after adding category
  autoSave()
}

// Category Management
const selectCategory = (catLayout: CategoryLayout) => {
  selectedCategory.value = catLayout
}

const removeCategory = (index: number) => {
  if (confirm('Remove this category from your design?')) {
    designedCategories.value.splice(index, 1)
    if (selectedCategory.value && designedCategories.value.length === 0) {
      selectedCategory.value = null
    }
    // Auto-save after removing category
    autoSave()
  }
}

const moveCategoryUp = (index: number) => {
  if (index > 0) {
    const temp = designedCategories.value[index]
    designedCategories.value[index] = designedCategories.value[index - 1]
    designedCategories.value[index - 1] = temp
    // Auto-save after moving category
    autoSave()
  }
}

const moveCategoryDown = (index: number) => {
  if (index < designedCategories.value.length - 1) {
    const temp = designedCategories.value[index]
    designedCategories.value[index] = designedCategories.value[index + 1]
    designedCategories.value[index + 1] = temp
    // Auto-save after moving category
    autoSave()
  }
}

// Helpers
const findCategoryById = (categories: any[], categoryId: string): any | null => {
  for (const category of categories) {
    if (category.id === categoryId) {
      return category
    }
    if (category.children && category.children.length > 0) {
      const found = findCategoryById(category.children, categoryId)
      if (found) return found
    }
  }
  return null
}

const getCategoryName = (categoryId: string) => {
  const cat = findCategoryById(availableCategories.value, categoryId)
  return cat?.name || 'Category'
}

const getCategoryBreadcrumb = (categoryId: string) => {
  const path: string[] = []
  const traverse = (categories: any[], breadcrumb: string[]): boolean => {
    for (const category of categories) {
      const currentPath = [...breadcrumb, category.name]
      if (category.id === categoryId) {
        path.splice(0, path.length, ...currentPath)
        return true
      }
      if (category.children && category.children.length > 0) {
        if (traverse(category.children, currentPath)) {
          return true
        }
      }
    }
    return false
  }

  traverse(availableCategories.value, [])
  return path.join(' / ') || getCategoryName(categoryId)
}

const countItemsRecursive = (category: any): number => {
  const currentCount = category.items?.length || 0
  const childCount = (category.children || []).reduce((acc: number, child: any) => acc + countItemsRecursive(child), 0)
  return currentCount + childCount
}

const getCategoryItemCount = (categoryId: string) => {
  const cat = findCategoryById(availableCategories.value, categoryId)
  if (!cat) return 0
  return countItemsRecursive(cat)
}

const getPreviewClass = (catLayout: CategoryLayout) => {
  if (catLayout.layout === 'grid') {
    return `grid grid-cols-${catLayout.columns || 3} gap-4`
  }
  return 'space-y-3'
}

const getItemPreviewClass = (catLayout: CategoryLayout) => {
  const classes = ['flex items-start gap-3 p-3 rounded-lg']
  
  if (catLayout.cardStyle === 'modern') {
    classes.push('bg-white shadow-md border border-neutral-200')
  } else if (catLayout.cardStyle === 'classic') {
    classes.push('bg-white border-2 border-neutral-300')
  } else {
    classes.push('bg-neutral-100')
  }

  return classes.join(' ')
}

const getImageClass = (catLayout: CategoryLayout) => {
  const classes = []
  
  // Size
  if (catLayout.imageSize === 'small') {
    classes.push('w-12 h-12')
  } else if (catLayout.imageSize === 'large') {
    classes.push('w-24 h-24')
  } else {
    classes.push('w-16 h-16')
  }

  // Shape
  if (catLayout.imageShape === 'circle') {
    classes.push('rounded-full')
  } else if (catLayout.imageShape === 'rounded') {
    classes.push('rounded-lg')
  }

  return classes.join(' ')
}

const handlePreviewCategoryClick = (category: any) => {
  const matchedLayout = designedCategories.value.find((layout) => layout.categoryId === category.id)
  if (matchedLayout) {
    selectedCategory.value = matchedLayout
  }
}

// Actions
const handleLogoUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (!file) return
  
  // Validate file type
  if (!file.type.startsWith('image/')) {
    toast.error('Please upload an image file')
    if (target) target.value = ''
    return
  }
  
  // Validate file size (5MB limit)
  if (file.size > 5 * 1024 * 1024) {
    toast.error('Image size must be less than 5MB')
    if (target) target.value = ''
    return
  }
  
  uploadingLogo.value = true
  
  try {
    const formData = new FormData()
    formData.append('file', file)
    
    const api = useApi()
    const response = await api.post('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    
    if (response.data.success) {
      const fileUrl = typeof response.data.data === 'string' ? response.data.data : response.data.data?.url
      if (!fileUrl) {
        throw new Error('File URL missing from upload response')
      }
      restaurantInfo.value.logoUrl = fileUrl
      toast.success('Logo uploaded successfully!', 'Success', 3000)
    } else {
      toast.error(response.data.message || 'Failed to upload logo')
    }
  } catch (error: any) {
    console.error('Logo upload error:', error)
    const errorMsg = error?.response?.data?.message || error?.message || 'An error occurred during upload'
    toast.error(errorMsg)
  } finally {
    uploadingLogo.value = false
    if (target) target.value = '' // Reset input
  }
}

const handleBackgroundUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]

  if (!file) return

  if (!file.type.startsWith('image/')) {
    toast.error('Please upload an image file')
    if (target) target.value = ''
    return
  }

  if (file.size > 5 * 1024 * 1024) {
    toast.error('Image size must be less than 5MB')
    if (target) target.value = ''
    return
  }

  uploadingBackground.value = true

  try {
    const formData = new FormData()
    formData.append('file', file)

    const response = await api.post('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })

    if (response.data.success) {
      const fileUrl = typeof response.data.data === 'string' ? response.data.data : response.data.data?.url
      if (!fileUrl) {
        throw new Error('File URL missing from upload response')
      }
      globalTheme.value.backgroundType = 'image'
      globalTheme.value.backgroundImageUrl = fileUrl
      toast.success('Background image uploaded!', 'Success', 3000)
    } else {
      toast.error(response.data.message || 'Failed to upload background')
    }
  } catch (error: any) {
    console.error('Background upload error:', error)
    const errorMsg = error?.response?.data?.message || error?.message || 'An error occurred while uploading background'
    toast.error(errorMsg)
  } finally {
    uploadingBackground.value = false
    if (target) target.value = ''
  }
}

const applyDefaultsToCategories = () => {
  if (!designedCategories.value.length) {
    toast.info('Add categories to apply defaults')
    return
  }

  designedCategories.value = designedCategories.value.map((cat) => ({
    ...cat,
    layout: globalTheme.value.layout || cat.layout,
    cardStyle: globalTheme.value.cardStyle || cat.cardStyle,
    spacing: globalTheme.value.spacing || cat.spacing,
    borderRadius: globalTheme.value.borderRadius || cat.borderRadius,
    imageSize: globalTheme.value.imageSize || cat.imageSize,
    imageShape: globalTheme.value.imageShape || cat.imageShape,
    showImages: globalTheme.value.showImages ?? cat.showImages
  }))

  toast.success('Applied global defaults to all categories', 'Defaults Updated', 3000)
}

// Auto-save debounce timer
let autoSaveTimer: ReturnType<typeof setTimeout> | null = null
const autoSaveDelay = 2000 // 2 seconds after last change

const saveDesign = async (silent = false) => {
  if (!authStore.restaurantId) {
    if (!silent) toast.error('No restaurant ID found')
    return
  }

  if (designedCategories.value.length === 0) {
    if (!silent) toast.error('Please add at least one category to your design')
    return
  }

  saving.value = true
  try {
    // Update display orders
    designedCategories.value.forEach((cat, index) => {
      cat.displayOrder = index + 1
    })

    const result = await saveDesignApi({
      restaurantId: authStore.restaurantId,
      layoutConfiguration: {
        categories: designedCategories.value,
        globalSettings: globalSettings.value
      },
      globalTheme: globalTheme.value,
      setAsActive: true
    })

    if (result) {
      if (!silent) {
        toast.success('Design saved and published! 🎉', 'Success', 5000)
      }
    } else {
      if (!silent) toast.error('Failed to save design')
    }
  } catch (error) {
    console.error('Failed to save design:', error)
    if (!silent) toast.error('An error occurred while saving')
  } finally {
    saving.value = false
  }
}

// Auto-save function with debounce
const autoSave = () => {
  // Clear existing timer
  if (autoSaveTimer) {
    clearTimeout(autoSaveTimer)
  }
  
  // Set new timer
  autoSaveTimer = setTimeout(() => {
    saveDesign(true) // Silent save
  }, autoSaveDelay)
}

const previewMenu = () => {
  // TODO: Fetch restaurant slug from API if needed
  toast.info('Preview feature - Open your public menu URL')
}

// Load Data
onMounted(async () => {
  if (!authStore.restaurantId) {
    toast.error('No restaurant ID found')
    router.push('/dashboard')
    return
  }

  try {
    // Load restaurant info
    const restaurantResponse = await api.get(`/restaurants/${authStore.restaurantId}`)
    const restaurantData = restaurantResponse.data || {}
    restaurantInfo.value = {
      name: restaurantData.name || '',
      logoUrl: restaurantData.logoUrl || '',
      address: restaurantData.address || ''
    }
    restaurantCurrency.value = restaurantData.currency || 'USD'
    if (restaurantStore) {
      // Keep store in sync for other pages
      restaurantStore.currentRestaurant = restaurantData
    }

    // Load categories
    await restaurantStore.fetchCategories(authStore.restaurantId)
    availableCategories.value = restaurantStore.categories

    // Load existing design
    const existingDesign = await getActiveDesign(authStore.restaurantId)
    if (existingDesign) {
      try {
        designedCategories.value = cloneDeep(existingDesign.layoutConfiguration?.categories || [])
        globalSettings.value = {
          ...globalSettings.value,
          ...(existingDesign.layoutConfiguration?.globalSettings || {})
        }
        globalTheme.value = {
          ...createDefaultTheme(),
          ...(existingDesign.globalTheme || {})
        }
        if (globalTheme.value.backgroundImageUrl) {
          globalTheme.value.backgroundType = globalTheme.value.backgroundType || 'image'
        }
        toast.info('Loaded existing design')
      } catch (error) {
        console.error('Failed to load existing design data:', error)
        toast.error('Failed to load some design settings')
      }
    }
  } catch (error) {
    console.error('Failed to load designer data:', error)
    toast.error('Failed to load menu data')
  } finally {
    initialLoading.value = false
    // Allow auto-save after initial load is complete
    setTimeout(() => {
      isInitialLoad.value = false
    }, 1000) // Small delay to ensure all data is loaded
  }
})

// Watch for changes to auto-save
// Watch designedCategories (deep watch to catch nested property changes)
watch(
  designedCategories,
  () => {
    if (!initialLoading.value && !isInitialLoad.value && designedCategories.value.length > 0) {
      autoSave()
    }
  },
  { deep: true }
)

// Watch selectedCategory changes (when user modifies category settings)
watch(
  selectedCategory,
  () => {
    if (!initialLoading.value && !isInitialLoad.value && selectedCategory.value) {
      autoSave()
    }
  },
  { deep: true }
)

// Watch globalTheme changes
watch(
  globalTheme,
  () => {
    if (!initialLoading.value && !isInitialLoad.value && designedCategories.value.length > 0) {
      autoSave()
    }
  },
  { deep: true }
)

// Watch globalSettings changes
watch(
  globalSettings,
  () => {
    if (!initialLoading.value && !isInitialLoad.value && designedCategories.value.length > 0) {
      autoSave()
    }
  },
  { deep: true }
)
</script>
