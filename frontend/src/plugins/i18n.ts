import { createI18n } from "vue-i18n";
import { en } from 'vuetify/locale';

const i18n = createI18n({
  legacy: false,
  locale: 'en',
  fallbackLocale: 'en',
  availableLocales: ['en'],
  messages: {
    en: {
      '$vuetify': en
    }
  },
});

export default i18n;
