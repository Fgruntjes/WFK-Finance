import polyglotI18nProvider from "ra-i18n-polyglot";
import { en } from "./i18n/en";

export const i18nProvider = polyglotI18nProvider(() => en, "en", [
  {
    locale: "en",
    name: "English",
  },
]);
