import { BankAccountsApi } from "@/api/generated";
import useServiceQuery from "@/api/useServiceQuery";
import DataLoaderSkeleton from "@/components/DataLoader";
import { FormattedMessage } from "react-intl";

const AccountList = () => {
    const { data, isLoading, error } = useServiceQuery(BankAccountsApi, {
        queryKey: ["BankAccountsApi.getBankAccounts"],
        queryFn: (service) => service.bankAccountList(),
    });
    
    return (
        <DataLoaderSkeleton isLoading={isLoading} error={error}>
            {!data || data.length === 0
                ? <FormattedMessage id="page.bank-accounts.no-accounts" defaultMessage="No bank accounts configured."/>
                : <p>List: {JSON.stringify(data)}</p>}
        </DataLoaderSkeleton>
    );
}

export default AccountList;