import { Avatar, Group, Select, SelectItem, SelectProps, Text } from "@mantine/core";
import { forwardRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Institution, InstitutionApi } from "../../api/generated";
import useServiceQuery from "../../api/useServiceQuery";

type InstitutionSelectProps = Omit<SelectProps, "data" | "onChange"> & {
    country?: string;
    onChange?: (institution?: Institution) => void;
}
type InstitutionSelectItem = Institution & SelectItem;

interface InstitutionSelectItemProps extends React.ComponentPropsWithoutRef<'div'> {
    logo: string;
    name: string;
}
const InstitutionSelectItemElement = forwardRef<HTMLDivElement, InstitutionSelectItemProps>(
    ({logo, name, ...others }: InstitutionSelectItemProps, ref) => (
        <div ref={ref} {...others}>
            <Group noWrap>
                <Avatar src={logo} />
          
                <Text size="sm">{name}</Text>
            </Group>
        </div>
    )
);

const searchFilter = (value: string, item: Institution) => 
    !!value && item.name.toLowerCase().includes(value.toLowerCase());

const InstitutionSelect = (props: InstitutionSelectProps) => {
    const { country, onChange, ...selectProps } = props;
    const { t } = useTranslation();
    const [searchValue, onSearchChange] = useState('');
    const [selectedInstitution, setSelectedInstitution] = useState<Institution>();
    const { data: institutionMap, isLoading, isError, error } = useServiceQuery(InstitutionApi, {
        queryKey: ["InstitutionApi.institutionList", country],
        queryFn: async (service) => {
            const data = await service.institutionList(country || "");
            const institutions =  data?.data.sort((a, b) => {
                const nameA = a.name;
                const nameB = b.name;
                if (!nameA) {
                    if (nameB) return -1;
                    return 0;
                }
        
                if (!nameB) {
                    if (nameA) return 1;
                    return 0;
                }
        
                return nameA.localeCompare(nameB);
            }) || [];

            return new Map(institutions.map((institution) => [institution.id, institution]));
        },
        enabled: !!country,
    });

    const selectInstitution = (id?: string|null) => {
        const institution = id ? institutionMap?.get(id) : undefined;
        setSelectedInstitution(institution);
        if (onChange) onChange(institution);
    }

    const selectOptions: InstitutionSelectItem[] = [];
    institutionMap?.forEach((institution) => {
        selectOptions.push({
            value: institution.id,
            ...institution,
        });
    });
    
    const errorMessage = isError ? 
        error.message :
        !isLoading && institutionMap.size == 0 ?
            t("components.form.institute-select.no-institutions", "No institutions available.")
            : null;

    return (
        <Select data={selectOptions}
                itemComponent={InstitutionSelectItemElement}
                disabled={isLoading}
                error={errorMessage}
                onChange={selectInstitution}
                value={selectedInstitution?.id ?? null}
                searchable
                onSearchChange={onSearchChange}
                searchValue={searchValue}
                placeholder={selectedInstitution?.name ?? t("components.form.institute-select.placeholder", "Select a institution")}
                filter={(value, item) => searchFilter(value, item as InstitutionSelectItem)}
                {...selectProps} />
    );
}

export default InstitutionSelect;