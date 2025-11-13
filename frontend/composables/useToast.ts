import { useToastStore } from '@/stores/toast'

export const useToast = () => {
  const toastStore = useToastStore()

  return {
    success: (message: string, title?: string, duration?: number) => {
      toastStore.success(message, title, duration)
    },
    error: (message: string, title?: string, duration?: number) => {
      toastStore.error(message, title, duration)
    },
    warning: (message: string, title?: string, duration?: number) => {
      toastStore.warning(message, title, duration)
    },
    info: (message: string, title?: string, duration?: number) => {
      toastStore.info(message, title, duration)
    },
    clearAll: () => {
      toastStore.clearAll()
    }
  }
}
