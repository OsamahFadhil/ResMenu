import { defineStore } from 'pinia';

interface User {
  id: string;
  name: string;
  email: string;
  role: string;
  restaurantId?: string | null;
}

interface AuthState {
  user: User | null;
  token: string | null;
  refreshToken: string | null;
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    user: null,
    token: null,
    refreshToken: null,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    isRestaurantOwner: (state) => state.user?.role === 'RestaurantOwner',
    isAdmin: (state) => state.user?.role === 'Admin',
    restaurantId: (state) => state.user?.restaurantId ?? null,
  },

  actions: {
    setAuth(data: { user: User; token: string; refreshToken: string }) {
      this.user = data.user;
      this.token = data.token;
      this.refreshToken = data.refreshToken;

      // Store in localStorage
      if (process.client) {
        localStorage.setItem('auth', JSON.stringify(data));
      }
    },

    logout() {
      this.user = null;
      this.token = null;
      this.refreshToken = null;

      if (process.client) {
        localStorage.removeItem('auth');
      }
    },

    loadFromStorage() {
      if (process.client) {
        const stored = localStorage.getItem('auth');
        if (stored) {
          const data = JSON.parse(stored);
          this.user = data.user;
          this.token = data.token;
          this.refreshToken = data.refreshToken;
        }
      }
    },

    async register(registerData: {
      name: string;
      email: string;
      password: string;
      restaurantName: string;
      contactPhone?: string;
    }) {
      const api = useApi();
      const response = await api.post('/auth/register', registerData);

      if (response.data.success) {
        this.setAuth(response.data.data);
        return response.data;
      }

      throw new Error(response.data.message || 'Registration failed');
    },

    async login(email: string, password: string) {
      const api = useApi();
      const response = await api.post('/auth/login', { email, password });

      if (response.data.success) {
        this.setAuth(response.data.data);
        return response.data;
      }

      throw new Error(response.data.message || 'Login failed');
    },
  },
});
