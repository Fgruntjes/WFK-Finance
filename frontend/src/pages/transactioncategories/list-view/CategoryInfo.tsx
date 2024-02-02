import { TransactionCategory } from "@api";
import { Typography } from "antd";
import styles from "./CategoryInfo.module.less";
import { GroupTag } from "./GroupTag";

type CategoryInfoProps = {
  item: TransactionCategory;
  children?: React.ReactNode;
};

function CategoryInfo({ item, children }: CategoryInfoProps) {
  return (
    <>
      <div>
        <Typography.Text>
          <GroupTag type={item.group} />
          {item.name}
        </Typography.Text>
        {item.description && (
          <p className={styles.description}>{item.description}</p>
        )}
      </div>
      {children && <div>{children}</div>}
    </>
  );
}

export default CategoryInfo;
