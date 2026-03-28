import { useNavigate } from "@tanstack/react-router";
import { LogOut, Star, Trophy, Zap } from "lucide-react";
import { logout } from "@/api/auth";
import { useChildProgress } from "@/hooks/useChildProgress";
import styles from "./ChildProgressPage.module.css";

const ChildProgressPage = () => {
  const { data: child, isLoading, error } = useChildProgress();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await logout();
    } finally {
      navigate({ to: "/login" });
    }
  };

  if (isLoading)
    return (
      <main className={styles.root}>
        <p className={styles.hint}>Загрузка...</p>
      </main>
    );

  if (error)
    return (
      <main className={styles.root}>
        <p className={styles.error}>
          {error instanceof Error ? error.message : "Ошибка загрузки данных"}
        </p>
      </main>
    );

  if (!child) return null;

  const expPercent = child.level > 0 ? Math.min((child.exp / 100) * 100, 100) : 0;

  return (
    <main className={styles.root}>
      <h1 className={styles.title}>Профиль ребёнка</h1>

      <section className={styles.card}>
        <div className={styles.avatar}>
          <span className={styles.avatarInitial}>{child.studentName.charAt(0)}</span>
        </div>

        <div className={styles.info}>
          <p className={styles.name}>{child.studentName}</p>
          <p className={styles.login}>{child.studentLogin}</p>

          <div className={styles.codeWrap}>
            <span className={styles.codeLabel}>Личный код студента</span>
            <span className={styles.code}>{child.studentCode}</span>
          </div>
        </div>

        <div className={styles.levelBadge}>
          <span className={styles.levelNum}>{child.level}</span>
          <span className={styles.levelLabel}>уровень</span>
        </div>

        <button type="button" className={styles.logoutButton} onClick={handleLogout}>
          <LogOut size={16} />
          Выйти
        </button>
      </section>

      <div className={styles.statsRow}>
        <div className={styles.statCard}>
          <Star size={22} className={styles.iconStar} />
          <div className={styles.statData}>
            <span className={styles.statValue}>{child.stars}</span>
            <span className={styles.statLabel}>Звёзд заработано</span>
          </div>
        </div>

        <div className={styles.statCard}>
          <Zap size={22} className={styles.iconExp} />
          <div className={styles.statData}>
            <span className={styles.statValue}>{child.exp}</span>
            <span className={styles.statLabel}>Очков опыта</span>
          </div>
        </div>

        <div className={styles.statCard}>
          <Trophy size={22} className={styles.iconTrophy} />
          <div className={styles.statData}>
            <span className={styles.statValue}>{child.achievements}</span>
            <span className={styles.statLabel}>Достижений</span>
          </div>
        </div>
      </div>

      <section className={styles.expSection}>
        <div className={styles.expHeader}>
          <span className={styles.expLabel}>До уровня {child.level + 1}</span>
          <span className={styles.expNumbers}>{child.exp} / 100 XP</span>
        </div>
        <div className={styles.expTrack}>
          <div className={styles.expFill} style={{ width: `${expPercent}%` }} />
          <div className={styles.expGlow} style={{ width: `${expPercent}%` }} />
        </div>
        <p className={styles.expHint}>
          {child.exp < 100
            ? `Ещё ${100 - child.exp} XP до следующего уровня`
            : "Уровень достигнут! 🎉"}
        </p>
      </section>
    </main>
  );
};

export { ChildProgressPage };
