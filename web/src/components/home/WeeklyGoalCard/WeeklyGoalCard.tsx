import s from "./WeeklyGoalCard.module.css";

interface WeeklyGoalCardProps {
  totalLessons?: number;
  completedLessons?: number;
  xpReward?: number;
  rewardBadge?: string;
}

export function WeeklyGoalCard({
  totalLessons = 5,
  completedLessons = 4,
  xpReward = 100,
  rewardBadge = "«Молодец»",
}: WeeklyGoalCardProps) {
  return (
    <div className={s.card}>
      <div className={s.header}>
        <div className={s.titleGroup}>
          <span className={s.label}>Цель недели</span>
          <h3 className={s.title}>
            Завершить {totalLessons} уроков
          </h3>
        </div>
        <div className={s.xpBadge}>{xpReward} XP</div>
      </div>

      <p className={s.desc}>
        Остался еще один урок, чтобы получить награду и открыть бейдж{" "}
        {rewardBadge}.
      </p>

      <div className={s.dots}>
        {Array.from({ length: totalLessons }).map((_, i) => (
          <div
            key={i}
            className={`${s.dot} ${i < completedLessons ? s.dotFilled : s.dotEmpty}`}
          />
        ))}
      </div>

      <div className={s.rewardBox}>
        <p className={s.rewardLabel}>Награда:</p>
        <p className={s.rewardValue}>
          Бейдж {rewardBadge} + {xpReward} XP
        </p>
      </div>

      <a href="#" className={s.btn}>
        Посмотреть все
      </a>
    </div>
  );
}
