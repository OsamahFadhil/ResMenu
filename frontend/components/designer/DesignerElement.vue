<template>
  <div
    v-if="element.visible"
    ref="elementRef"
    :style="getContainerStyle()"
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
        :style="getTextStyle()"
      >
        {{ element.text }}
      </div>

      <!-- Image Element -->
      <img
        v-else-if="element.type === 'image'"
        :src="element.imageUrl"
        :style="getImageStyle()"
        draggable="false"
      />
      
      <!-- Gradient Element -->
      <div
        v-else-if="element.type === 'gradient'"
        :style="getGradientStyle()"
      ></div>

      <!-- Shape Element -->
      <div
        v-else-if="element.type === 'shape'"
        :style="getShapeStyle()"
      ></div>

      <!-- Menu Item Element -->
      <div
        v-else-if="element.type === 'menuItem' || element.type === 'advancedCard'"
        :class="getMenuItemClass()"
        :style="getMenuItemStyle()"
      >
        <div
          v-if="element.itemImage"
          class="flex-shrink-0 overflow-hidden"
          :style="getItemImageStyle()"
        >
          <img :src="element.itemImage" class="w-full h-full object-cover" />
        </div>
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-2 mb-1">
            <div
              class="font-semibold truncate"
              :style="getItemNameStyle()"
            >
              {{ element.itemName }}
            </div>
            <span
              v-if="element.itemBadge"
              class="px-2 py-0.5 text-xs font-bold rounded-full"
              :style="getBadgeStyle()"
            >
              {{ element.itemBadge }}
            </span>
          </div>
          <div
            v-if="element.itemTag"
            class="text-xs mb-1"
            :style="getTagStyle()"
          >
            {{ element.itemTag }}
          </div>
          <div
            v-if="element.itemDescription"
            class="truncate"
            :style="getItemDescriptionStyle()"
          >
            {{ element.itemDescription }}
          </div>
        </div>
        <div
          class="font-bold whitespace-nowrap"
          :style="getPriceStyle()"
        >
          ${{ element.itemPrice?.toFixed(2) }}
        </div>
        <div
          v-if="element.showDivider"
          class="absolute bottom-0 left-0 right-0 h-px"
          :style="getDividerStyle()"
        ></div>
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

const getContainerStyle = () => {
  const el = props.element
  const transforms: string[] = []
  
  if (el.rotation) transforms.push(`rotate(${el.rotation}deg)`)
  if (el.scale !== undefined) transforms.push(`scale(${el.scale})`)
  if (el.skewX) transforms.push(`skewX(${el.skewX}deg)`)
  if (el.skewY) transforms.push(`skewY(${el.skewY}deg)`)
  
  const style: any = {
    position: 'absolute',
    left: `${el.x * (props.zoom / 100)}px`,
    top: `${el.y * (props.zoom / 100)}px`,
    width: `${el.width * (props.zoom / 100)}px`,
    height: `${el.height * (props.zoom / 100)}px`,
    transformOrigin: el.transformOrigin || 'center',
    cursor: el.locked ? 'not-allowed' : 'move',
    zIndex: el.zIndex
  }
  
  if (transforms.length > 0) {
    style.transform = transforms.join(' ')
  }
  
  // Apply filters
  const filters: string[] = []
  if (el.blur) filters.push(`blur(${el.blur}px)`)
  if (el.brightness !== undefined) filters.push(`brightness(${el.brightness})`)
  if (el.contrast !== undefined) filters.push(`contrast(${el.contrast})`)
  if (el.saturate !== undefined) filters.push(`saturate(${el.saturate})`)
  if (el.hueRotate !== undefined) filters.push(`hue-rotate(${el.hueRotate}deg)`)
  if (el.invert !== undefined) filters.push(`invert(${el.invert})`)
  if (el.sepia !== undefined) filters.push(`sepia(${el.sepia})`)
  if (filters.length > 0) {
    style.filter = filters.join(' ')
  }
  
  if (el.backdropFilter) {
    style.backdropFilter = el.backdropFilter
  }
  
  return style
}

const getElementStyle = () => {
  const baseStyle: any = {}

  if (props.element.type === 'text') {
    baseStyle.whiteSpace = 'pre-wrap'
    baseStyle.wordBreak = 'break-word'
  }

  return baseStyle
}

const getTextStyle = () => {
  const el = props.element
  const style: any = {
    fontSize: `${el.fontSize || 16}px`,
    fontFamily: el.fontFamily || 'Inter',
    fontWeight: el.fontWeight || 'normal',
    color: el.color || '#000000',
    textAlign: el.textAlign || 'left',
    lineHeight: el.lineHeight || 1.5,
    width: '100%',
    height: '100%',
    outline: 'none',
    padding: '4px'
  }
  
  if (el.letterSpacing) style.letterSpacing = `${el.letterSpacing}px`
  if (el.textTransform) style.textTransform = el.textTransform
  if (el.textShadow) style.textShadow = el.textShadow
  if (el.textStroke && el.textStrokeWidth) {
    style.WebkitTextStroke = `${el.textStrokeWidth}px ${el.textStroke}`
  }
  
  return style
}

const getImageStyle = () => {
  const el = props.element
  const filters: string[] = []
  
  if (el.imageBlur) filters.push(`blur(${el.imageBlur}px)`)
  if (el.imageBrightness !== undefined) filters.push(`brightness(${el.imageBrightness})`)
  if (el.imageContrast !== undefined) filters.push(`contrast(${el.imageContrast})`)
  if (el.imageSaturate !== undefined) filters.push(`saturate(${el.imageSaturate})`)
  
  const style: any = {
    width: '100%',
    height: '100%',
    objectFit: el.objectFit || 'cover',
    opacity: el.imageOpacity !== undefined ? el.imageOpacity : 1
  }
  
  if (filters.length > 0) {
    style.filter = filters.join(' ')
  } else if (el.imageFilter) {
    style.filter = el.imageFilter
  }
  
  return style
}

const getShapeStyle = () => {
  const el = props.element
  const style: any = {
    width: '100%',
    height: '100%',
    opacity: el.opacity !== undefined ? el.opacity : 1
  }
  
  // Background
  if (el.backgroundGradient) {
    style.background = el.backgroundGradient
  } else if (el.backgroundColor) {
    style.backgroundColor = el.backgroundColor
  }
  
  if (el.backgroundImage) {
    style.backgroundImage = `url(${el.backgroundImage})`
    style.backgroundSize = 'cover'
    style.backgroundPosition = 'center'
  }
  
  // Border
  if (el.borderWidth) {
    const borderStyle = el.borderStyle || 'solid'
    style.border = `${el.borderWidth}px ${borderStyle} ${el.borderColor || 'transparent'}`
  }
  
  // Border radius
  if (el.shapeType === 'circle' || el.shapeType === 'ellipse') {
    style.borderRadius = '50%'
  } else if (el.borderRadius !== undefined) {
    style.borderRadius = `${el.borderRadius}px`
  }
  
  // Shadows
  if (el.boxShadow) style.boxShadow = el.boxShadow
  if (el.dropShadow) style.filter = `drop-shadow(${el.dropShadow})`
  
  return style
}

const getGradientStyle = () => {
  const el = props.element
  const style: any = {
    width: '100%',
    height: '100%'
  }
  
  if (el.gradientType === 'radial') {
    style.background = `radial-gradient(circle, ${el.gradientColors?.join(', ') || '#000000, #ffffff'})`
  } else if (el.gradientType === 'conic') {
    style.background = `conic-gradient(from ${el.gradientAngle || 0}deg, ${el.gradientColors?.join(', ') || '#000000, #ffffff'})`
  } else {
    const angle = el.gradientAngle || 90
    style.background = `linear-gradient(${angle}deg, ${el.gradientColors?.join(', ') || '#000000, #ffffff'})`
  }
  
  if (el.opacity !== undefined) style.opacity = el.opacity
  if (el.borderRadius !== undefined) style.borderRadius = `${el.borderRadius}px`
  if (el.boxShadow) style.boxShadow = el.boxShadow
  
  return style
}

const getMenuItemClass = () => {
  const el = props.element
  const classes = ['flex', 'items-center', 'gap-3', 'p-3', 'h-full', 'relative']
  
  if (el.type === 'advancedCard') {
    if (el.cardStyle === 'glassmorphism') {
      classes.push('backdrop-blur-md', 'bg-white/10')
    }
  }
  
  return classes.join(' ')
}

const getMenuItemStyle = () => {
  const el = props.element
  const style: any = {}
  
  if (el.type === 'advancedCard') {
    if (el.cardStyle === 'glassmorphism') {
      style.backgroundColor = 'rgba(255, 255, 255, 0.1)'
      style.backdropFilter = 'blur(10px)'
    } else if (el.backgroundColor) {
      style.backgroundColor = el.backgroundColor
    }
    
    if (el.cardStyle === 'elegant') {
      style.borderRadius = '12px'
      style.boxShadow = '0 4px 6px rgba(0, 0, 0, 0.1)'
    } else if (el.cardStyle === 'bold') {
      style.borderRadius = '8px'
      style.boxShadow = '0 8px 16px rgba(0, 0, 0, 0.2)'
    } else if (el.cardStyle === 'minimal') {
      style.borderRadius = '4px'
      style.border = '1px solid rgba(0, 0, 0, 0.1)'
    } else {
      style.borderRadius = el.borderRadius !== undefined ? `${el.borderRadius}px` : '8px'
    }
    
    if (el.cardElevation) {
      style.boxShadow = `0 ${el.cardElevation * 2}px ${el.cardElevation * 4}px rgba(0, 0, 0, 0.${el.cardElevation})`
    }
  } else {
    style.backgroundColor = el.backgroundColor || '#1f2937'
    style.borderRadius = '8px'
  }
  
  if (el.boxShadow) style.boxShadow = el.boxShadow
  
  return style
}

const getItemImageStyle = () => {
  const el = props.element
  const style: any = {
    width: '64px',
    height: '64px',
    borderRadius: '8px'
  }
  
  if (el.cardStyle === 'elegant') {
    style.borderRadius = '12px'
  } else if (el.cardStyle === 'minimal') {
    style.borderRadius = '4px'
  }
  
  return style
}

const getItemNameStyle = () => {
  const el = props.element
  return {
    color: el.color || '#ffffff',
    fontSize: `${el.fontSize || 18}px`,
    fontFamily: el.fontFamily || 'Inter'
  }
}

const getItemDescriptionStyle = () => {
  const el = props.element
  return {
    color: el.color || '#ffffff',
    fontSize: `${(el.fontSize || 18) * 0.875}px`,
    opacity: 0.8
  }
}

const getPriceStyle = () => {
  const el = props.element
  return {
    color: el.color || '#ffffff',
    fontSize: `${el.fontSize || 18}px`,
    fontFamily: el.fontFamily || 'Inter'
  }
}

const getBadgeStyle = () => {
  return {
    backgroundColor: '#dc2626',
    color: '#ffffff'
  }
}

const getTagStyle = () => {
  return {
    color: '#9ca3af',
    fontStyle: 'italic'
  }
}

const getDividerStyle = () => {
  return {
    backgroundColor: 'rgba(255, 255, 255, 0.1)'
  }
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
