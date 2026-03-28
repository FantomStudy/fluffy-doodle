import { createFileRoute, useParams } from "@tanstack/react-router";
import { CoursePage } from "@/components/CoursePage";

const CourseIndexRoute = () => {
  const { courseId } = useParams({ from: "/_protected/_sidebar/course/$courseId/" });
  return <CoursePage courseId={Number(courseId)} />;
};

export const Route = createFileRoute("/_protected/_sidebar/course/$courseId/")({
  component: CourseIndexRoute,
});
