<template>
  <div class="w-full">
    <label v-if="label" :for="id" class="block text-sm font-medium text-neutral-700 mb-2">
      {{ label }}
      <span v-if="required" class="text-primary-600">*</span>
    </label>

    <div class="relative">
      <div v-if="$slots.prefix" class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none text-neutral-400">
        <slot name="prefix"></slot>
      </div>

      <input
        :id="id"
        :type="type"
        :value="modelValue"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :required="required"
        :min="min"
        :max="max"
        :step="step"
        :class="inputClasses"
        @input="handleInput"
        @blur="handleBlur"
        @focus="handleFocus"
      />

      <div v-if="$slots.suffix" class="absolute inset-y-0 right-0 pr-4 flex items-center text-neutral-400">
        <slot name="suffix"></slot>
      </div>
    </div>

    <p v-if="error" class="mt-2 text-sm text-primary-600">{{ error }}</p>
    <p v-else-if="hint" class="mt-2 text-sm text-neutral-500">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
interface Props {
  id?: string
  type?: 'text' | 'email' | 'password' | 'number' | 'tel' | 'url' | 'search' | 'date' | 'time'
  modelValue?: string | number
  label?: string
  placeholder?: string
  error?: string
  hint?: string
  disabled?: boolean
  readonly?: boolean
  required?: boolean
  min?: number
  max?: number
  step?: number
}

const props = withDefaults(defineProps<Props>(), {
  type: 'text',
  disabled: false,
  readonly: false,
  required: false
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string | number): void
  (e: 'blur'): void
  (e: 'focus'): void
}>()

const baseClasses = 'block w-full px-4 py-2.5 border rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-0 transition-all duration-200 sm:text-sm bg-white'

const slots = useSlots()

const inputClasses = computed(() => {
  const classes = [baseClasses]

  if (props.error) {
    classes.push('border-primary-300 text-primary-900 placeholder-primary-300 focus:ring-primary-500 focus:border-primary-500')
  } else {
    classes.push('border-neutral-300 focus:ring-primary-500 focus:border-primary-500 hover:border-neutral-400')
  }

  if (props.disabled) {
    classes.push('bg-neutral-50 text-neutral-500 cursor-not-allowed')
  }

  if (slots.prefix) {
    classes.push('pl-11')
  }

  if (slots.suffix) {
    classes.push('pr-11')
  }

  return classes.join(' ')
})

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}

const handleBlur = () => {
  emit('blur')
}

const handleFocus = () => {
  emit('focus')
}
</script>
