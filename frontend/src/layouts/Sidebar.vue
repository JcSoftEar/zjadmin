<template>
  <el-menu
    :default-active="route.path"
    :collapse="appStore.sidebarCollapsed"
    :router="true"
    class="sidebar-menu"
    background-color="#304156"
    text-color="#bfcbd9"
    active-text-color="#409EFF"
  >
    <template v-for="item in menuItems" :key="item.path">
      <el-menu-item v-if="!item.children?.length" :index="item.path">
        <el-icon><component :is="item.meta?.icon" /></el-icon>
        <template #title>{{ item.meta?.title }}</template>
      </el-menu-item>
      <el-sub-menu v-else :index="item.path">
        <template #title>
          <el-icon><component :is="item.meta?.icon" /></el-icon>
          <span>{{ item.meta?.title }}</span>
        </template>
        <el-menu-item v-for="child in item.children" :key="child.path" :index="child.path">
          <el-icon><component :is="child.meta?.icon" /></el-icon>
          <template #title>{{ child.meta?.title }}</template>
        </el-menu-item>
      </el-sub-menu>
    </template>
  </el-menu>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useAppStore } from '../stores/app'
import { useAuthStore } from '../stores/auth'

const route = useRoute()
const appStore = useAppStore()
const authStore = useAuthStore()

const menuItems = computed(() => {
  const allItems = [
    { path: '/dashboard', meta: { title: '首页', icon: 'HomeFilled', permission: null } },
    {
      path: '/system', meta: { title: '系统管理', icon: 'Setting' },
      children: [
        { path: '/system/users', meta: { title: '用户管理', icon: 'User', permission: 'system:user:query' } },
        { path: '/system/roles', meta: { title: '角色管理', icon: 'Avatar', permission: 'system:role:query' } },
        { path: '/system/permissions', meta: { title: '权限管理', icon: 'Lock', permission: 'system:permission:query' } },
        { path: '/system/settings', meta: { title: '系统设置', icon: 'Tools', permission: 'system:config:query' } }
      ]
    },
    {
      path: '/logs', meta: { title: '日志管理', icon: 'Document' },
      children: [
        { path: '/logs/operation', meta: { title: '操作日志', icon: 'Edit', permission: 'logs:operation:query' } },
        { path: '/logs/exception', meta: { title: '异常日志', icon: 'Warning', permission: 'logs:exception:query' } }
      ]
    },
    { path: '/profile', meta: { title: '个人中心', icon: 'UserFilled', permission: null } }
  ]

  return filterMenu(allItems)
})

function filterMenu(items) {
  return items.filter(item => {
    if (item.children) {
      item.children = filterMenu(item.children)
      return item.children.length > 0
    }
    if (!item.meta?.permission) return true
    return authStore.hasPermission(item.meta.permission)
  })
}
</script>

<style scoped>
.sidebar-menu {
  height: 100%;
  border-right: none;
  overflow-y: auto;
  overflow-x: hidden;
}
</style>
