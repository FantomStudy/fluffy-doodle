import s from "./CurrentCourseCard.module.css";

interface CurrentCourseCardProps {
  courseName?: string;
  nextLesson?: string;
  progress?: number; // 0–100
}

export function CurrentCourseCard({
  courseName = "Основы программирования",
  nextLesson = "Следующий урок: команды и последовательности. Короткий формат, чтобы не терять внимание.",
  progress = 35,
}: CurrentCourseCardProps) {
  return (
    <div className={s.card}>
      {/* Book icon */}
      <div className={s.bookIcon} aria-hidden="true">
        <div className={s.bookPageLeft} />
        <div className={s.bookPageRight} />
        <div className={s.bookPage} />
      </div>

      <div className={s.content}>
        <p className={s.label}>Продолжить обучение</p>
        <h3 className={s.title}>{courseName}</h3>
        <p className={s.desc}>{nextLesson}</p>
        <div className={s.progressRow}>
          <div className={s.progressTrack}>
            <div
              className={s.progressFill}
              style={{ width: `${progress}%` }}
            />
          </div>
          <span className={s.progressValue}>{progress}%</span>
        </div>
      </div>

      <a href="#" className={s.btn}>
        Продолжить
      </a>
    </div>
  );
}
