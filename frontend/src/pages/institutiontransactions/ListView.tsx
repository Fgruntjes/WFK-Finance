import { InstitutionTransaction } from "@api";
import CurrencyField from "@components/field/CurrencyField";
import useInstiutionNameList from "@hooks/useInstitutionNameList";
import InstitutionsRecordRepresentation from "@pages/institutions/RecordRepresentation";
import { DateField, List, TextField, useTable } from "@refinedev/antd";
import { HttpError, useTranslate } from "@refinedev/core";
import { Table } from "antd";

function ListView() {
  const translate = useTranslate();
  const {
    tableProps: { loading, ...tableProps },
  } = useTable<InstitutionTransaction, HttpError>({
    syncWithLocation: true,
    resource: "institutiontransactions",
    sorters: {
      initial: [{ field: "date", order: "desc" }],
    },
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
          sorter={{ multiple: 1 }}
          title={translate("institutiontransactions.fields.institutionId")}
          render={(value) => (
            <InstitutionsRecordRepresentation
              recordItem={institutionData?.data?.find(
                (item) => item.id === value,
              )}
            />
          )}
        />
        <Table.Column
          dataIndex={["accountIban"]}
          sorter={{ multiple: 2 }}
          title={translate("institutiontransactions.fields.accountIban")}
          render={(value) => <TextField value={value} />}
        />
        <Table.Column
          dataIndex={"date"}
          sorter={{ multiple: 3 }}
          title={translate("institutiontransactions.fields.date")}
          render={(value) => (
            <DateField format="ddd DD MMM YYYY" value={value} />
          )}
        />
        <Table.Column
          dataIndex={"amount"}
          sorter={{ multiple: 4 }}
          title={translate("institutiontransactions.fields.amount")}
          render={(value: number, record: InstitutionTransaction) => (
            <CurrencyField colorized currency={record.currency} value={value} />
          )}
        />
        <Table.Column
          dataIndex={"unstructuredInformation"}
          title={translate(
            "institutiontransactions.fields.unstructuredInformation",
          )}
          render={(value) => <TextField value={value} />}
        />
      </Table>
    </List>
  );
}

export default ListView;
