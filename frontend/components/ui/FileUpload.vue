<template>
  <div class="w-full">
    <label v-if="label" class="block text-sm font-medium text-gray-700 mb-2">
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>

    <div
      @drop.prevent="handleDrop"
      @dragover.prevent="isDragging = true"
      @dragleave.prevent="isDragging = false"
      :class="[
        'relative border-2 border-dashed rounded-lg p-6 transition-colors',
        isDragging ? 'border-indigo-500 bg-indigo-50' : 'border-gray-300 hover:border-gray-400',
        disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer'
      ]"
    >
      <input
        ref="fileInput"
        type="file"
        :accept="accept"
        :disabled="disabled"
        @change="handleFileSelect"
        class="hidden"
      />

      <div v-if="!preview && !uploading" class="text-center" @click="triggerFileInput">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
        </svg>
        <p class="mt-2 text-sm text-gray-600">
          <span class="font-semibold text-indigo-600 hover:text-indigo-500">{{ $t('files.selectFile') }}</span>
          {{ $t('common.or') }} {{ $t('files.dragDrop') }}
        </p>
        <p class="mt-1 text-xs text-gray-500">
          {{ accept || 'PNG, JPG, GIF up to 10MB' }}
        </p>
      </div>

      <div v-else-if="uploading" class="text-center">
        <LoadingSpinner size="lg" />
        <p class="mt-2 text-sm text-gray-600">{{ $t('common.uploading') }}...</p>
      </div>

      <div v-else class="relative">
        <img :src="preview" :alt="fileName" class="mx-auto max-h-48 rounded-lg" />
        <div class="mt-3 flex items-center justify-center gap-3">
          <p class="text-sm text-gray-600 truncate max-w-xs">{{ fileName }}</p>
          <button
            @click.stop="removeFile"
            type="button"
            class="text-red-600 hover:text-red-700 font-medium text-sm"
          >
            {{ $t('common.delete') }}
          </button>
        </div>
      </div>
    </div>

    <p v-if="error" class="mt-2 text-sm text-red-600">{{ error }}</p>
    <p v-else-if="hint" class="mt-2 text-sm text-gray-500">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
interface Props {
  modelValue?: string
  label?: string
  accept?: string
  maxSize?: number // in MB
  required?: boolean
  disabled?: boolean
  hint?: string
  error?: string
}

const props = withDefaults(defineProps<Props>(), {
  accept: 'image/*',
  maxSize: 10,
  disabled: false,
  required: false
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'upload', file: File): void
}>()

const fileInput = ref<HTMLInputElement | null>(null)
const isDragging = ref(false)
const uploading = ref(false)
const preview = ref(props.modelValue || '')
const fileName = ref('')

watch(() => props.modelValue, (newValue) => {
  if (newValue && newValue !== preview.value) {
    preview.value = newValue
  }
})

const triggerFileInput = () => {
  if (!props.disabled) {
    fileInput.value?.click()
  }
}

const validateFile = (file: File): string | null => {
  if (props.maxSize && file.size > props.maxSize * 1024 * 1024) {
    return `File size must be less than ${props.maxSize}MB`
  }
  
  if (props.accept && !file.type.match(props.accept.replace('*', '.*'))) {
    return 'Invalid file type'
  }
  
  return null
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    processFile(file)
  }
}

const handleDrop = (event: DragEvent) => {
  isDragging.value = false
  const file = event.dataTransfer?.files[0]
  if (file) {
    processFile(file)
  }
}

const processFile = async (file: File) => {
  const validationError = validateFile(file)
  if (validationError) {
    emit('update:modelValue', '')
    return
  }

  fileName.value = file.name
  
  // Create preview
  const reader = new FileReader()
  reader.onload = (e) => {
    preview.value = e.target?.result as string
  }
  reader.readAsDataURL(file)

  // Emit file for upload
  emit('upload', file)
}

const removeFile = () => {
  preview.value = ''
  fileName.value = ''
  emit('update:modelValue', '')
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}
</script>

