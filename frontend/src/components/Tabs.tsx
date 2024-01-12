import { Tabs as AntdTabs, TabsProps as AntdTabsProps } from "antd";
import { useSearchParams } from "react-router-dom";

type TabsProps = {
  syncWithLocation?: boolean;
} & AntdTabsProps;

function Tabs({
  syncWithLocation = true,
  defaultActiveKey,
  ...props
}: TabsProps) {
  const [searchParams, setSearchParams] = useSearchParams();

  if (syncWithLocation) {
    const activeTabKey = searchParams.get("tab");
    if (activeTabKey) {
      defaultActiveKey = activeTabKey;
    }
  }

  function onChange(activeKey: string) {
    if (syncWithLocation) {
      setSearchParams({ tab: activeKey });
    }
    props.onChange?.(activeKey);
  }

  return (
    <AntdTabs
      defaultActiveKey={defaultActiveKey}
      onChange={onChange}
      {...props}
    />
  );
}

export default Tabs;
