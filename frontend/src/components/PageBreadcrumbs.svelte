<script lang="ts">
	import { page } from '$app/stores';
	import { i18n } from '@/services/i18n';
	import { routesById, type RouteItem } from '@/services/routes';
	import { Breadcrumb, BreadcrumbItem } from 'carbon-components-svelte';
	export let title = 'WFK Finance';

	type Breadcrumb = RouteItem & { isCurrentPage?: boolean };
	let crumbs: Breadcrumb[] = [];

	export let route: string | undefined = undefined;

	$: {
		const activeRoute = routesById[route ?? $page.route.id ?? ''];
		crumbs = [];
		crumbs.push({
			...routesById['/'],
			isCurrentPage: activeRoute?.id === '/'
		});

		if (activeRoute) {
			let tokenPath = '';
			activeRoute.id
				.split('/')
				.filter((t) => t !== '')
				.forEach((token) => {
					tokenPath += '/' + token;
					const route = routesById[tokenPath];
					crumbs.push({
						...route,
						isCurrentPage: tokenPath === activeRoute.id
					});
				});
		}
	}
</script>

<svelte:head>
	<title>{title}</title>
</svelte:head>

<Breadcrumb>
	{#each crumbs as crumb}
		{@const label = $i18n.t(crumb.translationKey)}

		<BreadcrumbItem href={crumb.id} isCurrentPage={crumb.isCurrentPage}>{label}</BreadcrumbItem>
	{/each}
</Breadcrumb>
