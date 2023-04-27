import { Select } from "chakra-react-select";
import { countries } from 'country-data';

type CountrySelectProps = {
    availableCountries: string[];
    onChange?: (country?: string) => void;
}

const CountrySelect = ({ availableCountries, onChange }: CountrySelectProps) => {
    const options = availableCountries
        .map(countryCode => countries[countryCode])
        .map(country => ({
            label: `${country.emoji} ${country.name}`,
            value: country.alpha2,
        }))

    return (
        <Select onChange={newValue => onChange && onChange(newValue?.value)} options={options} />
    );
}

export default CountrySelect;