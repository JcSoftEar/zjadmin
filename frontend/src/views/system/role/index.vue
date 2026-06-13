<template>
  <div>
    <div class="search-form">
      <el-form :model="query" :inline="true">
        <el-form-item label="角色名称">
          <el-input v-model="query.name" placeholder="角色名称" clearable />
        </el-form-item>
        <el-form-item label="角色标识">
          <el-input v-model="query.code" placeholder="角色标识" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="search">查询</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
    </div>

    <div class="page-container">
      <div class="table-actions">
        <el-button v-permission="'system:role:add'" type="primary" @click="openCreate">新增角色</el-button>
      </div>

      <el-table :data="roleList" v-loading="loading" stripe>
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="name" label="角色名称" width="150" />
        <el-table-column prop="code" label="角色标识" width="150" />
        <el-table-column prop="description" label="描述" min-width="200" show-overflow-tooltip />
        <el-table-column prop="createTime" label="创建时间" width="170" />
        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button v-permission="'system:role:edit'" type="primary" link size="small" @click="openEdit(row)">编辑</el-button>
            <el-button v-permission="'system:role:edit'" type="success" link size="small" @click="openPermissionAssign(row)">分配权限</el-button>
            <el-button v-permission="'system:role:delete'" type="danger" link size="small" @click="handleDelete(row)">删除</el-button>
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

    <!-- Role Form Dialog -->
    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑角色' : '新增角色'" width="500px" :close-on-click-modal="false">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="100px">
        <el-form-item label="角色名称" prop="name">
          <el-input v-model="form.name" placeholder="角色名称" />
        </el-form-item>
        <el-form-item label="角色标识" prop="code" v-if="!isEdit">
          <el-input v-model="form.code" placeholder="角色标识" />
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input v-model="form.description" type="textarea" :rows="3" placeholder="角色描述" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleSubmit">确认</el-button>
      </template>
    </el-dialog>

    <!-- Permission Assignment Dialog -->
    <el-dialog v-model="permDialogVisible" title="分配权限" width="600px" :close-on-click-modal="false">
      <el-tree
        ref="treeRef"
        :data="permissionTree"
        show-checkbox
        node-key="id"
        :props="{ label: 'name', children: 'children' }"
        default-expand-all
      />
      <template #footer>
        <el-button @click="permDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="permLoading" @click="handleAssignPermissions">确认</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getRoleList, createRole, updateRole, deleteRole, assignPermissions, getRolePermissionIds } from '../../../api/roles'
import { getPermissionTree } from '../../../api/permissions'

const loading = ref(false)
const submitLoading = ref(false)
const roleList = ref([])
const total = ref(0)
const dialogVisible = ref(false)
const isEdit = ref(false)
const editId = ref(0)
const formRef = ref(null)

// Permission dialog
const permDialogVisible = ref(false)
const permLoading = ref(false)
const permissionTree = ref([])
const treeRef = ref(null)
const currentRoleId = ref(0)

const query = reactive({
  page: 1,
  pageSize: 10,
  name: '',
  code: ''
})

const form = reactive({
  name: '',
  code: '',
  description: ''
})

const formRules = {
  name: [{ required: true, message: '请输入角色名称', trigger: 'blur' }],
  code: [{ required: true, message: '请输入角色标识', trigger: 'blur' }]
}

onMounted(async () => {
  await fetchData()
  const res = await getPermissionTree()
  permissionTree.value = res.data || []
})

async function fetchData() {
  loading.value = true
  try {
    const res = await getRoleList(query)
    roleList.value = res.data || []
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
  query.name = ''
  query.code = ''
  query.page = 1
  fetchData()
}

function openCreate() {
  isEdit.value = false
  editId.value = 0
  form.name = ''
  form.code = ''
  form.description = ''
  dialogVisible.value = true
}

function openEdit(row) {
  isEdit.value = true
  editId.value = row.id
  form.name = row.name
  form.description = row.description
  dialogVisible.value = true
}

async function handleSubmit() {
  const valid = await formRef.value.validate().catch(() => false)
  if (!valid) return

  submitLoading.value = true
  try {
    if (isEdit.value) {
      await updateRole(editId.value, { name: form.name, description: form.description })
      ElMessage.success('更新成功')
    } else {
      await createRole({ name: form.name, code: form.code, description: form.description })
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    await fetchData()
  } finally {
    submitLoading.value = false
  }
}

async function openPermissionAssign(row) {
  currentRoleId.value = row.id
  permDialogVisible.value = true

  // Load checked permissions
  const res = await getRolePermissionIds(row.id)
  const checkedIds = res.data || []
  treeRef.value?.setCheckedKeys(checkedIds)
}

async function handleAssignPermissions() {
  const checkedIds = treeRef.value?.getCheckedKeys() || []
  permLoading.value = true
  try {
    await assignPermissions(currentRoleId.value, { permissionIds: checkedIds })
    ElMessage.success('权限分配成功')
    permDialogVisible.value = false
  } finally {
    permLoading.value = false
  }
}

async function handleDelete(row) {
  try {
    await ElMessageBox.confirm(`确认删除角色 "${row.name}"？`, '警告', { type: 'warning' })
    await deleteRole(row.id)
    ElMessage.success('删除成功')
    await fetchData()
  } catch {}
}
</script>
