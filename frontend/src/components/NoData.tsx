import { useTranslate } from "@refinedev/core";
import { Empty } from "antd";

type NoDataProps = {
  children?: React.ReactNode;
};

function NoData({ children }: NoDataProps) {
  const translate = useTranslate();

  return (
    <Empty description={translate("components.noData.text")}>{children}</Empty>
  );
}

export default NoData;
