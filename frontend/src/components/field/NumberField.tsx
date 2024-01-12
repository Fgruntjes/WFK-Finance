import {
  NumberField as RefineNumberField,
  NumberFieldProps as RefineNumberFieldProps,
} from "@refinedev/antd";

export type NumberFieldProps = {
  value: number;
  colorized?: boolean;
} & RefineNumberFieldProps;

function NumberField({ colorized, value, ...props }: NumberFieldProps) {
  const style: React.CSSProperties = {};
  if (colorized) {
    if (value && value < 0) {
      style.color = "red";
    } else {
      style.color = "green";
    }
  }

  return <RefineNumberField style={style} value={value} {...props} />;
}

export default NumberField;
