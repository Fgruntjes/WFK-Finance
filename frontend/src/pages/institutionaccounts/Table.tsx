import { InstitutionAccount, InstitutionAccountImportStatusEnum } from "@api";
import DateField from "@components/field/DateField";
import { NumberField, ShowButton } from "@refinedev/antd";
import { useTranslate } from "@refinedev/core";
import { Table as AntdTable, TableProps as AntdTableProps } from "antd";
import { ColumnsType } from "antd/es/table";
import { ImportStatusBadge } from "./ImportStatusBadge";

type TableProps = {
  items?: InstitutionAccount[];
  hideTransactionCount?: boolean;
} & AntdTableProps<InstitutionAccount>;

function Table({
  items,
  hideTransactionCount = false,
  ...tableProps
}: TableProps) {
  const translate = useTranslate();
  const columns: ColumnsType<InstitutionAccount> = [
    {
      title: translate("institutionaccounts.fields.iban"),
      dataIndex: "iban",
      key: "iban",
    },
    {
      title: translate("institutionaccounts.fields.importStatus"),
      dataIndex: "importStatus",
      key: "importStatus",
      render: (value: InstitutionAccountImportStatusEnum) => (
        <ImportStatusBadge status={value} />
      ),
    },
    {
      title: translate("institutionaccounts.fields.lastImport"),
      dataIndex: "lastImport",
      key: "lastImport",
      render: (value) => <DateField value={value} showTime />,
    },
    {
      title: "",
      dataIndex: "id",
      key: "id",
      render: (value) => (
        <ShowButton resource="institutionaccounts" recordItemId={value} />
      ),
    },
  ];

  if (!hideTransactionCount) {
    columns.splice(2, 0, {
      title: translate("institutionaccounts.fields.transactionCount"),
      dataIndex: "transactionCount",
      key: "transactionCount",
      render: (value) => <NumberField value={value} />,
    });
  }

  const dataSource = items?.map((item) => ({
    key: item.id,
    ...item,
  }));
  return (
    <AntdTable dataSource={dataSource} columns={columns} {...tableProps} />
  );
}

export default Table;
