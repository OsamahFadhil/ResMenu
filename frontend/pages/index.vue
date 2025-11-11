<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
    <!-- Navigation -->
    <nav class="bg-white shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <h1 class="text-2xl font-bold text-indigo-600">Menufy</h1>
          </div>
          <div class="flex items-center space-x-4">
            <NuxtLink to="/login" class="text-gray-700 hover:text-indigo-600 px-3 py-2 rounded-md text-sm font-medium">
              Login
            </NuxtLink>
            <NuxtLink to="/register" class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-md text-sm font-medium">
              Get Started
            </NuxtLink>
          </div>
        </div>
      </div>
    </nav>

    <!-- Hero Section -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
      <div class="text-center">
        <h2 class="text-5xl font-extrabold text-gray-900 mb-6">
          Digital Menus Made Simple
        </h2>
        <p class="text-xl text-gray-600 mb-8 max-w-2xl mx-auto">
          Create beautiful digital menus for your restaurant. Share them instantly with QR codes. No app required for customers.
        </p>
        <div class="flex justify-center space-x-4">
          <NuxtLink to="/register" class="bg-indigo-600 hover:bg-indigo-700 text-white px-8 py-3 rounded-lg text-lg font-semibold shadow-lg transform hover:scale-105 transition">
            Start Free Trial
          </NuxtLink>
          <button class="bg-white hover:bg-gray-50 text-indigo-600 px-8 py-3 rounded-lg text-lg font-semibold border-2 border-indigo-600 transform hover:scale-105 transition">
            View Demo
          </button>
        </div>
      </div>

      <!-- Features -->
      <div class="mt-24 grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="bg-white p-8 rounded-xl shadow-lg">
          <div class="text-indigo-600 text-4xl mb-4">📱</div>
          <h3 class="text-xl font-bold text-gray-900 mb-2">QR Code Menus</h3>
          <p class="text-gray-600">Generate QR codes that customers can scan to view your menu instantly.</p>
        </div>

        <div class="bg-white p-8 rounded-xl shadow-lg">
          <div class="text-indigo-600 text-4xl mb-4">✏️</div>
          <h3 class="text-xl font-bold text-gray-900 mb-2">Easy Management</h3>
          <p class="text-gray-600">Update your menu items, prices, and photos in real-time from any device.</p>
        </div>

        <div class="bg-white p-8 rounded-xl shadow-lg">
          <div class="text-indigo-600 text-4xl mb-4">🎨</div>
          <h3 class="text-xl font-bold text-gray-900 mb-2">Beautiful Design</h3>
          <p class="text-gray-600">Professional, mobile-friendly menus that make your dishes look amazing.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
</script>
