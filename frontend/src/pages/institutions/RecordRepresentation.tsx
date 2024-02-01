import { Institution } from "@api";
import styles from "./RecordRepresentation.module.less";

type RecordPresentationProps = {
  recordItem?: Institution;
};

function RecordRepresentation({ recordItem }: RecordPresentationProps) {
  if (!recordItem) {
    return null;
  }

  return (
    <span className={styles.container}>
      {!!recordItem.logo && (
        <img alt={recordItem.name} src={recordItem.logo as unknown as string} />
      )}
      {recordItem.name}
    </span>
  );
}

export default RecordRepresentation;
