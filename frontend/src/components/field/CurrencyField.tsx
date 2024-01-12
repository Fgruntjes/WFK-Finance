import NumberField, { NumberFieldProps } from "./NumberField";

type CurrencyFieldProps = {
  currency: string;
} & NumberFieldProps;

function CurrencyField({ currency, ...props }: CurrencyFieldProps) {
  return <NumberField options={{ style: "currency", currency }} {...props} />;
}

export default CurrencyField;
