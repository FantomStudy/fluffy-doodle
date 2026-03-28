import { createFileRoute, useParams } from "@tanstack/react-router";
import { LessonPage } from "@/components/LessonPage";

const LessonRoute = () => {
  const { courseId, lessonId } = useParams({
    from: "/_protected/_sidebar/course/$courseId/lesson/$lessonId",
  });
  return <LessonPage courseId={Number(courseId)} lessonId={Number(lessonId)} />;
};

export const Route = createFileRoute("/_protected/_sidebar/course/$courseId/lesson/$lessonId")({
  component: LessonRoute,
});
