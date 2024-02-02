import { PlusSquareOutlined } from "@ant-design/icons";
import { HttpError, useCreate, useTranslate } from "@refinedev/core";
import { Button } from "antd";
import {
  TransactionCategory,
  TransactionCategoryGroup,
  TransactionCategoryInput,
} from "../../../Api";

type DefaultCategoryDescription = Omit<TransactionCategoryInput, "sortOrder">;
type DefaultParentCategoryDescription = DefaultCategoryDescription & {
  children: DefaultCategoryDescription[];
};

const defaultCategories: DefaultParentCategoryDescription[] = [
  {
    name: "Fixed Costs",
    group: TransactionCategoryGroup.Expense,
    children: [
      {
        name: "Rent & Mortgage",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Utilities",
        description: "Electricity, water, gas, internet, etc.",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Financeial services",
        description:
          "Home, health and other insurances, accounting, banking, etc.",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Phone",
        description: "Phone subscription and hardware costs.",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Health & Fitness",
        description: "Gym / sports memberships, sport equipment, etc.",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Transportation",
        description:
          "Public transport, car, bike, etc. Including maintenance, fuel and insurances.",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Groceries",
        group: TransactionCategoryGroup.Expense,
      },
    ],
  },
  {
    name: "Saving goals",
    group: TransactionCategoryGroup.Savings,
    children: [
      {
        name: "Emergency Fund",
        group: TransactionCategoryGroup.Savings,
      },
      {
        name: "Holiday",
        group: TransactionCategoryGroup.Savings,
      },
    ],
  },
  {
    name: "Investments",
    group: TransactionCategoryGroup.Investment,
    children: [
      {
        name: "Stocks & Bonds",
        group: TransactionCategoryGroup.Investment,
      },
      {
        name: "Crypto",
        group: TransactionCategoryGroup.Investment,
      },
    ],
  },
  {
    name: "Guilt-free spending",
    group: TransactionCategoryGroup.Expense,
    children: [
      {
        name: "Dining out, takeaway and bars",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Entertainment",
        description: "Including streaming services, cinema, etc.",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Hobbies",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Clothing",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Gifts",
        group: TransactionCategoryGroup.Expense,
      },
      {
        name: "Wellness",
        description: "Spa, massage, hair dresser, etc.",
        group: TransactionCategoryGroup.Expense,
      },
    ],
  },
  {
    name: "Income",
    group: TransactionCategoryGroup.Income,
    children: [
      {
        name: "Salary",
        group: TransactionCategoryGroup.Income,
      },
      {
        name: "Bonusses",
        group: TransactionCategoryGroup.Income,
      },
      {
        name: "Investment returns",
        description: "Dividends, rent income, etc.",
        group: TransactionCategoryGroup.Income,
      },
      {
        name: "Other income",
        group: TransactionCategoryGroup.Income,
      },
    ],
  },
];

function CreateDefaultsButton() {
  const translate = useTranslate();
  const { mutate } = useCreate<
    TransactionCategory,
    HttpError,
    TransactionCategoryInput
  >();

  function createDefaultCategories() {
    defaultCategories.forEach((parentCategory, index) => {
      mutate({
        resource: "transactioncategories",
        values: {
          ...parentCategory,
          sortOrder: index,
        },
        successNotification: (data) => {
          if (!parentCategory.children) {
            return false;
          }

          if (data?.data.id === undefined) {
            throw new Error("No id returned from server");
          }

          parentCategory.children.forEach((child, childIndex) => {
            mutate({
              resource: "transactioncategories",
              values: {
                ...child,
                parentId: data?.data.id,
                sortOrder: childIndex,
              },
              successNotification: false,
            });
          });

          return false;
        },
      });
    });
  }

  return (
    <Button
      icon={<PlusSquareOutlined />}
      type="primary"
      onClick={createDefaultCategories}
    >
      {translate("transactioncategories.buttons.createDefaults")}
    </Button>
  );
}

export default CreateDefaultsButton;
