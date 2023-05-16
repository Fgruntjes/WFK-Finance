import { IResourceComponentsProps } from "@refinedev/core";
import { MantineInferencer } from "@refinedev/inferencer/mantine";

export const BankConnectionList: React.FC<IResourceComponentsProps> = () => {
    return <MantineInferencer meta={{
        BankConnection: {
            getList: {
                fields: [
                    "id",
                    "name",
                ],
            },
        },
    }} />;
};
