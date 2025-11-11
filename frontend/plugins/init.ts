export default defineNuxtPlugin(() => {
  const authStore = useAuthStore();

  // Load auth from storage on app init
  if (process.client) {
    authStore.loadFromStorage();
  }
});
