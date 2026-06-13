<template>
  <div>
    <div class="search-form">
      <el-form :model="query" :inline="true">
        <el-form-item label="操作人">
          <el-input v-model="query.operator" placeholder="操作人" clearable />
        </el-form-item>
        <el-form-item label="操作模块">
          <el-input v-model="query.module" placeholder="操作模块" clearable />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="query.status" placeholder="全部" clearable style="width: 120px">
            <el-option label="成功" :value="1" />
            <el-option label="失败" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item label="时间范围">
          <el-date-picker
            v-model="dateRange"
            type="datetimerange"
            range-separator="至"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            value-format="YYYY-MM-DDTHH:mm:ss"
            style="width: 360px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="search">查询</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
    </div>

    <div class="page-container">
      <div class="table-actions">
        <el-button v-permission="'logs:operation:query'" type="success" @click="handleExport">导出</el-button>
        <el-button v-permission="'logs:operation:delete'" type="danger" @click="handleClean">清理</el-button>
      </div>

      <el-table :data="logList" v-loading="loading" stripe>
        <el-table-column prop="id" label="ID" width="70" />
        <el-table-column prop="operator" label="操作人" width="100" />
        <el-table-column prop="module" label="操作模块" width="120" />
        <el-table-column prop="operationType" label="操作类型" width="100" />
        <el-table-column prop="requestUrl" label="请求地址" min-width="200" show-overflow-tooltip />
        <el-table-column prop="requestMethod" label="请求方法" width="100" />
        <el-table-column prop="ipAddress" label="IP 地址" width="130" />
        <el-table-column label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'danger'" size="small">
              {{ row.status === 1 ? '成功' : '失败' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="耗时" width="80">
          <template #default="{ row }">
            <span>{{ row.duration }}ms</span>
          </template>
        </el-table-column>
        <el-table-column prop="operationTime" label="操作时间" width="170" />
        <el-table-column label="操作" width="80" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="openDetail(row)">详情</el-button>
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

    <!-- Detail Dialog -->
    <el-dialog v-model="detailVisible" title="操作日志详情" width="700px">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="ID">{{ detail.id }}</el-descriptions-item>
        <el-descriptions-item label="操作人">{{ detail.operator }}</el-descriptions-item>
        <el-descriptions-item label="操作模块">{{ detail.module }}</el-descriptions-item>
        <el-descriptions-item label="操作类型">{{ detail.operationType }}</el-descriptions-item>
        <el-descriptions-item label="请求地址">{{ detail.requestUrl }}</el-descriptions-item>
        <el-descriptions-item label="请求方法">{{ detail.requestMethod }}</el-descriptions-item>
        <el-descriptions-item label="请求参数">
          <pre style="white-space: pre-wrap; max-height: 200px; overflow-y: auto;">{{ detail.requestParams }}</pre>
        </el-descriptions-item>
        <el-descriptions-item label="响应结果">
          <pre style="white-space: pre-wrap; max-height: 200px; overflow-y: auto;">{{ detail.responseResult }}</pre>
        </el-descriptions-item>
        <el-descriptions-item label="IP 地址">{{ detail.ipAddress }}</el-descriptions-item>
        <el-descriptions-item label="状态">{{ detail.status === 1 ? '成功' : '失败' }}</el-descriptions-item>
        <el-descriptions-item label="耗时">{{ detail.duration }}ms</el-descriptions-item>
        <el-descriptions-item label="操作时间">{{ detail.operationTime }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getOperationLogList, getOperationLogDetail, cleanOperationLogs, exportOperationLogs } from '../../../api/logs'

const loading = ref(false)
const logList = ref([])
const total = ref(0)
const detailVisible = ref(false)
const detail = ref({})
const dateRange = ref(null)

const query = reactive({
  page: 1,
  pageSize: 10,
  operator: '',
  module: '',
  status: null,
  startTime: null,
  endTime: null
})

watch(dateRange, (val) => {
  if (val) {
    query.startTime = val[0]
    query.endTime = val[1]
  } else {
    query.startTime = null
    query.endTime = null
  }
})

onMounted(async () => {
  await fetchData()
})

async function fetchData() {
  loading.value = true
  try {
    const res = await getOperationLogList(query)
    logList.value = res.data || []
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
  query.operator = ''
  query.module = ''
  query.status = null
  query.startTime = null
  query.endTime = null
  dateRange.value = null
  query.page = 1
  fetchData()
}

async function openDetail(row) {
  try {
    const res = await getOperationLogDetail(row.id)
    detail.value = res.data || {}
    detailVisible.value = true
  } catch {}
}

async function handleExport() {
  try {
    const res = await exportOperationLogs(query)
    const blob = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = '操作日志.xlsx'
    a.click()
    URL.revokeObjectURL(url)
    ElMessage.success('导出成功')
  } catch {}
}

async function handleClean() {
  try {
    await ElMessageBox.confirm('确认清理操作日志？', '警告', { type: 'warning' })
    await cleanOperationLogs({
      startTime: query.startTime,
      endTime: query.endTime
    })
    ElMessage.success('清理成功')
    await fetchData()
  } catch {}
}
</script>
