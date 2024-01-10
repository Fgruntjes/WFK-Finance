import { useAuth0 } from "@auth0/auth0-react";
import { useEffect } from "react";

function Login() {
  const { loginWithRedirect, isLoading } = useAuth0();

  useEffect(() => {
    if (!isLoading) {
      loginWithRedirect();
    }
  }, [loginWithRedirect, isLoading]);

  return null;
}
export default Login;
