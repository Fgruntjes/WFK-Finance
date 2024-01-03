import ButtonGroup from "@mui/material/ButtonGroup";
import {
  ArrayField,
  ChipField,
  CreateButton,
  Datagrid,
  List,
  ReferenceField,
  ShowButton,
  SingleFieldList,
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
          <SingleFieldList linkType={false}>
            <ChipField source="iban" size="small" />
          </SingleFieldList>
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
