import { countries } from "country-data";
import { SelectInput, SelectInputProps } from "react-admin";

type CountrySelectInputProps = SelectInputProps & {
  allowedCountries?: string[];
};

function CountrySelectInput({
  allowedCountries = Object.keys(countries),
  ...props
}: CountrySelectInputProps) {
  const choices = countries.all
    .filter((country) => allowedCountries.includes(country.alpha2))
    .sort((a, b) => a.name.localeCompare(b.name))
    .map((country) => ({
      id: country.alpha2,
      name: `${country.name} ${country.emoji ?? ""}`,
    }));

  return <SelectInput choices={choices} {...props} />;
}

export default CountrySelectInput;
