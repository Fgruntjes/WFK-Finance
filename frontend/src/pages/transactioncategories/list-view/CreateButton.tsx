import { TransactionCategory, TransactionCategoryInput } from "@api";
import {
  CreateButton as AntCreateButton,
  CreateButtonProps,
  useModalForm,
} from "@refinedev/antd";
import { HttpError } from "@refinedev/core";
import { CreateModal } from "./CreateModal";

type ChildCreateButtonProps = CreateButtonProps & {
  parentId?: string | null;
  siblingCount: number;
  initialValues?: Partial<TransactionCategoryInput>;
};

export function CreateButton({
  parentId,
  siblingCount,
  initialValues,
  ...buttonProps
}: ChildCreateButtonProps) {
  const { show, modalProps, formProps } = useModalForm<
    TransactionCategory,
    HttpError,
    TransactionCategoryInput
  >({
    action: "create",
    resource: "transactioncategories",
  });

  function onFinish(values: TransactionCategoryInput) {
    if (formProps.onFinish) {
      formProps.onFinish({
        ...values,
        parentId,
        sortOrder: siblingCount,
      });
    }
  }

  return (
    <>
      <CreateModal
        modalProps={modalProps}
        formProps={{
          ...formProps,
          initialValues,
          onFinish,
        }}
      />
      <AntCreateButton {...buttonProps} onClick={() => show()} />
    </>
  );
}
