import { InstitutionTransaction } from "@api";
import InstitutionTransactionTable from "@components/institutiontransactions/Table";
import { List, RefreshButton, useTable } from "@refinedev/antd";
import { HttpError, useTranslate } from "@refinedev/core";
import { Table } from "antd";
import { useEffect } from "react";
import styles from "./UncategorizedTransactionView.module.less";
import { TransactionCategorySelect } from "./uncategorized-transaction/TransactionCategorySelect";

function UncategorizedTransactionView() {
  const translate = useTranslate();

  const {
    tableQueryResult: { refetch, isRefetching },
    tableProps: { loading, ...tableProps },
  } = useTable<InstitutionTransaction, HttpError>({
    syncWithLocation: true,
    resource: "institutiontransactions",
    queryOptions: {
      enabled: false,
    },
    sorters: {
      initial: [{ field: "date", order: "desc" }],
    },
    filters: {
      permanent: [
        {
          field: "categoryId",
          operator: "null",
          value: "",
        },
      ],
    },
  });

  // Make sure query does not get refreshed on every category change
  useEffect(() => {
    refetch();
  }, [refetch]);

  return (
    <List
      headerButtons={() => (
        <>
          <RefreshButton onClick={() => refetch()} />
        </>
      )}
    >
      <InstitutionTransactionTable
        loading={isRefetching || loading}
        categoryColumn={
          <Table.Column
            dataIndex="categoryId"
            title={translate("institutiontransactions.fields.categoryId")}
            className={styles.category}
            render={(_, record: InstitutionTransaction, index) => (
              <TransactionCategorySelect
                fieldId={`transaction-category-${index}`}
                transaction={record}
              />
            )}
          />
        }
        {...tableProps}
      ></InstitutionTransactionTable>
    </List>
  );
}

export default UncategorizedTransactionView;
