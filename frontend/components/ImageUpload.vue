<template>
  <div class="space-y-4">
    <label v-if="label" class="block text-sm font-medium text-gray-700">
      {{ label }}
    </label>

    <div
      class="flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-lg hover:border-indigo-400 transition-colors"
      :class="{ 'border-indigo-500': isDragging }"
      @dragover.prevent="isDragging = true"
      @dragleave.prevent="isDragging = false"
      @drop.prevent="handleDrop"
    >
      <div class="space-y-2 text-center">
        <div v-if="preview" class="mb-4">
          <img :src="preview" alt="Preview" class="mx-auto h-32 w-32 object-cover rounded-lg" />
          <button
            @click="removeImage"
            class="mt-2 text-sm text-red-600 hover:text-red-700"
          >
            {{ $t('common.delete') }}
          </button>
        </div>

        <div v-else>
          <svg
            class="mx-auto h-12 w-12 text-gray-400"
            stroke="currentColor"
            fill="none"
            viewBox="0 0 48 48"
          >
            <path
              d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
          <div class="flex text-sm text-gray-600 justify-center">
            <label
              for="file-upload"
              class="relative cursor-pointer rounded-md font-medium text-indigo-600 hover:text-indigo-500 focus-within:outline-none"
            >
              <span>{{ $t('files.uploadImage') }}</span>
              <input
                id="file-upload"
                name="file-upload"
                type="file"
                class="sr-only"
                accept="image/*"
                @change="handleFileSelect"
              />
            </label>
            <p class="pl-1">{{ $t('files.dragDrop') }}</p>
          </div>
          <p class="text-xs text-gray-500">
            PNG, JPG, GIF {{ $t('files.maxSize') }}: {{ maxSizeMB }}MB
          </p>
        </div>

        <div v-if="uploading" class="mt-4">
          <LoadingSpinner size="sm" :text="$t('common.loading')" />
        </div>

        <Alert v-if="error" variant="error" dismissible @dismiss="error = ''">
          {{ error }}
        </Alert>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  modelValue?: string
  label?: string
  maxSizeMB?: number
}

const props = withDefaults(defineProps<Props>(), {
  maxSizeMB: 5
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'upload', file: File): void
}>()

const { $api } = useNuxtApp()

const isDragging = ref(false)
const preview = ref(props.modelValue || '')
const uploading = ref(false)
const error = ref('')

watch(() => props.modelValue, (newValue) => {
  preview.value = newValue || ''
})

const validateFile = (file: File): boolean => {
  const maxSize = props.maxSizeMB * 1024 * 1024

  if (!file.type.startsWith('image/')) {
    error.value = 'Please upload an image file'
    return false
  }

  if (file.size > maxSize) {
    error.value = `File size must be less than ${props.maxSizeMB}MB`
    return false
  }

  return true
}

const uploadFile = async (file: File) => {
  if (!validateFile(file)) return

  uploading.value = true
  error.value = ''

  try {
    const formData = new FormData()
    formData.append('file', file)

    const response = await $api('/files/upload', {
      method: 'POST',
      body: formData
    })

    const imageUrl = response.data
    preview.value = imageUrl
    emit('update:modelValue', imageUrl)
    emit('upload', file)
  } catch (err: any) {
    error.value = err.message || 'Upload failed'
  } finally {
    uploading.value = false
  }
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    uploadFile(file)
  }
}

const handleDrop = (event: DragEvent) => {
  isDragging.value = false
  const file = event.dataTransfer?.files[0]
  if (file) {
    uploadFile(file)
  }
}

const removeImage = () => {
  preview.value = ''
  emit('update:modelValue', '')
}
</script>
