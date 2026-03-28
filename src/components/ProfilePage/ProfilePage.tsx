import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { Camera, Download, Loader2, LogOut, Star, Zap } from "lucide-react";
import { useRef, useState } from "react";
import { logout } from "@/api/auth";
import { uploadAvatar } from "@/api/user";
import { useProfile } from "@/hooks/useProfile";
import { clearAuthenticated } from "@/lib/authSession";
import styles from "./ProfilePage.module.css";

const ProfilePage = () => {
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);

  const fileInputRef = useRef<HTMLInputElement>(null);

  const navigate = useNavigate();

  const queryClient = useQueryClient();
  const { data: profile, isLoading, error } = useProfile();

  const handleAvatarClick = () => {
    fileInputRef.current?.click();
  };

  const handleAvatarChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setUploadError(null);
    setUploading(true);
    try {
      await uploadAvatar(file);
      queryClient.invalidateQueries({ queryKey: ["profile"] });
    } catch (err) {
      setUploadError(err instanceof Error ? err.message : "Ошибка загрузки аватара");
    } finally {
      setUploading(false);
      if (fileInputRef.current) fileInputRef.current.value = "";
    }
  };

  const handleLogout = async () => {
    try {
      await logout();
    } finally {
      clearAuthenticated();
      navigate({ to: "/login" });
    }
  };

  if (isLoading)
    return (
      <main className={styles.root}>
        <p>Загрузка...</p>
      </main>
    );
  if (error)
    return (
      <main className={styles.root}>
        <p className={styles.error}>
          {error instanceof Error ? error.message : "Ошибка загрузки профиля"}
        </p>
      </main>
    );
  if (!profile) return null;

  const expPercent = profile.expToNextLevel > 0
    ? Math.min((profile.exp / profile.expToNextLevel) * 100, 100)
    : 0;
  const expRemaining = Math.max(profile.expToNextLevel - profile.exp, 0);

  return (
    <main className={styles.root}>
      <h1 className={styles.title}>Мой профиль</h1>

      {uploadError && <p className={styles.error}>{uploadError}</p>}

      <section className={styles.userCard}>
        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          className={styles.hiddenInput}
          onChange={handleAvatarChange}
        />

        <div className={styles.avatarCol}>
          {profile.avatar ? (
            <button type="button" className={styles.avatarButton} onClick={handleAvatarClick}>
              <img src={profile.avatar} alt="Аватар" className={styles.avatarImage} />
              <div className={styles.avatarOverlay}>
                {uploading ? (
                  <Loader2 size={24} className={styles.avatarSpinner} />
                ) : (
                  <>
                    <Camera size={24} />
                    <span>Изменить</span>
                  </>
                )}
              </div>
            </button>
          ) : (
            <button type="button" className={styles.avatarUpload} onClick={handleAvatarClick}>
              {uploading ? (
                <Loader2 size={48} className={styles.avatarSpinner} />
              ) : (
                <>
                  <Download size={48} className={styles.uploadIcon} />
                  <p className={styles.uploadText}>
                    Загрузить
                    <br />
                    аватар
                  </p>
                </>
              )}
            </button>
          )}

          <div className={styles.levelBadge}>
            <span className={styles.levelNum}>{profile.level}</span>
            <span className={styles.levelLabel}>уровень</span>
          </div>
        </div>

        <div className={styles.userInfo}>
          <p className={styles.userName}>{profile.fullName}</p>
          <p className={styles.userLogin}>{profile.login}</p>
          <p className={styles.userPhone}>{profile.phoneNumber}</p>

          <div className={styles.statsRow}>
            <div className={styles.statCard}>
              <Star size={20} className={styles.statIconStar} />
              <div className={styles.statData}>
                <span className={styles.statValue}>{profile.stars}</span>
                <span className={styles.statLabel}>звёзд</span>
              </div>
            </div>
            <div className={styles.statCard}>
              <Zap size={20} className={styles.statIconExp} />
              <div className={styles.statData}>
                <span className={styles.statValue}>{profile.exp}</span>
                <span className={styles.statLabel}>опыт</span>
              </div>
            </div>
          </div>

          <div className={styles.expSection}>
            <div className={styles.expHeader}>
              <span className={styles.expLabel}>
                До уровня {profile.level + 1}
              </span>
              <span className={styles.expNumbers}>
                {profile.exp} / {profile.expToNextLevel} XP
              </span>
            </div>
            <div className={styles.expTrack}>
              <div
                className={styles.expFill}
                style={{ width: `${expPercent}%` }}
              />
              <div className={styles.expGlow} style={{ width: `${expPercent}%` }} />
            </div>
            <p className={styles.expHint}>
              {expRemaining > 0
                ? `Ещё ${expRemaining} XP до следующего уровня`
                : "Уровень достигнут! 🎉"}
            </p>
          </div>
        </div>

        <div className={styles.userMeta}>
          <button type="button" className={styles.editButton}>
            Редактировать профиль
          </button>

          <button type="button" className={styles.logoutButton} onClick={handleLogout}>
            <LogOut size={16} />
            Выйти
          </button>
        </div>
      </section>

      <h2 className={styles.achievementsTitle}>Достижения</h2>

      {!profile.achievements?.length ? (
        <p className={styles.emptyText}>Пока нет достижений</p>
      ) : (
        <section className={styles.achievementsGrid}>
          {profile.achievements.map((item) => (
            <article key={item.id} className={styles.achievementCard}>
              <div className={styles.achievementImageWrap}>
                <img src={item.icon} alt={item.name} className={styles.achievementImage} />
              </div>
              <p className={styles.achievementName}>{item.name}</p>
              <p className={styles.achievementDesc}>{item.description}</p>
            </article>
          ))}
        </section>
      )}
    </main>
  );
};

export { ProfilePage };
