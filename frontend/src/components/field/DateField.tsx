import {
  DateField as RefineDateField,
  DateFieldProps as RefineDateFieldProps,
} from "@refinedev/antd";

type DateFieldProps = {
  showTime?: boolean;
  showDate?: boolean;
} & RefineDateFieldProps;

function DateField({
  showDate = true,
  showTime = false,
  format,
  value,
  ...props
}: DateFieldProps) {
  if (!format) {
    format = "";

    if (showDate) {
      format += "DD-MM-YYYY";
    }
    if (showTime) {
      if (showDate) {
        format += " ";
      }
      format += "HH:mm:ss";
    }
  }

  if (!value) {
    return null;
  }

  return <RefineDateField format={format} value={value} {...props} />;
}

export default DateField;
