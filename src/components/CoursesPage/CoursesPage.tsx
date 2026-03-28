import type { Course } from "@/api/courses";
import { Link } from "@tanstack/react-router";
import { useCourses } from "@/hooks/useCourses";
import styles from "./CoursesPage.module.css";

const CourseCard = ({
  id,
  imageUrl,
  categoryName,
  title,
  description,
  level,
  totalLessons,
}: Course) => (
  <Link to="/course/$courseId" params={{ courseId: String(id) }} className={styles.courseCard}>
    <div className={styles.courseImage}>
      <img src={imageUrl || "/assets/homepage/course-python.png"} alt={title} />
    </div>
    <span className={styles.categoryBadge}>{categoryName}</span>
    <h3 className={styles.courseTitle}>{title}</h3>
    <p className={styles.courseDesc}>{description}</p>
    <div className={styles.courseMeta}>
      <span className={styles.courseLevel}>{level}</span>
      <span className={styles.courseDivider} />
      <span className={styles.courseLessons}>{totalLessons} уроков</span>
    </div>
  </Link>
);

const CoursesPage = () => {
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
            <CourseCard key={course.id} {...course} />
          ))}
        </div>
      ) : (
        <p className={styles.emptyText}>Пока нет доступных курсов</p>
      )}
    </main>
  );
};

export { CoursesPage };
