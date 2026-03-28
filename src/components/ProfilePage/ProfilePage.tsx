import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import {
  Camera,
  CheckCircle2,
  Download,
  Flame,
  Loader2,
  LogOut,
  ShoppingCart,
  Star,
  Zap,
} from "lucide-react";
import { useRef, useState } from "react";
import { logout } from "@/api/auth";
import type { Frame } from "@/api/frames";
import { uploadAvatar } from "@/api/user";
import { useBuyFrame, useFrames, useSetActiveFrame } from "@/hooks/useFrames";
import { useMe } from "@/hooks/useMe";
import { useProfile } from "@/hooks/useProfile";
import styles from "./ProfilePage.module.css";

const DEFAULT_FRAMES: Frame[] = [
  { id: 1, name: "Золотая", price: 50, image: "/assets/frames/frame-gold.svg", owned: false },
  { id: 2, name: "Аметист", price: 75, image: "/assets/frames/frame-purple.svg", owned: false },
  { id: 3, name: "Изумруд", price: 100, image: "/assets/frames/frame-emerald.svg", owned: false },
  { id: 4, name: "Сапфир", price: 120, image: "/assets/frames/frame-sapphire.svg", owned: false },
];

const ProfilePage = () => {
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);

  const fileInputRef = useRef<HTMLInputElement>(null);

  const navigate = useNavigate();

  const queryClient = useQueryClient();
  const { data: profile, isLoading, error } = useProfile();
  const { data: me } = useMe();
  const { data: frames } = useFrames();
  const buyFrameMut = useBuyFrame();
  const setActiveMut = useSetActiveFrame();

  const displayFrames = frames?.length ? frames : DEFAULT_FRAMES;

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

  const expPercent =
    profile.expToNextLevel > 0 ? Math.min((profile.exp / profile.expToNextLevel) * 100, 100) : 0;
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
          <div className={styles.avatarWrap}>
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
            {profile.activeFrame && (
              <img src={profile.activeFrame.image} alt="" className={styles.frameOverlay} />
            )}
          </div>

          <div className={styles.levelBadge}>
            <span className={styles.levelNum}>{profile.level}</span>
            <span className={styles.levelLabel}>уровень</span>
          </div>
        </div>

        <div className={styles.userInfo}>
          <p className={styles.userName}>{profile.fullName}</p>
          <p className={styles.userLogin}>{profile.login}</p>
          <p className={styles.userPhone}>{profile.phoneNumber}</p>
          <div className={styles.studentCodeWrap}>
            <span className={styles.studentCodeLabel}>Личный код студента</span>
            <span className={styles.studentCode}>{profile.studentCode}</span>
          </div>

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
            <div className={styles.statCard}>
              <Flame size={20} className={styles.statIconStreak} />
              <div className={styles.statData}>
                <span className={styles.statValue}>{profile.streak}</span>
                <span className={styles.statLabel}>серия</span>
              </div>
            </div>
          </div>

          <div className={styles.expSection}>
            <div className={styles.expHeader}>
              <span className={styles.expLabel}>До уровня {profile.level + 1}</span>
              <span className={styles.expNumbers}>
                {profile.exp} / {profile.expToNextLevel} XP
              </span>
            </div>
            <div className={styles.expTrack}>
              <div className={styles.expFill} style={{ width: `${expPercent}%` }} />
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

      <h2 className={styles.framesTitle}>Рамки аватара</h2>
      <p className={styles.framesSubtitle}>
        <Star size={14} className={styles.starInline} />У вас {me?.stars ?? 0} звёзд
      </p>

      <section className={styles.framesGrid}>
        {displayFrames.map((frame) => {
          const isActive = profile.activeFrame?.id === frame.id;
          const canAfford = (me?.stars ?? 0) >= frame.price;

          return (
            <article
              key={frame.id}
              className={`${styles.frameCard} ${isActive ? styles.frameCardActive : ""}`}
            >
              <div className={styles.framePreview}>
                <div className={styles.frameAvatarDemo}>
                  {profile.avatar ? (
                    <img src={profile.avatar} alt="" className={styles.frameDemoImg} />
                  ) : (
                    <div className={styles.frameDemoPlaceholder} />
                  )}
                  <img src={frame.image} alt="" className={styles.frameDemoOverlay} />
                </div>
              </div>

              <p className={styles.frameName}>{frame.name}</p>

              <div className={styles.frameActions}>
                {!frame.owned ? (
                  <button
                    type="button"
                    className={styles.frameBuyBtn}
                    disabled={!canAfford || buyFrameMut.isPending}
                    onClick={() => buyFrameMut.mutate(frame.id)}
                  >
                    <ShoppingCart size={14} />
                    {frame.price}
                    <Star size={12} />
                  </button>
                ) : isActive ? (
                  <button
                    type="button"
                    className={styles.frameRemoveBtn}
                    disabled={setActiveMut.isPending}
                    onClick={() => setActiveMut.mutate(0)}
                  >
                    Снять
                  </button>
                ) : (
                  <button
                    type="button"
                    className={styles.frameEquipBtn}
                    disabled={setActiveMut.isPending}
                    onClick={() => setActiveMut.mutate(frame.id)}
                  >
                    <CheckCircle2 size={14} />
                    Надеть
                  </button>
                )}
              </div>

              {isActive && <span className={styles.frameActiveBadge}>Активна</span>}
            </article>
          );
        })}
      </section>
    </main>
  );
};

export { ProfilePage };
