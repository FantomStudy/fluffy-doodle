import { createFileRoute } from "@tanstack/react-router";
import { CoursesPage } from "@/components/CoursesPage";

const CoursesRoute = () => <CoursesPage />;

export const Route = createFileRoute("/_protected/_sidebar/courses")({
  component: CoursesRoute,
});
