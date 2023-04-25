import { Alert, AlertDescription, AlertIcon, AlertTitle } from "@chakra-ui/react";

type ErrorElementProps = {
    error: ErrorType;
}

function toError(error: ErrorType): Error
{
    if (error instanceof Error) {
        return error;
    }

    if (error === undefined || error === null) {
        return new Error("Unknown error");
    }

    return new Error(error);
}

function ErrorElement({error}: ErrorElementProps) {
    error = toError(error);

    return (
        <Alert status='error'>
            <AlertIcon />
            <AlertTitle>{error.name}</AlertTitle>
            <AlertDescription>{error.message}</AlertDescription>
        </Alert>
    );
}

export type ErrorType = string|Error|undefined|null;
export default ErrorElement;