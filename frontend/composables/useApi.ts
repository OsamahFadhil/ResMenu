import axios from 'axios';

export const useApi = () => {
  const config = useRuntimeConfig();
  const authStore = useAuthStore();

  const api = axios.create({
    baseURL: config.public.apiBase as string,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  // Request interceptor to add auth token
  api.interceptors.request.use(
    (config) => {
      const token = authStore.token;
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => {
      return Promise.reject(error);
    }
  );

  // Response interceptor for error handling
  api.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401) {
        authStore.logout();
        navigateTo('/login');
      }
      return Promise.reject(error);
    }
  );

  return api;
};
