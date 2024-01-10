import { InstitutionConnection } from "@Api";
import CountrySelectInput from "@Components/CountrySelectInput";
import { useState } from "react";
import { Create, SimpleForm } from "react-admin";
import InstitutionSelectInput from "./InstitutionSelectInput";

function transform(data: object) {
  return {
    ...data,
    returnUrl: `${window.location.origin}/institutionconnections/create-return`,
  };
}

function onSuccess(data: InstitutionConnection) {
  const connectUrl = data.connectUrl;
  if (connectUrl) {
    window.location.href = connectUrl;
  }
}

function CreateView() {
  const [selectedCountry, setSelectedCountry] = useState<string>("");

  return (
    <Create transform={transform} mutationOptions={{ onSuccess }}>
      <SimpleForm>
        <CountrySelectInput
          source="country"
          value={selectedCountry}
          onChange={(e) => setSelectedCountry(e.target.value)}
          allowedCountries={["NL", "DE", "GB", "AU"]}
          fullWidth
        />

        {selectedCountry && (
          <InstitutionSelectInput countryIso2={selectedCountry} />
        )}
      </SimpleForm>
    </Create>
  );
}

export default CreateView;
