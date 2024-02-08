/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export enum ImportStatus {
  Success = "Success",
  Queued = "Queued",
  Working = "Working",
  Failed = "Failed",
}

/** Institution */
export interface Institution {
  /** @format uuid */
  id: string;
  externalId: string;
  name: string;
  /** @format uri */
  logo?: string | null;
  countryIso2: string;
}

/** InstitutionAccount */
export interface InstitutionAccount {
  /** @format uuid */
  id: string;
  /** @format uuid */
  institutionId: string;
  externalId: string;
  iban: string;
  importStatus: ImportStatus;
  /** @format date-time */
  lastImport?: string | null;
  /** @format int32 */
  transactionCount: number;
}

/** InstitutionConnection */
export interface InstitutionConnection {
  /** @format uuid */
  id: string;
  externalId: string;
  /** @format uuid */
  institutionId: string;
  /** @format uri */
  connectUrl: string;
  accounts: InstitutionAccount[];
}

/** InstitutionConnectionInput */
export interface InstitutionConnectionInput {
  /** @format uuid */
  institutionId: string;
  /** @format uri */
  returnUrl: string;
}

/** InstitutionTransaction */
export interface InstitutionTransaction {
  /** @format uuid */
  id: string;
  /** @format uuid */
  institutionId: string;
  /** @format uuid */
  accountId: string;
  accountIban: string;
  /** @format uuid */
  categoryId?: string | null;
  /** @format date-time */
  date: string;
  /** @format double */
  amount: number;
  currency: string;
  counterPartyName?: string;
  counterPartyAccount?: string;
  unstructuredInformation: string;
  transactionCode?: string;
}

/** InstitutionTransactionPatch */
export interface InstitutionTransactionPatch {
  /** @format uuid */
  categoryId?: string | null;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  /** @format int32 */
  status?: number | null;
  detail?: string;
  instance?: string;
  [key: string]: any;
}

/** TransactionCategory */
export interface TransactionCategory {
  /** @format uuid */
  id: string;
  name: string;
  /** @format uuid */
  parentId?: string | null;
  /** @format int32 */
  sortOrder: number;
  group: TransactionCategoryGroup;
  description?: string;
}

export enum TransactionCategoryGroup {
  Income = "Income",
  Expense = "Expense",
  Transfer = "Transfer",
  Investment = "Investment",
  Savings = "Savings",
  Liquididation = "Liquididation",
  Other = "Other",
}

/** TransactionCategoryInput */
export interface TransactionCategoryInput {
  name: string;
  /** @format uuid */
  parentId?: string | null;
  /** @format int32 */
  sortOrder: number;
  group: TransactionCategoryGroup;
  description?: string;
}
