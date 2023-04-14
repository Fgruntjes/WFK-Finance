import messages from "@/i18n/en.json";
import { createI18n } from "vue-i18n";
import vuetifyDefaults from "vuetify/lib/locale/en.mjs";

const i18n = createI18n({
  legacy: false,
  locale: 'en',
  fallbackLocale: 'en',
  messages: {
    en: {
      $vuetify: vuetifyDefaults,
      ...messages,
    },
  },
});

export default i18n;