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
 * @interface ObjectId
 */
export interface ObjectId {
    /**
     * 
     * @type {number}
     * @memberof ObjectId
     */
    readonly timestamp?: number;
    /**
     * 
     * @type {number}
     * @memberof ObjectId
     * @deprecated
     */
    readonly machine?: number;
    /**
     * 
     * @type {number}
     * @memberof ObjectId
     * @deprecated
     */
    readonly pid?: number;
    /**
     * 
     * @type {number}
     * @memberof ObjectId
     * @deprecated
     */
    readonly increment?: number;
    /**
     * 
     * @type {Date}
     * @memberof ObjectId
     */
    readonly creationTime?: Date;
}

/**
 * Check if a given object implements the ObjectId interface.
 */
export function instanceOfObjectId(value: object): boolean {
    let isInstance = true;

    return isInstance;
}

export function ObjectIdFromJSON(json: any): ObjectId {
    return ObjectIdFromJSONTyped(json, false);
}

export function ObjectIdFromJSONTyped(json: any, ignoreDiscriminator: boolean): ObjectId {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'timestamp': !exists(json, 'timestamp') ? undefined : json['timestamp'],
        'machine': !exists(json, 'machine') ? undefined : json['machine'],
        'pid': !exists(json, 'pid') ? undefined : json['pid'],
        'increment': !exists(json, 'increment') ? undefined : json['increment'],
        'creationTime': !exists(json, 'creationTime') ? undefined : (new Date(json['creationTime'])),
    };
}

export function ObjectIdToJSON(value?: ObjectId | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
    };
}

