import { AuthGuard, AuthProvider } from "@/hoc/Auth";
import NotificationLayout from "@/layouts/NotificationLayout";
import { Outlet } from "react-router-dom";

const PrivateNotificationLayout = () => (
    <AuthProvider>
        <AuthGuard>
            <NotificationLayout><Outlet /></NotificationLayout>
        </AuthGuard>
    </AuthProvider>
);

export default PrivateNotificationLayout;