import { InstitutionTransaction } from "@api";
import InstitutionTransactionTable from "@components/institutiontransactions/Table";
import { List, useTable } from "@refinedev/antd";
import { HttpError } from "@refinedev/core";

function ListView() {
  const { tableProps } = useTable<InstitutionTransaction, HttpError>({
    syncWithLocation: true,
    resource: "institutiontransactions",
    sorters: {
      initial: [{ field: "date", order: "desc" }],
    },
  });

  return (
    <List>
      <InstitutionTransactionTable {...tableProps} />;
    </List>
  );
}

export default ListView;
