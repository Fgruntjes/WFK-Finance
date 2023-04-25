import { Center } from "@chakra-ui/react";

const NotificationLayout = ({children}: {children: React.ReactNode}) => (
    <Center h="100vh">{children}</Center>
);

export default NotificationLayout;