import { InstitutionAccountTransaction } from "@api";
import useInstiutionNameList from "@hooks/useInstitutionNameList";
import InstitutionsRecordRepresentation from "@pages/institutions/RecordRepresentation";
import { List, useTable } from "@refinedev/antd";
import { HttpError, useTranslate } from "@refinedev/core";
import { Table } from "antd";

function ListView() {
  const translate = useTranslate();
  const {
    tableProps: { loading, ...tableProps },
  } = useTable<InstitutionAccountTransaction, HttpError>({
    syncWithLocation: true,
    resource: "institutiontransactions",
  });

  const { institutionData, institutionIsLoading } = useInstiutionNameList(
    tableProps?.dataSource?.map((item) => item?.institutionId),
  );

  return (
    <List>
      <Table
        loading={loading || institutionIsLoading}
        {...tableProps}
        rowKey="id"
      >
        <Table.Column
          dataIndex={["institutionId"]}
          title={translate("institutionconnections.fields.institutionId")}
          render={(value) => (
            <InstitutionsRecordRepresentation
              recordItem={institutionData?.data?.find(
                (item) => item.id === value,
              )}
            />
          )}
        />
      </Table>
    </List>
  );
}

export default ListView;
