import { Tag } from "antd";
import { PresetColorType } from "antd/es/_util/colors";
import { TransactionCategoryGroup } from "../../../Api";

type CategoryGroupProps = {
  type: TransactionCategoryGroup;
};

const ColorTypeMap: Record<
  TransactionCategoryGroup,
  PresetColorType | undefined
> = {
  [TransactionCategoryGroup.Expense]: "red",
  [TransactionCategoryGroup.Income]: "green",
  [TransactionCategoryGroup.Investment]: "lime",
  [TransactionCategoryGroup.Transfer]: "blue",
  [TransactionCategoryGroup.Liquididation]: "red",
  [TransactionCategoryGroup.Other]: undefined,
};

export function GroupTag({ type }: CategoryGroupProps) {
  return <Tag color={ColorTypeMap[type]}>{type}</Tag>;
}
