<template>
  <div class="min-h-screen bg-neutral-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="absolute top-6 right-6">
      <LanguageSwitcher />
    </div>

    <div class="max-w-md w-full">
      <div class="bg-white rounded-2xl shadow-large p-8 sm:p-10">
        <div class="text-center mb-8">
          <div class="inline-flex items-center justify-center w-16 h-16 bg-primary-100 rounded-xl mb-4">
            <svg class="w-8 h-8 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
            </svg>
          </div>
          <h2 class="text-3xl font-bold text-neutral-900">{{ $t('common.appName') }}</h2>
          <p class="mt-2 text-neutral-600 font-medium">{{ $t('auth.loginTitle') }}</p>
          <p class="text-sm text-neutral-500 mt-1">{{ $t('auth.loginSubtitle') }}</p>
        </div>

        <form @submit.prevent="handleLogin" class="space-y-5">
          <div>
            <label for="email" class="block text-sm font-medium text-neutral-700 mb-2">{{ $t('common.email') }}</label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              :placeholder="$t('auth.emailPlaceholder')"
              class="block w-full px-4 py-2.5 border border-neutral-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all duration-200 hover:border-neutral-400"
            />
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-neutral-700 mb-2">{{ $t('common.password') }}</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              :placeholder="$t('auth.passwordPlaceholder')"
              class="block w-full px-4 py-2.5 border border-neutral-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all duration-200 hover:border-neutral-400"
            />
          </div>

          <div v-if="error" class="bg-primary-50 border border-primary-200 text-primary-700 px-4 py-3 rounded-lg text-sm">
            {{ error }}
          </div>

          <button
            type="submit"
            :disabled="loading"
            class="w-full flex justify-center items-center gap-2 py-3 px-4 border border-transparent rounded-lg shadow-md text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 hover:shadow-lg focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 active:scale-[0.98]"
          >
            <svg v-if="loading" class="animate-spin h-5 w-5" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            {{ loading ? $t('common.loading') : $t('common.login') }}
          </button>
        </form>

        <div class="mt-6 text-center">
          <p class="text-sm text-neutral-600">
            {{ $t('auth.noAccount') }}
            <NuxtLink to="/register" class="font-semibold text-primary-600 hover:text-primary-700 transition-colors">
              {{ $t('auth.signUpHere') }}
            </NuxtLink>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

definePageMeta({
  layout: false,
  middleware: ['guest'],
});

const authStore = useAuthStore();
const router = useRouter();
const { t } = useI18n();

const form = ref({
  email: '',
  password: '',
});

const loading = ref(false);
const error = ref('');

const handleLogin = async () => {
  loading.value = true;
  error.value = '';

  try {
    await authStore.login(form.value.email, form.value.password);
    redirectAfterAuth();
  } catch (err: any) {
    error.value = err.message || t('auth.loginError');
  } finally {
    loading.value = false;
  }
};

const redirectAfterAuth = () => {
  if (authStore.user?.role === 'Admin') {
    router.push('/admin');
    return;
  }

  router.push('/dashboard');
};

onMounted(() => {
  authStore.loadFromStorage();
  if (authStore.isAuthenticated) {
    redirectAfterAuth();
  }
});
</script>
