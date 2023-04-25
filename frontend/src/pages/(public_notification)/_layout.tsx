import NotificationLayout from "@/layouts/NotificationLayout";
import { Outlet } from "react-router-dom";

const PublicNotificationLayout = () => <NotificationLayout><Outlet /></NotificationLayout>;

export default PublicNotificationLayout;