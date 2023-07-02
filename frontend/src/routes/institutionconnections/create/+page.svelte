<script lang="ts">
	import type {
		Institution,
		InstitutionConnection,
		InstitutionConnectionCreateRequest
	} from '@/api/generated';
	import { institutionConnectionMutation } from '@/api/queries/institutionConnectionQuery';
	import Button from '@/components/Button.svelte';
	import CountrySelect from '@/components/CountrySelect.svelte';
	import LocalError from '@/components/LocalError.svelte';
	import PageBreadcrumbs from '@/components/PageBreadcrumbs.svelte';
	import { i18n } from '@/services/i18n';
	import { createMutation } from '@tanstack/svelte-query';
	import { Form, FormGroup } from 'carbon-components-svelte';
	import InstitutionSelect from './InstitutionSelect.svelte';

	let selectedCountry: string;
	let selectedInstitution: Institution | undefined;

	const mutation = createMutation<
		InstitutionConnection,
		Error,
		InstitutionConnectionCreateRequest,
		unknown
	>({
		...institutionConnectionMutation.create()
	});

	const handleConnect = () => {
		$mutation.mutate({
			institutionId: selectedInstitution?.id ?? '',
			returnUri: `${window.location.origin}/institutionconnections/create-return`
		});
	};

	$: {
		if ($mutation.data?.connectUrl) {
			window.location.href = $mutation.data?.connectUrl;
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
			<InstitutionSelect country={selectedCountry} bind:selected={selectedInstitution} />
		{/if}
	</FormGroup>

	{#if $mutation.error}
		<LocalError error={$mutation.error} />
	{/if}
	<Button
		on:click={() => handleConnect()}
		disabled={!selectedInstitution}
		isLoading={$mutation.isLoading}
		title={$i18n.t('institutionconnections:create.submit')}
	/>
</Form>
