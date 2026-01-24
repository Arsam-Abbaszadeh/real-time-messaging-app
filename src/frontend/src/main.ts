import { createApp } from 'vue'
import App from './App.vue'
import { router } from './routing/router'
import { createPinia } from 'pinia'
import '@/styles/main.scss'

const app = createApp(App)
const pinia = createPinia()

app
.use(pinia)
.use(router)
.mount('#app')
