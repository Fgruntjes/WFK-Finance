import { mergeQueryKeys } from "@lukemorales/query-key-factory";
import { institutionConnectionMutation, institutionConnectionQuery } from "./institutionConnectionQuery";
import { institutionQuery } from "./institutionQuery";

export const queries = mergeQueryKeys(
    institutionConnectionQuery,
    institutionConnectionMutation,
    institutionQuery,
);

export default queries;