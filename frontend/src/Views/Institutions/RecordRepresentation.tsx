import { Institution } from "@Api";
import { useRecordContext } from "react-admin";

function RecordRepresentation() {
  const institution = useRecordContext<Institution>();
  if (!institution) {
    return null;
  }

  return (
    <>
      {!!institution.logo && (
        <img
          alt={institution.name}
          src={institution.logo as unknown as string}
        />
      )}
      {institution.name}
    </>
  );
}

export default RecordRepresentation;
