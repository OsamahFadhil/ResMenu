<template>
  <NuxtLayout name="dashboard">
    <div class="space-y-6">
      <div class="flex items-center justify-between">
        <h1 class="text-2xl font-bold text-slate-800">{{ $t('qr.title') }}</h1>
        <UiButton v-if="!qrCode" @click="generateQRCode" variant="primary" :loading="generating">
          <svg class="h-5 w-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          {{ $t('qr.generate') }}
        </UiButton>
        <UiButton v-else @click="regenerateQRCode" variant="secondary" :loading="generating">
          <svg class="h-5 w-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          {{ $t('qr.regenerate') }}
        </UiButton>
      </div>

      <div v-if="loading" class="flex justify-center py-12">
        <LoadingSpinner size="lg" />
      </div>

      <div v-else class="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <!-- QR Code Display -->
        <Card>
          <div v-if="qrCode" class="text-center space-y-6">
            <div class="bg-gradient-to-br from-indigo-50 to-purple-50 p-8 inline-block rounded-2xl shadow-lg">
              <img :src="qrCode.imageUrl" alt="QR Code" class="w-64 h-64 mx-auto rounded-lg" />
            </div>
            <div class="space-y-3">
              <p class="text-sm font-medium text-gray-700">{{ $t('qr.link') }}</p>
              <div class="flex items-center gap-2">
                <input
                  :value="qrCode.link"
                  readonly
                  class="flex-1 px-4 py-2 text-sm border border-gray-300 rounded-lg bg-gray-50 text-gray-700"
                />
                <button
                  @click="copyLink"
                  class="px-4 py-2 text-sm font-medium text-indigo-600 hover:bg-indigo-50 rounded-lg transition"
                >
                  <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                  </svg>
                </button>
              </div>
            </div>
            <div class="flex gap-3">
              <UiButton @click="downloadQRCode" variant="primary" class="flex-1">
                <svg class="h-5 w-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
                </svg>
                {{ $t('qr.download') }}
              </UiButton>
            </div>
          </div>
          <EmptyState
            v-else
            title="No QR Code Yet"
            description="Generate a QR code for your restaurant menu that customers can scan"
          >
            <template #action>
              <UiButton @click="generateQRCode" variant="primary" :loading="generating">
                {{ $t('qr.generate') }}
              </UiButton>
            </template>
          </EmptyState>
        </Card>

        <!-- QR Code Info -->
        <Card>
          <div class="space-y-6">
            <div v-if="qrCode" class="p-4 bg-indigo-50 rounded-lg border border-indigo-200">
              <div class="flex items-center justify-between">
                <span class="text-sm font-medium text-indigo-900">{{ $t('qr.scanCount') }}</span>
                <span class="text-2xl font-bold text-indigo-600">{{ qrCode.scanCount || 0 }}</span>
              </div>
            </div>

            <div class="prose prose-sm max-w-none">
              <h4 class="text-lg font-semibold text-gray-900">How to use your QR Code:</h4>
              <ul class="space-y-2 text-gray-600">
                <li>Download and print the QR code in high quality</li>
                <li>Place it prominently on your tables, menus, or storefront</li>
                <li>Customers scan it with their phone camera to instantly view your menu</li>
                <li>No app download required - works with any smartphone</li>
                <li>Update your menu anytime without reprinting</li>
              </ul>
            </div>

            <Alert variant="info">
              <p class="text-sm">
                <strong>Pro Tip:</strong> Print multiple copies and place them at different locations in your restaurant for maximum visibility.
              </p>
            </Alert>
          </div>
        </Card>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
definePageMeta({
  layout: false,
  middleware: ['owner']
})

const { t } = useI18n({ useScope: 'global' })
const authStore = useAuthStore()
const restaurantStore = useRestaurantStore()

const loading = ref(false)
const generating = ref(false)
const qrCode = computed(() => restaurantStore.qrCode)

const restaurantId = computed(() => authStore.restaurantId)

// Load existing QR code on mount
onMounted(async () => {
  if (!restaurantId.value) {
    return
  }
  
  loading.value = true
  try {
    await restaurantStore.generateQRCode(restaurantId.value)
  } catch (error) {
    console.error('Failed to load QR code:', error)
  } finally {
    loading.value = false
  }
})

const generateQRCode = async () => {
  if (!restaurantId.value) {
    alert(t('messages.errorOccurred'))
    return
  }

  generating.value = true
  try {
    await restaurantStore.generateQRCode(restaurantId.value)
  } catch (error) {
    console.error('Failed to generate QR code:', error)
    alert(t('messages.errorOccurred'))
  } finally {
    generating.value = false
  }
}

const regenerateQRCode = async () => {
  const confirmed = confirm('Are you sure you want to regenerate the QR code? The old one will stop working.')
  if (!confirmed) {
    return
  }
  await generateQRCode()
}

const copyLink = async () => {
  if (qrCode.value?.link) {
    try {
      await navigator.clipboard.writeText(qrCode.value.link)
      alert('Link copied to clipboard!')
    } catch (error) {
      console.error('Failed to copy link:', error)
    }
  }
}

const downloadQRCode = () => {
  if (!qrCode.value?.imageUrl) {
    return
  }

  // Create a temporary link element and trigger download
  const link = document.createElement('a')
  link.href = qrCode.value.imageUrl
  link.download = `qr-code-${restaurantId.value}.png`
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}
</script>
