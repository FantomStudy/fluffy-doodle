import { createFileRoute } from "@tanstack/react-router";
import { ChildProgressPage } from "@/components/ChildProgressPage/ChildProgressPage";

export const Route = createFileRoute("/_protected/_sidebar/child")({
  component: ChildProgressPage,
});
