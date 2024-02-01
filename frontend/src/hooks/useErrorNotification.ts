import {
  HttpError,
  OpenNotificationParams,
  useTranslate,
} from "@refinedev/core";

function useErrorNotification() {
  const translate = useTranslate();

  function errorNotification(error?: unknown): OpenNotificationParams {
    const httpError = error as HttpError;
    console.log(error);
    return {
      message: translate(`error.${httpError.message}.description`, httpError),
      type: "error",
      description: translate(
        `error.${httpError.message}.title`,
        httpError,
        httpError.message,
      ),
    };
  }

  return errorNotification;
}

export default useErrorNotification;
