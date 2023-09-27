<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/stores';
	import { graphql } from '$houdini';
	import LocalError from '@/components/LocalError.svelte';
	import PageBreadcrumbs from '@/components/PageBreadcrumbs.svelte';
	import { i18n } from '@/services/i18n';
	import {
		Link,
		StructuredList,
		StructuredListBody,
		StructuredListCell,
		StructuredListHead,
		StructuredListRow,
		StructuredListSkeleton
	} from 'carbon-components-svelte';

	let isMutationCalled = false;
	const refreshMutation = graphql(`
		mutation institutionConnectionRefresh($externalId: String!) {
			institutionConnection {
				refreshExternalId(externalId: $externalId) {
					institution {
						name
						logo
					}
					accounts {
						iban
					}
				}
			}
		}
	`);

	$: {
		const externalConnectionId = $page.url.searchParams.get('ref');
		if (!externalConnectionId) {
			goto('/institutionconnections');
		} else if (!isMutationCalled) {
			isMutationCalled = true;
			refreshMutation.mutate({
				externalId: externalConnectionId
			});
		}
	}
</script>

<PageBreadcrumbs
	route="/institutionconnections/create"
	title={$i18n.t('institutionconnections:create-return.title')}
/>

<h2>{$i18n.t('institutionconnections:create-return.title')}</h2>

{#if $refreshMutation.errors}
	<LocalError error={$refreshMutation.errors} />
{:else if $refreshMutation.fetching}
	<StructuredListSkeleton rows={3} />
{:else}
	{@const institution =
		$refreshMutation.data?.institutionConnection?.refreshExternalId?.institution}
	{@const accounts =
		$refreshMutation.data?.institutionConnection?.refreshExternalId?.accounts || []}

	<StructuredList condensed>
		<StructuredListHead>
			<StructuredListRow head>
				<StructuredListCell head>
					{$i18n.t('institutionconnections:create-return.institution')}
				</StructuredListCell>
				<StructuredListCell head>
					{$i18n.t('institutionconnections:create-return.account')}
				</StructuredListCell>
			</StructuredListRow>
		</StructuredListHead>
		<StructuredListBody>
			{#each accounts as account}
				<StructuredListRow>
					<StructuredListCell>
						<img alt={institution?.name} src={institution?.logo} class="institution-logo" />
						{institution?.name}
					</StructuredListCell>
					<StructuredListCell>
						{account.iban}
					</StructuredListCell>
				</StructuredListRow>
			{/each}
		</StructuredListBody>
	</StructuredList>
{/if}
<Link href="/institutionconnections">
	{$i18n.t('institutionconnections:create-return.return')}
</Link>

<style lang="scss">
	.institution-logo {
		border-radius: 10%;
		max-height: 2em;
		max-width: 2em;
	}
</style>
