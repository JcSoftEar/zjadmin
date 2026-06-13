import request from '../utils/request'

export function getPermissionTree() {
  return request.get('/api/v1/permissions/tree')
}

export function getPermissionDetail(id) {
  return request.get(`/api/v1/permissions/${id}`)
}

export function createPermission(data) {
  return request.post('/api/v1/permissions', data)
}

export function updatePermission(id, data) {
  return request.put(`/api/v1/permissions/${id}`, data)
}

export function deletePermission(id) {
  return request.delete(`/api/v1/permissions/${id}`)
}
