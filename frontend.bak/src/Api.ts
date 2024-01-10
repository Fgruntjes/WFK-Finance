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

export type FilterParameterValue = Record<string, string | number | string[] | number[]>;

export interface Institution {
  /** @format uuid */
  id: string;
  externalId: string;
  name: string;
  /** @format uri */
  logo?: string | null;
  countryIso2: string;
}

export interface InstitutionAccount {
  /** @format uuid */
  id: string;
  externalId: string;
  iban: string;
  importStatus: "Success" | "Queued" | "Working" | "Failed";
  /** @format date-time */
  lastImport?: string | null;
  /** @format int32 */
  transactionCount: number;
}

export interface InstitutionAccountTransaction {
  /** @format uuid */
  id: string;
  /** @format date-time */
  createdAt: string;
  /** @format uuid */
  accountId: string;
  externalId: string;
  unstructuredInformation: string;
  transactionCode?: string;
  counterPartyName?: string;
  counterPartyAccount?: string;
  /** @format double */
  amount: number;
  currency: string;
  /** @format date-time */
  date: string;
}

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

export interface InstitutionConnectionCreate {
  /** @format uuid */
  institutionId: string;
  /** @format uri */
  returnUrl: string;
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
