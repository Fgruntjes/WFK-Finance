import {
  ExclamationOutlined,
  LoadingOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import { InstitutionTransaction } from "@api";
import LocalError from "@components/LocalError";
import { useCustom, useTranslate } from "@refinedev/core";
import { Button, Tooltip } from "antd";
import { useState } from "react";
import SimilarTransactionsModal from "./SimilarTransactionsModal";

type FindSimilarButtonProps = {
  transaction: InstitutionTransaction;
  show: boolean;
};

function FindSimilarButton({ transaction, show }: FindSimilarButtonProps) {
  const translate = useTranslate();
  const [openModal, setOpenModal] = useState(false);
  const { data, isFetched, isRefetching, error, isError, refetch } = useCustom<
    InstitutionTransaction[]
  >({
    url: `/institutiontransactions/${transaction.id}/similar`,
    method: "get",
    queryOptions: {
      enabled: show,
      retry: false,
    },
  });

  if (!show) {
    return <Button type="text" />;
  }

  if (isRefetching) {
    return <Button type="text" icon={<LoadingOutlined />} />;
  }

  if (isError) {
    return (
      <Tooltip title={<LocalError error={error} />}>
        <Button type="text" icon={<ExclamationOutlined />} />
      </Tooltip>
    );
  }

  if (!isFetched) {
    return (
      <Button type="text" icon={<SearchOutlined />} onClick={() => refetch()} />
    );
  }

  const itemsFound = data?.data?.length || 0;
  return (
    <>
      <SimilarTransactionsModal
        open={openModal}
        setOpen={setOpenModal}
        transaction={transaction}
        similarTransactions={data?.data}
      />
      <Tooltip
        title={translate("uncategorizedtransactions.inputs.foundSimilar")}
      >
        <Button
          shape="circle"
          type={itemsFound > 0 ? "primary" : "default"}
          onClick={() => setOpenModal(true)}
        >
          {itemsFound}
        </Button>
      </Tooltip>
    </>
  );
}

export default FindSimilarButton;
