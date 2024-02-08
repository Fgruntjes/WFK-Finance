import { InstitutionTransaction } from "@api";
import { DateField } from "@refinedev/antd";
import { useTranslate } from "@refinedev/core";
import { Modal, Table } from "antd";
import styles from "./SimilarTransactionsModal.module.less";

type SimilarTransactionModalProps = {
  open: boolean;
  setOpen: (open: boolean) => void;
  transaction: InstitutionTransaction;
  similarTransactions: InstitutionTransaction[];
};

function SimilarTransactionsModal({
  open,
  setOpen,
  transaction,
  similarTransactions,
}: SimilarTransactionModalProps) {
  const translate = useTranslate();
  return (
    <Modal
      open={open}
      title={translate("institutiontransactions.titles.similar")}
      onCancel={() => setOpen(false)}
      okText={translate("buttons.save")}
    >
      <Table dataSource={similarTransactions} pagination={false}>
        <Table.Column
          dataIndex={"date"}
          title={translate("institutiontransactions.fields.date")}
          render={(value) => (
            <DateField
              className={styles.noWrap}
              format="ddd DD MMM YYYY"
              value={value}
            />
          )}
        />
        <Table.Column
          dataIndex="accountIban"
          title={translate("institutiontransactions.fields.accountIban")}
        />
        <Table.Column
          dataIndex="counterPartyName"
          title={translate("institutiontransactions.fields.counterPartyName")}
        />
        <Table.Column
          dataIndex="amount"
          title={translate("institutiontransactions.fields.amount")}
        />
      </Table>
    </Modal>
  );
}

export default SimilarTransactionsModal;
