import { createFileRoute } from "@tanstack/react-router";
import { ProfilePage } from "@/components/ProfilePage";

const RouteComponent = () => {
  return <ProfilePage />;
};

export const Route = createFileRoute("/_protected/_sidebar/profile")({
  component: RouteComponent,
});
