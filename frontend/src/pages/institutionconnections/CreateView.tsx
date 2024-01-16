import { InstitutionConnection, InstitutionConnectionCreate } from "@api";
import CountrySelect from "@components/form/CountrySelect";
import { Edit, useForm } from "@refinedev/antd";
import { HttpError, useTranslate } from "@refinedev/core";
import { Form } from "antd";
import { useState } from "react";
import InstitutionSelect from "./InstitutionSelect";

function CreateView() {
  const translate = useTranslate();
  const [selectedCountry, setSelectedCountry] = useState<string>("");
  const { formProps, saveButtonProps } = useForm<
    InstitutionConnection,
    HttpError,
    InstitutionConnectionCreate
  >({
    resource: "institutionconnections",
    onMutationSuccess: (result) => {
      window.location.href = result.data.connectUrl;
    },
    meta: {
      variables: {
        returnUrl: `${window.location.origin}/bank-accounts/create-return`,
      },
    },
  });

  return (
    <Edit
      saveButtonProps={saveButtonProps}
      title={translate("institutionconnections.titles.create")}
    >
      <Form {...formProps} layout="vertical">
        <Form.Item
          label={translate("institutionconnections.fields.countryIso2")}
          name="countryIso2"
        >
          <CountrySelect
            value={selectedCountry}
            onChange={(value) => setSelectedCountry(value)}
            allowedCountries={["NL", "DE", "GB", "AU"]}
          />
        </Form.Item>

        {selectedCountry && (
          <Form.Item
            label={translate("institutionconnections.fields.institutionId")}
            name="institutionId"
          >
            <InstitutionSelect countryIso2={selectedCountry} />
          </Form.Item>
        )}
      </Form>
    </Edit>
  );
}

export default CreateView;
