import { createFileRoute } from "@tanstack/react-router";
import { useCourses } from "@/hooks/useCourses";
import { CourseCard } from "../-components/CourseCard";
import styles from "./index.module.css";

const RouteComponent = () => {
  const { data: courses, isLoading, error } = useCourses();

  if (isLoading)
    return (
      <main className={styles.root}>
        <p>Загрузка...</p>
      </main>
    );

  if (error)
    return (
      <main className={styles.root}>
        <p className={styles.error}>Ошибка загрузки курсов</p>
      </main>
    );

  return (
    <main className={styles.root}>
      <h1 className={styles.title}>Уроки</h1>
      <p className={styles.subtitle}>Выбери курс и начни обучение</p>

      {courses && courses.length > 0 ? (
        <div className={styles.courseGrid}>
          {courses.map((course) => (
            <CourseCard key={course.id} course={course} />
          ))}
        </div>
      ) : (
        <p className={styles.emptyText}>Пока нет доступных курсов</p>
      )}
    </main>
  );
};

export const Route = createFileRoute("/_protected/_sidebar/courses/")({
  component: RouteComponent,
});
