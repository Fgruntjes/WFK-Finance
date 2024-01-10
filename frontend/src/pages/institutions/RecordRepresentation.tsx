import { Institution } from "@api";

type RecordPresentationProps = {
  recordItem?: Institution;
};

function RecordRepresentation({ recordItem }: RecordPresentationProps) {
  if (!recordItem) {
    return null;
  }

  return (
    <>
      {!!recordItem.logo && (
        <img alt={recordItem.name} src={recordItem.logo as unknown as string} />
      )}
      {recordItem.name}
    </>
  );
}

export default RecordRepresentation;
