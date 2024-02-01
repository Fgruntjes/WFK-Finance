import { TransactionCategory } from "@api";

export type CategoryGroup = TransactionCategory & {
  children: TransactionCategory[];
};
