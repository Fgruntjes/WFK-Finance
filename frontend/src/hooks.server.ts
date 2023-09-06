import { setSession } from '$houdini'
import type { Handle } from '@sveltejs/kit'
import { auth } from './services/auth';

export const handle: Handle = async ({ event, resolve }) => {
    console.log(event, 'auth_handle.event');
    const token = await auth.getAccessToken();
    console.log(token, 'auth_handle.token');
    setSession(event, { token })
    return await resolve(event)
}