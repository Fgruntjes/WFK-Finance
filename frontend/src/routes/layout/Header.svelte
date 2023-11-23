<script lang="ts">
	import { page } from '$app/stores';
	import { i18n } from '@/services/i18n';
	import { routes } from '@/services/routes';
	import { Header, HeaderNav, HeaderNavItem, HeaderUtilities } from 'carbon-components-svelte';
	import 'carbon-components-svelte/css/g90.css';
	import { base } from '$app/paths';
</script>

<div />
<Header
	company="WFK"
	platformName="Finance"
	href={base}
	data-testid={`activeroute:${$page.route.id}`}
>
	<HeaderNav>
		{#each routes.filter((r) => r.includeInMenu) as route}
			{@const label = $i18n.t(route.translationKey)}
			{@const isSelected = $page.route.id == route.id}
			<HeaderNavItem href={`${base}${route.id}`} {isSelected}>{label}</HeaderNavItem>
		{/each}
	</HeaderNav>

	<HeaderUtilities />
</Header>
