import { ref } from 'vue'

export const useDragDrop = <T extends { id: string; displayOrder: number }>(
  items: T[],
  onReorder: (reorderedItems: T[]) => void
) => {
  const draggedItem = ref<T | null>(null)
  const draggedOverIndex = ref<number | null>(null)

  const handleDragStart = (item: T) => {
    draggedItem.value = item
  }

  const handleDragOver = (index: number, event: DragEvent) => {
    event.preventDefault()
    draggedOverIndex.value = index
  }

  const handleDragLeave = () => {
    draggedOverIndex.value = null
  }

  const handleDrop = (targetIndex: number) => {
    if (!draggedItem.value) return

    const draggedIndex = items.findIndex(item => item.id === draggedItem.value!.id)
    if (draggedIndex === targetIndex) {
      draggedItem.value = null
      draggedOverIndex.value = null
      return
    }

    // Reorder the array
    const reordered = [...items]
    const [removed] = reordered.splice(draggedIndex, 1)
    reordered.splice(targetIndex, 0, removed)

    // Update display orders
    const updated = reordered.map((item, index) => ({
      ...item,
      displayOrder: index + 1
    }))

    onReorder(updated as T[])

    draggedItem.value = null
    draggedOverIndex.value = null
  }

  const handleDragEnd = () => {
    draggedItem.value = null
    draggedOverIndex.value = null
  }

  return {
    draggedItem,
    draggedOverIndex,
    handleDragStart,
    handleDragOver,
    handleDragLeave,
    handleDrop,
    handleDragEnd
  }
}
