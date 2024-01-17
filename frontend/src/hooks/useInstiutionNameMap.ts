import { Institution } from "@api";
import { useMany } from "@refinedev/core";

type InstitutionList = Record<string, Institution>;

function useInstiutionNameMap(ids?: string[]) {
  ids = ids ?? [];
  // Filter unique ids
  ids = [...new Set(ids)];

  const { data, isLoading, isError, error } = useMany<Institution>({
    resource: "institutions",
    ids: ids,
    queryOptions: {
      enabled: ids.length > 0,
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
