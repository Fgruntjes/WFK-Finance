import DateField from "@Components/DateField";
import ButtonGroup from "@mui/material/ButtonGroup";
import {
  ArrayField,
  CreateButton,
  Datagrid,
  List,
  NumberField,
  ReferenceField,
  ShowButton,
  TextField,
  TopToolbar,
} from "react-admin";
import RefreshButton from "./RefreshButton";

const ListActions = () => (
  <TopToolbar>
    <CreateButton />
  </TopToolbar>
);

function ListView() {
  return (
    <List actions={<ListActions />} storeKey="createdAt">
      <Datagrid>
        <ReferenceField source="institutionId" reference="institutions" />
        <ArrayField source="accounts">
          <Datagrid bulkActionButtons={false}>
            <TextField source="iban" />
            <TextField source="importStatus" />
            <DateField source="lastImport" showTime />
            <NumberField source="transactionCount" />
            <ShowButton resource="institutionaccounts" />
          </Datagrid>
        </ArrayField>
        <ButtonGroup>
          <RefreshButton />
          <ShowButton />
        </ButtonGroup>
      </Datagrid>
    </List>
  );
}

export default ListView;
