/** @type {import('houdini').ConfigFile} */
const config = {
    "watchSchema": {
        "url": "http://localhost:5204/graphql",
    },
    "plugins": {
        "houdini-svelte": {
            "client": "./src/api/client.ts"
        }
    }
}

export default config