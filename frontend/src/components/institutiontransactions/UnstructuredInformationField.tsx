import { DownOutlined, RightOutlined } from "@ant-design/icons";
import SafeHtml from "@components/SafeHtml";
import { useState } from "react";
import styles from "./UnstructuredInformationField.module.less";

export function UnstructuredInformationField({ value }: { value: string }) {
  const [expanded, setExpanded] = useState(false);

  return (
    <div
      className={`${styles.container} ${expanded || styles.collapsed}`}
      onClick={() => setExpanded(!expanded)}
    >
      {expanded ? (
        <DownOutlined className={styles.icon} />
      ) : (
        <RightOutlined className={styles.icon} />
      )}
      <SafeHtml className={styles.text} html={value} />
    </div>
  );
}
