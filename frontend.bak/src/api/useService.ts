import { Configuration } from "./generated";
import useConfiguration from "./useConfiguration";

function useService<T>(type: new (configuration: Configuration) => T): T {
    const configuration = useConfiguration();
    return new type(configuration);
}

export default useService;