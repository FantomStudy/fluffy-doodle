import s from "./ModuleCard.module.css";

export interface ModuleCardProps {
  title: string;
  description: string;
  /** CSS gradient string for the card header background */
  headerGradient: string;
  /** CSS gradient for the action button */
  btnGradient: string;
  /** Box shadow for the action button */
  btnShadow?: string;
  /** Progress label e.g. "35% завершено" or "Новый модуль" */
  progressLabel: string;
  /** Action button label */
  actionLabel: string;
  /** Optional large letter/symbol shown in header */
  headerLetter?: string;
}

export function ModuleCard({
  title,
  description,
  headerGradient,
  btnGradient,
  btnShadow = "0px 10px 18px 0px rgba(71, 132, 238, 0.2)",
  progressLabel,
  actionLabel,
  headerLetter,
}: ModuleCardProps) {
  return (
    <div className={s.card}>
      <div
        className={s.cardHeader}
        style={{ background: headerGradient }}
      >
        <div className={s.cardHeaderInner}>
          {headerLetter && (
            <span className={s.cardHeaderLetter}>{headerLetter}</span>
          )}
        </div>
      </div>

      <div className={s.cardBody}>
        <h4 className={s.cardTitle}>{title}</h4>
        <p className={s.cardDesc}>{description}</p>
        <div className={s.cardFooter}>
          <span className={s.cardProgress}>{progressLabel}</span>
          <a
            href="#"
            className={s.cardBtn}
            style={{
              background: btnGradient,
              boxShadow: btnShadow,
            }}
          >
            {actionLabel}
          </a>
        </div>
      </div>
    </div>
  );
}
