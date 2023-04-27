import { Bank, BankAccountsApi } from "@/api/generated";
import useServiceQuery from "@/api/useServiceQuery";
import ErrorElement from "@/components/ErrorElement";
import CountrySelect from "@/components/form/CountrySelect";
import { Button, FormControl, FormLabel, Modal, ModalBody, ModalCloseButton, ModalContent, ModalFooter, ModalHeader, ModalOverlay } from "@chakra-ui/react";
import { useState } from "react";
import { FormattedMessage } from "react-intl";
import AccountBankSelect from "./AccountBankSelect";

type AccountAddModalProps = {
    isOpen: boolean;
    onClose: () => void;
}

const AccountAddModal = ({isOpen, onClose}: AccountAddModalProps) => {
    const [selectedCountry, setSelectedCountry] = useState<string>();
    const [selectedBank, setSelectedBank] = useState<Bank>();
    const bankConnectParams = {
        bankId: selectedBank?.id || undefined,
        returnUrl: window.location.href,
    };

    const { data, isLoading, error } = useServiceQuery(BankAccountsApi, {
        queryKey: ["BankAccountsApi.bankConnect", bankConnectParams],
        queryFn: (service) => service.bankConnect(bankConnectParams),
        enabled: !!selectedBank,
    });

    function closePopup() {
        setSelectedCountry(undefined);
        setSelectedBank(undefined);
        onClose();
    }

    function handleConnectClick() {
        window.location.href = data?.url || "";
    }
    
    return (
        <Modal isOpen={isOpen} onClose={closePopup}>
            <ModalOverlay />
            <ModalContent>
                <ModalHeader><FormattedMessage id="page.bank-accounts.add-title" defaultMessage="Add account"/></ModalHeader>
                <ModalCloseButton />
                <ModalBody>
                    <FormControl>
                        <FormLabel>
                            <FormattedMessage id="page.bank-accounts.country-select-label" defaultMessage="Select your country"/>
                        </FormLabel>
                        <CountrySelect availableCountries={['NL', 'DE', 'GB', 'AU']} onChange={setSelectedCountry} />
                    </FormControl>

                    {selectedCountry && <FormControl>
                        <FormLabel>
                            <FormattedMessage id="page.bank-accounts.bank-select-label" defaultMessage="Select your bank"/>
                        </FormLabel>
                        <AccountBankSelect country={selectedCountry} onChange={setSelectedBank}/>
                    </FormControl>}
                </ModalBody>

                <ModalFooter>
                    {error && <ErrorElement error={error} />}
                    <Button colorScheme="green" isDisabled={!data && !!error} isLoading={isLoading} onClick={handleConnectClick}>
                        <FormattedMessage id="page.bank-accounts.save-label" defaultMessage="Connect"/>
                    </Button>
                </ModalFooter>
            </ModalContent>
        </Modal>
    );
}

export default AccountAddModal;