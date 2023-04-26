import { BankAccountsPageIcon, BankAccountsPageTitle } from "@/pages/(private_default)/bank-accounts";
import { DashboardPageIcon, DashboardPageTitle } from "@/pages/(private_default)/dashboard";
import { LogoutPageIcon, LogoutPageTitle } from "@/pages/(public_notification)/logout";
import { Card, CardBody, Divider, VStack } from "@chakra-ui/react";
import styles from './AppMenu.module.scss';
import AppMenuItem from "./AppMenuItem";

function AppMenu() {
   
    return (
        <Card>
            <CardBody>
                <VStack alignItems="flex-start">
                    <AppMenuItem to="/" icon={DashboardPageIcon}>{DashboardPageTitle}</AppMenuItem>
                    <AppMenuItem to="/bank-accounts" icon={BankAccountsPageIcon}>{BankAccountsPageTitle}</AppMenuItem>
                    <Divider className={styles['divider']} />
                    <AppMenuItem to="/logout" icon={LogoutPageIcon}>{LogoutPageTitle}</AppMenuItem>
                </VStack>
            </CardBody>
        </Card>
    )
}

export default AppMenu
