<template>
  <div class="relative inline-block text-left">
    <button
      @click="toggleDropdown"
      class="inline-flex items-center justify-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
    >
      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5h12M9 3v2m1.048 9.5A18.022 18.022 0 016.412 9m6.088 9h7M11 21l5-10 5 10M12.751 5C11.783 10.77 8.07 15.61 3 18.129" />
      </svg>
      <span>{{ currentLocaleName }}</span>
      <svg class="w-4 h-4" :class="{ 'rotate-180': isOpen }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
      </svg>
    </button>

    <transition
      enter-active-class="transition duration-100 ease-out"
      enter-from-class="transform scale-95 opacity-0"
      enter-to-class="transform scale-100 opacity-100"
      leave-active-class="transition duration-75 ease-in"
      leave-from-class="transform scale-100 opacity-100"
      leave-to-class="transform scale-95 opacity-0"
    >
      <div
        v-if="isOpen"
        class="absolute right-0 z-10 mt-2 w-48 origin-top-right rounded-lg bg-white shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none"
      >
        <div class="py-1">
          <button
            v-for="locale in availableLocales"
            :key="locale.code"
            @click="changeLocale(locale.code)"
            class="flex items-center w-full px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
            :class="{ 'bg-gray-50 font-semibold': locale.code === currentLocale }"
          >
            <span class="flex-1" :class="locale.dir === 'rtl' ? 'text-right' : 'text-left'">
              {{ locale.name }}
            </span>
            <svg
              v-if="locale.code === currentLocale"
              class="w-5 h-5 text-blue-600"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
            </svg>
          </button>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
const { locale, locales, setLocale } = useI18n({ useScope: 'global' })
const isOpen = ref(false)

const currentLocale = computed(() => locale.value)
const currentLocaleName = computed(() => {
  const current = (locales.value as any[]).find((l: any) => l.code === locale.value)
  return current?.name || 'English'
})

const availableLocales = computed(() => locales.value)

const toggleDropdown = () => {
  isOpen.value = !isOpen.value
}

const changeLocale = async (newLocale: string) => {
  await setLocale(newLocale)
  isOpen.value = false

  // Update document direction for RTL support
  const localeObj = (locales.value as any[]).find((l: any) => l.code === newLocale)
  if (localeObj) {
    document.documentElement.dir = localeObj.dir || 'ltr'
    document.documentElement.lang = newLocale
  }
}

// Set initial direction
onMounted(() => {
  const localeObj = (locales.value as any[]).find((l: any) => l.code === locale.value)
  if (localeObj) {
    document.documentElement.dir = localeObj.dir || 'ltr'
    document.documentElement.lang = locale.value
  }
})

// Close dropdown when clicking outside
onMounted(() => {
  document.addEventListener('click', (e) => {
    const target = e.target as HTMLElement
    if (!target.closest('.relative')) {
      isOpen.value = false
    }
  })
})
</script>
