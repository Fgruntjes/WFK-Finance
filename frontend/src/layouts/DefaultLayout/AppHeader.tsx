import { useAuth0 } from "@auth0/auth0-react";
import { Flex, Heading, Icon, Spacer, Stack } from "@chakra-ui/react";
import { TbPigMoney } from "react-icons/tb";
import { FormattedMessage } from "react-intl";
import UserAvatar from "./AppHeader/UserAvatar.tsx";

function AppHeader() {
    const { user } = useAuth0();

    return (
        <Flex minWidth='max-content' alignItems='center' gap='2'>
            <Heading size='md'>
                <Stack direction="row" alignItems="center">
                    <Icon as={TbPigMoney} boxSize={10} />
                    <span>
                        <FormattedMessage id="layout.title" defaultMessage="WFK Finance"/>
                    </span>
                </Stack>
            </Heading>
            <Spacer />
            <UserAvatar name={user?.name} src={user?.picture} />
        </Flex>
    )
}

export default AppHeader
