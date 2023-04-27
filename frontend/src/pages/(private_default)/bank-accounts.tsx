import AppPanel from "@/components/AppPanel";
import { Divider, Icon } from "@chakra-ui/react";
import { TbBuildingBank } from "react-icons/tb";
import { FormattedMessage } from "react-intl";
import AccountAddButton from "./_bank-accounts/AccountAddButton";
import AccountList from "./_bank-accounts/AccountList";
import styles from './bank-accounts.module.scss';

function BankAccountsPage() {
    return (
        <AppPanel fullWidht title="Fkae title">
            <AccountList />
            <Divider className={styles['button-divider']}/>
            <AccountAddButton />
        </AppPanel>
    )
}

export const BankAccountsPageIcon = <Icon as={TbBuildingBank} /> 
export const BankAccountsPageTitle = <FormattedMessage id="page.bank-accounts.title" defaultMessage="Bank Accounts"/>;

export default BankAccountsPage
