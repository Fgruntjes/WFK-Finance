import { HoudiniClient } from '$houdini';
import { auth } from '@/services/auth';
import { get } from 'svelte/store';

console.log(import.meta.env.APP_API_URI)

export default new HoudiniClient({
    url: `${import.meta.env.APP_API_URI}/graphql`,
    fetchParams() {
        const token = get(auth.accessToken);
        if (!token) {
            return {};
        }

        return {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        }
    },
})