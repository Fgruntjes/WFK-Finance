import type { TransactionCategory, TransactionCategoryInput } from "@api";
import NoData from "@components/NoData";
import { List as RefineList } from "@refinedev/antd";
import { HttpError, useList, useUpdate } from "@refinedev/core";
import Search from "antd/es/input/Search";
// When we import the enum trough @api we get a failed to resolve error
import Loader from "@components/Loader";
import {
  CategoryGroup,
  groupCategoryData,
} from "@hooks/useTransactionCategoryGroups";
import { List, Space } from "antd";
import { useEffect, useState } from "react";
import styles from "./ListView.module.less";
import { ActionButtons } from "./list-view/ActionButtons";
import CategoryInfo from "./list-view/CategoryInfo";
import { CreateButton } from "./list-view/CreateButton";
import CreateDefaultsButton from "./list-view/CreateDefaultsButton";

function ListView() {
  const [groups, setGroups] = useState<CategoryGroup[]>([]);
  const { isLoading, data } = useList<TransactionCategory, HttpError>({
    resource: "transactioncategories",
    pagination: {
      pageSize: 250,
    },
  });

  const { mutate } = useUpdate<
    TransactionCategory,
    HttpError,
    TransactionCategoryInput
  >();

  // Set initial group
  useEffect(() => {
    setGroups(groupCategoryData(data?.data));
  }, [data]);

  function moveItem<T extends TransactionCategory>(
    item: T,
    siblings: T[],
    direction: number,
  ): T[] {
    // Find the current index of the item
    const currentIndex = siblings.findIndex(
      (sibling) => sibling.id === item.id,
    );

    if (currentIndex === -1) {
      console.error("Item not found in the array");
      return siblings;
    }

    const newIndex = currentIndex + direction;
    if (newIndex < 0 || newIndex >= siblings.length) {
      // New index is out of bounds, no movement needed
      return siblings;
    }

    // Remove the item from its current position
    const newSiblings = [...siblings];
    newSiblings.splice(currentIndex, 1);
    newSiblings.splice(newIndex, 0, item);

    // Update the sort order of the items
    newSiblings.forEach((sibling, index) => {
      if (sibling.sortOrder == index) {
        return;
      }

      sibling.sortOrder = index;
      mutate({
        resource: "transactioncategories",
        id: sibling.id,
        values: sibling,
        successNotification: false,
        invalidates: [],
      });
    });

    return newSiblings;
  }

  function moveGroup(group: CategoryGroup, direction: number) {
    setGroups(moveItem(group, groups, direction));
  }

  function moveChild(
    group: CategoryGroup,
    child: TransactionCategory,
    direction: number,
  ) {
    group.children = moveItem(child, group.children, direction);
  }

  return (
    <RefineList
      headerButtons={() => (
        <>
          {groups.length > 0 && <CreateButton siblingCount={groups.length} />}
        </>
      )}
    >
      {isLoading && <Loader />}
      {!isLoading && groups.length === 0 ? (
        <NoData>
          <Space direction="horizontal" size="small">
            <CreateDefaultsButton />
            <CreateButton type="dashed" siblingCount={groups.length} />
          </Space>
        </NoData>
      ) : (
        <>
          <Search className={styles.search} placeholder="Search" />
          {groups.map((group) => (
            <List
              header={
                <>
                  <CategoryInfo item={group}>
                    <ActionButtons
                      item={group}
                      onMoveUp={
                        group != groups[0]
                          ? () => moveGroup(group, -1)
                          : undefined
                      }
                      onMoveDown={
                        group != groups[groups.length - 1]
                          ? () => moveGroup(group, 1)
                          : undefined
                      }
                    >
                      <CreateButton
                        siblingCount={group.children.length}
                        parentId={group.id}
                        initialValues={{ group: group.group }}
                        type="text"
                        hideText
                      />
                    </ActionButtons>
                  </CategoryInfo>
                </>
              }
              loading={isLoading}
              key={group.id}
              bordered
              dataSource={group.children}
              className={styles.list}
              renderItem={(item) => (
                <List.Item key={item.id}>
                  <CategoryInfo item={item}>
                    <ActionButtons
                      item={item}
                      onMoveUp={
                        item != group.children[0]
                          ? () => moveChild(group, item, -1)
                          : undefined
                      }
                      onMoveDown={
                        item != group.children[group.children.length - 1]
                          ? () => moveChild(group, item, 1)
                          : undefined
                      }
                    />
                  </CategoryInfo>
                </List.Item>
              )}
            />
          ))}
        </>
      )}
    </RefineList>
  );
}

export default ListView;
