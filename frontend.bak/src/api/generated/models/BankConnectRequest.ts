/* tslint:disable */
/* eslint-disable */
/**
 * WFK Finance API
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { exists, mapValues } from '../runtime';
/**
 * 
 * @export
 * @interface BankConnectRequest
 */
export interface BankConnectRequest {
    /**
     * 
     * @type {string}
     * @memberof BankConnectRequest
     */
    returnUrl?: string | null;
    /**
     * 
     * @type {string}
     * @memberof BankConnectRequest
     */
    bankId?: string | null;
}

/**
 * Check if a given object implements the BankConnectRequest interface.
 */
export function instanceOfBankConnectRequest(value: object): boolean {
    let isInstance = true;

    return isInstance;
}

export function BankConnectRequestFromJSON(json: any): BankConnectRequest {
    return BankConnectRequestFromJSONTyped(json, false);
}

export function BankConnectRequestFromJSONTyped(json: any, ignoreDiscriminator: boolean): BankConnectRequest {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'returnUrl': !exists(json, 'returnUrl') ? undefined : json['returnUrl'],
        'bankId': !exists(json, 'bankId') ? undefined : json['bankId'],
    };
}

export function BankConnectRequestToJSON(value?: BankConnectRequest | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'returnUrl': value.returnUrl,
        'bankId': value.bankId,
    };
}

