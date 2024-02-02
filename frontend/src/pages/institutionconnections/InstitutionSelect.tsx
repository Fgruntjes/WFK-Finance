import { Institution } from "@api";
import Loader from "@components/Loader";
import LocalError from "@components/LocalError";
import { HttpError, useList } from "@refinedev/core";
import { Select, SelectProps } from "antd";

type InstitutionSelectProps = {
  countryIso2: string;
} & SelectProps;

function InstitutionSelect({
  countryIso2,
  ...otherProps
}: InstitutionSelectProps) {
  const { data, isLoading, isError, error } = useList<Institution, HttpError>({
    resource: "institutions",
    pagination: {
      pageSize: 250,
    },
    filters: [
      {
        field: "countryIso2",
        operator: "eq",
        value: countryIso2,
      },
    ],
  });

  if (isLoading) return <Loader />;
  if (isError) return <LocalError error={error} />;
  if (!data) return <LocalError error="No institutions found" />;

  const options =
    data.data
      .sort((a, b) => a.name.localeCompare(b.name))
      .map((institution) => ({
        value: institution.id,
        label: institution.name,
      })) ?? [];

  return <Select options={options} {...otherProps} />;
}

export default InstitutionSelect;
