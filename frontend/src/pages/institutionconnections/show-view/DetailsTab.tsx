import { Institution, InstitutionAccount } from "@api";
import Loader from "@components/Loader";
import LocalHttpError from "@components/LocalHttpError";
import DateField from "@components/field/DateField";
import InstitutionsRecordRepresentation from "@pages/institutions/RecordRepresentation";
import { NumberField, TextField } from "@refinedev/antd";
import { useOne, useTranslate } from "@refinedev/core";
import { Space, Typography } from "antd";
import { ImportStatusBadge } from "../ImportStatusBadge";

const { Title, Text } = Typography;

type DetailsTabProps = {
  record: InstitutionAccount;
};
export function DetailsTab({ record }: DetailsTabProps) {
  const translate = useTranslate();
  const {
    data: institutionData,
    isLoading: institutionIsLoading,
    isError: institutionIsError,
    error: institutionError,
  } = useOne<Institution>({
    resource: "institutions",
    id: record.institutionId,
  });

  if (!record) {
    return null;
  }

  return (
    <>
      <Title level={5}>
        {translate("institutionaccounts.fields.institution")}
      </Title>
      <Text>
        {institutionIsLoading && <Loader />}
        {institutionIsError && <LocalHttpError error={institutionError} />}
        {institutionData?.data && (
          <InstitutionsRecordRepresentation
            recordItem={institutionData?.data}
          />
        )}
      </Text>
      <Title level={5}>{translate("institutionaccounts.fields.iban")}</Title>
      <TextField value={record.iban} />

      <Title level={5}>
        {translate("institutionaccounts.fields.importStatus")}
      </Title>
      <TextField value={record.importStatus} />

      <Title level={5}>
        {translate("institutionaccounts.fields.lastImport")}
      </Title>
      <Text>
        <Space direction="horizontal" size="small">
          {record.lastImport ? (
            <DateField value={record.lastImport} showTime />
          ) : (
            translate("institutionaccounts.fields.lastImportNever")
          )}
          <ImportStatusBadge status={record.importStatus} />
        </Space>
      </Text>

      <Title level={5}>
        {translate("institutionaccounts.fields.transactionCount")}
      </Title>
      <NumberField value={record.transactionCount ?? ""} />
    </>
  );
}
