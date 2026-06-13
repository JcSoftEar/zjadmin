<template>
  <div class="app-layout">
    <div class="sidebar" :class="{ collapsed: appStore.sidebarCollapsed }">
      <div class="logo">
        <span v-if="!appStore.sidebarCollapsed" class="logo-text">ZJAdmin</span>
        <span v-else class="logo-text-small">ZJ</span>
      </div>
      <Sidebar />
    </div>
    <div class="main-container">
      <Navbar />
      <div class="main-content">
        <router-view />
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import { useAppStore } from '../stores/app'
import Navbar from './Navbar.vue'
import Sidebar from './Sidebar.vue'

const authStore = useAuthStore()
const appStore = useAppStore()

onMounted(async () => {
  if (authStore.token) {
    await authStore.fetchProfile()
  }
})
</script>

<style scoped>
.app-layout {
  display: flex;
  height: 100vh;
  overflow: hidden;
}

.sidebar {
  width: 240px;
  background: #304156;
  transition: width 0.3s;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.sidebar.collapsed {
  width: 64px;
}

.logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.logo-text {
  font-size: 20px;
  font-weight: bold;
  color: #fff;
}

.logo-text-small {
  font-size: 16px;
  font-weight: bold;
  color: #fff;
}

.main-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  background: #f5f7fa;
}

.main-content {
  flex: 1;
  padding: 20px;
  overflow-y: auto;
}
</style>
