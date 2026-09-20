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
      <div class="footer" v-if="appStore.config.site_copyright || appStore.config.site_icp">
        <span>{{ appStore.config.site_copyright }}</span>
        <a href="https://github.com/JcSoftEar/zjadmin" target="_blank">GitHub</a>
        <a v-if="appStore.config.site_icp" href="https://beian.miit.gov.cn/" target="_blank">
          {{ appStore.config.site_icp }}
        </a>
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
    await authStore.fetchMenus()
  }
  appStore.fetchConfig()
})
</script>

<style scoped>
.app-layout {
  display: flex;
  height: 100vh;
  overflow: hidden;
}

.sidebar {
  width: var(--sidebar-width);
  background: var(--c-sidebar-bg);
  background-image: var(--c-sidebar-bg-gradient);
  transition: width 0.3s;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  border-right: 1px solid var(--c-sidebar-border);
}

.sidebar.collapsed {
  width: var(--sidebar-collapsed-width);
}

.logo {
  height: var(--navbar-height);
  display: flex;
  align-items: center;
  justify-content: center;
  border-bottom: 1px solid var(--c-sidebar-border);
  background: var(--c-sidebar-logo-bg);
}

.logo-text {
  font-size: var(--font-size-xl);
  font-weight: bold;
  color: var(--c-text-inverse);
}

.logo-text-small {
  font-size: var(--font-size-md);
  font-weight: bold;
  color: var(--c-text-inverse);
}

.main-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  background: var(--c-bg);
}

.main-content {
  flex: 1;
  padding: var(--content-padding);
  overflow-y: auto;
}

.footer {
  text-align: center;
  padding: 12px 20px;
  font-size: var(--font-size-xs);
  color: var(--c-text-3);
  background: var(--c-surface);
  border-top: 1px solid var(--c-line);
  display: flex;
  justify-content: center;
  gap: 16px;
}

.footer a {
  color: var(--c-text-3);
  text-decoration: none;
}

.footer a:hover {
  color: var(--c-primary);
}
</style>
