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
      returnToList: "Return to list",
    },
    error: {
      undefined: {
        title: "Error",
        description: "An error has occurred.",
      },
      404: {
        title: "Page not found",
        description: "The page you are looking for does not exist.",
      },
    },
    table: {
      actions: "Actions",
    },
    institutionaccounts: {
      institutionaccounts: "Bank accounts",
      tabs: {
        general: "General",
        transactions: "Transactions",
      },
      fields: {
        institution: "Bank institution",
        iban: "IBAN",
        importStatus: "Import status",
        lastImport: "Last import",
        transactionCount: "Transaction count",
      },
      importStatus: {
        success: "Success",
        queued: "Queued",
        working: "Working",
        failed: "Failed",
      },
      titles: {
        show: "Bank account",
      },
    },
    institutionaccounttransactions: {
      fields: {
        date: "Date",
        amount: "Amount",
        transactionCode: "Transaction code",
        unstructuredInformation: "Description",
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
        institutionId: "Bank",
        accounts: "Accounts",
      },
    },
    warnWhenUnsavedChanges:
      "You have unsaved changes. Are you sure you want to leave?",
    documentTitle: {
      default: "WFK Finance",
      suffix: " | WFK Finance",
      institutionconnections: {
        show: "Bank accounts | WFK Finance",
        list: "Bank accounts | WFK Finance",
        create: "Connect bank account | WFK Finance",
      },
      institutionaccounts: {
        show: "Bank accounts | WFK Finance",
      },
    },
  },
};

export default en;
