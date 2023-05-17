import { auth } from '@/services/auth';
import { Configuration } from "./generated";

export function createConfiguration() {
    return new Configuration({
        basePath: import.meta.env.APP_API_URI,
        middleware: [
            {
                pre: async (context) => {
                    const token = await auth.getAccessToken();
                    context.init.headers = {
                        authorization: `Bearer ${token}`,
                        ...context.init.headers
                    }
                }
            }
        ]
    });
}