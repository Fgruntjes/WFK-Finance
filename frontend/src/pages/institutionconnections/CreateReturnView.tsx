import { Institution, InstitutionConnection } from "@api";
import LocalError from "@components/LocalError";
import LocalHttpError from "@components/LocalHttpError";
import InstitutionAccountsTable from "@pages/institutionconnections/AccountTable";
import InstitutionsRecordRepresentation from "@pages/institutions/RecordRepresentation";
import { ListButton, Show } from "@refinedev/antd";
import { useCustom, useOne, useTranslate } from "@refinedev/core";
import { Typography } from "antd";
import Title from "antd/es/typography/Title";
import { useSearchParams } from "react-router-dom";

function CreateReturnView() {
  const [searchParams] = useSearchParams();
  const translate = useTranslate();

  const isConnectError = !!searchParams.get("error");
  const connectError = searchParams.get("details");
  const externalId = searchParams.get("ref") || "";

  const { data, isLoading, error, isError } = useCustom<InstitutionConnection>({
    url: `/institutionconnections/refresh/external/${externalId}`,
    method: "patch",
    queryOptions: {
      enabled: !!externalId,
      retry: false,
    },
  });
  const {
    data: institutionData,
    isLoading: institutionIsLoading,
    isError: institutionIsError,
    error: institutionError,
  } = useOne<Institution>({
    resource: "institutions",
    id: data?.data?.institutionId || "",
    queryOptions: {
      enabled: !isLoading && !isError,
    },
  });

  if (isError || institutionIsError) {
    return <LocalHttpError error={error || institutionError || undefined} />;
  }
  if (isConnectError) {
    return <LocalError error={connectError} />;
  }

  return (
    <Show
      isLoading={isLoading || institutionIsLoading}
      title={translate("institutionconnections.titles.return")}
      recordItemId={data?.data?.id}
      resource="institutionconnections"
      goBack={false}
      headerButtons={({ listButtonProps }) => (
        <ListButton
          {...listButtonProps}
          resource="institutionaccounts"
          title={translate("buttons.returnToList")}
        >
          {translate("buttons.returnToList")}
        </ListButton>
      )}
    >
      <Title level={5}>
        {translate("institutionconnections.fields.institutionId")}
      </Title>
      <Typography.Text>
        <InstitutionsRecordRepresentation recordItem={institutionData?.data} />
      </Typography.Text>

      <Title level={5}>
        {translate("institutionconnections.fields.accounts")}
      </Title>
      <InstitutionAccountsTable
        items={data?.data.accounts}
        hideTransactionCount
        size="small"
        pagination={false}
      />
    </Show>
  );
}

export default CreateReturnView;
