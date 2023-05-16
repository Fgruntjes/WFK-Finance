import { Bank, BankAccountsApi } from "@/api/generated";
import useServiceQuery from "@/api/useServiceQuery";
import DataLoaderSkeleton from "@/components/DataLoader";
import { Select } from "chakra-react-select";
import { FormattedMessage } from "react-intl";

type AccountBankSelectProps = {
    country: string;
    onChange?: (bank?: Bank) => void;
}


const AccountBankSelect = ({country, onChange}: AccountBankSelectProps) => {
    const { data, isLoading, error } = useServiceQuery(BankAccountsApi, {
        queryKey: ["BankAccountsApi.getBanks", country],
        queryFn: (service) => service.bankList({countryCode: country}),
    });

    const options = data?.sort((a, b) => {
        const nameA = a.name;
        const nameB = b.name;
        if (!nameA) {
            if (nameB) return -1;
            return 0;
        }

        if (!nameB) {
            if (nameA) return 1;
            return 0;
        }

        return nameA.localeCompare(nameB);
    }).map(bank => ({...bank, label: bank.name}));

    return (
        <DataLoaderSkeleton isLoading={isLoading} error={error}>
            {!data || data.length === 0
                ? <FormattedMessage id="page.bank-accounts.no-banks" defaultMessage="No banks available."/>
                : <Select options={options} onChange={bank => onChange && onChange(bank || undefined)} />}
        </DataLoaderSkeleton>
    );
}

export default AccountBankSelect;