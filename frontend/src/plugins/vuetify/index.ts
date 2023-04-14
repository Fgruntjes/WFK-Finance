import { createVuetify } from 'vuetify'
import { createVueI18nAdapter } from 'vuetify/locale/adapters/vue-i18n'
import i18n from '../i18n'
import defaults from './defaults'
import { icons } from './icons'
import theme from './theme'

// Styles
import '@core/scss/libs/vuetify/index.scss'
import 'vuetify/styles'

export default createVuetify({
  defaults,
  icons,
  theme,
  locale: createVueI18nAdapter({ i18n, useI18n }),
})
