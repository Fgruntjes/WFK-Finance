import i18next from 'i18next';
import HttpBackend from 'i18next-http-backend';
import { createI18nStore } from 'svelte-i18next';

i18next.use(HttpBackend).init({
    fallbackLng: 'en',
    ns: [
        'common',
        'institutionconnections'
    ],
    debug: import.meta.env.DEV,
    backend: {
        loadPath: '/locales/{{lng}}/{{ns}}.json'
    }
});

export const i18n = createI18nStore(i18next);
