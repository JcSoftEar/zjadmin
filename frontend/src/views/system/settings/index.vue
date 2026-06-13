<template>
  <div class="page-container">
    <div class="page-header">
      <span class="page-title">系统设置</span>
    </div>

    <el-form ref="formRef" :model="form" :rules="rules" label-width="120px" v-loading="loading">
      <el-form-item label="网站标题" prop="site_title">
        <el-input v-model="form.site_title" placeholder="网站标题" maxlength="100" show-word-limit />
      </el-form-item>

      <el-form-item label="网站关键词" prop="site_keywords">
        <el-input v-model="form.site_keywords" placeholder="关键词用逗号分隔" maxlength="200" show-word-limit />
      </el-form-item>

      <el-form-item label="备案信息" prop="site_icp">
        <el-input v-model="form.site_icp" placeholder="如: 京ICP备xxxxxxxx号" maxlength="100" show-word-limit />
      </el-form-item>

      <el-form-item label="版权信息" prop="site_copyright">
        <el-input v-model="form.site_copyright" placeholder="版权信息" maxlength="200" show-word-limit />
      </el-form-item>

      <el-form-item>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
        <el-button @click="handleReset">重置</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { getConfig, updateConfig } from '../../../api/config'

const loading = ref(false)
const saving = ref(false)
const formRef = ref(null)

const form = reactive({
  site_title: '',
  site_keywords: '',
  site_icp: '',
  site_copyright: ''
})

const rules = {
  site_title: [{ required: true, message: '请输入网站标题', trigger: 'blur' }]
}

const defaults = {
  site_title: 'ZJAdmin 最简后台',
  site_keywords: 'ZJAdmin,最简后台,后台管理,RBAC',
  site_icp: '',
  site_copyright: 'Copyright © 2026 ZJAdmin. All rights reserved.'
}

onMounted(async () => {
  await fetchConfig()
})

async function fetchConfig() {
  loading.value = true
  try {
    const res = await getConfig()
    if (res.data) {
      form.site_title = res.data.site_title || defaults.site_title
      form.site_keywords = res.data.site_keywords || defaults.site_keywords
      form.site_icp = res.data.site_icp || defaults.site_icp
      form.site_copyright = res.data.site_copyright || defaults.site_copyright
    }
  } finally {
    loading.value = false
  }
}

async function handleSave() {
  const valid = await formRef.value.validate().catch(() => false)
  if (!valid) return

  saving.value = true
  try {
    await updateConfig({
      site_title: form.site_title,
      site_keywords: form.site_keywords,
      site_icp: form.site_icp,
      site_copyright: form.site_copyright
    })
    ElMessage.success('保存成功')
  } finally {
    saving.value = false
  }
}

function handleReset() {
  form.site_title = defaults.site_title
  form.site_keywords = defaults.site_keywords
  form.site_icp = defaults.site_icp
  form.site_copyright = defaults.site_copyright
}
</script>
