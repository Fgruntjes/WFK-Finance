import { TransactionCategory } from "@api";
import { useMany } from "@refinedev/core";

type IdType = string | undefined | null;
type TransactionCategoryList = Record<string, TransactionCategory>;

function useTransactionCategoryMap(ids?: IdType[]) {
  let actualIds = (ids ?? []).filter((id) => !!id) as string[];

  // Filter unique ids
  actualIds = [...new Set(actualIds)];

  const { data, isLoading, isError, error } = useMany<TransactionCategory>({
    resource: "transactioncategories",
    ids: actualIds,
    queryOptions: {
      enabled: actualIds.length > 0,
    },
  });

  const institutionMap = data?.data.reduce(
    (obj: TransactionCategoryList, item) => {
      obj[item.id] = item;
      return obj;
    },
    {},
  );

  return {
    categoryMap: institutionMap ?? {},
    categoryMapIsLoading: isLoading && actualIds.length > 0,
    categoryMapIsError: isError,
    categoryMapError: error,
  };
}

export default useTransactionCategoryMap;
