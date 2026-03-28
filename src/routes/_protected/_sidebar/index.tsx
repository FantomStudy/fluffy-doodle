import { createFileRoute, Link, redirect } from "@tanstack/react-router";
import { getMe } from "@/api/user";
import { useCourses } from "@/hooks/useCourses";
import { useCourseLessons } from "@/hooks/useCourses";
import { useMe } from "@/hooks/useMe";
import { CourseCard } from "./-components/CourseCard";
import styles from "./index.module.css";

const getDayWord = (n: number) => {
  if (n % 100 >= 11 && n % 100 <= 14) return "дней подряд";
  const r = n % 10;
  if (r === 1) return "день подряд";
  if (r >= 2 && r <= 4) return "дня подряд";
  return "дней подряд";
};

const RouteComponent = () => {
  const courses = useCourses();
  const { data: me } = useMe();

  const firstCourse = courses.data?.[0];
  const { data: lessons } = useCourseLessons(firstCourse?.id ?? 0);

  const totalLessons = lessons?.length ?? 0;
  const completedLessons =
    lessons?.filter(
      (l) => l.tasks.length > 0 && l.tasks.every((t) => ("completed" in t ? t.completed : false)),
    ).length ?? 0;
  const currentLessonIndex = completedLessons;
  const currentLesson = lessons?.[currentLessonIndex] ?? lessons?.[0];

  const greeting = me?.fullName
    ? `Рад твоему возращению, ${me.fullName}`
    : "Рад твоему возращению!";

  return (
    <main className={styles.root}>
      <header className={styles.header}>
        <h1 className={styles.greeting}>{greeting}</h1>
        <p className={styles.subtitle}>Твой учебный процесс</p>
      </header>

      <section className={styles.learningCard}>
        <div className={styles.cardHeader}>
          <span className={styles.cardLabel}>Твой учебный процесс</span>
          <div className={styles.cardActions}>
            <div className={styles.starCount}>
              <img src="/assets/homepage/star-badge.svg" alt="очки" className={styles.starIcon} />
              <span>{me?.stars ?? 0}</span>
            </div>
            <div className={styles.streakBadge}>
              <img src="/assets/homepage/fire.png" alt="серия" />
              <span>
                Серия: {me?.streak ?? 0} {getDayWord(me?.streak ?? 0)}
              </span>
            </div>
          </div>
        </div>

        <div className={styles.cardFooter}>
          <div className={styles.currentLesson}>
            <span className={styles.lessonLabel}>Текущий урок</span>
            <h2 className={styles.lessonTitle}>{currentLesson?.title ?? "—"}</h2>
            <p className={styles.lessonDesc}>{currentLesson?.description ?? ""}</p>
            {firstCourse && currentLesson && (
              <Link
                to="/courses/$courseId"
                params={{ courseId: String(firstCourse.id) }}
                className={styles.continueBtn}
              >
                Продолжить
              </Link>
            )}
          </div>

          <div className={styles.progressSection}>
            <span className={styles.progressLabel}>Твой прогресс</span>
            <p className={styles.lessonCount}>
              Урок {Math.min(currentLessonIndex + 1, totalLessons)} из {totalLessons}
            </p>
            <div className={styles.progressTrack}>
              <div
                className={styles.progressFill}
                style={{
                  width: totalLessons > 0 ? `${(currentLessonIndex / totalLessons) * 100}%` : "0%",
                }}
              />
            </div>
            {lessons && (
              <div className={styles.badges}>
                {lessons.map((lesson, i) => (
                  <div
                    key={lesson.id}
                    className={`${styles.badge} ${i < completedLessons ? styles.badgeDone : styles.badgeStar}`}
                  >
                    <img
                      src="/assets/homepage/badge.png"
                      alt={i < completedLessons ? "Выполнено" : "Задание"}
                    />
                  </div>
                ))}
              </div>
            )}
          </div>

          <div className={styles.characterSection}>
            <img
              src="/assets/homepage/character.png"
              alt="Персонаж"
              className={styles.characterImg}
            />
          </div>
        </div>
      </section>

      <section className={styles.coursesSection}>
        <div className={styles.sectionHeader}>
          <h2 className={styles.sectionTitle}>Другие курсы</h2>
          <button className={styles.allCoursesBtn}>
            <img src="/assets/homepage/expand.svg" alt="" />
            <span>Все курсы</span>
          </button>
        </div>
        <div className={styles.courseGrid}>
          {courses.data?.map((course) => (
            <CourseCard key={course.title} course={course} />
          ))}
        </div>
      </section>
    </main>
  );
};

export const Route = createFileRoute("/_protected/_sidebar/")({
  component: RouteComponent,
  beforeLoad: async () => {
    const me = await getMe().catch(() => null);
    if (me?.roleName === "parent") throw redirect({ to: "/child" });
  },
});
