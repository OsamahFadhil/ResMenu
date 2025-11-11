<template>
  <div class="min-h-screen bg-gradient-to-br from-amber-50 via-orange-50 to-rose-50 text-slate-700">
    <!-- Sidebar -->
    <aside
      class="fixed inset-y-0 z-40 w-72 bg-white/80 backdrop-blur border-orange-100 shadow-lg transition-transform duration-300 ease-in-out md:translate-x-0"
      :class="sidebarClasses"
    >
      <div class="flex items-center justify-between px-6 py-5 border-b border-orange-100">
        <div>
          <p class="text-xs uppercase tracking-[0.3em] text-orange-400">{{ $t('common.appName') }}</p>
          <h1 class="text-xl font-semibold text-slate-800">{{ restaurantName }}</h1>
        </div>
        <button
          @click="toggleSidebar"
          class="rounded-full p-2 text-orange-400 hover:bg-orange-100 md:hidden"
        >
          <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <nav class="mt-6 space-y-2 px-4">
        <NuxtLink
          v-for="item in navigation"
          :key="item.name"
          :to="item.href"
          :class="navClass(item.href)"
        >
          <div
            :class="iconClass(item.href)"
          >
            <component :is="item.icon" class="h-4 w-4" />
          </div>
          <span>{{ item.name }}</span>
        </NuxtLink>
      </nav>

      <div class="mt-auto px-4 pb-6 pt-8">
        <div class="rounded-2xl border border-orange-100 bg-orange-50/80 p-4 text-sm">
          <p class="font-semibold text-slate-800">{{ authStore.user?.name }}</p>
          <p class="text-xs text-orange-500 uppercase tracking-wide">{{ authStore.user?.role }}</p>
          <button
            @click="handleLogout"
            class="mt-4 flex w-full items-center justify-center gap-2 rounded-xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white shadow hover:bg-slate-800 transition"
          >
            <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
            {{ $t('common.logout') }}
          </button>
        </div>
      </div>
    </aside>

    <!-- Mobile backdrop -->
    <div
      v-if="sidebarOpen"
      class="fixed inset-0 z-30 bg-slate-900/40 md:hidden"
      @click="toggleSidebar"
    ></div>

    <!-- Main content -->
    <div :class="mainContainerClasses">
      <header :class="headerClasses">
        <div :class="headerLeftClasses">
          <button
            @click="toggleSidebar"
            :class="menuButtonClasses"
          >
            <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>
          <div :class="headerTextClasses">
            <p class="text-xs uppercase tracking-widest text-orange-400">{{ pageTitle }}</p>
            <h2 class="text-lg font-semibold text-slate-800">{{ pageSubtitle }}</h2>
          </div>
        </div>

        <div class="flex items-center gap-4">
          <LanguageSwitcher />
          <div class="hidden sm:flex items-center gap-2 rounded-xl border border-orange-100 bg-white px-3 py-2 text-sm shadow-sm">
            <span class="font-medium text-slate-700">{{ $t('common.welcome') }}</span>
            <span class="text-orange-500">{{ authStore.user?.name }}</span>
          </div>
        </div>
      </header>

      <main class="px-4 py-8 sm:px-8">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { h } from 'vue'
import { useRestaurantStore } from '@/stores/restaurant'

const authStore = useAuthStore()
const restaurantStore = useRestaurantStore()
const router = useRouter()
const route = useRoute()
const { t, locale } = useI18n({ useScope: 'global' })

const rtlLocales = ['ar']
const sidebarOpen = ref(false)

const isRtl = computed(() => rtlLocales.includes(locale.value))

const restaurantName = computed(() => {
  const localized = restaurantStore.publicMenu?.restaurantLocalizedName
  if (localized && localized.trim().length > 0) {
    return localized
  }
  const baseName = restaurantStore.publicMenu?.restaurantName
  if (baseName && baseName.trim().length > 0) {
    return baseName
  }
  return authStore.user?.name || t('common.appName')
})

const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value
}

const navigation = computed(() => [
  {
    name: t('navigation.dashboard'),
    href: '/dashboard',
    icon: () =>
      h('svg', { class: 'h-4 w-4', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
        h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M3 13h8V3H3v10zm10 8h8v-6h-8v6zm0-8h8V3h-8v10zM3 21h8v-6H3v6z' })
      ])
  },
  {
    name: t('navigation.categories'),
    href: '/dashboard/categories',
    icon: () =>
      h('svg', { class: 'h-4 w-4', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
        h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M5 4h4v4H5V4zm0 6h4v4H5v-4zm0 6h4v4H5v-4zM11 6h8M11 12h8M11 18h8' })
      ])
  },
  {
    name: t('navigation.templates'),
    href: '/dashboard/templates',
    icon: () =>
      h('svg', { class: 'h-4 w-4', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
        h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M12 20h9' }),
        h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M16.5 3.5a2.121 2.121 0 013 3L7 19l-4 1 1-4 12.5-12.5z' })
      ])
  },
  {
    name: t('navigation.qrcodes'),
    href: '/dashboard/qrcodes',
    icon: () =>
      h('svg', { class: 'h-4 w-4', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
        h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M4 4h6v6H4V4zm10 0h6v6h-6V4zM4 14h6v6H4v-6zm10 4h2m2 0h2m-6-4h2m2 0h2' })
      ])
  }
])

const navClass = (href: string) => [
  'group flex items-center gap-3 rounded-xl px-4 py-3 text-sm font-medium transition-all',
  route.path === href
    ? 'bg-gradient-to-r from-orange-500 to-amber-500 text-white shadow-md'
    : 'text-slate-600 hover:bg-orange-100 hover:text-orange-600'
]

const iconClass = (href: string) => [
  'flex h-9 w-9 items-center justify-center rounded-lg border transition',
  route.path === href
    ? 'border-white bg-white text-orange-500'
    : 'border-transparent bg-orange-100 text-orange-500 group-hover:bg-white group-hover:border-orange-200'
]

const pageTitle = computed(() => {
  const item = navigation.value.find((link) => link.href === route.path)
  return item ? item.name : t('navigation.dashboard')
})

const pageSubtitle = computed(() => {
  switch (route.path) {
    case '/dashboard/categories':
      return t('menu.categories')
    case '/dashboard/qrcodes':
      return t('qr.title')
    case '/dashboard/templates':
      return t('navigation.templates')
    default:
      return t('dashboard.overview')
  }
})

const handleLogout = async () => {
  await authStore.logout()
  router.push('/login')
}

const sidebarClasses = computed(() => ({
  'left-0': !isRtl.value,
  'right-0': isRtl.value,
  '-translate-x-full md:translate-x-0': !isRtl.value && !sidebarOpen.value,
  'translate-x-full md:translate-x-0': isRtl.value && !sidebarOpen.value
}))

const mainContainerClasses = computed(() => [
  isRtl.value ? 'md:pr-72 md:pl-0' : 'md:pl-72 md:pr-0',
  'transition-all'
])

const headerClasses = computed(() => [
  'sticky top-0 z-20 flex h-20 items-center justify-between border-b border-orange-100 bg-white/70 px-6 backdrop-blur',
  isRtl.value ? 'flex-row-reverse' : 'flex-row'
])

const headerLeftClasses = computed(() => [
  'flex items-center gap-3',
  isRtl.value ? 'flex-row-reverse' : 'flex-row'
])

const headerTextClasses = computed(() => [
  'hidden sm:block',
  isRtl.value ? 'text-right' : 'text-left'
])

const menuButtonClasses = computed(() => [
  'rounded-xl border border-orange-200 bg-white p-2 text-orange-400 shadow-sm md:hidden',
  isRtl.value ? 'ml-3' : 'mr-3'
])

const applyDocumentDirection = (localeCode: string) => {
  if (process.client) {
    const dir = rtlLocales.includes(localeCode) ? 'rtl' : 'ltr'
    document.documentElement.dir = dir
    document.documentElement.lang = localeCode
  }
}

onMounted(async () => {
  authStore.loadFromStorage()
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }

  const restaurantId = authStore.restaurantId
  if (restaurantId) {
    await restaurantStore.fetchCategories(restaurantId, locale.value)
  }

  applyDocumentDirection(locale.value)
})

watch(
  () => locale.value,
  async (newLocale, oldLocale) => {
    if (newLocale === oldLocale) {
      return
    }
    const restaurantId = authStore.restaurantId
    if (restaurantId) {
      await restaurantStore.fetchCategories(restaurantId, newLocale)
    }

    applyDocumentDirection(newLocale)
  }
)
</script>
