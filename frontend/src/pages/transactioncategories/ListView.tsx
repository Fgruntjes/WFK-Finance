import type { TransactionCategory, TransactionCategoryInput } from "@api";
import NoData from "@components/NoData";
import { List as RefineList } from "@refinedev/antd";
import { HttpError, useTable, useTranslate, useUpdate } from "@refinedev/core";
import Search from "antd/es/input/Search";
// When we import the enum trough @api we get a failed to resolve error
import { List, Typography } from "antd";
import React, { useEffect, useState } from "react";
import styles from "./ListView.module.less";
import { ActionButtons } from "./list-view/ActionButtons";
import { CategoryGroup } from "./list-view/CategoryGroup";
import { CreateButton } from "./list-view/CreateButton";
import { GroupTag } from "./list-view/GroupTag";

type CategoryInfoProps = {
  item: TransactionCategory;
  children: React.ReactNode;
};

function CategoryInfo({ item, children }: CategoryInfoProps) {
  const translate = useTranslate();
  return (
    <>
      <div>
        <Typography.Text>
          <GroupTag type={item.group} />
          {item.name}
        </Typography.Text>
        {item.description && (
          <p className={styles.description}>{item.description}</p>
        )}
      </div>
      <div>{children}</div>
    </>
  );
}

function ListView() {
  const [groups, setGroups] = useState<CategoryGroup[]>([]);
  const {
    tableQueryResult: { isLoading, data },
  } = useTable<TransactionCategory, HttpError>({
    resource: "transactioncategories",
    syncWithLocation: false,
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
    const groups: { [key: string]: CategoryGroup } = {};
    const items = data?.data || [];
    items
      .filter((category) => !category.parentId)
      .forEach((category) => {
        groups[category.id] = {
          ...category,
          children: [],
        };
      });

    items
      .filter((category) => !!category.parentId)
      .forEach((category) => {
        groups[category.parentId ?? ""].children.push(category);
      });

    const groupsArray = Object.values(groups).sort(
      (a, b) => a.sortOrder - b.sortOrder,
    );
    groupsArray.forEach((group) => {
      group.children = group.children.sort((a, b) => a.sortOrder - b.sortOrder);
    });
    setGroups(groupsArray);
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
      {!isLoading && groups.length === 0 ? (
        <NoData>
          <CreateButton siblingCount={groups.length} />
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
