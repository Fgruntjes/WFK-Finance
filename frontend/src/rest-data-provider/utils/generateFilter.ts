import {
  ConditionalFilter,
  CrudFilter,
  CrudFilters,
  CrudOperators,
  LogicalFilter,
} from "@refinedev/core";
import { ConditionalOperator, GridifyQueryBuilder } from "gridify-client";

const filterMap = {
  eq: ConditionalOperator.Equal,
  ne: ConditionalOperator.NotEqual,
  lt: ConditionalOperator.LessThan,
  gt: ConditionalOperator.GreaterThan,
  lte: ConditionalOperator.LessThanOrEqual,
  gte: ConditionalOperator.GreaterThanOrEqual,
  contains: ConditionalOperator.Contains,
  ncontains: ConditionalOperator.NotContains,
  containss: ConditionalOperator.Contains,
  ncontainss: ConditionalOperator.NotContains,

  startswith: ConditionalOperator.StartsWith,
  nstartswith: ConditionalOperator.NotStartsWith,
  startswiths: ConditionalOperator.StartsWith,
  nstartswiths: ConditionalOperator.NotStartsWith,
  endswith: ConditionalOperator.EndsWith,
  nendswith: ConditionalOperator.NotEndsWith,
  endswiths: ConditionalOperator.EndsWith,
  nendswiths: ConditionalOperator.NotEndsWith,
};

function addCrudFilterCondition(
  query: GridifyQueryBuilder,
  filter: CrudFilter,
) {
  if (filter.operator == "or" || filter.operator == "and") {
    addLogicalCondition(query, filter as ConditionalFilter);
    return;
  }

  const logicalFilter = filter as LogicalFilter;
  switch (logicalFilter.operator) {
    case "in":
      addInCondition(query, logicalFilter);
      break;
    case "nin":
      addNotInCondition(query, logicalFilter);
      break;
    case "between":
      addBetweenCondition(query, logicalFilter);
      break;
    case "nbetween":
      addNotBetweenCondition(query, logicalFilter);
      break;
    case "null":
      addNullCondition(query, logicalFilter);
      break;
    case "nnull":
      addNotNullCondition(query, logicalFilter);
      break;
    default: {
      addCondition(
        query,
        logicalFilter.field,
        logicalFilter.operator,
        logicalFilter.value,
      );
    }
  }
}

function addLogicalCondition(
  query: GridifyQueryBuilder,
  conditionalFilter: ConditionalFilter,
) {
  query.startGroup();
  conditionalFilter.value.forEach((filter) =>
    addCrudFilterCondition(query, filter),
  );
  query.endGroup();
}

function addNotNullCondition(
  query: GridifyQueryBuilder,
  logicalFilter: LogicalFilter,
) {
  query.addCondition(
    logicalFilter.field,
    ConditionalOperator.NotEqual,
    "null",
    undefined,
    false,
  );
}

function addNullCondition(
  query: GridifyQueryBuilder,
  logicalFilter: LogicalFilter,
) {
  query.addCondition(
    logicalFilter.field,
    ConditionalOperator.Equal,
    "",
    undefined,
    false,
  );
}

function addCondition(
  query: GridifyQueryBuilder,
  field: string,
  operator: Exclude<
    CrudOperators,
    "in" | "nin" | "between" | "nbetween" | "null" | "nnull" | "or" | "and"
  >,
  value: unknown,
) {
  const escapeValue = typeof value === "string";
  const caseSensitive =
    operator == "containss" ||
    operator == "ncontainss" ||
    operator == "startswiths" ||
    operator == "nstartswiths" ||
    operator == "endswiths" ||
    operator == "nendswiths";

  query.addCondition(
    field,
    filterMap[operator],
    value as string | number | boolean,
    caseSensitive,
    escapeValue,
  );
}

function addInCondition(query: GridifyQueryBuilder, filter: LogicalFilter) {
  query.startGroup();
  filter.value.forEach((value: unknown, index: number) => {
    addCondition(query, filter.field, "eq", value);
    if (index < filter.value.length - 1) {
      query.or();
    }
  });
  query.endGroup();
}

function addNotInCondition(query: GridifyQueryBuilder, filter: LogicalFilter) {
  query.startGroup();
  filter.value.forEach((value: unknown, index: number) => {
    addCondition(query, filter.field, "ne", value);
    if (index < filter.value.length - 1) {
      query.and();
    }
  });
  query.endGroup();
}

function addNotBetweenCondition(
  query: GridifyQueryBuilder,
  logicalFilter: LogicalFilter,
) {
  query.startGroup();
  query.addCondition(
    logicalFilter.field,
    ConditionalOperator.LessThan,
    logicalFilter.value[0],
  );
  query.or();
  query.addCondition(
    logicalFilter.field,
    ConditionalOperator.GreaterThan,
    logicalFilter.value[1],
  );
  query.startGroup();
}

function addBetweenCondition(
  query: GridifyQueryBuilder,
  logicalFilter: LogicalFilter,
) {
  query.startGroup();
  query.addCondition(
    logicalFilter.field,
    ConditionalOperator.GreaterThanOrEqual,
    logicalFilter.value[0],
  );
  query.and();
  query.addCondition(
    logicalFilter.field,
    ConditionalOperator.LessThanOrEqual,
    logicalFilter.value[1],
  );
  query.endGroup();
}

function generateFilter(filters?: CrudFilters) {
  if (!filters) {
    return "";
  }

  const query = new GridifyQueryBuilder();
  filters.forEach((filter) => {
    addCrudFilterCondition(query, filter);
  });
  return query.build().filter;
}

export default generateFilter;
