<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-header">
        <h2>ZJAdmin 最简后台</h2>
        <p>轻量级后台管理系统</p>
      </div>
      <el-form ref="formRef" :model="form" :rules="rules" class="login-form">
        <el-form-item prop="username">
          <el-input v-model="form.username" placeholder="用户名" :prefix-icon="User" size="large" />
        </el-form-item>
        <el-form-item prop="password">
          <el-input v-model="form.password" type="password" placeholder="密码" :prefix-icon="Lock" size="large" show-password />
        </el-form-item>
        <el-form-item>
          <el-checkbox v-model="remember">记住密码</el-checkbox>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" :loading="loading" size="large" class="login-btn" @click="handleLogin">
            登 录
          </el-button>
        </el-form-item>
      </el-form>
      <div class="login-footer" v-if="appStore.config.site_copyright || appStore.config.site_icp">
        <span class="copyright">{{ appStore.config.site_copyright }}</span>
        <a class="github" href="https://github.com/JcSoftEar/zjadmin" target="_blank">GitHub: github.com/JcSoftEar/zjadmin</a>
        <a v-if="appStore.config.site_icp" class="icp" href="https://beian.miit.gov.cn/" target="_blank">
          {{ appStore.config.site_icp }}
        </a>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'
import { useAppStore } from '../../stores/app'
import { User, Lock } from '@element-plus/icons-vue'

const router = useRouter()
const authStore = useAuthStore()
const appStore = useAppStore()
const formRef = ref(null)
const loading = ref(false)
const remember = ref(false)

const form = reactive({
  username: '',
  password: ''
})

const rules = {
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
}

onMounted(() => {
  appStore.fetchConfig()
  const saved = localStorage.getItem('rememberedUser')
  if (saved) {
    const data = JSON.parse(saved)
    form.username = data.username
    form.password = data.password
    remember.value = true
  }
})

async function handleLogin() {
  const valid = await formRef.value.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    await authStore.login({ username: form.username, password: form.password })

    if (remember.value) {
      localStorage.setItem('rememberedUser', JSON.stringify({ username: form.username, password: form.password }))
    } else {
      localStorage.removeItem('rememberedUser')
    }

    router.push('/dashboard')
  } catch {
    // error handled by interceptor
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-page {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-card {
  width: 420px;
  padding: 40px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.15);
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-header h2 {
  font-size: 24px;
  color: #303133;
  margin-bottom: 8px;
}

.login-header p {
  font-size: 14px;
  color: #909399;
}

.login-form {
  margin-top: 20px;
}

.login-btn {
  width: 100%;
}

.login-footer {
  text-align: center;
  margin-top: 24px;
  font-size: 12px;
  color: #909399;
  line-height: 1.8;
}

.login-footer .copyright {
  display: block;
}

.login-footer .icp {
  color: #909399;
  text-decoration: none;
}

.login-footer .icp:hover {
  color: #409eff;
}

.login-footer .github {
  display: block;
  color: #909399;
  text-decoration: none;
}

.login-footer .github:hover {
  color: #409eff;
}
</style>
