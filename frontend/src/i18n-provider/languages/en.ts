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
    notifications: {
      deleteSuccess: "Successfully deleted",
      createSuccess: "Successfully created",
      editSuccess: "Successfully edited",
      deleteError: "Error deleting",
      success: "Successfull",
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
      ["Cannot delete a category with children."]: {
        title: "Error",
        description: "Cannot delete a category with children.",
      },
    },
    table: {
      actions: "Actions",
    },
    institutions: {
      institutions: "Bank institutions",
    },
    institutionaccounts: {
      institutionaccounts: "Accounts",
      tabs: {
        general: "General",
        transactions: "Transactions",
      },
      fields: {
        institution: "Bank institution",
        iban: "IBAN",
        importStatus: "Import status",
        lastImport: "Last import",
        lastImportNever: "Never",
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
        list: "Bank accounts",
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
    institutiontransactions: {
      institutiontransactions: "Transactions",
      titles: {
        list: "Bank transactions",
      },
      fields: {
        institutionId: "Bank",
        accountIban: "Iban",
        date: "Date",
        amount: "Amount",
        transactionCode: "Transaction code",
        unstructuredInformation: "Description",
      },
    },
    institutionconnections: {
      institutionconnections: "Accounts",
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
    transactioncategories: {
      transactioncategories: "Categories",
      titles: {
        list: "Categories",
        create: "Create category",
      },
      fields: {
        name: "Name",
        parentId: "Parent",
        group: "Group",
      },
      groupNames: {
        income: "Income",
        expense: "Expense",
        transfer: "Transfer",
        investment: "Investment",
        liquididation: "Liquididation",
        other: "Other",
      },
    },
    components: {
      noData: {
        text: "No Data",
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
        list: "Bank accounts | WFK Finance",
        create: "Create bank account | WFK Finance",
      },
      institutiontransactions: {
        list: "Bank transactions | WFK Finance",
      },
      transactioncategories: {
        list: "Categories | WFK Finance",
      },
    },
  },
};

export default en;
