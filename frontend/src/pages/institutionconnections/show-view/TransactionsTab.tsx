import { InstitutionAccount, InstitutionTransaction } from "@api";
import InstitutionTransactionTable from "@components/institutiontransactions/Table";
import { useTable } from "@refinedev/antd";
import { HttpError } from "@refinedev/core";

type TransactionTabTabProps = {
  record: InstitutionAccount;
};
export function TransactionsTab({ record }: TransactionTabTabProps) {
  const { tableProps } = useTable<InstitutionTransaction, HttpError>({
    resource: "institutionaccounttransactions",
    sorters: {
      initial: [{ field: "date", order: "desc" }],
    },
    meta: {
      operation: `institutionaccounts/${record.id}/transactions`,
    },
    syncWithLocation: true,
  });

  return (
    <InstitutionTransactionTable
      {...tableProps}
      showInstitutionColumn={false}
      categoryColumn={false}
    />
  );
}
