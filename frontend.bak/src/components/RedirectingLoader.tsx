import { FormattedMessage } from "react-intl";
import Loader from "./Loader";

const RedirectingLoader = () => <Loader label={<FormattedMessage id="component.redirecting.label" defaultMessage="Redirecting..."/>} />;

export default RedirectingLoader;