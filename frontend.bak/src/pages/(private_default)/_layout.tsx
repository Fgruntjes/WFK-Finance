import { AuthGuard, AuthProvider } from "@/hoc/Auth";
import DefaultLayout from "@/layouts/DefaultLayout";
import { Outlet } from "react-router-dom";

const PrivateLayout = () => (
    <AuthProvider>
        <AuthGuard>
            <DefaultLayout>
                <Outlet />
            </DefaultLayout>
        </AuthGuard>
    </AuthProvider>
)

export default PrivateLayout;
