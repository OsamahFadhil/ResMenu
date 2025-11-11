import axios from 'axios';

export const useApi = () => {
  const config = useRuntimeConfig();
  const authStore = useAuthStore();
  const nuxtApp = useNuxtApp();

  const api = axios.create({
    baseURL: config.public.apiBase as string,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  api.interceptors.request.use(
    (requestConfig) => {
      const token = authStore.token;
      const activeLocale =
        nuxtApp.$i18n?.locale?.value ??
        (typeof nuxtApp.$i18n?.locale === 'string' ? nuxtApp.$i18n?.locale : undefined);

      if (token) {
        if (typeof requestConfig.headers?.set === 'function') {
          requestConfig.headers.set('Authorization', `Bearer ${token}`);
        } else {
          requestConfig.headers = {
            ...(requestConfig.headers ?? {}),
            Authorization: `Bearer ${token}`,
          };
        }
      }

      if (activeLocale) {
        if (typeof requestConfig.headers?.set === 'function') {
          requestConfig.headers.set('Accept-Language', activeLocale);
        } else {
          requestConfig.headers = {
            ...(requestConfig.headers ?? {}),
            'Accept-Language': activeLocale,
          };
        }
      }

      return requestConfig;
    },
    (error) => Promise.reject(error)
  );

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
