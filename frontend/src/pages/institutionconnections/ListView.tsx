import { Institution, InstitutionAccount, InstitutionConnection } from "@api";
import {
  InstitutionAccountsTable,
  InstitutionsRecordRepresentation,
} from "@pages";
import { DeleteButton, List, useTable } from "@refinedev/antd";
import { HttpError, useMany, useTranslate } from "@refinedev/core";
import { Space, Table } from "antd";

function ListView() {
  const translate = useTranslate();
  const {
    tableProps: { loading, ...tableProps },
  } = useTable<InstitutionConnection, HttpError>({
    syncWithLocation: true,
    resource: "institutionconnections",
  });

  const { data: institutionData, isLoading: institutionIsLoading } =
    useMany<Institution>({
      resource: "institutions",
      ids: tableProps?.dataSource?.map((item) => item?.institutionId) ?? [],
      queryOptions: {
        enabled: !!tableProps?.dataSource,
      },
    });

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
