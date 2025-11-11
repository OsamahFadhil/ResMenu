export default defineNuxtRouteMiddleware((to, from) => {
  const authStore = useAuthStore()

  // Load auth state from storage if not already loaded
  authStore.loadFromStorage()

  // Redirect authenticated users away from guest pages
  if (authStore.isAuthenticated) {
    if (authStore.user?.role === 'Admin') {
      return navigateTo('/admin')
    }
    return navigateTo('/dashboard')
  }
})
