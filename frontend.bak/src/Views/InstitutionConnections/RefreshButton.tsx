import { InstitutionConnection } from "@Api";
import RefreshIcon from "@mui/icons-material/Refresh";
import Button from "@mui/material/Button";
import {
  useDataProvider,
  useRecordContext,
  useRefresh,
  useTranslate,
} from "react-admin";
import { useMutation } from "react-query";

function RefreshButton() {
  const translate = useTranslate();
  const record = useRecordContext<InstitutionConnection>();
  const dataProvider = useDataProvider();
  const refresh = useRefresh();
  const { mutate, isLoading } = useMutation(() =>
    dataProvider
      .institutionConnectionRefreshById(record.id)
      .then(() => refresh()),
  );

  return (
    <Button
      startIcon={<RefreshIcon />}
      onClick={() => mutate()}
      disabled={isLoading}
    >
      {translate("app.button.refresh")}
    </Button>
  );
}

export default RefreshButton;
