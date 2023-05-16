import { Box } from "@mantine/core";
import { Create, useForm } from "@refinedev/mantine";
import { useTranslation } from "react-i18next";
import CountrySelect from "../../components/form/CountrySelect";
import InstitutionSelect from "../../components/form/InstitutionSelect";

export const BankConnectionCreate = () => {
    const { t } = useTranslation();
    const {
        saveButtonProps,
        getInputProps,
        refineCore: { formLoading },
        values: {country}
    } = useForm({
        initialValues: {
            country: null,
        },
    });
    

    return (
        <Create
            isLoading={formLoading}
            saveButtonProps={saveButtonProps}
        >
            <Box h={"100%"} maw={300} mx="auto">
                <form>
                    <CountrySelect
                        availableCountries={['NL', 'DE', 'GB', 'AU']}
                        label={t("pages.bankconnections.create.country-label", "Country")}
                        {...getInputProps("country")} />

                    <InstitutionSelect country={country || undefined}
                                       label={t("pages.bankconnections.create.institution-label", "Institution")}
                                       {...getInputProps("institution")} />
                </form>
            </Box>
        </Create>
    );
};
