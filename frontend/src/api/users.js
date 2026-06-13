import request from '../utils/request'

export function getUserList(params) {
  return request.get('/api/v1/users', { params })
}

export function getUserDetail(id) {
  return request.get(`/api/v1/users/${id}`)
}

export function createUser(data) {
  return request.post('/api/v1/users', data)
}

export function updateUser(id, data) {
  return request.put(`/api/v1/users/${id}`, data)
}

export function deleteUser(id) {
  return request.delete(`/api/v1/users/${id}`)
}

export function resetPassword(id) {
  return request.put(`/api/v1/users/${id}/reset-password`)
}

export function setUserStatus(id, status) {
  return request.put(`/api/v1/users/${id}/status`, status)
}
