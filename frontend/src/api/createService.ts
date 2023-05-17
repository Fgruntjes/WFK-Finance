import { createConfiguration } from "./createConfiguration";
import type { Configuration } from "./generated";

export function createService<T>(type: new (configuration: Configuration) => T): T {
    const configuration = createConfiguration();
    return new type(configuration);
}
