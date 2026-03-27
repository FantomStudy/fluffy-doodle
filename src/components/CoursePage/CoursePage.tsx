import { Link } from "@tanstack/react-router";
import { ArrowLeftIcon, CheckIcon, ClockIcon, LockIcon } from "lucide-react";
import styles from "./CoursePage.module.css";

type LessonStatus = "completed" | "current" | "locked";

interface Lesson {
  number: number;
  title: string;
  description: string;
  image: string;
  status: LessonStatus;
}

interface Achievement {
  title: string;
  description: string;
}

const LESSONS: Lesson[] = [
  {
    number: 1,
    title: "Знакомство с Python",
    description:
      "Краткое описание курса Краткое описание курсаКраткое описание курсаКраткое описание курса",
    image: "/assets/course/course-python.png",
    status: "completed",
  },
  {
    number: 2,
    title: "Знакомство с Python",
    description:
      "Краткое описание курса Краткое описание курсаКраткое описание курсаКраткое описание курса",
    image: "/assets/course/course-python.png",
    status: "completed",
  },
  {
    number: 3,
    title: "Знакомство с Python",
    description:
      "Краткое описание курса Краткое описание курсаКраткое описание курсаКраткое описание курса",
    image: "/assets/course/course-python.png",
    status: "completed",
  },
  {
    number: 4,
    title: "Знакомство с Python",
    description:
      "Краткое описание курса Краткое описание курсаКраткое описание курсаКраткое описание курса",
    image: "/assets/course/course-python.png",
    status: "current",
  },
  {
    number: 5,
    title: "Знакомство с Python",
    description:
      "Краткое описание курса Краткое описание курсаКраткое описание курсаКраткое описание курса",
    image: "/assets/course/course-python.png",
    status: "locked",
  },
  {
    number: 6,
    title: "Знакомство с Python",
    description:
      "Краткое описание курса Краткое описание курсаКраткое описание курсаКраткое описание курса",
    image: "/assets/course/course-python.png",
    status: "locked",
  },
];

const ACHIEVEMENTS: Achievement[] = [
  { title: "Первая программа", description: "Написал свой первых код" },
  { title: "Первая программа", description: "Написал свой первых код" },
  { title: "Первая программа", description: "Написал свой первых код" },
  { title: "Первая программа", description: "Написал свой первых код" },
];

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

const LessonCard = ({ number, title, description, image, status }: Lesson) => {
  const config = STATUS_CONFIG[status];

  return (
    <div className={styles.lessonCard}>
      <div className={`${styles.lessonNumber} ${config.numberClass}`}>{number}</div>
      <div className={styles.lessonContent}>
        <h3 className={styles.lessonTitle}>{title}</h3>
        <p className={styles.lessonDesc}>{description}</p>
        <span className={`${styles.lessonBadge} ${config.badgeClass}`}>
          {config.icon}
          {config.label}
        </span>
      </div>
      <div className={styles.lessonImage}>
        <img src={image} alt={title} />
      </div>
    </div>
  );
};

const CoursePage = () => (
  <main className={styles.root}>
    <Link to="/" className={styles.backLink}>
      <ArrowLeftIcon size={20} />
      <span>К списку курсов</span>
    </Link>

    <div className={styles.layout}>
      <div className={styles.mainColumn}>
        <section className={styles.courseHeader}>
          <div className={styles.courseImage}>
            <img src="/assets/course/course-python.png" alt="Python для начинающих" />
          </div>
          <div className={styles.courseInfo}>
            <h1 className={styles.courseTitle}>Python для начинающих</h1>
            <p className={styles.courseDesc}>Краткое описание курса</p>
            <div className={styles.courseMeta}>
              <span className={styles.metaBadge}>12 уроков</span>
              <span className={`${styles.metaBadge} ${styles.metaLevel}`}>Средний уровень</span>
              <span className={styles.metaTime}>
                <ClockIcon size={16} />
                6 часов
              </span>
            </div>
          </div>
        </section>

        <section className={styles.lessonsList}>
          {LESSONS.map((lesson) => (
            <LessonCard key={lesson.number} {...lesson} />
          ))}
        </section>
      </div>

      <aside className={styles.sidebar}>
        <div className={styles.progressCard}>
          <h2 className={styles.sidebarTitle}>Твой прогресс</h2>
          <div className={styles.progressRow}>
            <div className={styles.progressTrack}>
              <div className={styles.progressFill} />
            </div>
            <div className={styles.starCount}>
              <img src="/assets/course/star-badge.svg" alt="очки" className={styles.starIcon} />
              <span>18</span>
            </div>
          </div>
          <p className={styles.progressText}>5 из 12 урока</p>
        </div>

        <div className={styles.achievementsCard}>
          <h2 className={styles.sidebarTitle}>Достижения курса</h2>
          <p className={styles.achievementsSubtitle}>Собирай награды за свои успехи!</p>
          <div className={styles.achievementsList}>
            {ACHIEVEMENTS.map((a, i) => (
              <div key={i} className={styles.achievementItem}>
                <div className={styles.achievementIcon} />
                <div>
                  <p className={styles.achievementTitle}>{a.title}</p>
                  <p className={styles.achievementDesc}>{a.description}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </aside>
    </div>
  </main>
);

export { CoursePage };
