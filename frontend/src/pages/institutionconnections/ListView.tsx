import { InstitutionAccount, InstitutionConnection } from "@api";
import useInstiutionNameList from "@hooks/useInstitutionNameList";
import InstitutionAccountsTable from "@pages/institutionconnections/AccountTable";
import InstitutionsRecordRepresentation from "@pages/institutions/RecordRepresentation";
import { DeleteButton, List, useTable } from "@refinedev/antd";
import { HttpError, useTranslate } from "@refinedev/core";
import { Space, Table } from "antd";

function ListView() {
  const translate = useTranslate();
  const {
    tableProps: { loading, ...tableProps },
  } = useTable<InstitutionConnection, HttpError>({
    syncWithLocation: true,
    resource: "institutionconnections",
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

        <Table.Column
          dataIndex="accounts"
          title={translate("institutionconnections.fields.accounts")}
          render={(value: InstitutionAccount[]) => (
            <InstitutionAccountsTable
              items={value}
              size="small"
              pagination={false}
            />
          )}
        />
        <Table.Column
          title={translate("table.actions")}
          dataIndex="actions"
          render={(_, record: InstitutionConnection) => (
            <Space>
              <DeleteButton
                hideText
                size="small"
                recordItemId={record.id}
                resource="institutionconnections"
              />
            </Space>
          )}
        />
      </Table>
    </List>
  );
}

export default ListView;
