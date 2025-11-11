<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="absolute top-4 right-4">
      <LanguageSwitcher />
    </div>

    <div class="max-w-md w-full">
      <div class="bg-white rounded-lg shadow-xl p-8">
        <div class="text-center mb-8">
          <h2 class="text-3xl font-bold text-gray-900">{{ $t('common.appName') }}</h2>
          <p class="mt-2 text-gray-600">{{ $t('auth.loginTitle') }}</p>
          <p class="text-sm text-gray-500">{{ $t('auth.loginSubtitle') }}</p>
        </div>

        <form @submit.prevent="handleLogin" class="space-y-6">
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700">{{ $t('common.email') }}</label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              :placeholder="$t('auth.emailPlaceholder')"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-gray-700">{{ $t('common.password') }}</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              :placeholder="$t('auth.passwordPlaceholder')"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>

          <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
            {{ error }}
          </div>

          <button
            type="submit"
            :disabled="loading"
            class="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
          >
            {{ loading ? $t('common.loading') : $t('common.login') }}
          </button>
        </form>

        <div class="mt-6 text-center">
          <p class="text-sm text-gray-600">
            {{ $t('auth.noAccount') }}
            <NuxtLink to="/register" class="font-medium text-indigo-600 hover:text-indigo-500">
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
