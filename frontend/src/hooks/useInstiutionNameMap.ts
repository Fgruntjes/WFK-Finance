import { Institution } from "@api";
import { GetManyResponse, HttpError, useMany } from "@refinedev/core";
import { UseQueryOptions } from "@tanstack/react-query";

type InstitutionList = Record<string, Institution>;
type QueryOptions = UseQueryOptions<
  GetManyResponse<Institution>,
  HttpError,
  GetManyResponse<Institution>
>;

function useInstiutionNameMap(
  ids?: string[],
  { enabled, ...queryOptions }: QueryOptions = {},
) {
  ids = ids ?? [];
  // Filter unique ids
  ids = [...new Set(ids)];

  const { data, isLoading, isError, error } = useMany<Institution>({
    resource: "institutions",
    ids: ids,
    queryOptions: {
      enabled: enabled && ids.length > 0,
      ...queryOptions,
    },
  });

  const institutionMap = data?.data.reduce((obj: InstitutionList, item) => {
    obj[item.id] = item;
    return obj;
  }, {});

  return {
    institutionMap: institutionMap ?? {},
    institutionIsLoading: isLoading && ids.length > 0,
    institutionIsError: isError,
    institutionError: error,
  };
}

export default useInstiutionNameMap;
