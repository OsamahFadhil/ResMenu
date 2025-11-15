<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="show"
        class="fixed inset-0 z-50 flex items-center justify-center p-2 sm:p-4 bg-black bg-opacity-60 backdrop-blur-md"
        @click.self="$emit('close')"
      >
        <div
          class="relative w-full bg-white shadow-2xl overflow-hidden transform transition-all max-h-[95vh] sm:max-h-[90vh] flex flex-col"
          :class="[
            getMaxWidthClass(props.modalSettings?.maxWidth || '5xl'),
            getBorderRadiusClass(props.modalSettings?.borderRadius || 'large')
          ]"
          :style="{ fontFamily: settings?.fontFamily || 'Inter' }"
        >
          <!-- Close Button -->
          <button
            v-if="props.modalSettings?.closeButtonStyle !== 'hidden'"
            @click="$emit('close')"
            :class="[
              'absolute z-20 transition-all',
              props.modalSettings?.closeButtonStyle === 'minimal'
                ? 'top-2 right-2 p-1.5 rounded-lg bg-white/50 hover:bg-white/80'
                : 'top-3 right-3 sm:top-6 sm:right-6 p-2 sm:p-3 rounded-full bg-white/90 backdrop-blur-sm shadow-xl hover:bg-white hover:scale-110'
            ]"
            aria-label="Close"
          >
            <svg class="w-5 h-5 sm:w-6 sm:h-6 text-neutral-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>

          <!-- Image Gallery Section -->
          <div
            v-if="itemImages.length > 0"
            class="relative bg-neutral-900 overflow-hidden"
            :class="getImageHeightClass()"
          >
            <!-- Main Image -->
            <div class="relative h-full">
              <img
                :src="itemImages[currentImageIndex]"
                :alt="item.localizedName || item.name"
                class="w-full h-full object-cover transition-opacity duration-500"
                :key="currentImageIndex"
              />
              <div class="absolute inset-0 bg-gradient-to-t from-black/50 via-transparent to-black/20"></div>
            </div>

            <!-- Navigation Arrows (if multiple images) -->
            <template v-if="itemImages.length > 1">
              <button
                @click="previousImage"
                class="absolute left-2 sm:left-4 top-1/2 -translate-y-1/2 p-2 sm:p-3 rounded-full bg-white/90 backdrop-blur-sm shadow-xl hover:bg-white transition-all hover:scale-110"
                aria-label="Previous image"
              >
                <svg class="w-5 h-5 sm:w-6 sm:h-6 text-neutral-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
                </svg>
              </button>
              <button
                @click="nextImage"
                class="absolute right-2 sm:right-4 top-1/2 -translate-y-1/2 p-2 sm:p-3 rounded-full bg-white/90 backdrop-blur-sm shadow-xl hover:bg-white transition-all hover:scale-110"
                aria-label="Next image"
              >
                <svg class="w-5 h-5 sm:w-6 sm:h-6 text-neutral-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </button>

              <!-- Image Indicators -->
              <div class="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2">
                <button
                  v-for="(_, index) in itemImages"
                  :key="index"
                  @click="currentImageIndex = index"
                  :class="[
                    'w-2 h-2 rounded-full transition-all',
                    currentImageIndex === index
                      ? 'bg-white w-8'
                      : 'bg-white/50 hover:bg-white/75'
                  ]"
                  :aria-label="`Go to image ${index + 1}`"
                ></button>
              </div>
            </template>

            <!-- Image Counter -->
            <div
              v-if="itemImages.length > 1"
              class="absolute top-2 left-2 sm:top-4 sm:left-4 px-2 py-1 sm:px-3 sm:py-1.5 rounded-full bg-black/60 backdrop-blur-sm text-white text-xs sm:text-sm font-medium"
            >
              {{ currentImageIndex + 1 }} / {{ itemImages.length }}
            </div>
          </div>

          <!-- Content Section -->
          <div class="flex-1 overflow-y-auto">
            <div class="p-4 sm:p-6 md:p-8 space-y-4 sm:space-y-6">
              <!-- Header -->
              <div class="space-y-3 sm:space-y-4">
                <div class="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-3 sm:gap-4">
                  <h2
                    class="text-2xl sm:text-3xl md:text-4xl font-serif font-bold leading-tight"
                    :style="{ 
                      color: settings?.textColor || '#1f2937',
                      fontFamily: settings?.fontFamily || 'Playfair Display, serif'
                    }"
                  >
                    {{ item.localizedName || item.name }}
                  </h2>
                  <span
                    v-if="displayPrice"
                    class="text-2xl sm:text-3xl font-bold whitespace-nowrap"
                    :style="{ color: settings?.primaryColor || '#dc2626' }"
                  >
                    {{ formatPrice(item.price) }}
                  </span>
                </div>

                <!-- Category and Availability Badges -->
                <div class="flex flex-wrap items-center gap-3">
                  <!-- Category Badge -->
                  <div
                    v-if="categoryName"
                    class="inline-flex items-center gap-2 px-4 py-2 rounded-full text-sm font-semibold"
                    :style="{
                      backgroundColor: `${settings?.primaryColor || '#dc2626'}15`,
                      color: settings?.primaryColor || '#dc2626'
                    }"
                  >
                    <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M7 3a1 1 0 000 2h6a1 1 0 100-2H7zM4 7a1 1 0 011-1h10a1 1 0 110 2H5a1 1 0 01-1-1zM2 11a2 2 0 012-2h12a2 2 0 012 2v4a2 2 0 01-2 2H4a2 2 0 01-2-2v-4z" />
                    </svg>
                    {{ categoryName }}
                  </div>

                  <!-- Availability Badge -->
                  <div
                    :class="[
                      'inline-flex items-center gap-2 px-4 py-2 rounded-full text-sm font-semibold',
                      item.isAvailable ? 'bg-green-50 text-green-700' : 'bg-red-50 text-red-700'
                    ]"
                  >
                    <svg
                      class="w-4 h-4"
                      fill="currentColor"
                      viewBox="0 0 20 20"
                    >
                      <path
                        v-if="item.isAvailable"
                        fill-rule="evenodd"
                        d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                        clip-rule="evenodd"
                      />
                      <path
                        v-else
                        fill-rule="evenodd"
                        d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                        clip-rule="evenodd"
                      />
                    </svg>
                    {{ item.isAvailable ? 'Available Now' : 'Currently Unavailable' }}
                  </div>
                </div>
              </div>

              <!-- Description -->
              <div
                v-if="item.localizedDescription || item.description"
                class="prose prose-lg max-w-none"
              >
                <p
                  class="text-base sm:text-lg leading-relaxed"
                  :style="{ color: settings?.textColor || '#374151' }"
                >
                  {{ item.localizedDescription || item.description }}
                </p>
              </div>

              <!-- Settings & Information Section -->
              <div v-if="shouldShowInfoSection()" class="pt-4 sm:pt-6 border-t-2 border-neutral-300 space-y-4">
                <div class="flex items-center gap-3 mb-2">
                  <svg class="w-6 h-6" :style="{ color: settings?.primaryColor || '#dc2626' }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                  <h3 class="text-lg sm:text-xl font-serif font-bold" :style="{ color: settings?.textColor || '#1f2937' }">
                    Item Information & Settings
                  </h3>
                </div>
                
                <!-- Expandable Settings Sections -->
                <div class="space-y-3">
                  <!-- Dietary Information -->
                  <div v-if="props.modalSettings?.showDietaryInfo !== false" class="border border-neutral-200 rounded-xl overflow-hidden">
                    <button
                      @click="expandedSections.dietary = !expandedSections.dietary"
                      class="w-full px-4 py-3 sm:px-6 sm:py-4 flex items-center justify-between bg-neutral-50 hover:bg-neutral-100 transition-colors"
                    >
                      <div class="flex items-center gap-3">
                        <div class="p-2 rounded-lg" :style="{ backgroundColor: `${settings?.primaryColor || '#dc2626'}15` }">
                          <svg class="w-5 h-5" :style="{ color: settings?.primaryColor || '#dc2626' }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
                          </svg>
                        </div>
                        <span class="font-semibold text-sm sm:text-base" :style="{ color: settings?.textColor || '#1f2937' }">
                          Dietary Information
                        </span>
                      </div>
                      <svg 
                        class="w-5 h-5 transition-transform" 
                        :class="{ 'rotate-180': expandedSections.dietary }"
                        :style="{ color: settings?.textColor || '#1f2937' }"
                        fill="none" 
                        stroke="currentColor" 
                        viewBox="0 0 24 24"
                      >
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                      </svg>
                    </button>
                    <div v-if="expandedSections.dietary" class="px-4 sm:px-6 py-4 bg-white">
                      <div class="flex flex-wrap gap-2">
                        <span class="px-3 py-1.5 rounded-full text-xs sm:text-sm font-medium bg-green-50 text-green-700 border border-green-200">
                          Vegetarian
                        </span>
                        <span class="px-3 py-1.5 rounded-full text-xs sm:text-sm font-medium bg-green-50 text-green-700 border border-green-200">
                          Gluten-Free
                        </span>
                        <span class="px-3 py-1.5 rounded-full text-xs sm:text-sm font-medium bg-blue-50 text-blue-700 border border-blue-200">
                          Dairy-Free
                        </span>
                      </div>
                      <p class="mt-3 text-sm text-neutral-600">
                        This item can be customized to meet your dietary needs. Please inform your server of any allergies.
                      </p>
                    </div>
                  </div>

                  <!-- Allergen Information -->
                  <div v-if="props.modalSettings?.showAllergenInfo !== false" class="border border-neutral-200 rounded-xl overflow-hidden">
                    <button
                      @click="expandedSections.allergens = !expandedSections.allergens"
                      class="w-full px-4 py-3 sm:px-6 sm:py-4 flex items-center justify-between bg-neutral-50 hover:bg-neutral-100 transition-colors"
                    >
                      <div class="flex items-center gap-3">
                        <div class="p-2 rounded-lg" :style="{ backgroundColor: `${settings?.primaryColor || '#dc2626'}15` }">
                          <svg class="w-5 h-5" :style="{ color: settings?.primaryColor || '#dc2626' }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                          </svg>
                        </div>
                        <span class="font-semibold text-sm sm:text-base" :style="{ color: settings?.textColor || '#1f2937' }">
                          Allergen Information
                        </span>
                      </div>
                      <svg 
                        class="w-5 h-5 transition-transform" 
                        :class="{ 'rotate-180': expandedSections.allergens }"
                        :style="{ color: settings?.textColor || '#1f2937' }"
                        fill="none" 
                        stroke="currentColor" 
                        viewBox="0 0 24 24"
                      >
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                      </svg>
                    </button>
                    <div v-if="expandedSections.allergens" class="px-4 sm:px-6 py-4 bg-white">
                      <div class="space-y-2">
                        <p class="text-sm font-medium text-neutral-700 mb-3">Contains:</p>
                        <div class="flex flex-wrap gap-2">
                          <span class="px-3 py-1.5 rounded-full text-xs sm:text-sm font-medium bg-red-50 text-red-700 border border-red-200">
                            Contains Gluten
                          </span>
                          <span class="px-3 py-1.5 rounded-full text-xs sm:text-sm font-medium bg-red-50 text-red-700 border border-red-200">
                            Contains Dairy
                          </span>
                        </div>
                        <p class="mt-3 text-xs sm:text-sm text-neutral-600">
                          Please inform your server if you have any food allergies or dietary restrictions.
                        </p>
                      </div>
                    </div>
                  </div>

                  <!-- Spice Level & Details -->
                  <div v-if="props.modalSettings?.showAdditionalDetails !== false" class="border border-neutral-200 rounded-xl overflow-hidden">
                    <button
                      @click="expandedSections.details = !expandedSections.details"
                      class="w-full px-4 py-3 sm:px-6 sm:py-4 flex items-center justify-between bg-neutral-50 hover:bg-neutral-100 transition-colors"
                    >
                      <div class="flex items-center gap-3">
                        <div class="p-2 rounded-lg" :style="{ backgroundColor: `${settings?.primaryColor || '#dc2626'}15` }">
                          <svg class="w-5 h-5" :style="{ color: settings?.primaryColor || '#dc2626' }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                          </svg>
                        </div>
                        <span class="font-semibold text-sm sm:text-base" :style="{ color: settings?.textColor || '#1f2937' }">
                          Additional Details
                        </span>
                      </div>
                      <svg 
                        class="w-5 h-5 transition-transform" 
                        :class="{ 'rotate-180': expandedSections.details }"
                        :style="{ color: settings?.textColor || '#1f2937' }"
                        fill="none" 
                        stroke="currentColor" 
                        viewBox="0 0 24 24"
                      >
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                      </svg>
                    </button>
                    <div v-if="expandedSections.details" class="px-4 sm:px-6 py-4 bg-white">
                      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <!-- Spice Level -->
                        <div>
                          <p class="text-xs sm:text-sm font-medium text-neutral-500 uppercase tracking-wide mb-2">Spice Level</p>
                          <div class="flex items-center gap-2">
                            <div class="flex gap-1">
                              <span class="text-lg">🌶️</span>
                              <span class="text-lg opacity-50">🌶️</span>
                              <span class="text-lg opacity-50">🌶️</span>
                            </div>
                            <span class="text-sm font-medium text-neutral-700">Mild</span>
                          </div>
                        </div>
                        <!-- Preparation Time -->
                        <div>
                          <p class="text-xs sm:text-sm font-medium text-neutral-500 uppercase tracking-wide mb-2">Prep Time</p>
                          <div class="flex items-center gap-2">
                            <svg class="w-5 h-5 text-neutral-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <span class="text-sm font-medium text-neutral-700">15-20 minutes</span>
                          </div>
                        </div>
                        <!-- Serving Size -->
                        <div>
                          <p class="text-xs sm:text-sm font-medium text-neutral-500 uppercase tracking-wide mb-2">Serving Size</p>
                          <div class="flex items-center gap-2">
                            <svg class="w-5 h-5 text-neutral-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                            </svg>
                            <span class="text-sm font-medium text-neutral-700">1 serving</span>
                          </div>
                        </div>
                        <!-- Calories (if available) -->
                        <div>
                          <p class="text-xs sm:text-sm font-medium text-neutral-500 uppercase tracking-wide mb-2">Calories</p>
                          <div class="flex items-center gap-2">
                            <svg class="w-5 h-5 text-neutral-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                            </svg>
                            <span class="text-sm font-medium text-neutral-700">~450 kcal</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Additional Info Grid -->
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4 pt-4 sm:pt-6 border-t border-neutral-200">
                <!-- Price Info (if showing price) -->
                <div v-if="displayPrice" class="flex items-start gap-3 sm:gap-4 p-3 sm:p-4 bg-neutral-50 rounded-xl sm:rounded-2xl">
                  <div
                    class="p-2 sm:p-3 rounded-lg sm:rounded-xl flex-shrink-0"
                    :style="{ backgroundColor: `${settings?.accentColor || '#f59e0b'}20` }"
                  >
                    <svg
                      class="w-5 h-5 sm:w-6 sm:h-6"
                      :style="{ color: settings?.accentColor || '#f59e0b' }"
                      fill="currentColor"
                      viewBox="0 0 20 20"
                    >
                      <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
                    </svg>
                  </div>
                  <div class="min-w-0">
                    <div class="text-xs sm:text-sm font-medium text-neutral-500 uppercase tracking-wide mb-1">Price</div>
                    <div
                      class="text-xl sm:text-2xl font-bold"
                      :style="{ color: settings?.primaryColor || '#dc2626' }"
                    >
                      {{ formatPrice(item.price) }}
                    </div>
                  </div>
                </div>

                <!-- Status Info -->
                <div class="flex items-start gap-3 sm:gap-4 p-3 sm:p-4 bg-neutral-50 rounded-xl sm:rounded-2xl">
                  <div
                    class="p-2 sm:p-3 rounded-lg sm:rounded-xl flex-shrink-0"
                    :style="{
                      backgroundColor: item.isAvailable ? '#10b98120' : '#ef444420'
                    }"
                  >
                    <svg
                      class="w-5 h-5 sm:w-6 sm:h-6"
                      :class="item.isAvailable ? 'text-green-600' : 'text-red-500'"
                      fill="currentColor"
                      viewBox="0 0 20 20"
                    >
                      <path
                        v-if="item.isAvailable"
                        fill-rule="evenodd"
                        d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                        clip-rule="evenodd"
                      />
                      <path
                        v-else
                        fill-rule="evenodd"
                        d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                        clip-rule="evenodd"
                      />
                    </svg>
                  </div>
                  <div class="min-w-0">
                    <div class="text-xs sm:text-sm font-medium text-neutral-500 uppercase tracking-wide mb-1">Status</div>
                    <div
                      class="text-base sm:text-lg font-semibold"
                      :class="item.isAvailable ? 'text-green-700' : 'text-red-700'"
                    >
                      {{ item.isAvailable ? 'Available' : 'Unavailable' }}
                    </div>
                  </div>
                </div>
              </div>

              <!-- Thumbnail Gallery (if multiple images) -->
              <div v-if="itemImages.length > 1" class="pt-4 sm:pt-6 border-t border-neutral-200">
                <h3 class="text-sm sm:text-base font-semibold text-neutral-700 uppercase tracking-wide mb-3">Gallery</h3>
                <div class="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-6 gap-2 sm:gap-3">
                  <button
                    v-for="(image, index) in itemImages"
                    :key="index"
                    @click="currentImageIndex = index"
                    :class="[
                      'relative aspect-square rounded-xl overflow-hidden transition-all',
                      currentImageIndex === index
                        ? 'ring-4 ring-offset-2 scale-105'
                        : 'hover:scale-105 opacity-70 hover:opacity-100'
                    ]"
                    :style="{
                      ringColor: currentImageIndex === index ? settings?.primaryColor || '#dc2626' : 'transparent'
                    }"
                  >
                    <img
                      :src="image"
                      :alt="`${item.localizedName || item.name} - Image ${index + 1}`"
                      class="w-full h-full object-cover"
                    />
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Footer Actions -->
          <div class="p-4 sm:p-6 bg-neutral-50 border-t border-neutral-200">
            <button
              @click="$emit('close')"
              class="w-full px-6 sm:px-8 py-3 sm:py-4 rounded-xl font-semibold text-base sm:text-lg transition-all hover:scale-[1.02] hover:shadow-lg"
              :style="{
                backgroundColor: settings?.primaryColor || '#dc2626',
                color: '#ffffff'
              }"
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount, watch } from 'vue'
import type { MenuItem } from '~/stores/restaurant'

const props = defineProps<{
  show: boolean
  item: MenuItem
  categoryName?: string
  settings?: {
    primaryColor?: string
    accentColor?: string
    textColor?: string
    fontFamily?: string
  }
  displayPrice?: boolean
  currency?: string
  modalSettings?: {
    maxWidth?: 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '4xl' | '5xl' | '6xl' | 'full'
    borderRadius?: 'none' | 'small' | 'medium' | 'large'
    showDietaryInfo?: boolean
    showAllergenInfo?: boolean
    showAdditionalDetails?: boolean
    imageHeight?: 'small' | 'medium' | 'large'
    closeButtonStyle?: 'default' | 'minimal' | 'hidden'
  }
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()

const { locale } = useI18n()
const currentImageIndex = ref(0)

// Expanded sections state - expanded by default for better visibility
const expandedSections = ref({
  dietary: true,
  allergens: true,
  details: true
})

// Combine imageUrl and images array, with fallback
const itemImages = computed(() => {
  const images: string[] = []

  // Add images array first (if exists)
  if (props.item.images && Array.isArray(props.item.images) && props.item.images.length > 0) {
    images.push(...props.item.images)
  }

  // Add imageUrl if exists and not already in images array
  if (props.item.imageUrl && !images.includes(props.item.imageUrl)) {
    images.push(props.item.imageUrl)
  }

  return images
})

const nextImage = () => {
  currentImageIndex.value = (currentImageIndex.value + 1) % itemImages.value.length
}

const previousImage = () => {
  currentImageIndex.value = currentImageIndex.value === 0
    ? itemImages.value.length - 1
    : currentImageIndex.value - 1
}

// Keyboard navigation
const handleKeydown = (e: KeyboardEvent) => {
  if (!props.show) return

  if (e.key === 'ArrowRight' && itemImages.value.length > 1) {
    e.preventDefault()
    nextImage()
  } else if (e.key === 'ArrowLeft' && itemImages.value.length > 1) {
    e.preventDefault()
    previousImage()
  } else if (e.key === 'Escape') {
    e.preventDefault()
    emit('close')
  }
}

// Add/remove keyboard listener
watch(() => props.show, (isShowing) => {
  if (typeof window === 'undefined') return
  
  if (isShowing) {
    window.addEventListener('keydown', handleKeydown)
    document.body.style.overflow = 'hidden'
  } else {
    window.removeEventListener('keydown', handleKeydown)
    document.body.style.overflow = ''
  }
}, { immediate: true })

onBeforeUnmount(() => {
  if (typeof window !== 'undefined') {
    window.removeEventListener('keydown', handleKeydown)
    document.body.style.overflow = ''
  }
})

const formatPrice = (price: number) => {
  try {
    return new Intl.NumberFormat(locale.value, {
      style: 'currency',
      currency: props.currency || 'USD',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(price)
  } catch (error) {
    return `${props.currency || 'USD'} ${price.toFixed(2)}`
  }
}

// Helper functions for modal settings
const getMaxWidthClass = (width: string) => {
  switch (width) {
    case 'sm': return 'max-w-sm'
    case 'md': return 'max-w-md'
    case 'lg': return 'max-w-lg'
    case 'xl': return 'max-w-xl'
    case '2xl': return 'max-w-2xl'
    case '4xl': return 'max-w-4xl'
    case '5xl': return 'max-w-5xl'
    case '6xl': return 'max-w-6xl'
    case 'full': return 'max-w-full'
    default: return 'max-w-5xl'
  }
}

const getBorderRadiusClass = (radius: string) => {
  switch (radius) {
    case 'none': return 'rounded-none'
    case 'small': return 'rounded-lg sm:rounded-xl'
    case 'medium': return 'rounded-xl sm:rounded-2xl'
    case 'large': return 'rounded-2xl sm:rounded-3xl'
    default: return 'rounded-2xl sm:rounded-3xl'
  }
}

const getImageHeightClass = () => {
  const height = props.modalSettings?.imageHeight || 'medium'
  const hasMultiple = itemImages.value.length > 1
  
  if (height === 'small') {
    return hasMultiple ? 'h-32 sm:h-48 md:h-64' : 'h-32 sm:h-40 md:h-48'
  } else if (height === 'large') {
    return hasMultiple ? 'h-64 sm:h-96 md:h-[28rem]' : 'h-64 sm:h-80 md:h-96'
  } else {
    // medium (default)
    return hasMultiple ? 'h-48 sm:h-80 md:h-96' : 'h-48 sm:h-72 md:h-80'
  }
}

const shouldShowInfoSection = () => {
  return (
    props.modalSettings?.showDietaryInfo !== false ||
    props.modalSettings?.showAllergenInfo !== false ||
    props.modalSettings?.showAdditionalDetails !== false
  )
}
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-active > div,
.modal-leave-active > div {
  transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1), opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from > div,
.modal-leave-to > div {
  transform: scale(0.9) translateY(20px);
  opacity: 0;
}

/* Responsive improvements */
@media (max-width: 640px) {
  .modal-enter-from > div,
  .modal-leave-to > div {
    transform: scale(0.95) translateY(10px);
  }
}

.prose p {
  margin: 0;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb:hover {
  background: #555;
}
</style>

