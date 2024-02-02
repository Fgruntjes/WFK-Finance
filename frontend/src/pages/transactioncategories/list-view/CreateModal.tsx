import { useTranslate } from "@refinedev/core";
import { Form, FormProps, Input, Modal, ModalProps, Select } from "antd";
import TextArea from "antd/es/input/TextArea";
import {
  TransactionCategoryGroup,
  TransactionCategoryInput,
} from "../../../Api";

export type CreateModalProps = {
  modalProps: ModalProps;
  formProps: FormProps<TransactionCategoryInput>;
};

export function CreateModal({ modalProps, formProps }: CreateModalProps) {
  const translate = useTranslate();
  return (
    <Modal {...modalProps}>
      <Form {...formProps} layout="vertical">
        <Form.Item
          label={translate("transactioncategories.fields.name")}
          name="name"
          rules={[
            {
              required: true,
            },
          ]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label={translate("transactioncategories.fields.group")}
          name="group"
          rules={[
            {
              required: true,
            },
          ]}
        >
          <Select
            options={Object.keys(TransactionCategoryGroup).map((key) => ({
              label: translate(
                `transactioncategories.groupNames.${key.toLowerCase()}`,
              ),
              value: key,
            }))}
          />
        </Form.Item>
        <Form.Item
          label={translate("transactioncategories.fields.description")}
          name="description"
        >
          <TextArea />
        </Form.Item>
      </Form>
    </Modal>
  );
}
