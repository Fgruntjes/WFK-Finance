import { Institution } from "@Api";
import Loader from "@Components/Loader";
import LocalError from "@Components/LocalError";
import { useEffect } from "react";
import { SelectFieldProps, SelectInput, useGetList } from "react-admin";

type InstitutionSelectInputProps = {
  countryIso2: string;
} & SelectFieldProps;

function InstitutionSelectInput({ countryIso2 }: InstitutionSelectInputProps) {
  const { data, isLoading, error, refetch } = useGetList<Institution>(
    "institutions",
    {
      pagination: { page: 1, perPage: 999 },
      filter: { countryIso2 },
    },
  );
  useEffect(() => {
    console.log("refetching");
    refetch();
  }, [countryIso2, refetch]);

  if (isLoading) return <Loader />;
  if (error) return <LocalError error={error} />;
  if (!data) return <LocalError error="No institutions found" />;

  const choices =
    data
      .sort((a, b) => a.name.localeCompare(b.name))
      .map((institution) => ({
        id: institution.id,
        name: institution.name,
      })) ?? [];

  return <SelectInput source="institutionId" choices={choices} />;
}

export default InstitutionSelectInput;
