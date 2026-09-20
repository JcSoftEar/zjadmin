import { defineStore } from 'pinia'
import { getConfig } from '../api/config'

export const useAppStore = defineStore('app', {
  state: () => ({
    sidebarCollapsed: false,
    config: {
      site_title: 'ZJAdmin 最简后台',
      site_keywords: 'ZJAdmin,最简后台,后台管理,RBAC',
      site_icp: '',
      site_copyright: 'Copyright © 2026 ZJAdmin. All rights reserved.'
    },
    configLoaded: false,
    currentTheme: localStorage.getItem('zjadmin-theme') || 'classic-deep'
  }),

  actions: {
    toggleSidebar() {
      this.sidebarCollapsed = !this.sidebarCollapsed
    },

    setTheme(themeId) {
      this.currentTheme = themeId
      document.documentElement.setAttribute('data-theme', themeId)
      localStorage.setItem('zjadmin-theme', themeId)
    },

    initTheme() {
      const saved = localStorage.getItem('zjadmin-theme') || 'classic-deep'
      this.setTheme(saved)
    },

    async fetchConfig() {
      if (this.configLoaded) return
      try {
        const res = await getConfig()
        if (res.data) {
          this.config = {
            site_title: res.data.site_title || this.config.site_title,
            site_keywords: res.data.site_keywords || this.config.site_keywords,
            site_icp: res.data.site_icp || '',
            site_copyright: res.data.site_copyright || this.config.site_copyright
          }
          this.configLoaded = true
        }
      } catch {
        // silent — use defaults
      }
    }
  }
})
