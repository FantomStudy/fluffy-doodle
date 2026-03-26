import s from "./StatsBar.module.css";

interface StatItem {
  icon: string;
  iconGradient: string;
  value: string;
  label: string;
}

const stats: StatItem[] = [
  {
    icon: "↗",
    iconGradient: "linear-gradient(180deg, #6cd4ff 0%, #5596ff 100%)",
    value: "Уровень 7",
    label: "Еще 150 XP до нового ранга",
  },
  {
    icon: "★",
    iconGradient: "linear-gradient(180deg, #ffd56d 0%, #ff9722 100%)",
    value: "1480 XP",
    label: "Уверенный темп за неделю",
  },
  {
    icon: "✦",
    iconGradient: "linear-gradient(180deg, #ffb24b 0%, #ff6e33 100%)",
    value: "5 дней",
    label: "Серия без пропусков",
  },
  {
    icon: "▣",
    iconGradient: "linear-gradient(180deg, #8bd68b 0%, #4fb978 100%)",
    value: "4 урока",
    label: "Завершено за последние 7 дней",
  },
];

export function StatsBar() {
  return (
    <div className={s.bar}>
      {stats.map((stat) => (
        <div key={stat.value} className={s.card}>
          <div
            className={s.iconWrap}
            style={{ background: stat.iconGradient }}
          >
            <span className={s.icon}>{stat.icon}</span>
          </div>
          <div className={s.info}>
            <span className={s.value}>{stat.value}</span>
            <span className={s.statLabel}>{stat.label}</span>
          </div>
        </div>
      ))}
    </div>
  );
}
