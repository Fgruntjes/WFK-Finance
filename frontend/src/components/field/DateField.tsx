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

  return <RefineDateField format={format} {...props} />;
}

export default DateField;
