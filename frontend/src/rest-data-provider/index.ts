import { ProblemDetails } from "@api";
import {
  BaseKey,
  BaseRecord,
  CreateManyParams,
  CreateManyResponse,
  CreateParams,
  CreateResponse,
  CustomParams,
  CustomResponse,
  DataProvider,
  DeleteManyParams,
  DeleteManyResponse,
  DeleteOneParams,
  DeleteOneResponse,
  GetListParams,
  GetListResponse,
  GetManyParams,
  GetManyResponse,
  GetOneParams,
  GetOneResponse,
  HttpError,
  MetaQuery,
  UpdateManyParams,
  UpdateManyResponse,
  UpdateParams,
  UpdateResponse,
} from "@refinedev/core";
import { AxiosError, AxiosInstance, AxiosResponse } from "axios";
import { stringify } from "query-string";
import { axiosInstance, generateFilter, generateSort } from "./utils";

type MethodTypes = "get" | "delete" | "head" | "options";
type MethodTypesWithBody = "post" | "put" | "patch";

function dataProvider(
  apiUrl: string,
  httpClient: AxiosInstance = axiosInstance,
): Required<DataProvider> {
  function resourceListUrl(resource: string, meta?: MetaQuery, query?: object) {
    const url = `${apiUrl}/${meta?.operation ?? resource}`;

    return query ? `${url}?${stringify(query)}` : url;
  }
  function resourceUrl(
    resource: string,
    id: BaseKey,
    meta?: MetaQuery,
    query?: object,
  ) {
    const url = `${apiUrl}/${meta?.operation ?? resource}/${id}`;

    return query ? `${url}?${stringify(query)}` : url;
  }

  async function request<TResponseData = unknown>(
    method: MethodTypes,
    url: string,
    meta?: MetaQuery,
  ): Promise<AxiosResponse<TResponseData>> {
    const { headers, method: methodFromMeta } = meta ?? {};
    const requestMethod = (methodFromMeta as MethodTypes) ?? method;

    try {
      return await httpClient[requestMethod](url, { headers });
    } catch (error: unknown) {
      const axiosError = error as AxiosError;
      if (axiosError.response && axiosError.response.data) {
        const problemDetails = axiosError.response.data as ProblemDetails;

        throw {
          message: problemDetails.detail,
          statusCode: problemDetails.status,
        } as HttpError;
      }

      throw error;
    }
  }

  async function requestWithData<
    TResponseData = unknown,
    TRequestData = unknown,
  >(
    method: MethodTypesWithBody,
    url: string,
    variables: TRequestData,
    meta?: MetaQuery,
  ): Promise<AxiosResponse<TResponseData, TRequestData>> {
    const {
      headers,
      method: methodFromMeta,
      variables: metaVariables,
    } = meta ?? {};
    const requestMethod = (methodFromMeta as MethodTypesWithBody) ?? method;

    return await httpClient[requestMethod](
      url,
      { ...variables, ...metaVariables },
      { headers },
    );
  }

  async function getList<TData extends BaseRecord = BaseRecord>({
    resource,
    pagination,
    sorters,
    filters,
    meta,
  }: GetListParams): Promise<GetListResponse<TData>> {
    const { current = 1, pageSize = 25, mode = "server" } = pagination ?? {};

    const query: {
      page?: number;
      pageSize?: number;
      orderBy?: string;
      filter?: string;
    } = {};

    if (mode === "server") {
      query.page = current;
      query.pageSize = pageSize;
    }

    const generatedSort = generateSort(sorters);
    if (generatedSort) {
      query.orderBy = generatedSort;
    }

    const generatedFilter = generateFilter(filters);
    if (generatedFilter) {
      query.filter = generatedFilter;
    }
    const { data, headers } = await request<TData[]>(
      "get",
      resourceListUrl(resource, meta, query),
      meta,
    );

    const dataRangeHeader = headers["content-range"] ?? "";
    const total = parseInt(dataRangeHeader.split("/")[1], 10);

    return {
      data,
      total: total || data.length,
    };
  }

  async function getMany<TData extends BaseRecord = BaseRecord>({
    resource,
    ids,
    meta,
  }: GetManyParams): Promise<GetManyResponse<TData>> {
    return await request<TData[]>(
      "get",
      resourceListUrl(resource, meta, { id: ids }),
      meta,
    );
  }

  async function getOne<TData extends BaseRecord = BaseRecord>({
    resource,
    id,
    meta,
  }: GetOneParams): Promise<GetOneResponse<TData>> {
    return await request<TData>("get", resourceUrl(resource, id, meta), meta);
  }

  async function create<
    TData extends BaseRecord = BaseRecord,
    TVariables = object,
  >({
    resource,
    variables,
    meta,
  }: CreateParams<TVariables>): Promise<CreateResponse<TData>> {
    return await requestWithData<TData>(
      "post",
      resourceListUrl(resource),
      variables,
      meta,
    );
  }

  async function createMany<
    TData extends BaseRecord = BaseRecord,
    TVariables = object,
  >({
    resource,
    variables,
    meta,
  }: CreateManyParams<TVariables>): Promise<CreateManyResponse<TData>> {
    return await requestWithData<TData[]>(
      "post",
      `${resourceListUrl(resource, meta)}/bulk`,
      variables,
      meta,
    );
  }

  async function update<
    TData extends BaseRecord = BaseRecord,
    TVariables = object,
  >({
    resource,
    id,
    variables,
    meta,
  }: UpdateParams<TVariables>): Promise<UpdateResponse<TData>> {
    return await requestWithData<TData>(
      "put",
      resourceUrl(resource, id, meta),
      variables,
      meta,
    );
  }

  async function updateMany<
    TData extends BaseRecord = BaseRecord,
    TVariables = object,
  >({
    resource,
    ids,
    variables,
    meta,
  }: UpdateManyParams<TVariables>): Promise<UpdateManyResponse<TData>> {
    return await requestWithData<TData[]>(
      "patch",
      resourceListUrl(resource, meta, { id: ids }),
      variables,
      meta,
    );
  }

  async function deleteOne<
    TData extends BaseRecord = BaseRecord,
    TVariables = object,
  >({
    resource,
    id,
    meta,
  }: DeleteOneParams<TVariables>): Promise<DeleteOneResponse<TData>> {
    const result = await deleteMany<TData, TVariables>({
      resource,
      ids: [id],
      meta,
    });
    return {
      data: result.data[0],
    };
  }

  async function deleteMany<
    TData extends BaseRecord = BaseRecord,
    TVariables = object,
  >({
    resource,
    ids,
    meta,
  }: DeleteManyParams<TVariables>): Promise<DeleteManyResponse<TData>> {
    return await request<TData[]>(
      "delete",
      resourceListUrl(resource, meta, { id: ids }),
      meta,
    );
  }

  async function custom<
    TData extends BaseRecord = BaseRecord,
    TQuery = unknown,
    TPayload = unknown,
  >({
    url,
    method,
    filters,
    sorters,
    payload,
    query,
    headers,
  }: CustomParams<TQuery, TPayload>): Promise<CustomResponse<TData>> {
    let requestUrl = `${apiUrl}${url}`;

    const generatedSort = generateSort(sorters);
    if (generatedSort) {
      const sortQuery = {
        sortBy: generatedSort,
      };
      requestUrl = `${requestUrl}&${stringify(sortQuery)}`;
    }

    const generatedFilter = generateFilter(filters);
    if (generatedFilter) {
      const filterQuery = {
        filter: generatedFilter,
      };
      requestUrl = `${requestUrl}&${stringify(filterQuery)}`;
    }

    if (query) {
      requestUrl = `${requestUrl}&${stringify(query)}`;
    }

    let axiosResponse;
    switch (method) {
      case "put":
      case "post":
      case "patch":
        axiosResponse = await httpClient[method](requestUrl, payload, {
          headers,
        });
        break;
      case "delete":
        axiosResponse = await httpClient.delete(requestUrl, {
          data: payload,
          headers: headers,
        });
        break;
      default:
        axiosResponse = await httpClient.get(requestUrl, {
          headers,
        });
        break;
    }

    const { data } = axiosResponse;

    return Promise.resolve({ data });
  }

  return {
    getOne,
    getList,
    getMany,

    create,
    createMany,

    update,
    updateMany,

    deleteOne,
    deleteMany,

    custom,

    getApiUrl: () => apiUrl,
  };
}
export default dataProvider;
