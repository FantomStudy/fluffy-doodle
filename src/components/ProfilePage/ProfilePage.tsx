import type { UserProfile } from "@/api/profile";
import { useNavigate } from "@tanstack/react-router";
import { Download } from "lucide-react";
import { useCallback, useEffect, useRef, useState } from "react";
import { logout } from "@/api/auth";
import { getProfile, uploadAvatar } from "@/api/profile";
import { clearAuthenticated } from "@/lib/authSession";
import styles from "./ProfilePage.module.css";

const ProfilePage = () => {
  const navigate = useNavigate();
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const fetchProfile = useCallback(async () => {
    try {
      const data = await getProfile();
      setProfile(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Ошибка загрузки профиля");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchProfile();
  }, [fetchProfile]);

  const handleAvatarClick = () => {
    fileInputRef.current?.click();
  };

  const handleAvatarChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    try {
      await uploadAvatar(file);
      fetchProfile();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Ошибка загрузки аватара");
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

  if (loading) return <main className={styles.root}><p>Загрузка...</p></main>;
  if (error) return <main className={styles.root}><p className={styles.error}>{error}</p></main>;
  if (!profile) return null;

  return (
    <main className={styles.root}>
      <h1 className={styles.title}>Мой профиль</h1>

      <section className={styles.userCard}>
        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          className={styles.hiddenInput}
          onChange={handleAvatarChange}
        />

        {profile.avatar ? (
          <button type="button" className={styles.avatarButton} onClick={handleAvatarClick}>
            <img src={profile.avatar} alt="Аватар" className={styles.avatarImage} />
          </button>
        ) : (
          <button type="button" className={styles.avatarUpload} onClick={handleAvatarClick}>
            <Download size={48} className={styles.uploadIcon} />
            <p className={styles.uploadText}>
              Перетащите файлы или нажмите
              <br />
              для выбора
            </p>
          </button>
        )}

        <div className={styles.userInfo}>
          <p className={styles.userName}>{profile.fullName}</p>
          <p className={styles.userPhone}>{profile.phoneNumber}</p>
        </div>

        <div className={styles.userMeta}>
          <div className={styles.stars}>
            <img src="/assets/profile/star.svg" alt="" className={styles.starIcon} />
            <span className={styles.starCount}>{profile.stars}</span>
          </div>

          <button type="button" className={styles.editButton}>
            Редактировать профиль
          </button>

          <button type="button" className={styles.logoutButton} onClick={handleLogout}>
            Выйти
          </button>
        </div>
      </section>

      <h2 className={styles.achievementsTitle}>Достижения</h2>

      {profile.achievements.length === 0 ? (
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
