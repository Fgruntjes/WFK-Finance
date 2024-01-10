import { ReferenceField, Show, SimpleShowLayout } from "react-admin";

function ShowView() {
  return (
    <Show>
      <SimpleShowLayout>
        <ReferenceField source="institution" reference="institutions" />
      </SimpleShowLayout>
    </Show>
  );
}

export default ShowView;
