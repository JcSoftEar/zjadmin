<template>
  <div>
    <div class="search-form">
      <el-form :model="query" :inline="true">
        <el-form-item label="异常信息">
          <el-input v-model="query.message" placeholder="异常信息" clearable />
        </el-form-item>
        <el-form-item label="异常类型">
          <el-input v-model="query.exceptionType" placeholder="异常类型" clearable />
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
        <el-button v-permission="'logs:exception:query'" type="success" @click="handleExport">导出</el-button>
        <el-button v-permission="'logs:exception:delete'" type="danger" @click="handleClean">清理</el-button>
      </div>

      <el-table :data="logList" v-loading="loading" stripe>
        <el-table-column prop="id" label="ID" width="70" />
        <el-table-column prop="message" label="异常信息" min-width="250" show-overflow-tooltip />
        <el-table-column prop="exceptionType" label="异常类型" width="180" show-overflow-tooltip />
        <el-table-column prop="requestUrl" label="请求地址" min-width="200" show-overflow-tooltip />
        <el-table-column prop="requestMethod" label="请求方法" width="100" />
        <el-table-column prop="ipAddress" label="IP 地址" width="130" />
        <el-table-column prop="operator" label="操作人" width="100" />
        <el-table-column prop="occurTime" label="发生时间" width="170" />
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
    <el-dialog v-model="detailVisible" title="异常日志详情" width="800px">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="ID">{{ detail.id }}</el-descriptions-item>
        <el-descriptions-item label="异常信息">{{ detail.message }}</el-descriptions-item>
        <el-descriptions-item label="异常类型">{{ detail.exceptionType }}</el-descriptions-item>
        <el-descriptions-item label="请求地址">{{ detail.requestUrl }}</el-descriptions-item>
        <el-descriptions-item label="请求方法">{{ detail.requestMethod }}</el-descriptions-item>
        <el-descriptions-item label="请求参数">
          <pre style="white-space: pre-wrap; max-height: 200px; overflow-y: auto;">{{ detail.requestParams }}</pre>
        </el-descriptions-item>
        <el-descriptions-item label="IP 地址">{{ detail.ipAddress }}</el-descriptions-item>
        <el-descriptions-item label="操作人">{{ detail.operator }}</el-descriptions-item>
        <el-descriptions-item label="发生时间">{{ detail.occurTime }}</el-descriptions-item>
        <el-descriptions-item label="异常堆栈">
          <pre style="white-space: pre-wrap; max-height: 400px; overflow-y: auto; background: #f5f7fa; padding: 12px; border-radius: 4px; font-size: 12px;">{{ detail.stackTrace }}</pre>
        </el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getExceptionLogList, getExceptionLogDetail, cleanExceptionLogs, exportExceptionLogs } from '../../../api/logs'

const loading = ref(false)
const logList = ref([])
const total = ref(0)
const detailVisible = ref(false)
const detail = ref({})
const dateRange = ref(null)

const query = reactive({
  page: 1,
  pageSize: 10,
  message: '',
  exceptionType: '',
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
    const res = await getExceptionLogList(query)
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
  query.message = ''
  query.exceptionType = ''
  query.startTime = null
  query.endTime = null
  dateRange.value = null
  query.page = 1
  fetchData()
}

async function openDetail(row) {
  try {
    const res = await getExceptionLogDetail(row.id)
    detail.value = res.data || {}
    detailVisible.value = true
  } catch {}
}

async function handleExport() {
  try {
    const res = await exportExceptionLogs(query)
    const blob = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = '异常日志.xlsx'
    a.click()
    URL.revokeObjectURL(url)
    ElMessage.success('导出成功')
  } catch {}
}

async function handleClean() {
  try {
    await ElMessageBox.confirm('确认清理异常日志？', '警告', { type: 'warning' })
    await cleanExceptionLogs({
      startTime: query.startTime,
      endTime: query.endTime
    })
    ElMessage.success('清理成功')
    await fetchData()
  } catch {}
}
</script>
