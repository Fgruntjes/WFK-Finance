import { NumberField, NumberFieldProps, useRecordContext } from "react-admin";

type CurrencyFieldProps = Omit<NumberFieldProps, "options"> & {
  currency?: string;
  minimumFractionDigits?: number;
  options?: Intl.NumberFormatOptions;
};

function CurrencyField({
  currency = "EUR",
  minimumFractionDigits = 2,
  options = {},
  ...otherProps
}: CurrencyFieldProps) {
  const record = useRecordContext(otherProps);

  if ("currency" in record) {
    currency = record.currency;
  }

  return (
    <NumberField
      {...otherProps}
      options={{
        style: "currency",
        currency,
        minimumFractionDigits,
        ...options,
      }}
    />
  );
}

export default CurrencyField;
