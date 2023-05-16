import useBankAccountRefresh from "@/api/useBankAccountRefresh";
import { Button, Icon } from "@chakra-ui/react";
import { TbRefresh } from "react-icons/tb";
import { FormattedMessage } from "react-intl";

const AccountAddButton = () => {
    const { isInitialLoading, refetch } = useBankAccountRefresh();

    return (
        <>
            <Button leftIcon={<Icon as={TbRefresh} />} isLoading={isInitialLoading} isDisabled={isInitialLoading} onClick={() => refetch()}>
                <FormattedMessage id="page.bank-accounts.refresh-label" defaultMessage="Refresh"/>
            </Button>
        </>
    );
}

export default AccountAddButton;