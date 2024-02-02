import { InstitutionTransaction, InstitutionTransactionPatch } from "@api";
import LocalError from "@components/LocalError";
import useTransactionCategoryGroups from "@hooks/useTransactionCategoryGroups";
import { HttpError, useTranslate, useUpdateMany } from "@refinedev/core";
import { Select } from "antd";
import { useMemo, useState } from "react";
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
  const {
    groups,
    isLoading: groupsIsLoading,
    isError,
    error,
  } = useTransactionCategoryGroups();
  const [categoryId, setCategoryId] = useState<string | undefined>(
    transaction.categoryId || undefined,
  );
  const { mutate, isLoading } = useUpdateMany<
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

  function handleSelectChange(value: string) {
    setCategoryId(value);
    mutate({
      resource: "institutiontransactions",
      ids: [transaction.id],
      values: {
        categoryId: value,
      },
      successNotification: false,
      invalidates: [],
    });
  }

  if (isError) {
    return <LocalError error={error} />;
  }

  return (
    <Select
      className={styles.select}
      showSearch
      loading={isLoading || groupsIsLoading}
      placeholder={translate("uncategorizedtransactions.inputs.selectCategory")}
      options={options}
      value={categoryId}
      onChange={handleSelectChange}
      id={fieldId}
      filterOption={(input, option) =>
        (option?.searchValue ?? "").toLowerCase().includes(input.toLowerCase())
      }
    />
  );
}
