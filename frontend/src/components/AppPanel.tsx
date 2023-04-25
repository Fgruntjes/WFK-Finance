import { Card, CardBody, CardHeader, Heading } from "@chakra-ui/react";
import classnames from "classnames";
import styles from './AppPanel.module.scss';

type AppPanelProps = {
    title?: React.ReactNode;
    children: React.ReactNode;
    fullWidht?: boolean;
}

const AppPanel = ({title, children, fullWidht = false}: AppPanelProps) => (
    <Card className={classnames({
        [styles['full-width']]: fullWidht,
    })}>
        {title && <CardHeader><Heading size='md'>{title}</Heading></CardHeader>}
        <CardBody>{children}</CardBody>
    </Card>
)

export default AppPanel;