import type { Lesson as ApiLesson } from "@/api/courses";
import { Link } from "@tanstack/react-router";
import { ArrowLeftIcon, CheckIcon, ClockIcon, LockIcon } from "lucide-react";
import { useCourse, useCourseLessons } from "@/hooks/useCourses";
import styles from "./CoursePage.module.css";

type LessonStatus = "completed" | "current" | "locked";

interface LessonDisplay {
  id: number;
  courseId: number;
  number: number;
  title: string;
  description: string;
  status: LessonStatus;
  tasks: ApiLesson["tasks"];
}

const STATUS_CONFIG = {
  completed: {
    numberClass: styles.numberCompleted,
    label: "Пройдено",
    badgeClass: styles.badgeCompleted,
    icon: <CheckIcon size={12} />,
  },
  current: {
    numberClass: styles.numberCurrent,
    label: "Текущий урок",
    badgeClass: styles.badgeCurrent,
    icon: null,
  },
  locked: {
    numberClass: styles.numberLocked,
    label: "Заблокировано",
    badgeClass: styles.badgeLocked,
    icon: <LockIcon size={12} />,
  },
} as const;

const LessonCard = ({ id, courseId, number, title, description, status }: LessonDisplay) => {
  const config = STATUS_CONFIG[status];

  return (
    <Link
      to="/course/$courseId/lesson/$lessonId"
      params={{ courseId: String(courseId), lessonId: String(id) }}
      className={styles.lessonCard}
    >
      <div className={`${styles.lessonNumber} ${config.numberClass}`}>{number}</div>
      <div className={styles.lessonContent}>
        <h3 className={styles.lessonTitle}>{title}</h3>
        <p className={styles.lessonDesc}>{description}</p>
        <span className={`${styles.lessonBadge} ${config.badgeClass}`}>
          {config.icon}
          {config.label}
        </span>
      </div>
    </Link>
  );
};

interface CoursePageProps {
  courseId: number;
}

const CoursePage = ({ courseId }: CoursePageProps) => {
  const { data: course, isLoading: courseLoading } = useCourse(courseId);
  const { data: lessons, isLoading: lessonsLoading } = useCourseLessons(courseId);

  if (courseLoading || lessonsLoading) {
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

  const displayLessons: LessonDisplay[] = (lessons ?? [])
    .sort((a, b) => a.order - b.order)
    .map((lesson, _i, arr) => {
      const idx = arr.indexOf(lesson);
      const allPrevDone = arr.slice(0, idx).every((l) => l.tasks.length === 0);
      const isCurrent = idx === 0 || allPrevDone;

      return {
        id: lesson.id,
        courseId,
        number: lesson.order,
        title: lesson.title,
        description: lesson.description,
        status: isCurrent ? ("current" as const) : ("locked" as const),
        tasks: lesson.tasks,
      };
    });

  const completedCount = displayLessons.filter((l) => l.status === "completed").length;
  const totalCount = displayLessons.length;
  const progressPercent = totalCount > 0 ? (completedCount / totalCount) * 100 : 0;

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
              <img src={course.imageUrl || "/assets/course/course-python.png"} alt={course.title} />
            </div>
            <div className={styles.courseInfo}>
              <h1 className={styles.courseTitle}>{course.title}</h1>
              <p className={styles.courseDesc}>{course.description}</p>
              <div className={styles.courseMeta}>
                <span className={styles.metaBadge}>{course.totalLessons} уроков</span>
                <span className={`${styles.metaBadge} ${styles.metaLevel}`}>{course.level}</span>
                <span className={styles.metaTime}>
                  <ClockIcon size={16} />
                  {course.hours} ч
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

export { CoursePage };
