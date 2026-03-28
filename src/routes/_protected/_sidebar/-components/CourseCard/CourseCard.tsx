import { StarIcon } from "lucide-react";
import { Badge } from "@/components/ui/Badge";
import { Card } from "@/components/ui/Card";
import styles from "./CourseCard.module.css";

interface CourseCardProps {
  image: string;
  category: string;
  title: string;
  description: string;
  level: "Начальный" | "Средний" | "Продвинутый";
  lessons: number;
}

export const CourseCard = ({
  image,
  category,
  title,
  description,
  level,
  lessons,
}: CourseCardProps) => {
  return (
    <Card className={styles.card}>
      <img src={image} alt={title} className={styles.courseImage} />

      <Badge>{category}</Badge>
      <div>
        <h3 className={styles.courseTitle}>{title}</h3>
        <p className={styles.courseDesc}>{description}</p>
      </div>
      <div className={styles.courseMeta}>
        <StarIcon className={styles.star} />
        <span className={styles.courseLevel}>{level}</span>
        <span className={styles.courseDivider} />
        <span className={styles.courseLessons}>{lessons} уроков</span>
      </div>
    </Card>
  );
};
