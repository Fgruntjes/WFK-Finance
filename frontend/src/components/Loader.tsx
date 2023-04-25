import { Card, CardBody, CardHeader, Heading, Spinner } from '@chakra-ui/react';
import { FormattedMessage } from 'react-intl';

type LoaderProps = {
    label?: React.ReactNode; 
}

function Loader({label}: LoaderProps) {
    label = label ?? <FormattedMessage id="component.loader.label" defaultMessage="Loading..."/>;
    return (
        <Card>
            <CardHeader><Heading size='md'>{label}</Heading></CardHeader>
            <CardBody textAlign="center"><Spinner size="xl" color="blue.500" /></CardBody>
        </Card>
    );
}

export default Loader;