import { createFileRoute, Link, useParams } from "@tanstack/react-router";
import { ArrowLeftIcon, ClockIcon } from "lucide-react";
import { useCourse, useCourseLessons } from "@/hooks/useCourses";
import { LessonCard } from "./-components/LessonCard/LessonCard";
import styles from "./index.module.css";

type LessonStatus = "completed" | "current" | "locked";

const getLessonStatus = (lesson: { tasks: unknown[] }, hasCurrentLesson: boolean): LessonStatus => {
  if (lesson.tasks.length === 0) return "completed";
  if (!hasCurrentLesson) return "current";

  return "locked";
};

const RouteComponent = () => {
  const { courseId } = useParams({ from: "/_protected/_sidebar/courses/$courseId/" });

  const course = useCourse(Number(courseId));
  const lessons = useCourseLessons(Number(courseId));

  let hasCurrentLesson = false;
  const displayLessons = (lessons.data ?? [])
    .sort((a, b) => a.order - b.order)
    .map((lesson) => {
      const status = getLessonStatus(lesson, hasCurrentLesson);

      if (status === "current") {
        hasCurrentLesson = true;
      }

      return {
        id: lesson.id,
        courseId: Number(courseId),
        number: lesson.order,
        title: lesson.title,
        description: lesson.description,
        tasks: lesson.tasks,
        status,
      };
    });

  const completedCount = displayLessons.filter((l) => l.status === "completed").length;
  const totalCount = displayLessons.length;
  const progressPercent = totalCount > 0 ? (completedCount / totalCount) * 100 : 0;

  if (course.isPending || lessons.isLoading) {
    return (
      <main className={styles.root}>
        <p>Загрузка...</p>
      </main>
    );
  }

  if (!course) {
    return (
      <main className={styles.root}>
        <p>Курс не найден</p>
      </main>
    );
  }

  return (
    <main className={styles.root}>
      <Link to="/" className={styles.backLink}>
        <ArrowLeftIcon size={20} />
        <span>К списку курсов</span>
      </Link>

      <div className={styles.layout}>
        <div className={styles.mainColumn}>
          <section className={styles.courseHeader}>
            <div className={styles.courseImage}>
              <img
                src={course.data?.imageUrl || "/assets/course/course-python.png"}
                alt={course.data?.title}
              />
            </div>
            <div className={styles.courseInfo}>
              <h1 className={styles.courseTitle}>{course.data?.title}</h1>
              <p className={styles.courseDesc}>{course.data?.description}</p>
              <div className={styles.courseMeta}>
                <span className={styles.metaBadge}>{course.data?.totalLessons} уроков</span>
                <span className={`${styles.metaBadge} ${styles.metaLevel}`}>
                  {course.data?.level}
                </span>
                <span className={styles.metaTime}>
                  <ClockIcon size={16} />
                  {course.data?.hours} ч
                </span>
              </div>
            </div>
          </section>

          <section className={styles.lessonsList}>
            {displayLessons.map((lesson) => (
              <LessonCard key={lesson.id} {...lesson} />
            ))}
          </section>
        </div>

        <aside className={styles.sidebar}>
          <div className={styles.progressCard}>
            <h2 className={styles.sidebarTitle}>Твой прогресс</h2>
            <div className={styles.progressRow}>
              <div className={styles.progressTrack}>
                <div className={styles.progressFill} style={{ width: `${progressPercent}%` }} />
              </div>
              <div className={styles.starCount}>
                <img src="/assets/course/star-badge.svg" alt="очки" className={styles.starIcon} />
                <span>{completedCount}</span>
              </div>
            </div>
            <p className={styles.progressText}>
              {completedCount} из {totalCount} уроков
            </p>
          </div>
        </aside>
      </div>
    </main>
  );
};

export const Route = createFileRoute("/_protected/_sidebar/courses/$courseId/")({
  component: RouteComponent,
});
