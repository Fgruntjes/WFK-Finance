import { Institution } from "@api";
import { useMany } from "@refinedev/core";

function useInstiutionNameList(ids?: string[]) {
  ids = ids ?? [];
  // Filter unique ids
  ids = [...new Set(ids)];

  const {
    data: institutionData,
    isLoading: institutionIsLoading,
    isError: institutionIsError,
    error: institutionError,
  } = useMany<Institution>({
    resource: "institutions",
    ids: ids,
    queryOptions: {
      enabled: ids.length > 0,
    },
  });

  return {
    institutionData,
    institutionIsLoading,
    institutionIsError,
    institutionError,
  };
}
export default useInstiutionNameList;
