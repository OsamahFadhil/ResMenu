<template>
  <div class="min-h-screen bg-white">
    <!-- Navigation -->
    <nav class="bg-white border-b border-gray-200">
      <div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div class="flex h-16 justify-between items-center">
          <div class="flex items-center">
            <NuxtLink to="/" class="flex items-center">
              <span class="text-2xl font-bold text-indigo-600">{{ $t('common.appName') }}</span>
            </NuxtLink>
          </div>

          <div class="flex items-center gap-4">
            <LanguageSwitcher />

            <template v-if="!authStore.isAuthenticated">
              <NuxtLink
                to="/login"
                class="text-gray-700 hover:text-indigo-600 px-3 py-2 rounded-md text-sm font-medium"
              >
                {{ $t('common.login') }}
              </NuxtLink>
              <NuxtLink
                to="/register"
                class="bg-indigo-600 text-white hover:bg-indigo-700 px-4 py-2 rounded-md text-sm font-medium"
              >
                {{ $t('common.register') }}
              </NuxtLink>
            </template>
            <template v-else>
              <NuxtLink
                to="/dashboard"
                class="text-gray-700 hover:text-indigo-600 px-3 py-2 rounded-md text-sm font-medium"
              >
                {{ $t('navigation.dashboard') }}
              </NuxtLink>
              <button
                @click="handleLogout"
                class="text-gray-700 hover:text-indigo-600 px-3 py-2 rounded-md text-sm font-medium"
              >
                {{ $t('common.logout') }}
              </button>
            </template>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main content -->
    <main>
      <slot />
    </main>

    <!-- Footer -->
    <footer class="bg-gray-50 border-t border-gray-200">
      <div class="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div>
            <h3 class="text-sm font-semibold text-gray-900 uppercase tracking-wider">
              {{ $t('common.appName') }}
            </h3>
            <p class="mt-4 text-sm text-gray-600">
              Professional restaurant menu management platform
            </p>
          </div>
          <div>
            <h3 class="text-sm font-semibold text-gray-900 uppercase tracking-wider">
              Links
            </h3>
            <ul class="mt-4 space-y-2">
              <li>
                <NuxtLink to="/" class="text-sm text-gray-600 hover:text-indigo-600">
                  {{ $t('navigation.home') }}
                </NuxtLink>
              </li>
              <li>
                <NuxtLink to="/login" class="text-sm text-gray-600 hover:text-indigo-600">
                  {{ $t('common.login') }}
                </NuxtLink>
              </li>
              <li>
                <NuxtLink to="/register" class="text-sm text-gray-600 hover:text-indigo-600">
                  {{ $t('common.register') }}
                </NuxtLink>
              </li>
            </ul>
          </div>
          <div>
            <h3 class="text-sm font-semibold text-gray-900 uppercase tracking-wider">
              {{ $t('settings.language') }}
            </h3>
            <div class="mt-4">
              <LanguageSwitcher />
            </div>
          </div>
        </div>
        <div class="mt-8 border-t border-gray-200 pt-8">
          <p class="text-sm text-gray-600 text-center">
            © {{ new Date().getFullYear() }} {{ $t('common.appName') }}. All rights reserved.
          </p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
const authStore = useAuthStore()
const router = useRouter()

const handleLogout = async () => {
  await authStore.logout()
  router.push('/')
}
</script>
