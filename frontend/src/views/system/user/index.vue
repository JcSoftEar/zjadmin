<template>
  <div>
    <div class="search-form">
      <el-form :model="query" :inline="true">
        <el-form-item label="用户名">
          <el-input v-model="query.username" placeholder="用户名" clearable />
        </el-form-item>
        <el-form-item label="昵称">
          <el-input v-model="query.nickname" placeholder="昵称" clearable />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="query.status" placeholder="全部" clearable style="width: 120px">
            <el-option label="正常" :value="1" />
            <el-option label="禁用" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="search">查询</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
    </div>

    <div class="page-container">
      <div class="table-actions">
        <el-button v-permission="'system:user:add'" type="primary" @click="openCreate">新增用户</el-button>
      </div>

      <el-table :data="userList" stripe v-loading="loading" @sort-change="handleSortChange">
        <el-table-column prop="id" label="ID" width="80" sortable="custom" />
        <el-table-column prop="username" label="用户名" width="120" />
        <el-table-column prop="nickname" label="昵称" width="120" />
        <el-table-column prop="email" label="邮箱" min-width="180" />
        <el-table-column prop="phone" label="手机号" width="130" />
        <el-table-column label="所属角色" width="150">
          <template #default="{ row }">
            <el-tag v-for="role in row.roles" :key="role.id" size="small" style="margin: 2px">
              {{ role.name }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'danger'" size="small">
              {{ row.status === 1 ? '正常' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="创建时间" width="170" sortable="custom" />
        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button v-permission="'system:user:edit'" type="primary" link size="small" @click="openEdit(row)">编辑</el-button>
            <el-button v-permission="'system:user:edit'" type="warning" link size="small" @click="handleResetPwd(row)">重置密码</el-button>
            <el-button type="success" link size="small" @click="handleToggleStatus(row)">
              {{ row.status === 1 ? '禁用' : '启用' }}
            </el-button>
            <el-button v-permission="'system:user:delete'" type="danger" link size="small" @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <el-pagination
        v-model:current-page="query.page"
        v-model:page-size="query.pageSize"
        :total="total"
        :page-sizes="[10, 20, 50]"
        layout="total, sizes, prev, pager, next, jumper"
        @change="fetchData"
        style="margin-top: 16px; justify-content: flex-end"
      />
    </div>

    <!-- User Form Dialog -->
    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑用户' : '新增用户'" width="600px" :close-on-click-modal="false">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="100px">
        <el-form-item label="用户名" prop="username" v-if="!isEdit">
          <el-input v-model="form.username" placeholder="用户名" />
        </el-form-item>
        <el-form-item label="昵称" prop="nickname">
          <el-input v-model="form.nickname" placeholder="昵称" />
        </el-form-item>
        <el-form-item label="密码" prop="password" v-if="!isEdit">
          <el-input v-model="form.password" type="password" placeholder="密码" show-password />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPassword" v-if="!isEdit">
          <el-input v-model="form.confirmPassword" type="password" placeholder="确认密码" show-password />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="form.email" placeholder="邮箱" />
        </el-form-item>
        <el-form-item label="手机号" prop="phone">
          <el-input v-model="form.phone" placeholder="手机号" />
        </el-form-item>
        <el-form-item label="所属角色">
          <el-select v-model="form.roleIds" multiple placeholder="请选择角色" style="width: 100%">
            <el-option v-for="role in allRoles" :key="role.id" :label="role.name" :value="role.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-radio-group v-model="form.status">
            <el-radio :value="1">正常</el-radio>
            <el-radio :value="0">禁用</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleSubmit">确认</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getUserList, createUser, updateUser, deleteUser, resetPassword, setUserStatus } from '../../../api/users'
import { getAllRoles } from '../../../api/roles'

const loading = ref(false)
const submitLoading = ref(false)
const userList = ref([])
const total = ref(0)
const allRoles = ref([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const editId = ref(0)
const formRef = ref(null)

const query = reactive({
  page: 1,
  pageSize: 10,
  username: '',
  nickname: '',
  status: null
})

const form = reactive({
  username: '',
  password: '',
  confirmPassword: '',
  nickname: '',
  email: '',
  phone: '',
  roleIds: [],
  status: 1
})

const formRules = {
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  nickname: [{ required: true, message: '请输入昵称', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
  confirmPassword: [{ required: true, message: '请确认密码', trigger: 'blur' }]
}

onMounted(async () => {
  await fetchData()
  const res = await getAllRoles()
  allRoles.value = res.data || []
})

async function fetchData() {
  loading.value = true
  try {
    const res = await getUserList(query)
    userList.value = res.data || []
    total.value = res.total || 0
  } finally {
    loading.value = false
  }
}

function search() {
  query.page = 1
  fetchData()
}

function resetSearch() {
  query.username = ''
  query.nickname = ''
  query.status = null
  query.page = 1
  fetchData()
}

function handleSortChange(sort) {
  // simplified sort handling
  fetchData()
}

function openCreate() {
  isEdit.value = false
  editId.value = 0
  form.username = ''
  form.password = ''
  form.confirmPassword = ''
  form.nickname = ''
  form.email = ''
  form.phone = ''
  form.roleIds = []
  form.status = 1
  dialogVisible.value = true
}

function openEdit(row) {
  isEdit.value = true
  editId.value = row.id
  form.nickname = row.nickname
  form.email = row.email
  form.phone = row.phone
  form.roleIds = row.roles?.map(r => r.id) || []
  form.status = row.status
  dialogVisible.value = true
}

async function handleSubmit() {
  const valid = await formRef.value.validate().catch(() => false)
  if (!valid) return

  submitLoading.value = true
  try {
    if (isEdit.value) {
      await updateUser(editId.value, {
        nickname: form.nickname,
        email: form.email,
        phone: form.phone,
        roleIds: form.roleIds,
        status: form.status
      })
      ElMessage.success('更新成功')
    } else {
      await createUser({
        username: form.username,
        password: form.password,
        confirmPassword: form.confirmPassword,
        nickname: form.nickname,
        email: form.email,
        phone: form.phone,
        roleIds: form.roleIds,
        status: form.status
      })
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    await fetchData()
  } finally {
    submitLoading.value = false
  }
}

async function handleResetPwd(row) {
  try {
    await ElMessageBox.confirm(`确认重置用户 "${row.username}" 的密码为 123456？`, '提示')
    await resetPassword(row.id)
    ElMessage.success('密码已重置')
  } catch {}
}

async function handleToggleStatus(row) {
  const action = row.status === 1 ? '禁用' : '启用'
  try {
    await ElMessageBox.confirm(`确认${action}用户 "${row.username}"？`, '提示')
    await setUserStatus(row.id, row.status === 1 ? 0 : 1)
    ElMessage.success(`${action}成功`)
    await fetchData()
  } catch {}
}

async function handleDelete(row) {
  try {
    await ElMessageBox.confirm(`确认删除用户 "${row.username}"？`, '警告', { type: 'warning' })
    await deleteUser(row.id)
    ElMessage.success('删除成功')
    await fetchData()
  } catch {}
}
</script>
