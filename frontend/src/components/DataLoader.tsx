import { SkeletonText } from '@chakra-ui/react';
import ErrorElement, { ErrorType } from './ErrorElement';

type DataLoaderProps = {
    children: React.ReactNode;
    isLoading: boolean;
    error: ErrorType;
};

function DataLoaderSkeleton({isLoading, error, children}: DataLoaderProps) {
    if (error) {
        return <ErrorElement error={error} />;
    }

    return <SkeletonText mt='4' noOfLines={4} spacing='4' skeletonHeight='2' isLoaded={!isLoading}>{children}</SkeletonText>;
}

export default DataLoaderSkeleton;