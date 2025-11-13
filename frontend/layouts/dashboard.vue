<template>
  <div class="min-h-screen bg-neutral-50 text-neutral-800">
    <!-- Sidebar -->
    <aside
      class="fixed inset-y-0 z-40 w-72 bg-white border-r border-neutral-200 shadow-soft transition-transform duration-300 ease-in-out md:translate-x-0"
      :class="sidebarClasses"
    >
      <div class="flex items-center justify-between px-6 py-6 border-b border-neutral-200">
        <div>
          <p class="text-xs uppercase tracking-widest text-primary-600 font-semibold">{{ $t('common.appName') }}</p>
          <h1 class="text-xl font-bold text-neutral-900 mt-1">{{ restaurantName }}</h1>
        </div>
        <button
          @click="toggleSidebar"
          class="rounded-lg p-2 text-neutral-400 hover:bg-neutral-100 hover:text-neutral-600 transition-colors md:hidden"
        >
          <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <nav class="mt-6 space-y-1 px-4">
        <NuxtLink
          v-for="item in navigation"
          :key="item.name"
          :to="item.href"
          :class="navClass(item.href)"
        >
          <div
            :class="iconClass(item.href)"
          >
            <component :is="item.icon" class="h-5 w-5" />
          </div>
          <span>{{ item.name }}</span>
        </NuxtLink>
      </nav>

      <div class="mt-auto px-4 pb-6 pt-8">
        <div class="rounded-xl border border-neutral-200 bg-neutral-50 p-4 text-sm">
          <p class="font-semibold text-neutral-900">{{ authStore.user?.name }}</p>
          <p class="text-xs text-primary-600 uppercase tracking-wide mt-1">{{ authStore.user?.role }}</p>
          <button
            @click="handleLogout"
            class="mt-4 flex w-full items-center justify-center gap-2 rounded-lg bg-primary-600 px-4 py-2.5 text-sm font-medium text-white shadow-md hover:bg-primary-700 hover:shadow-lg transition-all duration-200 active:scale-[0.98]"
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
      class="fixed inset-0 z-30 bg-black/40 backdrop-blur-sm md:hidden"
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
            <p class="text-xs uppercase tracking-widest text-primary-600 font-semibold">{{ pageTitle }}</p>
            <h2 class="text-lg font-semibold text-neutral-900">{{ pageSubtitle }}</h2>
          </div>
        </div>

        <div class="flex items-center gap-4">
          <LanguageSwitcher />
          <div class="hidden sm:flex items-center gap-2 rounded-lg border border-neutral-200 bg-white px-4 py-2 text-sm shadow-sm">
            <span class="font-medium text-neutral-700">{{ $t('common.welcome') }}</span>
            <span class="text-primary-600 font-semibold">{{ authStore.user?.name }}</span>
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
    name: 'Menu Designer',
    href: '/dashboard/designer',
    icon: () =>
      h('svg', { class: 'h-4 w-4', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
        h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01' })
      ])
  },
  {
    name: 'Settings',
    href: '/dashboard/settings',
    icon: () =>
      h('svg', { class: 'h-4 w-4', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
        h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z' }),
        h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M15 12a3 3 0 11-6 0 3 3 0 016 0z' })
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
  'group flex items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium transition-all duration-200',
  route.path === href
    ? 'bg-primary-600 text-white shadow-md'
    : 'text-neutral-700 hover:bg-neutral-100 hover:text-neutral-900'
]

const iconClass = (href: string) => [
  'flex h-10 w-10 items-center justify-center rounded-lg transition-all duration-200',
  route.path === href
    ? 'bg-white text-primary-600'
    : 'bg-neutral-100 text-neutral-600 group-hover:bg-neutral-200'
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
    case '/dashboard/designer':
      return 'Create custom menu designs'
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
  'sticky top-0 z-20 flex h-20 items-center justify-between border-b border-neutral-200 bg-white/90 px-6 backdrop-blur-sm shadow-sm',
  isRtl.value ? 'flex-row-reverse' : 'flex-row'
])

const headerLeftClasses = computed(() => [
  'flex items-center gap-4',
  isRtl.value ? 'flex-row-reverse' : 'flex-row'
])

const headerTextClasses = computed(() => [
  'hidden sm:block',
  isRtl.value ? 'text-right' : 'text-left'
])

const menuButtonClasses = computed(() => [
  'rounded-lg border border-neutral-300 bg-white p-2 text-neutral-600 shadow-sm hover:bg-neutral-50 transition-colors md:hidden',
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
