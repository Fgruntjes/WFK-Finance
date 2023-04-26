import { BankAccountsApi } from "@/api/generated";
import useServiceQuery from "@/api/useServiceQuery";
import AppPanel from "@/components/AppPanel";
import DataLoaderSkeleton from "@/components/DataLoader";
import { Icon } from "@chakra-ui/react";
import { TbBuildingBank } from "react-icons/tb";
import { FormattedMessage } from "react-intl";

function BankAccountsPage() {
    const { data, isLoading, error } = useServiceQuery(BankAccountsApi, {
        queryKey: ["BankAccountsApi.getBankAccounts"],
        queryFn: (service) => service.getBankAccounts(),
    });
    
    return (
        <AppPanel fullWidht title="Fkae title">
            <DataLoaderSkeleton isLoading={isLoading} error={error}>
                {data?.length === 0 ? <FormattedMessage id="page.bank-accounts.no-accounts" defaultMessage="No bank accounts configured."/> : null}
            </DataLoaderSkeleton>
        </AppPanel>
    )
}

export const BankAccountsPageIcon = <Icon as={TbBuildingBank} /> 
export const BankAccountsPageTitle = <FormattedMessage id="page.bank-accounts.title" defaultMessage="Bank Accounts"/>;

export default BankAccountsPage
