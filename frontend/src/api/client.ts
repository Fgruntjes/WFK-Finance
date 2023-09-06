import { HoudiniClient } from '$houdini';

export default new HoudiniClient({
    url: 'http://localhost:5204/graphql',
    fetchParams({ session }) {
        return {
            headers: {
                Authorization: `Bearer ${session?.token}`,
            },
        }
    },
})