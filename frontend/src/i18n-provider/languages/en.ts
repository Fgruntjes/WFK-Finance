import { ResourceLanguage } from "i18next";

const en: ResourceLanguage = {
  translation: {
    actions: {
      list: "List",
      create: "Create",
      show: "Show",
    },
    buttons: {
      show: "Show",
      logout: "Logout",
      cancel: "Cancel",
      create: "Create",
      delete: "Delete",
      save: "Save",
      refresh: "Refresh",
      confirm: "Are you sure?",
    },
    error: {
      404: {
        title: "Page not found",
        description: "The page you are looking for does not exist.",
      },
    },
    table: {
      actions: "Actions",
    },
    institutionaccounts: {
      fields: {
        iban: "IBAN",
        importStatus: "Import status",
        lastImport: "Last import",
        transactionCount: "Transaction count",
      },
      importStatus: {
        success: "Success",
        qQueued: "Queued",
        working: "Working",
        failed: "Failed",
      },
    },
    institutionconnections: {
      institutionconnections: "Bank accounts",
      titles: {
        list: "Bank accounts",
        return: "Connected to bank account",
        create: "Connect bank account",
      },
      fields: {
        countryIso2: "Country",
        institutionId: "Institution",
        accounts: "Accounts",
      },
    },
    warnWhenUnsavedChanges:
      "You have unsaved changes. Are you sure you want to leave?",
    documentTitle: {
      default: "WFK Finance",
      suffix: " | WFK Finance",
      institutionconnections: {
        show: "Show | Bank accounts | WFK Finance",
        list: "List | Bank accounts | WFK Finance",
        create: "Connect bank account | WFK Finance",
      },
    },
  },
};

export default en;
