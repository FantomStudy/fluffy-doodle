import type { Course } from "@/api/courses";
import { Link } from "@tanstack/react-router";
import { useCourses } from "@/hooks/useCourses";
import { useProfile } from "@/hooks/useProfile";
import styles from "./HomePage.module.css";

const WAYPOINTS = [
  "/assets/homepage/wp1.svg",
  "/assets/homepage/wp2.svg",
  "/assets/homepage/wp3.svg",
  "/assets/homepage/wp4.svg",
  "/assets/homepage/wp5.svg",
  "/assets/homepage/wp6.svg",
  "/assets/homepage/wp7.svg",
  "/assets/homepage/wp8.svg",
];

const WAYPOINT_POSITIONS = [
  { left: 12.6, top: 83.1 },
  { left: 22.6, top: 71.7 },
  { left: 25.7, top: 41.1 },
  { left: 30.6, top: 3.8 },
  { left: 45.3, top: 9.6 },
  { left: 58.0, top: 35.9 },
  { left: 72.0, top: 61.2 },
  { left: 88.3, top: 47.2 },
];

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
      <img src="/assets/homepage/star.svg" alt="" className={styles.courseStar} />
      <span className={styles.courseLevel}>{level}</span>
      <span className={styles.courseDivider} />
      <span className={styles.courseLessons}>{totalLessons} уроков</span>
    </div>
  </Link>
);

const HomePage = () => {
  const { data: profile } = useProfile();
  const { data: courses } = useCourses();

  const greeting = profile?.fullName
    ? `Рад твоему возращению, ${profile.fullName}`
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
              <span>{profile?.stars ?? 0}</span>
            </div>
            <div className={styles.streakBadge}>
              <img src="/assets/homepage/fire.png" alt="серия" />
              <span>Серия: 3 дня подряд</span>
            </div>
          </div>
        </div>

        <div className={styles.mapSection}>
          <img src="/assets/homepage/map-bg.png" alt="Карта обучения" className={styles.mapBg} />
          <img src="/assets/homepage/map-path.svg" alt="" className={styles.mapPath} aria-hidden />
          {WAYPOINTS.map((src, i) => (
            <img
              key={i}
              src={src}
              alt=""
              aria-hidden
              className={styles.waypoint}
              style={{
                left: `${WAYPOINT_POSITIONS[i].left}%`,
                top: `${WAYPOINT_POSITIONS[i].top}%`,
              }}
            />
          ))}
          <div className={styles.expandMap}>
            <img src="/assets/homepage/expand.svg" alt="" />
            <span>Открыть на весь экран</span>
          </div>
        </div>

        <div className={styles.cardFooter}>
          <div className={styles.currentLesson}>
            <span className={styles.lessonLabel}>Уровень</span>
            <h2 className={styles.lessonTitle}>Уровень {profile?.level ?? 1}</h2>
            <p className={styles.lessonDesc}>
              {profile?.exp ?? 0} / {profile?.expToNextLevel ?? 100} XP
            </p>
            <Link to="/courses" className={styles.continueBtn}>
              Продолжить
            </Link>
          </div>

          <div className={styles.progressSection}>
            <span className={styles.progressLabel}>Твой прогресс</span>
            <p className={styles.lessonCount}>{courses?.length ?? 0} курсов доступно</p>
            <div className={styles.progressTrack}>
              <div
                className={styles.progressFill}
                style={{
                  width: `${profile?.expToNextLevel ? (profile.exp / profile.expToNextLevel) * 100 : 0}%`,
                }}
              />
            </div>
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
          <Link to="/courses" className={styles.allCoursesBtn}>
            <img src="/assets/homepage/expand.svg" alt="" />
            <span>Все курсы</span>
          </Link>
        </div>
        <div className={styles.courseGrid}>
          {courses?.map((course) => (
            <CourseCard key={course.id} {...course} />
          ))}
        </div>
      </section>
    </main>
  );
};

export { HomePage };
