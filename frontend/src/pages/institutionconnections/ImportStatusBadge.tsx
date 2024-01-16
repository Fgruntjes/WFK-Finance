import { InstitutionAccountImportStatusEnum } from "@api";
import { useTranslate } from "@refinedev/core";
import { Tag } from "antd";

type ImportStatusBadgeProps = {
  status?: InstitutionAccountImportStatusEnum;
};

export function ImportStatusBadge({ status }: ImportStatusBadgeProps) {
  const translate = useTranslate();

  if (!status) {
    return null;
  }

  return (
    <Tag
      data-testid="import-status-badge"
      color={
        status === "Success" ? "green" : status === "Failed" ? "red" : "blue"
      }
    >
      {translate(`institutionaccounts.importStatus.${status.toLowerCase()}`)}
    </Tag>
  );
}
