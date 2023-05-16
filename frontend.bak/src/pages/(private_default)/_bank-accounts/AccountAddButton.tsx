import { Button, Icon, useDisclosure } from "@chakra-ui/react";
import { TbPlus } from "react-icons/tb";
import { FormattedMessage } from "react-intl";
import AccountAddModal from "./AccountAddModal";

const AccountAddButton = () => {
    const { isOpen, onOpen, onClose } = useDisclosure();

    return (
        <>
            <Button colorScheme="green" variant="outline" leftIcon={<Icon as={TbPlus} />} onClick={onOpen}>
                <FormattedMessage id="page.bank-accounts.add-label" defaultMessage="Add account"/>
            </Button>
            <AccountAddModal isOpen={isOpen} onClose={onClose} />
        </>
    );
}

export default AccountAddButton;