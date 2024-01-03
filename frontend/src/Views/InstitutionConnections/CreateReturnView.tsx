import { InstitutionConnection } from "@Api";
import Loader from "@Components/Loader";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import CardHeader from "@mui/material/CardHeader";
import Link from "@mui/material/Link";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemText from "@mui/material/ListItemText";
import { useEffect, useMemo, useState } from "react";
import { useDataProvider, useTranslate } from "react-admin";
import { useQuery } from "react-query";
import { Link as RouterLink, useSearchParams } from "react-router-dom";

function CreateReturnView() {
  const [searchParams] = useSearchParams();
  const [externalId, setExternalId] = useState<string>("");
  const dataProvider = useDataProvider();
  const translate = useTranslate();
  const { data, isLoading, error } = useQuery<InstitutionConnection>(
    ["institutionconnections", "refreshExternal", externalId],
    () => dataProvider.institutionConnectionRefreshByExternal(externalId),
    {
      enabled: !!externalId,
    },
  );

  useMemo(() => {
    const error = searchParams.get("error");
    const errorDetails = searchParams.get("details");
    if (error) {
      throw new Error(errorDetails || error || "Unknown error");
    }

    setExternalId(searchParams.get("ref") || "");
  }, [searchParams]);

  useEffect(() => {
    if (error) {
      throw error;
    }
  }, [error]);

  return (
    <Card>
      <CardHeader
        title={translate("app.institutionconnections.createreturn.title")}
      />
      <CardContent>
        {isLoading && <Loader />}
        {!isLoading && !error && data && (
          <>
            <List disablePadding>
              {data.accounts.map((account) => (
                <ListItem>
                  <ListItemText primary={account.iban} />
                </ListItem>
              ))}
            </List>
            <Link to={`/institutionconnections`} component={RouterLink}>
              {translate("app.institutionconnections.createreturn.return")}
            </Link>
          </>
        )}
      </CardContent>
    </Card>
  );
}

export default CreateReturnView;
