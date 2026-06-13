<template>
  <div>
    <div class="search-form">
      <el-form :model="query" :inline="true">
        <el-form-item label="权限名称">
          <el-input v-model="query.keyword" placeholder="权限名称" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="search">查询</el-button>
          <el-button @click="resetSearch">重置</el-button>
          <el-button @click="expandAll = !expandAll">{{ expandAll ? '折叠' : '展开' }}全部</el-button>
        </el-form-item>
      </el-form>
    </div>

    <div class="page-container">
      <div class="table-actions">
        <el-button v-permission="'system:permission:add'" type="primary" @click="openCreate(0)">新增顶级权限</el-button>
      </div>

      <el-table
        :data="permissionList"
        v-loading="loading"
        row-key="id"
        :default-expand-all="expandAll"
        :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
        stripe
      >
        <el-table-column prop="name" label="权限名称" min-width="200">
          <template #default="{ row }">
            <el-icon v-if="row.icon" style="margin-right: 6px; vertical-align: middle">
              <component :is="row.icon" />
            </el-icon>
            {{ row.name }}
          </template>
        </el-table-column>
        <el-table-column prop="code" label="权限标识" width="180" />
        <el-table-column label="权限类型" width="100">
          <template #default="{ row }">
            <el-tag :type="row.type === 0 ? 'primary' : 'success'" size="small">
              {{ row.type === 0 ? '菜单' : '按钮' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="sort" label="排序" width="70" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button v-permission="'system:permission:add'" type="primary" link size="small" @click="openCreate(row.id)">新增</el-button>
            <el-button v-permission="'system:permission:edit'" type="primary" link size="small" @click="openEdit(row)">编辑</el-button>
            <el-button v-permission="'system:permission:delete'" type="danger" link size="small" @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- Permission Form Dialog -->
    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑权限' : '新增权限'" width="600px" :close-on-click-modal="false">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="100px">
        <el-form-item label="上级权限">
          <el-cascader
            v-model="form.parentId"
            :options="permissionOptions"
            :props="{ value: 'id', label: 'name', emitPath: false, checkStrictly: true }"
            clearable
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="权限名称" prop="name">
          <el-input v-model="form.name" placeholder="权限名称" />
        </el-form-item>
        <el-form-item label="权限标识" prop="code" v-if="!isEdit">
          <el-input v-model="form.code" placeholder="权限标识" />
        </el-form-item>
        <el-form-item label="权限类型" prop="type">
          <el-radio-group v-model="form.type">
            <el-radio :value="0">菜单</el-radio>
            <el-radio :value="1">按钮</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="路由地址" v-if="form.type === 0">
          <el-input v-model="form.path" placeholder="路由地址" />
        </el-form-item>
        <el-form-item label="组件路径" v-if="form.type === 0">
          <el-input v-model="form.component" placeholder="组件路径" />
        </el-form-item>
        <el-form-item label="图标" v-if="form.type === 0">
          <el-input v-model="form.icon" placeholder="图标名称" />
        </el-form-item>
        <el-form-item label="排序">
          <el-input-number v-model="form.sort" :min="0" :max="999" />
        </el-form-item>
        <el-form-item label="是否显示" v-if="form.type === 0">
          <el-radio-group v-model="form.visible">
            <el-radio :value="1">显示</el-radio>
            <el-radio :value="0">隐藏</el-radio>
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
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getPermissionTree, createPermission, updatePermission, deletePermission, getPermissionDetail } from '../../../api/permissions'

const loading = ref(false)
const submitLoading = ref(false)
const permissionList = ref([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const editId = ref(0)
const expandAll = ref(true)
const formRef = ref(null)

const query = reactive({
  keyword: ''
})

// Flat list of all permissions for cascader
const permissionOptions = ref([])

const form = reactive({
  parentId: null,
  name: '',
  code: '',
  type: 0,
  path: '',
  component: '',
  icon: '',
  sort: 0,
  visible: 1
})

const formRules = {
  name: [{ required: true, message: '请输入权限名称', trigger: 'blur' }],
  code: [{ required: true, message: '请输入权限标识', trigger: 'blur' }]
}

onMounted(async () => {
  await fetchData()
})

async function fetchData() {
  loading.value = true
  try {
    const res = await getPermissionTree()
    permissionList.value = res.data || []
    permissionOptions.value = res.data || []
  } finally {
    loading.value = false
  }
}

function search() {
  fetchData()
}

function resetSearch() {
  query.keyword = ''
  fetchData()
}

async function openCreate(parentId) {
  isEdit.value = false
  editId.value = 0
  form.parentId = parentId || null
  form.name = ''
  form.code = ''
  form.type = 0
  form.path = ''
  form.component = ''
  form.icon = ''
  form.sort = 0
  form.visible = 1
  dialogVisible.value = true
}

async function openEdit(row) {
  isEdit.value = true
  editId.value = row.id
  form.parentId = row.parentId || null
  form.name = row.name
  form.type = row.type
  form.path = row.path
  form.component = row.component
  form.icon = row.icon
  form.sort = row.sort
  form.visible = row.visible
  dialogVisible.value = true
}

async function handleSubmit() {
  const valid = await formRef.value.validate().catch(() => false)
  if (!valid) return

  submitLoading.value = true
  try {
    if (isEdit.value) {
      await updatePermission(editId.value, {
        parentId: form.parentId || 0,
        name: form.name,
        type: form.type,
        path: form.path,
        component: form.component,
        icon: form.icon,
        sort: form.sort,
        visible: form.visible
      })
      ElMessage.success('更新成功')
    } else {
      await createPermission({
        parentId: form.parentId || 0,
        name: form.name,
        code: form.code,
        type: form.type,
        path: form.path,
        component: form.component,
        icon: form.icon,
        sort: form.sort,
        visible: form.visible
      })
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    await fetchData()
  } finally {
    submitLoading.value = false
  }
}

async function handleDelete(row) {
  try {
    await ElMessageBox.confirm(`确认删除权限 "${row.name}"？`, '警告', { type: 'warning' })
    await deletePermission(row.id)
    ElMessage.success('删除成功')
    await fetchData()
  } catch {}
}
</script>
