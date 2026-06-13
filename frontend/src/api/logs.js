import request from '../utils/request'

export function getOperationLogList(params) {
  return request.get('/api/v1/operation-logs', { params })
}

export function getOperationLogDetail(id) {
  return request.get(`/api/v1/operation-logs/${id}`)
}

export function cleanOperationLogs(data) {
  return request.delete('/api/v1/operation-logs/clean', { data })
}

export function exportOperationLogs(params) {
  return request.get('/api/v1/operation-logs/export', { params, responseType: 'blob' })
}

export function getExceptionLogList(params) {
  return request.get('/api/v1/exception-logs', { params })
}

export function getExceptionLogDetail(id) {
  return request.get(`/api/v1/exception-logs/${id}`)
}

export function cleanExceptionLogs(data) {
  return request.delete('/api/v1/exception-logs/clean', { data })
}

export function exportExceptionLogs(params) {
  return request.get('/api/v1/exception-logs/export', { params, responseType: 'blob' })
}
