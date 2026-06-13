<template>
  <div class="dashboard">
    <div class="welcome-card card">
      <h2>欢迎回来，{{ authStore.userInfo?.nickname || authStore.userInfo?.username }}</h2>
      <p>今天也是充满活力的一天！</p>
    </div>

    <el-row :gutter="20" class="stats-row">
      <el-col :span="6" v-for="stat in stats" :key="stat.label">
        <div class="stat-card card" :style="{ borderLeft: `4px solid ${stat.color}` }">
          <div class="stat-value">{{ stat.value }}</div>
          <div class="stat-label">{{ stat.label }}</div>
        </div>
      </el-col>
    </el-row>

    <el-row :gutter="20">
      <el-col :span="12">
        <div class="card">
          <h3 class="card-title">系统信息</h3>
          <el-descriptions :column="1" border>
            <el-descriptions-item label="系统名称">ZJAdmin 最简后台</el-descriptions-item>
            <el-descriptions-item label="系统版本">v1.0.0</el-descriptions-item>
            <el-descriptions-item label="后端框架">.NET Core 8</el-descriptions-item>
            <el-descriptions-item label="前端框架">Vue 3 + Element Plus</el-descriptions-item>
            <el-descriptions-item label="数据库">SQLite</el-descriptions-item>
          </el-descriptions>
        </div>
      </el-col>
      <el-col :span="12">
        <div class="card">
          <h3 class="card-title">快捷操作</h3>
          <div class="quick-actions">
            <el-button v-permission="'system:user:query'" @click="$router.push('/system/users')">用户管理</el-button>
            <el-button v-permission="'system:role:query'" @click="$router.push('/system/roles')">角色管理</el-button>
            <el-button v-permission="'system:permission:query'" @click="$router.push('/system/permissions')">权限管理</el-button>
            <el-button @click="$router.push('/profile')">个人中心</el-button>
          </div>
        </div>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useAuthStore } from '../../stores/auth'

const authStore = useAuthStore()

const stats = ref([
  { label: '系统版本', value: 'v1.0.0', color: '#409EFF' },
  { label: '技术栈', value: '.NET 8 + Vue 3', color: '#67C23A' },
  { label: 'UI 框架', value: 'Element Plus', color: '#E6A23C' },
  { label: '数据库', value: 'SQLite', color: '#F56C6C' }
])
</script>

<style scoped>
.welcome-card {
  margin-bottom: 20px;
}

.welcome-card h2 {
  font-size: 22px;
  margin-bottom: 8px;
}

.welcome-card p {
  color: #909399;
}

.stats-row {
  margin-bottom: 20px;
}

.stat-card {
  text-align: center;
  padding: 20px;
}

.stat-value {
  font-size: 28px;
  font-weight: bold;
  color: #303133;
  margin-bottom: 8px;
}

.stat-label {
  font-size: 14px;
  color: #909399;
}

.card-title {
  font-size: 16px;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid #ebeef5;
}

.quick-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}
</style>
