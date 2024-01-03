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

export interface Institution {
  /** @format uuid */
  id: string;
  externalId: string;
  name: string;
  /** @format uri */
  logo?: string | null;
  country: string;
}

export interface InstitutionConnection {
  /** @format uuid */
  id: string;
  externalId: string;
  /** @format uuid */
  institutionId: string;
  /** @format uri */
  connectUrl: string;
  accounts: InstitutionConnectionAccount[];
}

export interface InstitutionConnectionAccount {
  /** @format uuid */
  id: string;
  externalId: string;
  iban: string;
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
