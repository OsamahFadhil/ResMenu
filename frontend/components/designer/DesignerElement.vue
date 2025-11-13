<template>
  <div
    v-if="element.visible"
    ref="elementRef"
    :style="{
      position: 'absolute',
      left: `${element.x * (zoom / 100)}px`,
      top: `${element.y * (zoom / 100)}px`,
      width: `${element.width * (zoom / 100)}px`,
      height: `${element.height * (zoom / 100)}px`,
      transform: `rotate(${element.rotation}deg)`,
      transformOrigin: 'center',
      cursor: element.locked ? 'not-allowed' : 'move',
      zIndex: element.zIndex
    }"
    :class="[
      'group',
      isSelected && 'ring-2 ring-blue-500'
    ]"
    @mousedown="handleMouseDown"
    @click.stop="emit('select')"
  >
    <!-- Element Content -->
    <div class="w-full h-full" :style="getElementStyle()">
      <!-- Text Element -->
      <div
        v-if="element.type === 'text'"
        contenteditable
        @input="handleTextInput"
        @blur="handleTextBlur"
        :style="{
          fontSize: `${element.fontSize}px`,
          fontFamily: element.fontFamily,
          fontWeight: element.fontWeight,
          color: element.color,
          textAlign: element.textAlign,
          lineHeight: element.lineHeight,
          width: '100%',
          height: '100%',
          outline: 'none',
          padding: '4px'
        }"
      >
        {{ element.text }}
      </div>

      <!-- Image Element -->
      <img
        v-else-if="element.type === 'image'"
        :src="element.imageUrl"
        :style="{
          width: '100%',
          height: '100%',
          objectFit: 'cover',
          opacity: element.imageOpacity,
          filter: element.imageFilter
        }"
        draggable="false"
      />

      <!-- Shape Element -->
      <div
        v-else-if="element.type === 'shape'"
        :style="{
          width: '100%',
          height: '100%',
          backgroundColor: element.backgroundColor,
          border: `${element.borderWidth}px solid ${element.borderColor}`,
          borderRadius: element.shapeType === 'circle' ? '50%' : `${element.borderRadius}px`,
          opacity: element.opacity
        }"
      ></div>

      <!-- Menu Item Element -->
      <div
        v-else-if="element.type === 'menuItem'"
        class="flex items-center gap-3 p-3 h-full"
        :style="{
          backgroundColor: element.backgroundColor,
          borderRadius: '8px'
        }"
      >
        <div
          v-if="element.itemImage"
          class="w-16 h-16 rounded-lg bg-neutral-700 flex-shrink-0 overflow-hidden"
        >
          <img :src="element.itemImage" class="w-full h-full object-cover" />
        </div>
        <div class="flex-1 min-w-0">
          <div
            class="font-semibold truncate"
            :style="{
              color: element.color,
              fontSize: `${element.fontSize}px`,
              fontFamily: element.fontFamily
            }"
          >
            {{ element.itemName }}
          </div>
          <div
            class="text-sm opacity-70 truncate"
            :style="{
              color: element.color,
              fontSize: `${(element.fontSize || 16) * 0.875}px`
            }"
          >
            {{ element.itemDescription }}
          </div>
        </div>
        <div
          class="font-bold whitespace-nowrap"
          :style="{
            color: element.color,
            fontSize: `${element.fontSize}px`
          }"
        >
          ${{ element.itemPrice?.toFixed(2) }}
        </div>
      </div>
    </div>

    <!-- Selection Handles -->
    <template v-if="isSelected && !element.locked">
      <!-- Resize Handles -->
      <div
        v-for="handle in resizeHandles"
        :key="handle.position"
        :class="[
          'absolute w-4 h-4 bg-white border-2 border-blue-500 rounded-full',
          'transition-opacity z-10',
          handle.cursor
        ]"
        :style="handle.style"
        @mousedown.stop="handleResizeStart($event, handle.position)"
      ></div>

      <!-- Rotate Handle -->
      <div
        class="absolute -top-8 left-1/2 -translate-x-1/2 w-6 h-6 bg-white border-2 border-blue-500 rounded-full cursor-grab transition-opacity flex items-center justify-center z-10"
        @mousedown.stop="handleRotateStart"
      >
        <svg class="w-4 h-4 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
        </svg>
      </div>

      <!-- Delete Button -->
      <button
        class="absolute -top-8 -right-8 w-6 h-6 bg-red-500 hover:bg-red-600 text-white rounded-full transition-opacity flex items-center justify-center z-10"
        @click.stop="emit('delete')"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import type { MenuDesignElement } from '@/stores/menuDesigner'

const props = defineProps<{
  element: MenuDesignElement
  zoom: number
  isSelected: boolean
}>()

const emit = defineEmits<{
  (e: 'select'): void
  (e: 'update', updates: Partial<MenuDesignElement>): void
  (e: 'delete'): void
}>()

const elementRef = ref<HTMLElement>()
const isDragging = ref(false)
const isResizing = ref(false)
const isRotating = ref(false)
const resizePosition = ref('')
const startX = ref(0)
const startY = ref(0)
const startWidth = ref(0)
const startHeight = ref(0)
const startRotation = ref(0)

const resizeHandles = [
  { position: 'nw', style: { top: '-6px', left: '-6px' }, cursor: 'cursor-nw-resize' },
  { position: 'n', style: { top: '-6px', left: '50%', transform: 'translateX(-50%)' }, cursor: 'cursor-n-resize' },
  { position: 'ne', style: { top: '-6px', right: '-6px' }, cursor: 'cursor-ne-resize' },
  { position: 'e', style: { top: '50%', right: '-6px', transform: 'translateY(-50%)' }, cursor: 'cursor-e-resize' },
  { position: 'se', style: { bottom: '-6px', right: '-6px' }, cursor: 'cursor-se-resize' },
  { position: 's', style: { bottom: '-6px', left: '50%', transform: 'translateX(-50%)' }, cursor: 'cursor-s-resize' },
  { position: 'sw', style: { bottom: '-6px', left: '-6px' }, cursor: 'cursor-sw-resize' },
  { position: 'w', style: { top: '50%', left: '-6px', transform: 'translateY(-50%)' }, cursor: 'cursor-w-resize' }
]

const getElementStyle = () => {
  const baseStyle: any = {}

  if (props.element.type === 'text') {
    baseStyle.whiteSpace = 'pre-wrap'
    baseStyle.wordBreak = 'break-word'
  }

  return baseStyle
}

const handleMouseDown = (e: MouseEvent) => {
  if (props.element.locked) return

  isDragging.value = true
  startX.value = e.clientX - props.element.x * (props.zoom / 100)
  startY.value = e.clientY - props.element.y * (props.zoom / 100)

  const handleMouseMove = (e: MouseEvent) => {
    if (!isDragging.value) return

    const x = (e.clientX - startX.value) / (props.zoom / 100)
    const y = (e.clientY - startY.value) / (props.zoom / 100)

    emit('update', { x, y })
  }

  const handleMouseUp = () => {
    isDragging.value = false
    document.removeEventListener('mousemove', handleMouseMove)
    document.removeEventListener('mouseup', handleMouseUp)
  }

  document.addEventListener('mousemove', handleMouseMove)
  document.addEventListener('mouseup', handleMouseUp)
}

const handleResizeStart = (e: MouseEvent, position: string) => {
  if (props.element.locked) return

  isResizing.value = true
  resizePosition.value = position
  startX.value = e.clientX
  startY.value = e.clientY
  startWidth.value = props.element.width
  startHeight.value = props.element.height

  const handleMouseMove = (e: MouseEvent) => {
    if (!isResizing.value) return

    const deltaX = (e.clientX - startX.value) / (props.zoom / 100)
    const deltaY = (e.clientY - startY.value) / (props.zoom / 100)

    let width = startWidth.value
    let height = startHeight.value
    let x = props.element.x
    let y = props.element.y

    if (position.includes('e')) width = Math.max(20, startWidth.value + deltaX)
    if (position.includes('w')) {
      width = Math.max(20, startWidth.value - deltaX)
      x = props.element.x + (startWidth.value - width)
    }
    if (position.includes('s')) height = Math.max(20, startHeight.value + deltaY)
    if (position.includes('n')) {
      height = Math.max(20, startHeight.value - deltaY)
      y = props.element.y + (startHeight.value - height)
    }

    emit('update', { width, height, x, y })
  }

  const handleMouseUp = () => {
    isResizing.value = false
    document.removeEventListener('mousemove', handleMouseMove)
    document.removeEventListener('mouseup', handleMouseUp)
  }

  document.addEventListener('mousemove', handleMouseMove)
  document.addEventListener('mouseup', handleMouseUp)
}

const handleRotateStart = (e: MouseEvent) => {
  if (props.element.locked || !elementRef.value) return

  isRotating.value = true
  const rect = elementRef.value.getBoundingClientRect()
  const centerX = rect.left + rect.width / 2
  const centerY = rect.top + rect.height / 2
  startRotation.value = props.element.rotation

  const getAngle = (clientX: number, clientY: number) => {
    const dx = clientX - centerX
    const dy = clientY - centerY
    return Math.atan2(dy, dx) * (180 / Math.PI) + 90
  }

  const startAngle = getAngle(e.clientX, e.clientY)

  const handleMouseMove = (e: MouseEvent) => {
    if (!isRotating.value) return

    const currentAngle = getAngle(e.clientX, e.clientY)
    const rotation = startRotation.value + (currentAngle - startAngle)

    emit('update', { rotation: rotation % 360 })
  }

  const handleMouseUp = () => {
    isRotating.value = false
    document.removeEventListener('mousemove', handleMouseMove)
    document.removeEventListener('mouseup', handleMouseUp)
  }

  document.addEventListener('mousemove', handleMouseMove)
  document.addEventListener('mouseup', handleMouseUp)
}

const handleTextInput = (e: Event) => {
  const target = e.target as HTMLElement
  emit('update', { text: target.innerText })
}

const handleTextBlur = () => {
  // Save text changes
}
</script>
