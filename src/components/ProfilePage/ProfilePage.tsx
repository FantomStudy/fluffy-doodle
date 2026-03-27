import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { Download } from "lucide-react";
import { useRef, useState } from "react";
import { logout } from "@/api/auth";
import { uploadAvatar } from "@/api/user";
import { useProfile } from "@/hooks/useProfile";
import { clearAuthenticated } from "@/lib/authSession";
import styles from "./ProfilePage.module.css";

const ProfilePage = () => {
  const [uploadError, setUploadError] = useState<string | null>(null);

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

    try {
      await uploadAvatar(file);
      queryClient.invalidateQueries({ queryKey: ["profile"] });
    } catch (err) {
      setUploadError(err instanceof Error ? err.message : "Ошибка загрузки аватара");
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
