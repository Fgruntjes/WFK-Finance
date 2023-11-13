<script lang="ts">
	import { i18n } from '@/services/i18n';
	import { Select, SelectItem } from 'carbon-components-svelte';
	import { countries } from 'country-data';

	export let labelText: string = $i18n.t('components.countryselect.label');
	export let availableCountries: string[];
	export let selected: string | undefined = undefined;

	const options = availableCountries
		.map((countryCode) => countries[countryCode])
		.map((country) => ({
			label: `${country.emoji} ${country.name}`,
			value: country.alpha2
		}));
</script>

<Select {labelText} bind:selected {...$$restProps}>
	<SelectItem text="" />
	{#each options as { label, value }}
		<SelectItem text={label} {value} />
	{/each}
</Select>
