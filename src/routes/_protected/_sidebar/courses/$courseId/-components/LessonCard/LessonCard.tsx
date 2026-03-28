import type { Lesson } from "@/api/courses";
import { Link } from "@tanstack/react-router";
import clsx from "clsx";
import { CheckIcon, LockIcon } from "lucide-react";
import styles from "./LessonCard.module.css";

type LessonStatus = "completed" | "current" | "locked";

interface LessonDisplay {
  id: number;
  courseId: number;
  number: number;
  title: string;
  description: string;
  status: LessonStatus;
  tasks: Lesson["tasks"];
}

const STATUS_CONFIG = {
  completed: {
    numberClass: styles.numberCompleted,
    label: "Пройдено",
    badgeClass: styles.badgeCompleted,
    icon: <CheckIcon size={12} />,
  },
  current: {
    numberClass: styles.numberCurrent,
    label: "Текущий урок",
    badgeClass: styles.badgeCurrent,
    icon: null,
  },
  locked: {
    numberClass: styles.numberLocked,
    label: "Заблокировано",
    badgeClass: styles.badgeLocked,
    icon: <LockIcon size={12} />,
  },
} as const;

export const LessonCard = ({ id, courseId, number, title, description, status }: LessonDisplay) => {
  const config = STATUS_CONFIG[status];

  return (
    <Link
      to="/courses/$courseId/lessons/$lessonId"
      params={{ courseId: String(courseId), lessonId: String(id) }}
      className={styles.lessonCard}
    >
      <div className={clsx(styles.lessonNumber, config.numberClass)}>{number}</div>
      <div className={styles.lessonContent}>
        <h3 className={styles.lessonTitle}>{title}</h3>
        <p className={styles.lessonDesc}>{description}</p>
        <span className={`${styles.lessonBadge} ${config.badgeClass}`}>
          {config.icon}
          {config.label}
        </span>
      </div>
    </Link>
  );
};
