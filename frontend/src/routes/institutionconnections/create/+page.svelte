<script lang="ts">
	import Button from '@/components/Button.svelte';
	import CountrySelect from '@/components/CountrySelect.svelte';
	import LocalError from '@/components/LocalError.svelte';
	import PageBreadcrumbs from '@/components/PageBreadcrumbs.svelte';
	import { i18n } from '@/services/i18n';
	import { Form, FormGroup } from 'carbon-components-svelte';
	import InstitutionSelect from './InstitutionSelect.svelte';
	import { graphql } from '$houdini';

	let selectedCountry: string;
	let selectedInstitutionId: Guid | undefined;

	const connectMutation = graphql(`
		mutation CreateInstitutionConnection($institutionId: Guid!, $returnUrl: Uri!) {
			institutionConnection {
				create(institutionId: $institutionId, returnUrl: $returnUrl) {
					externalId
					connectUrl
				}
			}
		}
	`);

	function handleConnect() {
		if (!selectedInstitutionId) {
			console.log('Could not connect, no institution selected.');
			return;
		}

		connectMutation.mutate({
			institutionId: selectedInstitutionId as string,
			returnUrl: `${window.location.origin}/institutionconnections/create-return`
		});
	}

	$: {
		const connectUrl = $connectMutation.data?.institutionConnection?.create?.connectUrl;
		if (!!connectUrl) {
			window.location.href = connectUrl;
		}
	}
</script>

<PageBreadcrumbs title={$i18n.t('institutionconnections:create.title')} />

<h2>{$i18n.t('institutionconnections:create.title')}</h2>

<Form>
	<FormGroup>
		<CountrySelect availableCountries={['NL', 'DE', 'GB', 'AU']} bind:selected={selectedCountry} />
	</FormGroup>

	<FormGroup>
		{#if selectedCountry}
			<InstitutionSelect countryIso2={selectedCountry} bind:selectedId={selectedInstitutionId} />
		{/if}
	</FormGroup>

	{#if $connectMutation.errors}
		<LocalError error={$connectMutation.errors} />
	{/if}
	<Button
		on:click={() => handleConnect()}
		disabled={!selectedInstitutionId}
		isLoading={$connectMutation.fetching}
		title={$i18n.t('institutionconnections:create.submit')}
	/>
</Form>
