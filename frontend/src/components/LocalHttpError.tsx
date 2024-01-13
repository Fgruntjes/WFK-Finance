import { HttpError, useTranslate } from "@refinedev/core";
import { Alert } from "antd";

type LocalHttpErrorProps = {
  error?: HttpError;
};

const LocalHttpError = ({ error }: LocalHttpErrorProps) => {
  const translate = useTranslate();
  if (!error) {
    return null;
  }

  return (
    <Alert
      message={translate(`error.${error.statusCode}.title`)}
      description={
        error.message ?? translate(`error.${error.statusCode}.description`)
      }
      type="error"
      showIcon
    />
  );
};

export default LocalHttpError;
