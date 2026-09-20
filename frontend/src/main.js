import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import 'element-plus/dist/index.css'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import './assets/styles/themes/index.css'
import './assets/styles/global.scss'

const app = createApp(App)

// Register all Element Plus icons
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

app.use(ElementPlus)
app.use(createPinia())
app.use(router)

// Initialize theme from localStorage
const savedTheme = localStorage.getItem('zjadmin-theme') || 'classic-deep'
document.documentElement.setAttribute('data-theme', savedTheme)

app.mount('#app')
