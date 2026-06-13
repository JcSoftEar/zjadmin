import request from '../utils/request'

export function getRoleList(params) {
  return request.get('/api/v1/roles', { params })
}

export function getAllRoles() {
  return request.get('/api/v1/roles/all')
}

export function getRoleDetail(id) {
  return request.get(`/api/v1/roles/${id}`)
}

export function createRole(data) {
  return request.post('/api/v1/roles', data)
}

export function updateRole(id, data) {
  return request.put(`/api/v1/roles/${id}`, data)
}

export function deleteRole(id) {
  return request.delete(`/api/v1/roles/${id}`)
}

export function assignPermissions(id, data) {
  return request.put(`/api/v1/roles/${id}/permissions`, data)
}

export function getRolePermissionIds(id) {
  return request.get(`/api/v1/roles/${id}/permissions`)
}
