import { defineStore } from 'pinia'
import { login as loginApi, getProfile, logout as logoutApi } from '../api/auth'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || '',
    userInfo: JSON.parse(localStorage.getItem('userInfo') || 'null')
  }),

  getters: {
    isLoggedIn: (state) => !!state.token,
    permissions: (state) => state.userInfo?.permissions || [],
    roles: (state) => state.userInfo?.roles || []
  },

  actions: {
    async login(credentials) {
      const res = await loginApi(credentials)
      const { token, user } = res.data
      this.token = token
      this.userInfo = user
      localStorage.setItem('token', token)
      localStorage.setItem('userInfo', JSON.stringify(user))
      return res
    },

    async fetchProfile() {
      try {
        const res = await getProfile()
        this.userInfo = res.data
        localStorage.setItem('userInfo', JSON.stringify(res.data))
      } catch {
        this.logout()
      }
    },

    async logout() {
      try {
        await logoutApi()
      } catch {
        // ignore
      }
      this.token = ''
      this.userInfo = null
      localStorage.removeItem('token')
      localStorage.removeItem('userInfo')
    },

    hasPermission(code) {
      if (this.permissions.includes('*:*:*')) return true
      return this.permissions.includes(code)
    }
  }
})
