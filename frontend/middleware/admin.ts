export default defineNuxtRouteMiddleware((to, from) => {
  const authStore = useAuthStore()

  // Load auth state from storage if not already loaded
  if (!authStore.isAuthenticated) {
    authStore.loadFromStorage()
  }

  // Check if user is authenticated
  if (!authStore.isAuthenticated) {
    return navigateTo('/login')
  }

  // Check if user is admin
  if (authStore.user?.role !== 'Admin') {
    return navigateTo('/dashboard')
  }
})
