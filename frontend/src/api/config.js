import request from '../utils/request'

export function getConfig() {
  return request.get('/api/v1/config')
}

export function updateConfig(data) {
  return request.put('/api/v1/config', data)
}
