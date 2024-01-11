import { CrudSorting } from "@refinedev/core";
import { GridifyQueryBuilder } from "gridify-client";

function generateSort(sorters?: CrudSorting) {
  if (!sorters) {
    return "";
  }

  const query = new GridifyQueryBuilder();
  sorters.forEach((sorter) => {
    query.addOrderBy(sorter.field, sorter.order == "desc");
  });
  return query.build().orderBy;
}

export default generateSort;
