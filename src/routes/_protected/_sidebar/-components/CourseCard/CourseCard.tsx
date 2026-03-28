import type { Course } from "@/api/courses";
import { Link } from "@tanstack/react-router";
import { StarIcon } from "lucide-react";
import { Badge } from "@/components/ui/Badge";
import { Card } from "@/components/ui/Card";
import styles from "./CourseCard.module.css";

interface CourseCardProps {
  course: Course;
}

export const CourseCard = ({ course }: CourseCardProps) => {
  return (
    <Link to="/courses/$courseId" params={{ courseId: String(course.id) }}>
      <Card className={styles.card}>
        <img src={course.imageUrl} alt={course.title} className={styles.courseImage} />

        <Badge>{course.categoryName}</Badge>
        <div>
          <h3 className={styles.courseTitle}>{course.title}</h3>
          <p className={styles.courseDesc}>{course.description}</p>
        </div>
        <div className={styles.courseMeta}>
          <StarIcon className={styles.star} />
          <span className={styles.courseLevel}>{course.level}</span>
          <span className={styles.courseDivider} />
          <span className={styles.courseLessons}>{course.totalLessons} уроков</span>
        </div>
      </Card>
    </Link>
  );
};
