import englishMessages from "ra-language-english";

export const en = {
  ...englishMessages,
  resources: {
    institutionaccounttransaction: {
      fields: {
        date: {
          name: "Date",
        },
        amount: {
          name: "Amount",
        },
        transactionCode: {
          name: "Code",
        },
        unstructuredInformation: {
          name: "Details",
        },
      },
    },
  },
  app: {
    institutionconnections: {
      createreturn: {
        title: "Connected bank account(s)",
        return: "Return to list",
      },
    },
    button: {
      refresh: "Refresh",
    },
  },
};
