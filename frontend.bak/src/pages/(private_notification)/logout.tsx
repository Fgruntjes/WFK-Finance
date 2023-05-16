import RedirectingLoader from "@/components/RedirectingLoader";
import { useAuth0 } from "@auth0/auth0-react";
import { Icon } from "@chakra-ui/react";
import { useEffect } from "react";
import { TbLogout } from "react-icons/tb";
import { FormattedMessage } from "react-intl";

function LogoutPage() {
    const auth = useAuth0();

    useEffect(() => {
        auth.logout();
    }, [auth]);

    return <RedirectingLoader />;
}


export const LogoutPageIcon = <Icon as={TbLogout} /> 
export const LogoutPageTitle = <FormattedMessage id="page.dashboard.title" defaultMessage="Logout"/>;
export default LogoutPage
