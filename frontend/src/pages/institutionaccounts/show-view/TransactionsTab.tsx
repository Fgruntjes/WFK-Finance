import { InstitutionAccount, InstitutionAccountTransaction } from "@api";
import CurrencyField from "@components/field/CurrencyField";
import DateField from "@components/field/DateField";
import { TextField, useTable } from "@refinedev/antd";
import { HttpError, useTranslate } from "@refinedev/core";
import { Table } from "antd";

type TransactionTabTabProps = {
  record: InstitutionAccount;
};
export function TransactionsTab({ record }: TransactionTabTabProps) {
  const translate = useTranslate();
  const {
    tableProps: { loading, ...tableProps },
  } = useTable<InstitutionAccountTransaction, HttpError>({
    resource: "institutionaccounttransactions",
    sorters: {
      initial: [{ field: "date", order: "desc" }],
    },
    meta: {
      operation: `institutionaccount/${record.id}/transactions`,
    },
    syncWithLocation: true,
  });

  return (
    <Table loading={loading} {...tableProps} rowKey="id">
      <Table.Column
        dataIndex={"date"}
        title={translate("institutionaccounttransactions.fields.date")}
        render={(value) => <DateField format="ddd DD MMM YYYY" value={value} />}
      />
      <Table.Column
        dataIndex={"amount"}
        title={translate("institutionaccounttransactions.fields.amount")}
        render={(value: number, record: InstitutionAccountTransaction) => (
          <CurrencyField colorized currency={record.currency} value={value} />
        )}
      />
      <Table.Column
        dataIndex={"transactionCode"}
        title={translate(
          "institutionaccounttransactions.fields.transactionCode",
        )}
        render={(value) => <TextField value={value} />}
      />
      <Table.Column
        dataIndex={"unstructuredInformation"}
        title={translate(
          "institutionaccounttransactions.fields.unstructuredInformation",
        )}
        render={(value) => <TextField value={value} />}
      />
    </Table>
  );
}
