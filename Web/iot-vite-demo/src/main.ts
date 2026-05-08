import { createApp } from 'vue'
import './style.css'
import App from './App.vue'

import router from '@/router'
import { createPinia } from 'pinia'

// Element Plus
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

// 中文
import zhCn from 'element-plus/es/locale/lang/zh-cn'

const app = createApp(App)

app.use(createPinia())

app.use(router)

app.use(ElementPlus, {
  locale: zhCn,
})

app.mount('#app')