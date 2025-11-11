<template>
  <NuxtLayout name="admin">
    <div class="space-y-6">
      <div class="flex items-center justify-between">
        <h1 class="text-2xl font-bold text-gray-900">{{ $t('dashboard.title') }}</h1>
      </div>

      <LoadingSpinner v-if="loading" size="lg" :text="$t('common.loading')" />

      <div v-else>
        <!-- Statistics Cards -->
        <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
          <Card>
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="rounded-md bg-indigo-500 p-3">
                  <svg class="h-6 w-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">{{ $t('dashboard.totalRestaurants') }}</dt>
                  <dd class="text-2xl font-bold text-gray-900">{{ analytics?.totalRestaurants || 0 }}</dd>
                </dl>
              </div>
            </div>
          </Card>

          <Card>
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="rounded-md bg-green-500 p-3">
                  <svg class="h-6 w-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">{{ $t('dashboard.activeRestaurants') }}</dt>
                  <dd class="text-2xl font-bold text-gray-900">{{ analytics?.activeRestaurants || 0 }}</dd>
                </dl>
              </div>
            </div>
          </Card>

          <Card>
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="rounded-md bg-blue-500 p-3">
                  <svg class="h-6 w-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">{{ $t('dashboard.totalUsers') }}</dt>
                  <dd class="text-2xl font-bold text-gray-900">{{ analytics?.totalUsers || 0 }}</dd>
                </dl>
              </div>
            </div>
          </Card>

          <Card>
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="rounded-md bg-yellow-500 p-3">
                  <svg class="h-6 w-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 10h16M4 14h16M4 18h16" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">{{ $t('dashboard.totalMenuItems') }}</dt>
                  <dd class="text-2xl font-bold text-gray-900">{{ analytics?.totalMenuItems || 0 }}</dd>
                </dl>
              </div>
            </div>
          </Card>
        </div>

        <!-- Charts Section -->
        <div class="grid grid-cols-1 gap-5 lg:grid-cols-2 mt-6">
          <Card :title="$t('dashboard.monthlyStats')">
            <div class="text-sm text-gray-600">
              {{ $t('dashboard.recentActivity') }}
            </div>
          </Card>

          <Card :title="$t('dashboard.analytics')">
            <div class="space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">{{ $t('dashboard.totalCategories') }}</span>
                <span class="font-medium">{{ analytics?.totalCategories || 0 }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">{{ $t('dashboard.totalQRScans') }}</span>
                <span class="font-medium">{{ analytics?.totalQRScans || 0 }}</span>
              </div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
definePageMeta({
  layout: false,
  middleware: ['admin']
})

const { $api } = useNuxtApp()
const loading = ref(true)
const analytics = ref<any>(null)

onMounted(async () => {
  try {
    const response = await $api('/admin/analytics')
    analytics.value = response.data
  } catch (error) {
    console.error('Failed to load analytics:', error)
  } finally {
    loading.value = false
  }
})
</script>
