import { I18nProvider } from "@refinedev/core";
import { useTranslation } from "react-i18next";

function useI18nProvider(): I18nProvider {
  const { t, i18n } = useTranslation();

  const i18nProvider = {
    translate: (key: string, params: object) => t(key, params),
    changeLocale: (lang: string) => i18n.changeLanguage(lang),
    getLocale: () => i18n.language,
  };

  return i18nProvider;
}

export default useI18nProvider;