import { Card, CardBody, CardHeader, Heading } from "@chakra-ui/react";

type AppPanelProps = {
    title?: React.ReactNode;
    children: React.ReactNode;
    fullWidht?: boolean;
}

const AppPanel = ({title, children}: AppPanelProps) => (
    <Card>
        {title && <CardHeader paddingBottom="0"><Heading size='md'>{title}</Heading></CardHeader>}
        <CardBody>{children}</CardBody>
    </Card>
)

export default AppPanel;