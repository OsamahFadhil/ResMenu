<template>
  <NuxtLayout name="admin">
    <div class="space-y-6">
      <div class="flex items-center justify-between">
        <h1 class="text-2xl font-bold text-gray-900">{{ $t('restaurant.title') }}</h1>
      </div>

      <!-- Filters -->
      <Card>
        <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <Input
            v-model="filters.search"
            type="search"
            :placeholder="$t('common.search')"
            @input="handleSearch"
          />
          <div>
            <select
              v-model="filters.isActive"
              class="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
              @change="loadRestaurants"
            >
              <option :value="null">{{ $t('common.all') }}</option>
              <option :value="true">{{ $t('common.active') }}</option>
              <option :value="false">{{ $t('common.inactive') }}</option>
            </select>
          </div>
        </div>
      </Card>

      <!-- Restaurants Table -->
      <Card>
        <UiTable
          :columns="columns"
          :data="restaurants"
          :loading="loading"
        >
          <template #cell-isActive="{ value }">
            <Badge :variant="value ? 'success' : 'secondary'">
              {{ value ? $t('common.active') : $t('common.inactive') }}
            </Badge>
          </template>

          <template #cell-createdAt="{ value }">
            {{ new Date(value).toLocaleDateString() }}
          </template>

          <template #actions="{ row }">
            <button
              @click="viewRestaurant(row)"
              class="text-indigo-600 hover:text-indigo-900 mr-3"
            >
              {{ $t('common.view') }}
            </button>
          </template>
        </UiTable>

        <Pagination
          v-if="pagination.totalPages > 1"
          :current-page="pagination.page"
          :total-pages="pagination.totalPages"
          :total="pagination.total"
          :per-page="pagination.pageSize"
          @page-change="handlePageChange"
        />
      </Card>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
definePageMeta({
  layout: false,
  middleware: ['admin']
})

const { $api } = useNuxtApp()
const { t } = useI18n()

const loading = ref(false)
const restaurants = ref<any[]>([])
const filters = ref({
  search: '',
  isActive: null
})
const pagination = ref({
  page: 1,
  pageSize: 10,
  total: 0,
  totalPages: 0
})

const columns = computed(() => [
  { key: 'name', label: t('restaurant.restaurantName') },
  { key: 'slug', label: t('restaurant.slug') },
  { key: 'ownerName', label: t('restaurant.owner') },
  { key: 'isActive', label: t('restaurant.isActive') },
  { key: 'createdAt', label: t('restaurant.createdAt') }
])

const loadRestaurants = async () => {
  loading.value = true
  try {
    const params = new URLSearchParams({
      page: pagination.value.page.toString(),
      pageSize: pagination.value.pageSize.toString()
    })

    if (filters.value.search) {
      params.append('searchTerm', filters.value.search)
    }
    if (filters.value.isActive !== null) {
      params.append('isActive', filters.value.isActive.toString())
    }

    const response = await $api(`/restaurants?${params.toString()}`)
    restaurants.value = response.data.items
    pagination.value.total = response.data.totalCount
    pagination.value.totalPages = response.data.totalPages
  } catch (error) {
    console.error('Failed to load restaurants:', error)
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  pagination.value.page = 1
  loadRestaurants()
}

const handlePageChange = (page: number) => {
  pagination.value.page = page
  loadRestaurants()
}

const viewRestaurant = (restaurant: any) => {
  // Navigate to restaurant details
  console.log('View restaurant:', restaurant)
}

onMounted(() => {
  loadRestaurants()
})
</script>
