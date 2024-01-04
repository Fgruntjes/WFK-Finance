import CurrencyField from "@Components/CurrencyField";
import DateField from "@Components/DateField";
import {
  Datagrid,
  List,
  NumberField,
  Show,
  TabbedShowLayout,
  TextField,
  useTranslate,
} from "react-admin";
import { useParams } from "react-router-dom";

function ShowView() {
  const { id } = useParams();
  const translate = useTranslate();

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
          <List resource={`institutionaccount/${id}/transactions`}>
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
        </TabbedShowLayout.Tab>
      </TabbedShowLayout>
    </Show>
  );
}

export default ShowView;
