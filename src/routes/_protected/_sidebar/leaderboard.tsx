import { createFileRoute } from "@tanstack/react-router";
import { Star } from "lucide-react";
import { useLeaderboard } from "@/hooks/useLeaderboard";
import { useMe } from "@/hooks/useMe";
import styles from "./leaderboard.module.css";

const getRankClass = (index: number) => {
  if (index === 0) return styles.rankGold;
  if (index === 1) return styles.rankSilver;
  if (index === 2) return styles.rankBronze;
  return "";
};

const RouteComponent = () => {
  const { data: entries, isLoading, error } = useLeaderboard(20);
  const { data: me } = useMe();

  if (isLoading)
    return (
      <main className={styles.root}>
        <p>Загрузка...</p>
      </main>
    );

  if (error)
    return (
      <main className={styles.root}>
        <p>Ошибка загрузки лидерборда</p>
      </main>
    );

  return (
    <main className={styles.root}>
      <h1 className={styles.title}>Лидерборд</h1>

      {!entries?.length ? (
        <p className={styles.emptyText}>Пока нет участников</p>
      ) : (
        <div className={styles.list}>
          {entries.map((entry, index) => {
            const isSelf = me?.id === entry.id;

            return (
              <div key={entry.id} className={`${styles.row} ${isSelf ? styles.rowSelf : ""}`}>
                <span className={`${styles.rank} ${getRankClass(index)}`}>{index + 1}</span>

                <div className={styles.avatarWrap}>
                  {entry.avatar ? (
                    <img src={entry.avatar} alt="" className={styles.avatar} />
                  ) : (
                    <div className={styles.avatarPlaceholder} />
                  )}
                  {entry.activeFrame && (
                    <img src={entry.activeFrame.image} alt="" className={styles.frameOverlay} />
                  )}
                </div>

                <div className={styles.info}>
                  <p className={styles.name}>{entry.fullName}</p>
                  <span className={styles.level}>Уровень {entry.level}</span>
                </div>

                <span className={styles.stars}>
                  <Star size={16} className={styles.starIcon} />
                  {entry.stars}
                </span>
              </div>
            );
          })}
        </div>
      )}
    </main>
  );
};

export const Route = createFileRoute("/_protected/_sidebar/leaderboard")({
  component: RouteComponent,
});
