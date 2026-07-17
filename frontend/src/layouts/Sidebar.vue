/**
 * ____________/\\\____        ______/\\\_        __/\\\\\\\\\\\\\\\_
 * __________/\\\\\____        __/\\\\\\\_        _\/////////////\\\_
 *  ________/\\\/\\\____        _\/////\\\_        ____________/\\\/__
 *   ______/\\\/\/\\\____        _____\/\\\_        __________/\\\/____
 *    ____/\\\/__\/\\\____        _____\/\\\_        ________/\\\/______
 *     __/\\\\\\\\\\\\\\\\_        _____\/\\\_        ______/\\\/________
 *      _\///////////\\\//__        _____\/\\\_        ____/\\\/__________
 *       ___________\/\\\____        _____\/\\\_        __/\\\/____________
 *        ___________\///_____        _____\///_         _\///______________
 *
 * @Author  : James YinG
 * @Email   : james@taogame.com
 * 业务：软件开发 | 功能定制 | Bug修复 | 项目部署
 * 专业接单，品质保障，欢迎合作！
 */

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
    <!-- 首页 - 始终显示 -->
    <el-menu-item index="/dashboard">
      <el-icon><HomeFilled /></el-icon>
      <template #title>首页</template>
    </el-menu-item>

    <!-- 动态菜单 - 根据权限加载 -->
    <template v-for="item in authStore.menus" :key="item.path">
      <SidebarMenuItem
        v-if="item.path !== '/dashboard'"
        :item="item"
      />
    </template>

    <!-- 个人中心 - 始终显示 -->
    <el-menu-item index="/profile">
      <el-icon><UserFilled /></el-icon>
      <template #title>个人中心</template>
    </el-menu-item>
  </el-menu>
</template>

<script setup>
import { useRoute } from 'vue-router'
import { useAppStore } from '../stores/app'
import { useAuthStore } from '../stores/auth'
import SidebarMenuItem from './SidebarMenuItem.vue'

const route = useRoute()
const appStore = useAppStore()
const authStore = useAuthStore()
</script>

<style scoped>
.sidebar-menu {
  height: 100%;
  border-right: none;
  overflow-y: auto;
  overflow-x: hidden;
}
</style>
