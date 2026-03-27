import { createFileRoute } from "@tanstack/react-router";
import { HomePage } from "@/components/HomePage";

const HomeRoute = () => <HomePage />;

export const Route = createFileRoute("/_sidebar/")({
  component: HomeRoute,
});
