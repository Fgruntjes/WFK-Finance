import { createServiceConfiguration } from './createServiceConfiguration';
import type { Configuration } from './generated';

export function createService<T>(type: new (configuration: Configuration) => T): T {
	const configuration = createServiceConfiguration();
	return new type(configuration);
}
