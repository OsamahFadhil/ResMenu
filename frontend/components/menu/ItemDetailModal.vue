<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="show"
        class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-60 backdrop-blur-md"
        @click.self="$emit('close')"
      >
        <div
          class="relative w-full max-w-4xl bg-white rounded-3xl shadow-2xl overflow-hidden transform transition-all max-h-[90vh] flex flex-col"
          :style="{ fontFamily: settings?.fontFamily || 'Inter' }"
        >
          <!-- Close Button -->
          <button
            @click="$emit('close')"
            class="absolute top-6 right-6 z-20 p-3 rounded-full bg-white/90 backdrop-blur-sm shadow-xl hover:bg-white transition-all hover:scale-110"
            aria-label="Close"
          >
            <svg class="w-6 h-6 text-neutral-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>

          <!-- Image Gallery Section -->
          <div
            v-if="itemImages.length > 0"
            class="relative bg-neutral-900 overflow-hidden"
            :class="itemImages.length > 1 ? 'h-96' : 'h-80'"
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
                class="absolute left-4 top-1/2 -translate-y-1/2 p-3 rounded-full bg-white/90 backdrop-blur-sm shadow-xl hover:bg-white transition-all hover:scale-110"
                aria-label="Previous image"
              >
                <svg class="w-6 h-6 text-neutral-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
                </svg>
              </button>
              <button
                @click="nextImage"
                class="absolute right-4 top-1/2 -translate-y-1/2 p-3 rounded-full bg-white/90 backdrop-blur-sm shadow-xl hover:bg-white transition-all hover:scale-110"
                aria-label="Next image"
              >
                <svg class="w-6 h-6 text-neutral-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
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
              class="absolute top-4 left-4 px-3 py-1.5 rounded-full bg-black/60 backdrop-blur-sm text-white text-sm font-medium"
            >
              {{ currentImageIndex + 1 }} / {{ itemImages.length }}
            </div>
          </div>

          <!-- Content Section -->
          <div class="flex-1 overflow-y-auto">
            <div class="p-8 space-y-6">
              <!-- Header -->
              <div class="space-y-3">
                <div class="flex items-start justify-between gap-4">
                  <h2
                    class="text-4xl font-bold leading-tight"
                    :style="{ color: settings?.textColor || '#1f2937' }"
                  >
                    {{ item.localizedName || item.name }}
                  </h2>
                  <span
                    v-if="displayPrice"
                    class="text-3xl font-bold whitespace-nowrap"
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
                  class="text-lg leading-relaxed"
                  :style="{ color: settings?.textColor || '#374151' }"
                >
                  {{ item.localizedDescription || item.description }}
                </p>
              </div>

              <!-- Additional Info Grid -->
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 pt-6 border-t border-neutral-200">
                <!-- Price Info (if showing price) -->
                <div v-if="displayPrice" class="flex items-start gap-4 p-4 bg-neutral-50 rounded-2xl">
                  <div
                    class="p-3 rounded-xl"
                    :style="{ backgroundColor: `${settings?.accentColor || '#f59e0b'}20` }"
                  >
                    <svg
                      class="w-6 h-6"
                      :style="{ color: settings?.accentColor || '#f59e0b' }"
                      fill="currentColor"
                      viewBox="0 0 20 20"
                    >
                      <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
                    </svg>
                  </div>
                  <div>
                    <div class="text-sm font-medium text-neutral-500 uppercase tracking-wide mb-1">Price</div>
                    <div
                      class="text-2xl font-bold"
                      :style="{ color: settings?.primaryColor || '#dc2626' }"
                    >
                      {{ formatPrice(item.price) }}
                    </div>
                  </div>
                </div>

                <!-- Status Info -->
                <div class="flex items-start gap-4 p-4 bg-neutral-50 rounded-2xl">
                  <div
                    class="p-3 rounded-xl"
                    :style="{
                      backgroundColor: item.isAvailable ? '#10b98120' : '#ef444420'
                    }"
                  >
                    <svg
                      class="w-6 h-6"
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
                  <div>
                    <div class="text-sm font-medium text-neutral-500 uppercase tracking-wide mb-1">Status</div>
                    <div
                      class="text-lg font-semibold"
                      :class="item.isAvailable ? 'text-green-700' : 'text-red-700'"
                    >
                      {{ item.isAvailable ? 'Available' : 'Unavailable' }}
                    </div>
                  </div>
                </div>
              </div>

              <!-- Thumbnail Gallery (if multiple images) -->
              <div v-if="itemImages.length > 1" class="pt-6 border-t border-neutral-200">
                <h3 class="text-sm font-semibold text-neutral-700 uppercase tracking-wide mb-3">Gallery</h3>
                <div class="grid grid-cols-4 sm:grid-cols-6 gap-3">
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
          <div class="p-6 bg-neutral-50 border-t border-neutral-200">
            <button
              @click="$emit('close')"
              class="w-full px-8 py-4 rounded-xl font-semibold text-lg transition-all hover:scale-[1.02] hover:shadow-lg"
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
import { ref, computed } from 'vue'
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
}>()

defineEmits<{
  (e: 'close'): void
}>()

const { locale } = useI18n()
const currentImageIndex = ref(0)

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

  if (e.key === 'ArrowRight') {
    nextImage()
  } else if (e.key === 'ArrowLeft') {
    previousImage()
  } else if (e.key === 'Escape') {
    // Close modal handled by parent
  }
}

// Add keyboard listener
if (typeof window !== 'undefined') {
  window.addEventListener('keydown', handleKeydown)
}

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
  transform: scale(0.95);
  opacity: 0;
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
