import { Institution } from "@api";
import styles from "./RecordRepresentation.module.less";

type RecordPresentationProps = {
  recordItem?: Institution;
  iconOnly?: boolean;
};

function RecordRepresentation({
  recordItem,
  iconOnly = false,
}: RecordPresentationProps) {
  if (!recordItem) {
    return null;
  }

  return (
    <span className={styles.container}>
      {!!recordItem.logo && (
        <img alt={recordItem.name} src={recordItem.logo as unknown as string} />
      )}
      {!iconOnly && recordItem.name}
    </span>
  );
}

export default RecordRepresentation;
