import Alert from "@mui/material/Alert";
import AlertTitle from "@mui/material/AlertTitle";
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
  const title = useMemo(
    () =>
      normalizedErrors.length === 1
        ? normalizedErrors[0].name ?? "Error"
        : "Errors",
    [normalizedErrors],
  );
  const description = useMemo(
    () =>
      normalizedErrors.length === 1
        ? normalizedErrors[0].message ?? "Unknown error"
        : normalizedErrors.map((error) => error.message).join("\n"),
    [normalizedErrors],
  );

  return (
    <Alert severity="error">
      <AlertTitle>{title}</AlertTitle>
      {description}
    </Alert>
  );
};

export default LocalError;
