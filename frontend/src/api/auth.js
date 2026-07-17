import request from '../utils/request'

export function login(data) {
  return request.post('/api/v1/auth/login', data)
}

export function logout() {
  return request.post('/api/v1/auth/logout')
}

export function getProfile() {
  return request.get('/api/v1/auth/profile')
}

export function getUserMenus() {
  return request.get('/api/v1/auth/menus')
}

export function updateProfile(data) {
  return request.put('/api/v1/auth/profile', data)
}

export function changePassword(data) {
  return request.put('/api/v1/auth/password', data)
}
