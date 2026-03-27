import styles from "./HomePage.module.css";

type CourseLevel = "Начальный" | "Средний" | "Продвинутый";

interface Course {
  image: string;
  category: string;
  title: string;
  description: string;
  level: CourseLevel;
  lessons: number;
}

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

const COURSES: Course[] = [
  {
    image: "/assets/homepage/course-python.png",
    category: "Программирование",
    title: "Изучение Python",
    description: "Краткое описание про курс",
    level: "Средний",
    lessons: 12,
  },
  {
    image: "/assets/homepage/course-js.png",
    category: "Программирование",
    title: "Основы JavaScript",
    description: "Введение в веб-разработку",
    level: "Начальный",
    lessons: 10,
  },
  {
    image: "/assets/homepage/course-design.png",
    category: "Дизайн",
    title: "Дизайн интерфейсов",
    description: "Создание удобных интерфейсов",
    level: "Средний",
    lessons: 15,
  },
];

const LESSON_BADGES: { done: boolean }[] = [
  { done: true },
  { done: true },
  { done: true },
  { done: false },
  { done: false },
];

const CourseCard = ({ image, category, title, description, level, lessons }: Course) => (
  <div className={styles.courseCard}>
    <div className={styles.courseImage}>
      <img src={image} alt={title} />
    </div>
    <span className={styles.categoryBadge}>{category}</span>
    <h3 className={styles.courseTitle}>{title}</h3>
    <p className={styles.courseDesc}>{description}</p>
    <div className={styles.courseMeta}>
      <img src="/assets/homepage/star.svg" alt="" className={styles.courseStar} />
      <span className={styles.courseLevel}>{level}</span>
      <span className={styles.courseDivider} />
      <span className={styles.courseLessons}>{lessons} уроков</span>
    </div>
  </div>
);

const HomePage = () => (
  <main className={styles.root}>
    <header className={styles.header}>
      <h1 className={styles.greeting}>Рад твоему возращению, Сергей Дубской</h1>
      <p className={styles.subtitle}>Твой учебный процесс</p>
    </header>

    <section className={styles.learningCard}>
      <div className={styles.cardHeader}>
        <span className={styles.cardLabel}>Твой учебный процесс</span>
        <div className={styles.cardActions}>
          <div className={styles.starCount}>
            <img src="/assets/homepage/star-badge.svg" alt="очки" className={styles.starIcon} />
            <span>18</span>
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
          <span className={styles.lessonLabel}>Текущий урок</span>
          <h2 className={styles.lessonTitle}>Цвет и Материалы</h2>
          <p className={styles.lessonDesc}>Краткое описание урока</p>
          <button className={styles.continueBtn}>Продолжить</button>
        </div>

        <div className={styles.progressSection}>
          <span className={styles.progressLabel}>Твой прогресс</span>
          <p className={styles.lessonCount}>Урок 4 из 5</p>
          <div className={styles.progressTrack}>
            <div className={styles.progressFill} />
          </div>
          <div className={styles.badges}>
            {LESSON_BADGES.map((badge, i) => (
              <div
                key={i}
                className={`${styles.badge} ${badge.done ? styles.badgeDone : styles.badgeStar}`}
              >
                <img src="/assets/homepage/badge.png" alt={badge.done ? "Выполнено" : "Задание"} />
              </div>
            ))}
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
        <button className={styles.allCoursesBtn}>
          <img src="/assets/homepage/expand.svg" alt="" />
          <span>Все курсы</span>
        </button>
      </div>
      <div className={styles.courseGrid}>
        {COURSES.map((course) => (
          <CourseCard key={course.title} {...course} />
        ))}
      </div>
    </section>
  </main>
);

export { HomePage };
