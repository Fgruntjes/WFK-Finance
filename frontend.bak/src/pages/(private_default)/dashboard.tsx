import { Icon } from "@chakra-ui/react";
import { TbLayoutDashboard } from "react-icons/tb";
import { FormattedMessage } from "react-intl";

function DashboardPage() {
    return (
        <>Dashboard</>
    )
}

export const DashboardPageIcon = <Icon as={TbLayoutDashboard} /> 
export const DashboardPageTitle = <FormattedMessage id="page.dashboard.title" defaultMessage="Dashboard"/>;
export default DashboardPage
