import {
  DateField as ReactAdminDateField,
  DateFieldProps as ReactAdminDateFieldProps,
} from "react-admin";

type DateFieldProps = ReactAdminDateFieldProps;

function DateField(props: DateFieldProps) {
  const options: Intl.DateTimeFormatOptions = {};
  if (props.showDate == undefined || props.showDate) {
    options.day = "2-digit";
    options.month = "2-digit";
    options.year = "numeric";
  }

  if (props.showTime) {
    options.hour = "2-digit";
    options.minute = "2-digit";
  }
  return <ReactAdminDateField locales="nl" options={options} {...props} />;
}

export default DateField;
