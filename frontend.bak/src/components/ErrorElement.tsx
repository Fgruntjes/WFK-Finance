import { Alert, AlertDescription, AlertIcon, AlertTitle } from "@chakra-ui/react";
import { toast } from "react-toastify";

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

export function notifyError(error: ErrorType)
{
    error = toError(error);

    toast.error(error.message);
}

export type ErrorType = string|Error|undefined|null;
export default ErrorElement;