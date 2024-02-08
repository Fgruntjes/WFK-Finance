import { InstitutionTransaction } from "@api";
import CurrencyField from "@components/field/CurrencyField";
import useInstiutionNameMap from "@hooks/useInstiutionNameMap";
import useTransactionCategoryMap from "@hooks/useTransactionCategoryMap";
import InstitutionsRecordRepresentation from "@pages/institutions/RecordRepresentation";
import { DateField, TextField } from "@refinedev/antd";
import { useTranslate } from "@refinedev/core";
import { Table as AntTable, type TableProps as AntTableProps } from "antd";
import { ReactNode } from "react";
import styles from "./Table.module.less";
import { UnstructuredInformationField } from "./UnstructuredInformationField";

type TableProps = {
  showInstitutionColumn?: boolean;
  categoryColumn?: boolean | ReactNode;
} & AntTableProps<InstitutionTransaction>;

function Table({
  showInstitutionColumn = true,
  categoryColumn = true,
  loading,
  ...tableProps
}: TableProps) {
  const translate = useTranslate();
  const { categoryMap, categoryMapIsLoading } = useTransactionCategoryMap(
    tableProps?.dataSource?.map((item) => item?.categoryId),
  );
  const { institutionMap, institutionIsLoading } = useInstiutionNameMap(
    tableProps?.dataSource?.map((item) => item?.institutionId),
    {
      enabled: showInstitutionColumn,
    },
  );

  const isLoading =
    loading ||
    (institutionIsLoading && showInstitutionColumn) ||
    categoryMapIsLoading;

  return (
    <AntTable
      {...tableProps}
      loading={isLoading}
      rowKey="id"
      className={styles.table}
    >
      {showInstitutionColumn && (
        <AntTable.Column
          dataIndex={["institutionId"]}
          sorter={{ multiple: 1 }}
          title={translate("institutiontransactions.fields.institutionId")}
          render={(value) => (
            <InstitutionsRecordRepresentation
              recordItem={institutionMap[value]}
            />
          )}
        />
      )}
      <AntTable.Column
        dataIndex={"date"}
        sorter={{ multiple: 3 }}
        title={translate("institutiontransactions.fields.date")}
        render={(value) => (
          <DateField
            className={styles.noWrap}
            format="ddd DD MMM YYYY"
            value={value}
          />
        )}
      />
      {categoryColumn === true && (
        <AntTable.Column
          dataIndex={"categoryId"}
          sorter={{ multiple: 4 }}
          title={translate("institutiontransactions.fields.categoryId")}
          render={(value) => categoryMap[value]?.name}
        />
      )}
      {typeof categoryColumn !== "boolean" && categoryColumn}
      <AntTable.Column
        dataIndex={"amount"}
        sorter={{ multiple: 5 }}
        title={translate("institutiontransactions.fields.amount")}
        render={(value: number, record: InstitutionTransaction) => (
          <CurrencyField
            className={styles.noWrap}
            colorized
            currency={record.currency}
            value={value}
          />
        )}
      />
      <AntTable.Column
        dataIndex={["counterPartyName"]}
        sorter={{ multiple: 2 }}
        title={translate("institutiontransactions.fields.counterPartyName")}
        render={(value) => (
          <TextField className={styles.noWrap} value={value} />
        )}
      />
      <AntTable.Column
        dataIndex={["counterPartyAccount"]}
        sorter={{ multiple: 2 }}
        title={translate("institutiontransactions.fields.counterPartyAccount")}
        render={(value) => (
          <TextField className={styles.noWrap} value={value} />
        )}
      />
      <AntTable.Column
        dataIndex={"unstructuredInformation"}
        title={translate(
          "institutiontransactions.fields.unstructuredInformation",
        )}
        render={(value: string) => (
          <UnstructuredInformationField value={value} />
        )}
      />
    </AntTable>
  );
}

export default Table;
