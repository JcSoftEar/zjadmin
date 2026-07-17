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
  <el-sub-menu v-if="hasChildren" :index="item.path || item.id">
    <template #title>
      <el-icon v-if="item.icon"><component :is="item.icon" /></el-icon>
      <span>{{ item.name }}</span>
    </template>
    <SidebarMenuItem
      v-for="child in item.children"
      :key="child.path || child.id"
      :item="child"
    />
  </el-sub-menu>
  <el-menu-item v-else :index="item.path">
    <el-icon v-if="item.icon"><component :is="item.icon" /></el-icon>
    <template #title>{{ item.name }}</template>
  </el-menu-item>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  item: { type: Object, required: true }
})

const hasChildren = computed(() => props.item.children?.length > 0)
</script>
