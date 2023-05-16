import { Select, SelectProps } from '@mantine/core';

import { countries } from 'country-data';

type CountrySelectProps = Omit<SelectProps, "onChange" | "data"> & {
    availableCountries: string[];
    onChange?: (country?: string) => void;
}

const CountrySelect = ({ availableCountries, onChange, ...selectProps }: CountrySelectProps) => {
    const selectOptions = availableCountries
        .map(countryCode => countries[countryCode])
        .map(country => ({
            label: `${country.emoji} ${country.name}`,
            value: country.alpha2,
        }))

    return (
        <Select onChange={newValue => onChange && onChange(newValue || undefined)}
                data={selectOptions}
                {...selectProps}/>
    );
}

export default CountrySelect;