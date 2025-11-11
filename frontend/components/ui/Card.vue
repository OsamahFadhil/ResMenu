<template>
  <div :class="cardClasses">
    <div v-if="$slots.header || title" class="px-4 py-5 border-b border-gray-200 sm:px-6">
      <slot name="header">
        <h3 class="text-lg leading-6 font-medium text-gray-900">{{ title }}</h3>
        <p v-if="subtitle" class="mt-1 text-sm text-gray-500">{{ subtitle }}</p>
      </slot>
    </div>

    <div :class="bodyClasses">
      <slot></slot>
    </div>

    <div v-if="$slots.footer" class="px-4 py-4 border-t border-gray-200 sm:px-6 bg-gray-50">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  title?: string
  subtitle?: string
  padding?: boolean
  hover?: boolean
  clickable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  padding: true,
  hover: false,
  clickable: false
})

const baseClasses = 'bg-white overflow-hidden shadow rounded-lg'

const cardClasses = computed(() => {
  const classes = [baseClasses]

  if (props.hover) {
    classes.push('transition-shadow hover:shadow-lg')
  }

  if (props.clickable) {
    classes.push('cursor-pointer')
  }

  return classes.join(' ')
})

const bodyClasses = computed(() => {
  return props.padding ? 'px-4 py-5 sm:p-6' : ''
})
</script>
