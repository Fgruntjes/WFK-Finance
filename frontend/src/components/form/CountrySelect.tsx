import { Select, SelectProps } from "antd";
import { countries } from "country-data";

type CountrySelectProps = SelectProps & {
  allowedCountries?: string[];
};

function CountrySelect({
  allowedCountries = Object.keys(countries),
  ...otherProps
}: CountrySelectProps) {
  const options = countries.all
    .filter((country) => allowedCountries.includes(country.alpha2))
    .sort((a, b) => a.name.localeCompare(b.name))
    .map((country) => ({
      value: country.alpha2,
      label: `${country.name} ${country.emoji ?? ""}`,
    }));

  return <Select options={options} {...otherProps} />;
}

export default CountrySelect;
