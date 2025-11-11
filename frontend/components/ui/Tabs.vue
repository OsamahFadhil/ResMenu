<template>
  <div>
    <div class="border-b border-gray-200">
      <nav class="-mb-px flex space-x-8" aria-label="Tabs">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          type="button"
          :class="tabClasses(tab.key)"
          @click="selectTab(tab.key)"
        >
          {{ tab.label }}
          <span v-if="tab.count !== undefined" :class="countClasses(tab.key)">
            {{ tab.count }}
          </span>
        </button>
      </nav>
    </div>
    <div class="mt-4">
      <slot :name="modelValue"></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Tab {
  key: string
  label: string
  count?: number
}

interface Props {
  tabs: Tab[]
  modelValue: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'change', value: string): void
}>()

const tabClasses = (key: string) => {
  const baseClasses = 'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm inline-flex items-center gap-2'
  const activeClasses = 'border-indigo-500 text-indigo-600'
  const inactiveClasses = 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'

  return key === props.modelValue
    ? `${baseClasses} ${activeClasses}`
    : `${baseClasses} ${inactiveClasses}`
}

const countClasses = (key: string) => {
  const baseClasses = 'ml-2 py-0.5 px-2.5 rounded-full text-xs font-medium'
  const activeClasses = 'bg-indigo-100 text-indigo-600'
  const inactiveClasses = 'bg-gray-100 text-gray-900'

  return key === props.modelValue
    ? `${baseClasses} ${activeClasses}`
    : `${baseClasses} ${inactiveClasses}`
}

const selectTab = (key: string) => {
  emit('update:modelValue', key)
  emit('change', key)
}
</script>
