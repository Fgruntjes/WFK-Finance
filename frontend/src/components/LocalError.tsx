import { Alert } from "antd";
import { useMemo } from "react";

type ErrorType = Error | string | unknown | Error[] | string[] | unknown[];
type ErrorProps = {
  error: ErrorType;
};

function normalizeErrors(error: ErrorType): Error[] {
  if (Array.isArray(error)) {
    return error.map(normalizeErrors).flat();
  }

  if (error instanceof Error) {
    return [error];
  } else if (typeof error === "string") {
    return [new Error(error)];
  } else if (typeof error == "object" && error !== null) {
    if ("message" in error && typeof error.message == "string") {
      return [new Error(error.message)];
    }
    return [new Error("Unknown error")];
  }

  return [new Error("Unknown error")];
}

const LocalError = (props: ErrorProps) => {
  const normalizedErrors = useMemo(
    () => normalizeErrors(props.error),
    [props.error],
  );
  const title = useMemo(() => {
    if (normalizedErrors.length > 1) {
      return "Errors";
    }
    if (normalizedErrors.length === 1) {
      if (
        "statusCode" in normalizedErrors[0] &&
        normalizedErrors[0].statusCode == 404
      ) {
        return "Not found";
      }
      return normalizedErrors[0].name ?? "Unknown error";
    }
    return "Unknown error";
  }, [normalizedErrors]);
  const description = useMemo(
    () => normalizedErrors.map((error) => error.message).join("\n"),
    [normalizedErrors],
  );

  return (
    <Alert message={title} description={description} type="error" showIcon />
  );
};

export default LocalError;
