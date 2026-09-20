<template>
  <div class="navbar">
    <div class="navbar-left">
      <el-icon class="collapse-btn" @click="appStore.toggleSidebar">
        <Fold v-if="!appStore.sidebarCollapsed" />
        <Expand v-else />
      </el-icon>
      <span class="system-name">ZJAdmin 最简后台</span>
    </div>
    <div class="navbar-right">
      <!-- Theme Switcher -->
      <el-dropdown trigger="click" @command="handleThemeChange">
        <div class="theme-switcher" title="切换主题">
          <el-icon><Monitor /></el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu class="theme-dropdown">
            <div class="theme-dropdown-title">选择主题</div>
            <el-dropdown-item
              v-for="theme in themes"
              :key="theme.id"
              :command="theme.id"
              :class="{ 'is-active': appStore.currentTheme === theme.id }"
            >
              <div class="theme-option">
                <div class="theme-colors">
                  <span class="color-dot" :style="{ background: theme.colors[0] }"></span>
                  <span class="color-dot" :style="{ background: theme.colors[1] }"></span>
                  <span class="color-dot" :style="{ background: theme.colors[2] }"></span>
                </div>
                <span class="theme-name">{{ theme.name }}</span>
                <el-icon v-if="appStore.currentTheme === theme.id" class="check-icon">
                  <Check />
                </el-icon>
              </div>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>

      <el-dropdown trigger="click">
        <div class="user-info">
          <el-avatar :size="32" icon="UserFilled" />
          <span class="username">{{ authStore.userInfo?.nickname || authStore.userInfo?.username }}</span>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item @click="goProfile">个人中心</el-dropdown-item>
            <el-dropdown-item divided @click="handleLogout">退出登录</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useAppStore } from '../stores/app'

const router = useRouter()
const authStore = useAuthStore()
const appStore = useAppStore()

// Theme definitions
const themes = [
  // 系列一 · 经典实用
  { id: 'classic-deep', name: '经典深侧栏', series: '经典实用', colors: ['#001529', '#1677FF', '#F0F2F5'] },
  { id: 'notion-light', name: '极简浅色', series: '经典实用', colors: ['#37352F', '#FBFBFA', '#E9E9E7'] },
  { id: 'dark-glass', name: '深色玻璃拟态', series: '经典实用', colors: ['#0B0F1A', '#6366F1', '#8B5CF6'] },
  // 系列二 · 暗色炫酷
  { id: 'cyber-nebula', name: '赛博星云', series: '暗色炫酷', colors: ['#05070D', '#22D3EE', '#A855F7'] },
  { id: 'aurora-glass', name: '极光玻璃', series: '暗色炫酷', colors: ['#0A0F1C', '#2563EB', '#7C3AED'] },
  { id: 'hud-cockpit', name: 'HUD 驾驶舱', series: '暗色炫酷', colors: ['#040806', '#7DFCB0', '#FF6B6B'] },
  { id: 'neo-pulse', name: '撞色极潮', series: '暗色炫酷', colors: ['#0D0D0F', '#C6FF3D', '#FF2D9B'] },
  // 系列三 · 亮色橙紫
  { id: 'sunset-glow', name: '橙紫渐变', series: '亮色橙紫', colors: ['#FB923C', '#A855F7', '#FFFAF5'] },
  { id: 'violet-bloom', name: '紫罗兰', series: '亮色橙紫', colors: ['#7C3AED', '#D946EF', '#FBFAFF'] },
  { id: 'amber-dash', name: '暖橙能量', series: '亮色橙紫', colors: ['#F97316', '#F59E0B', '#FFFBF7'] },
  { id: 'candy-pop', name: '撞色糖果', series: '亮色橙紫', colors: ['#FB7185', '#38BDF8', '#FFFDFA'] },
]

function goProfile() {
  router.push('/profile')
}

async function handleLogout() {
  await authStore.logout()
  router.push('/login')
}

function handleThemeChange(themeId) {
  appStore.setTheme(themeId)
}
</script>

<style scoped>
.navbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: var(--navbar-height);
  padding: 0 20px;
  background: var(--c-navbar-bg);
  border-bottom: 1px solid var(--c-navbar-border);
  box-shadow: var(--c-navbar-shadow);
}

.navbar-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.collapse-btn {
  font-size: 20px;
  cursor: pointer;
  color: var(--c-text-2);
}

.system-name {
  font-size: var(--font-size-md);
  font-weight: 600;
  color: var(--c-navbar-text);
}

.navbar-right {
  display: flex;
  align-items: center;
  gap: 16px;
}

.theme-switcher {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: var(--radius-sm);
  cursor: pointer;
  color: var(--c-text-2);
  transition: all var(--dur-fast);
}

.theme-switcher:hover {
  background: var(--c-surface-2);
  color: var(--c-primary);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.username {
  font-size: var(--font-size-base);
  color: var(--c-navbar-text);
}
</style>

<style>
/* Theme Dropdown Styles (not scoped) */
.theme-dropdown {
  width: 240px !important;
}

.theme-dropdown-title {
  padding: 8px 16px;
  font-size: 12px;
  color: #909399;
  border-bottom: 1px solid #EBEEF5;
}

.theme-option {
  display: flex;
  align-items: center;
  gap: 10px;
  width: 100%;
}

.theme-colors {
  display: flex;
  gap: 4px;
}

.color-dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  border: 1px solid rgba(0, 0, 0, 0.1);
}

.theme-name {
  flex: 1;
  font-size: 13px;
}

.check-icon {
  color: var(--el-color-primary);
}
</style>
