import CurrencyField from "@Components/CurrencyField";
import DateField from "@Components/DateField";
import {
  Datagrid,
  List,
  RefreshButton,
  TextField,
  TopToolbar,
  useTranslate,
} from "react-admin";

const ListActions = () => (
  <TopToolbar>
    <RefreshButton />
  </TopToolbar>
);

type ListViewProps = {
  accountId: string | undefined;
};

function ListView({ accountId }: ListViewProps) {
  const translate = useTranslate();

  if (!accountId) {
    return null;
  }

  return (
    <List
      actions={<ListActions />}
      resource={`institutionaccount/${accountId}/transactions`}
    >
      <Datagrid>
        <DateField
          label={translate(
            "resources.institutionaccounttransaction.fields.date.name",
          )}
          source="date"
        />
        <CurrencyField
          label={translate(
            "resources.institutionaccounttransaction.fields.amount.name",
          )}
          source="amount"
        />
        <TextField
          label={translate(
            "resources.institutionaccounttransaction.fields.transactionCode.name",
          )}
          source="transactionCode"
        />
        <TextField
          label={translate(
            "resources.institutionaccounttransaction.fields.unstructuredInformation.name",
          )}
          source="unstructuredInformation"
        />
      </Datagrid>
    </List>
  );
}

export default ListView;
