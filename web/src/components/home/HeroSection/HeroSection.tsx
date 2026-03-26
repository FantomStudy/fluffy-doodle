import s from "./HeroSection.module.css";

interface HeroSectionProps {
  userName?: string;
}

export function HeroSection({ userName = "Дима" }: HeroSectionProps) {
  return (
    <section className={s.hero}>
      <div className={s.content}>
        <p className={s.label}>Сегодняшняя миссия</p>
        <h2 className={s.title}>
          Привет, {userName}! Изучи основы безопасности в интернете
        </h2>
        <p className={s.desc}>
          Пройди короткий мини-урок, заработай{" "}
          <span className={s.descAccent}>150 XP</span> и открой следующий
          уровень без лишних шагов.
        </p>
        <div className={s.actions}>
          <a href="#" className={s.btnPrimary}>
            Пройти мини-урок
          </a>
          <a href="#" className={s.btnSecondary}>
            Открыть задания
          </a>
        </div>
        <div className={s.xpBox}>
          <div className={s.xpLabels}>
            <span className={s.xpLevelLabel}>До 8 уровня</span>
            <span className={s.xpValue}>1480 / 1630 XP</span>
          </div>
          <div className={s.xpTrack}>
            <div className={s.xpFill} />
          </div>
        </div>
      </div>

      {/* Robot illustration */}
      <div className={s.illustration} aria-hidden="true">
        <div className={s.illCircleBig} />
        <div className={s.illCircleSmall} />
        <div className={s.illPill1} />
        <div className={s.illPill2} />
        <div className={s.illStar1} />
        <div className={s.illStar2} />
        <div className={s.robot}>
          <div className={s.robotAntenna} />
          <div className={s.robotHead}>
            <div className={`${s.robotEye} ${s.robotEyeLeft}`} />
            <div className={`${s.robotEye} ${s.robotEyeRight}`} />
          </div>
          <div className={s.robotBody} />
          <div className={s.robotArmLeft} />
          <div className={s.robotArmRight} />
          <div className={s.robotLegLeft} />
          <div className={s.robotLegRight} />
        </div>
      </div>
    </section>
  );
}
