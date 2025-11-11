import en from '@/locales/en.json'
import ar from '@/locales/ar.json'

export default defineNuxtPlugin((nuxtApp) => {
  const i18n = nuxtApp.$i18n as any

  if (!i18n) {
    return
  }

  if (!i18n.getLocaleMessage('en') || Object.keys(i18n.getLocaleMessage('en') || {}).length === 0) {
    i18n.setLocaleMessage('en', en)
  }

  if (!i18n.getLocaleMessage('ar') || Object.keys(i18n.getLocaleMessage('ar') || {}).length === 0) {
    i18n.setLocaleMessage('ar', ar)
  }
})

