/* eslint-disable import/order */
import '@/@iconify/icons-bundle'
import App from '@/App.vue'
import i18n from '@/plugins/i18n'
import vuetify from '@/plugins/vuetify'
import { loadFonts } from '@/plugins/webfontloader'
import router from '@/router'
import '@/styles/styles.scss'
import '@core/scss/index.scss'
import { createPinia } from 'pinia'
import { createApp } from 'vue'

loadFonts()

const app = createApp(App)

app.use(vuetify)
app.use(createPinia())
app.use(i18n)
app.use(router)

app.mount('#app')
