<template>
  <div :class="cardClasses">
    <div v-if="$slots.header || title" class="px-6 py-5 border-b border-neutral-200">
      <slot name="header">
        <h3 class="text-lg leading-6 font-semibold text-neutral-900">{{ title }}</h3>
        <p v-if="subtitle" class="mt-1 text-sm text-neutral-600">{{ subtitle }}</p>
      </slot>
    </div>

    <div :class="bodyClasses">
      <slot></slot>
    </div>

    <div v-if="$slots.footer" class="px-6 py-4 border-t border-neutral-200 bg-neutral-50 rounded-b-xl">
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

const baseClasses = 'bg-white overflow-hidden rounded-xl'

const cardClasses = computed(() => {
  const classes = [baseClasses]

  if (props.hover) {
    classes.push('shadow-soft hover:shadow-medium transition-shadow duration-200')
  } else {
    classes.push('shadow-soft')
  }

  if (props.clickable) {
    classes.push('cursor-pointer active:scale-[0.99]')
  }

  return classes.join(' ')
})

const bodyClasses = computed(() => {
  return props.padding ? 'px-6 py-6' : ''
})
</script>
