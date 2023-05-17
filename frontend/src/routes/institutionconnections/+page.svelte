<script lang="ts">
	import { InstitutionConnectionApi } from '@/api/generated';
	import PageTitle from '@/components/PageTitle.svelte';
	import PaginatedTable from '@/components/PaginatedTable/PaginatedTable.svelte';
	import {
		StructuredListCell,
		StructuredListHead,
		StructuredListRow
	} from 'carbon-components-svelte';
</script>

<PageTitle title="Bank accounts" />

<PaginatedTable queryKey="InstitutionConnection" serviceType={InstitutionConnectionApi}>
	<StructuredListHead slot="head">
		<StructuredListRow head>
			<StructuredListCell head>Actions</StructuredListCell>
			<StructuredListCell head>Institution</StructuredListCell>
			<StructuredListCell head>Accounts</StructuredListCell>
		</StructuredListRow>
	</StructuredListHead>

	<StructuredListRow slot="item" let:item>
		<StructuredListCell>
			<a href="/institutionconnections/{item.id}/edit">Edit</a>
		</StructuredListCell>
		<StructuredListCell>{item.institutionId}</StructuredListCell>
		<StructuredListCell>
			{#each item.accounts as account}
				{account.iban}
				{account.ownerName}
			{/each}
		</StructuredListCell>
	</StructuredListRow>
</PaginatedTable>
