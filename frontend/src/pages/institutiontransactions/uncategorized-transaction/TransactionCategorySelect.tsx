import { InstitutionTransaction, InstitutionTransactionPatch } from "@api";
import LocalError from "@components/LocalError";
import useTransactionCategoryGroups from "@hooks/useTransactionCategoryGroups";
import { HttpError, useTranslate, useUpdateMany } from "@refinedev/core";
import { Select } from "antd";
import { useMemo } from "react";
import styles from "./TransactionCategorySelect.module.less";

type TransactionCategorySelectProps = {
  transaction: InstitutionTransaction;
  fieldId?: string;
};

export function TransactionCategorySelect({
  transaction,
  fieldId,
}: TransactionCategorySelectProps) {
  const translate = useTranslate();
  const { groups, isLoading, isError, error } = useTransactionCategoryGroups();
  const { mutate } = useUpdateMany<
    InstitutionTransaction,
    HttpError,
    InstitutionTransactionPatch
  >();

  const options = useMemo(() => {
    return groups.map((group) => ({
      label: group.name,
      searchValue: [group.name, group.description].join(" "),
      options: group.children.map((category) => ({
        label: category.name,
        searchValue: [category.name, category.description].join(" "),
        value: category.id,
      })),
    }));
  }, [groups]);

  function setTransactionCategory(categoryId: string) {
    transaction.categoryId = categoryId;
    mutate({
      resource: "institutiontransactions",
      ids: [transaction.id],
      values: {
        categoryId,
      },
    });
  }

  if (isError) {
    return <LocalError error={error} />;
  }

  return (
    <Select
      className={styles.select}
      showSearch
      loading={isLoading}
      placeholder={translate("uncategorizedtransactions.inputs.selectCategory")}
      options={options}
      value={transaction.categoryId}
      onChange={setTransactionCategory}
      id={fieldId}
      filterOption={(input, option) =>
        (option?.searchValue ?? "").toLowerCase().includes(input.toLowerCase())
      }
    />
  );
}
