import { InstitutionAccount } from "@api";
import Tabs from "@components/Tabs";
import { RefreshButton, Show } from "@refinedev/antd";
import { useInvalidate, useShow, useTranslate } from "@refinedev/core";
import { DetailsTab } from "./show-view/DetailsTab";
import { TransactionsTab } from "./show-view/TransactionsTab";

function ShowView() {
  const translate = useTranslate();
  const invalidate = useInvalidate();
  const {
    queryResult: { data, isLoading },
  } = useShow<InstitutionAccount>();

  const tabItems = [
    {
      key: "general",
      label: translate("institutionaccounts.tabs.general"),
      children: data && <DetailsTab record={data.data} />,
    },
    {
      key: "transactions",
      label: translate("institutionaccounts.tabs.transactions"),
      children: data && <TransactionsTab record={data.data} />,
    },
  ];

  function onRefreshClick() {
    invalidate({
      id: data?.data.id,
      invalidates: ["detail"],
      resource: "institutionaccounts",
    });
    invalidate({
      invalidates: ["list"],
      resource: "institutionaccounttransactions",
      invalidationFilters: {
        // Only refresh transactions on this page
        predicate: (query) =>
          (query.queryKey as { operation: string }[])[4].operation ==
          `institutionaccount/${data?.data.id}/transactions`,
      },
    });
  }

  return (
    <Show
      isLoading={isLoading}
      headerButtons={({ refreshButtonProps }) => (
        <>
          {refreshButtonProps && (
            <RefreshButton {...refreshButtonProps} onClick={onRefreshClick} />
          )}
        </>
      )}
    >
      <Tabs defaultActiveKey="1" items={tabItems} />
    </Show>
  );
}

export default ShowView;
