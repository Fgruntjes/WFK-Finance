import { BankAccount, BankAccountsApi } from "@/api/generated";
import useBankAccountRefresh from "@/api/useBankAccountRefresh";
import useServiceQuery from "@/api/useServiceQuery";
import DataLoaderSkeleton from "@/components/DataLoader";
import { Table, TableContainer, Tbody, Td, Th, Thead, Tr } from "@chakra-ui/react";
import { useEffect } from "react";
import { FormattedMessage } from "react-intl";

const AccoutListTable = ({ data }: { data: BankAccount[] }) => (
    <TableContainer>
        <Table variant='simple'>
            <Thead>
                <Tr>
                    <Th><FormattedMessage id="page.bank-accounts.label-bank" defaultMessage="Bank"/></Th>
                    <Th><FormattedMessage id="page.bank-accounts.label-accountNumber" defaultMessage="Account number"/></Th>
                    <Th><FormattedMessage id="page.bank-accounts.label-lastAccess" defaultMessage="Last update"/></Th>
                </Tr>
            </Thead>
            <Tbody>
                {data.map((account, index) => (
                    <Tr key={index}>
                        <Td>{account.bankName}</Td>
                        <Td>{account.accountNumber} {account.accountName}</Td>
                        <Td></Td>
                    </Tr>
                ))}
            </Tbody>
        </Table>
    </TableContainer>
)

const AccountList = () => {
    const { data, isLoading, error } = useServiceQuery(BankAccountsApi, {
        queryKey: ["BankAccountsApi.bankAccountList"],
        queryFn: (service) => service.bankAccountList(),
    });
    const { refetch } = useBankAccountRefresh();
    
    useEffect(() => {
        if (!isLoading && !error && data?.length === 0)
        {
            refetch();
        }
    }, [isLoading, error, data, refetch])
    
    return (
        <DataLoaderSkeleton isLoading={isLoading} error={error}>
            {!data || data.length === 0
                ? <FormattedMessage id="page.bank-accounts.no-accounts" defaultMessage="No bank accounts configured."/>
                : <AccoutListTable data={data} />}
        </DataLoaderSkeleton>
    );
}

export default AccountList;