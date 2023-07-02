<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/stores';
	import queries from '@/api/queries';
	import LocalError from '@/components/LocalError.svelte';
	import PageBreadcrumbs from '@/components/PageBreadcrumbs.svelte';
	import { i18n } from '@/services/i18n';
	import { createQuery } from '@tanstack/svelte-query';
	import {
		Link,
		StructuredList,
		StructuredListBody,
		StructuredListCell,
		StructuredListHead,
		StructuredListRow,
		StructuredListSkeleton
	} from 'carbon-components-svelte';

	const externalConnectionId = $page.url.searchParams.get('ref');

	$: refreshQuery = createQuery({
		...queries.institutionConnection.refresh({
			externalId: externalConnectionId || ''
		}),
		enabled: $page.url.searchParams.has('ref'),
		refetchOnWindowFocus: false
	});

	$: institutionData = createQuery({
		...queries.institution.getMany({
			id: [$refreshQuery.data?.institutionId || '']
		}),
		enabled: !!$refreshQuery.data?.institutionId
	});

	$: {
		if (!externalConnectionId) {
			goto('/institutionconnections');
		}
	}
</script>

<PageBreadcrumbs
	route="/institutionconnections/create"
	title={$i18n.t('institutionconnections:create-return.title')}
/>

<h2>{$i18n.t('institutionconnections:create-return.title')}</h2>

{#if $refreshQuery.error}
	<LocalError error={$refreshQuery.error} />
{/if}

{#if $refreshQuery.isLoading}
	<StructuredListSkeleton rows={3} />
{:else}
	{@const institution = $institutionData.data?.[0]}

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
			{#each $refreshQuery.data?.accounts || [] as account}
				<StructuredListRow>
					<StructuredListCell>
						{#if institution}
							<img alt={institution?.name} src={institution?.logo} class="institution-logo" />
							{institution?.name}
						{/if}
					</StructuredListCell>
					<StructuredListCell>
						{account.iban}
						{#if account.ownerName}({account.ownerName}){/if}
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
		// make the image round and no bigger then the text in the cell
		border-radius: 10%;
		max-height: 2em;
		max-width: 2em;
	}
</style>
