import { createFileRoute } from "@tanstack/react-router";
import { CoursePage } from "@/components/CoursePage";

const CourseRoute = () => <CoursePage />;

export const Route = createFileRoute("/_protected/_sidebar/course")({
  component: CourseRoute,
});
