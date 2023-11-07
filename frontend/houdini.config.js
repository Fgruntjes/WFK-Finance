/** @type {import('houdini').ConfigFile} */
const config = {
	watchSchema: {
		url: 'http://localhost:8080/graphql'
	},
	plugins: {
		'houdini-svelte': {
			client: './src/api/client.ts'
		}
	},
	scalars: {
		Guid: {
			type: 'Guid'
		},
		Uri: {
			type: 'Uri',
			unmarshal(val) {
				return val ? new URL(val) : null;
			},
			/** @param {URL} url */
			marshal(url) {
				return url && url.toString();
			}
		}
	}
};

export default config;
