import { TransactionCategory } from "@api";
import Loader from "@components/Loader";
import NoData from "@components/NoData";
import { CreateButton, List, useModalForm } from "@refinedev/antd";
import { HttpError, useTable, useTranslate } from "@refinedev/core";
import { Form, Input, Modal, Select, Tree } from "antd";
import Search from "antd/es/input/Search";
import { useMemo } from "react";

function ListView() {
  const translate = useTranslate();
  const {
    tableQueryResult: { isLoading, data },
  } = useTable<TransactionCategory, HttpError>({
    resource: "transactioncategories",
    pagination: {
      pageSize: 250,
    },
    //sorters: {
    //  initial: [{ field: "date", order: "desc" }],
    //},
  });

  const {
    modalProps: createModalProps,
    formProps: createFormProps,
    show: createModalShow,
  } = useModalForm<TransactionCategory>({
    action: "create",
  });

  const treeData = useMemo(
    () =>
      data?.data.map((category) => ({
        key: category.id || "",
        title: category.name,
      })) || [],
    [data],
  );

  return (
    <List>
      <Modal {...createModalProps}>
        <Form {...createFormProps} layout="vertical">
          <Form.Item
            label="Title"
            name="title"
            rules={[
              {
                required: true,
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Status"
            name="status"
            rules={[
              {
                required: true,
              },
            ]}
          >
            <Select
              options={[
                {
                  label: "Published",
                  value: "published",
                },
                {
                  label: "Draft",
                  value: "draft",
                },
                {
                  label: "Rejected",
                  value: "rejected",
                },
              ]}
            />
          </Form.Item>
        </Form>
      </Modal>

      {isLoading && <Loader />}
      {treeData.length === 0 ? (
        <NoData>
          <CreateButton
            onClick={() => createModalShow()}
            resource="transactioncategories"
          />
        </NoData>
      ) : (
        <>
          <Search style={{ marginBottom: 8 }} placeholder="Search" />
          <Tree treeData={treeData} />
        </>
      )}
    </List>
  );
}

export default ListView;
