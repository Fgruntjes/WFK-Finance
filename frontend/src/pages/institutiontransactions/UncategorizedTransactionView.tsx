import { InstitutionTransaction } from "@api";
import CurrencyField from "@components/field/CurrencyField";
import useInstiutionNameMap from "@hooks/useInstiutionNameMap";
import InstitutionsRecordRepresentation from "@pages/institutions/RecordRepresentation";
import {
  DateField,
  List,
  MarkdownField,
  TextField,
  useTable,
} from "@refinedev/antd";
import { HttpError, useTranslate } from "@refinedev/core";
import { Table } from "antd";
import styles from "./ListView.module.less";
import { TransactionCategorySelect } from "./uncategorized-transaction/TransactionCategorySelect";

function UncategorizedTransactionView() {
  const translate = useTranslate();
  const {
    tableProps: { loading, ...tableProps },
  } = useTable<InstitutionTransaction, HttpError>({
    syncWithLocation: true,
    resource: "institutiontransactions",
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

  const { institutionMap, institutionIsLoading } = useInstiutionNameMap(
    tableProps?.dataSource?.map((item) => item?.institutionId),
  );

  return (
    <List>
      <Table
        loading={loading || institutionIsLoading}
        {...tableProps}
        rowKey="id"
        className={styles.table}
      >
        <Table.Column
          dataIndex="institutionId"
          sorter={{ multiple: 1 }}
          title={translate("institutiontransactions.fields.institutionId")}
          render={(value) => (
            <InstitutionsRecordRepresentation
              recordItem={institutionMap[value]}
            />
          )}
        />
        <Table.Column
          dataIndex="accountIban"
          sorter={{ multiple: 2 }}
          title={translate("institutiontransactions.fields.accountIban")}
          render={(value) => (
            <TextField className={styles.noWrap} value={value} />
          )}
        />
        <Table.Column
          dataIndex="date"
          sorter={{ multiple: 3 }}
          title={translate("institutiontransactions.fields.date")}
          render={(value) => (
            <DateField
              className={styles.noWrap}
              format="ddd DD MMM YYYY"
              value={value}
            />
          )}
        />
        <Table.Column
          dataIndex="amount"
          sorter={{ multiple: 4 }}
          title={translate("institutiontransactions.fields.amount")}
          render={(value: number, record: InstitutionTransaction) => (
            <CurrencyField
              className={styles.noWrap}
              colorized
              currency={record.currency}
              value={value}
            />
          )}
        />
        <Table.Column
          dataIndex="categoryId"
          title={translate("institutiontransactions.fields.categoryId")}
          className={styles.description}
          render={(_, record: InstitutionTransaction) => (
            <TransactionCategorySelect transaction={record} />
          )}
        />
        <Table.Column
          dataIndex="unstructuredInformation"
          title={translate(
            "institutiontransactions.fields.unstructuredInformation",
          )}
          className={styles.description}
          render={(value: string) => (
            <MarkdownField value={value.replaceAll("<br>", "\n\n")} />
          )}
        />
      </Table>
    </List>
  );
}

export default UncategorizedTransactionView;
