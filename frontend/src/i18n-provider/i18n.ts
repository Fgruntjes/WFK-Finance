import i18next from "i18next";
import { initReactI18next } from "react-i18next";
import en from "./languages/en";

i18next.use(initReactI18next).init({
  lng: "en",
  debug: import.meta.env.NODE_ENV !== "production",
  resources: {
    en,
  },
});
