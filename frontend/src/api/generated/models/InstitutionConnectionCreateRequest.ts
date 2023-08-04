/* tslint:disable */
/* eslint-disable */
/**
 * App.Backend
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
 * @interface InstitutionConnectionCreateRequest
 */
export interface InstitutionConnectionCreateRequest {
    /**
     * 
     * @type {string}
     * @memberof InstitutionConnectionCreateRequest
     */
    institutionId: string;
    /**
     * 
     * @type {string}
     * @memberof InstitutionConnectionCreateRequest
     */
    returnUri: string;
}

/**
 * Check if a given object implements the InstitutionConnectionCreateRequest interface.
 */
export function instanceOfInstitutionConnectionCreateRequest(value: object): boolean {
    let isInstance = true;
    isInstance = isInstance && "institutionId" in value;
    isInstance = isInstance && "returnUri" in value;

    return isInstance;
}

export function InstitutionConnectionCreateRequestFromJSON(json: any): InstitutionConnectionCreateRequest {
    return InstitutionConnectionCreateRequestFromJSONTyped(json, false);
}

export function InstitutionConnectionCreateRequestFromJSONTyped(json: any, ignoreDiscriminator: boolean): InstitutionConnectionCreateRequest {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'institutionId': json['institutionId'],
        'returnUri': json['returnUri'],
    };
}

export function InstitutionConnectionCreateRequestToJSON(value?: InstitutionConnectionCreateRequest | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'institutionId': value.institutionId,
        'returnUri': value.returnUri,
    };
}
