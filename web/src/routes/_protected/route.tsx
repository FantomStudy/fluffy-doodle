import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";
import { refresh } from "@/api/auth";
import { getProfile } from "@/api/user";

const RouteComponent = () => {
  return (
    <>
      <Outlet />
    </>
  );
};

export const Route = createFileRoute("/_protected")({
  component: RouteComponent,
  beforeLoad: async () => {
    const profile = await getProfile().catch(() => null);
    if (profile) return;

    try {
      await refresh();
    } catch {
      throw redirect({ to: "/login" });
    }
  },
});
