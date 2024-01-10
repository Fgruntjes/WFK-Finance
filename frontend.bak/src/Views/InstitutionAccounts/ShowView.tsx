import DateField from "@Components/DateField";
import { InstitutionAccountTransactionsList } from "@Views";
import { NumberField, Show, TabbedShowLayout, TextField } from "react-admin";
import { useParams } from "react-router-dom";

function ShowView() {
  const { id } = useParams();

  return (
    <Show>
      <TabbedShowLayout>
        <TabbedShowLayout.Tab label="summary">
          <TextField source="iban" />
          <DateField source="lastImport" />
          <TextField source="importStatus" />
          <NumberField source="transactionCount" />
        </TabbedShowLayout.Tab>
        <TabbedShowLayout.Tab label="transactions">
          <InstitutionAccountTransactionsList accountId={id} />
        </TabbedShowLayout.Tab>
      </TabbedShowLayout>
    </Show>
  );
}

export default ShowView;
