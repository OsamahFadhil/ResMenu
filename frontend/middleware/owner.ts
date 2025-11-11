export default defineNuxtRouteMiddleware(() => {
  const authStore = useAuthStore();

  if (!authStore.isAuthenticated) {
    authStore.loadFromStorage();
  }

  if (!authStore.isAuthenticated) {
    return navigateTo('/login');
  }

  if (authStore.user?.role !== 'RestaurantOwner') {
    if (authStore.user?.role === 'Admin') {
      return navigateTo('/admin');
    }

    return navigateTo('/');
  }

  if (!authStore.restaurantId) {
    return navigateTo('/dashboard/settings');
  }
});

