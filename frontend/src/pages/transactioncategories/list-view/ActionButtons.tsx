import { DownOutlined, UpOutlined } from "@ant-design/icons";
import { TransactionCategory } from "@api";
import useErrorNotification from "@hooks/useErrorNotification";
import { DeleteButton } from "@refinedev/antd";
import { MoveButton } from "./MoveButton";

type ActionButtonsProps = {
  item: TransactionCategory;
  onMoveUp?: () => void;
  onMoveDown?: () => void;
  children?: React.ReactNode;
};

export function ActionButtons({
  item,
  children,
  onMoveUp,
  onMoveDown,
}: ActionButtonsProps) {
  const errorNotification = useErrorNotification();

  return (
    <div>
      {children}
      <DeleteButton
        resource="transactioncategories"
        recordItemId={item.id}
        type="text"
        errorNotification={errorNotification}
        hideText
      />
      <MoveButton icon={<UpOutlined />} onClick={onMoveUp} />
      <MoveButton icon={<DownOutlined />} onClick={onMoveDown} />
    </div>
  );
}
