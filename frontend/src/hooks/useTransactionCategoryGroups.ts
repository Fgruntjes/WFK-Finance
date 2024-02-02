import { TransactionCategory } from "@api";
import { HttpError, useList } from "@refinedev/core";
import { useMemo } from "react";

export type CategoryGroup = TransactionCategory & {
  children: TransactionCategory[];
};

export function groupCategoryData(
  data?: TransactionCategory[],
  parentData?: TransactionCategory[],
) {
  const groups: { [key: string]: CategoryGroup } = {};
  const items = data || [];
  const parentItems = parentData || items;

  parentItems
    .filter((category) => !category.parentId)
    .forEach((category) => {
      groups[category.id] = {
        ...category,
        children: [],
      };
    });

  items
    .filter((category) => !!category.parentId)
    .forEach((category) => {
      groups[category.parentId ?? ""].children.push(category);
    });

  const groupsArray = Object.values(groups).sort(
    (a, b) => a.sortOrder - b.sortOrder,
  );
  groupsArray.forEach((group) => {
    group.children = group.children.sort((a, b) => a.sortOrder - b.sortOrder);
  });

  return groupsArray;
}

function useTransactionCategoryGroups() {
  const { data, ...otherResult } = useList<TransactionCategory, HttpError>({
    resource: "transactioncategories",
    pagination: {
      pageSize: 250,
    },
  });

  const groups = useMemo(() => groupCategoryData(data?.data), [data]);

  return {
    groups,
    data,
    ...otherResult,
  };
}

export default useTransactionCategoryGroups;
