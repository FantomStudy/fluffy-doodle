import type { Task } from "@/api/courses";
import { Link } from "@tanstack/react-router";
import { ArrowLeftIcon, ChevronRightIcon, ClockIcon } from "lucide-react";
import { useState } from "react";
import { CodingChallenge } from "@/components/CodingChallenge";
import { FlowchartChallenge } from "@/components/FlowchartChallenge";
import { QuizChallenge } from "@/components/QuizChallenge";
import { useCourseLessons, useSubmitTask } from "@/hooks/useCourses";
import styles from "./LessonPage.module.css";

const TASK_ICONS: Record<Task["type"], { emoji: string; className: string }> = {
  quiz: { emoji: "📝", className: styles.taskIconQuiz },
  flowchart: { emoji: "🔷", className: styles.taskIconFlowchart },
  algorithm: { emoji: "⚙️", className: styles.taskIconAlgorithm },
};

const TASK_LABELS: Record<Task["type"], string> = {
  quiz: "Тест",
  flowchart: "Блок-схема",
  algorithm: "Алгоритм",
};

interface LessonPageProps {
  courseId: number;
  lessonId: number;
}

const LessonPage = ({ courseId, lessonId }: LessonPageProps) => {
  const { data: lessons, isLoading } = useCourseLessons(courseId);
  const submitMutation = useSubmitTask(courseId);
  const [activeTaskId, setActiveTaskId] = useState<number | null>(null);

  if (isLoading) {
    return (
      <main className={styles.root}>
        <p>Загрузка...</p>
      </main>
    );
  }

  const lesson = lessons?.find((l) => l.id === lessonId);

  if (!lesson) {
    return (
      <main className={styles.root}>
        <p>Урок не найден</p>
      </main>
    );
  }

  const activeTask = lesson.tasks.find((t) => t.id === activeTaskId);

  const handleQuizComplete = (task: Task, selectedOptionIds: string[] = []) => {
    submitMutation.mutate({ lessonId, taskId: task.id, body: { selectedOptionIds } });
  };

  const handleSolvedComplete = (task: Task) => {
    submitMutation.mutate({ lessonId, taskId: task.id, body: { solved: true } });
  };

  if (activeTask) {
    return (
      <>
        {activeTask.type === "quiz" && (
          <QuizChallenge
            task={activeTask}
            onComplete={(ids) => handleQuizComplete(activeTask, ids)}
          />
        )}
        {activeTask.type === "flowchart" && (
          <FlowchartChallenge
            task={activeTask}
            onComplete={() => handleSolvedComplete(activeTask)}
          />
        )}
        {activeTask.type === "algorithm" && (
          <CodingChallenge task={activeTask} onComplete={() => handleSolvedComplete(activeTask)} />
        )}
      </>
    );
  }

  return (
    <main className={styles.root}>
      <Link
        to="/course/$courseId"
        params={{ courseId: String(courseId) }}
        className={styles.backLink}
      >
        <ArrowLeftIcon size={20} />
        <span>К урокам курса</span>
      </Link>

      <section className={styles.header}>
        <div className={styles.lessonMeta}>
          <span className={`${styles.badge} ${styles.badgeOrder}`}>Модуль {lesson.order}</span>
          {lesson.estimatedMinutes > 0 && (
            <span className={`${styles.badge} ${styles.badgeTime}`}>
              <ClockIcon size={12} />
              {lesson.estimatedMinutes} мин
            </span>
          )}
          {lesson.isFreePreview && (
            <span className={`${styles.badge} ${styles.badgeFree}`}>Бесплатно</span>
          )}
        </div>
        <h1 className={styles.title}>{lesson.title}</h1>
        {lesson.description && <p className={styles.description}>{lesson.description}</p>}
      </section>

      <section className={styles.tasksSection}>
        <h2 className={styles.tasksTitle}>Задания ({lesson.tasks.length})</h2>
        {lesson.tasks.map((task) => {
          const icon = TASK_ICONS[task.type];
          return (
            <div
              key={task.id}
              className={styles.taskCard}
              onClick={() => setActiveTaskId(task.id)}
              role="button"
              tabIndex={0}
              onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") setActiveTaskId(task.id);
              }}
            >
              <div className={`${styles.taskIcon} ${icon.className}`}>{icon.emoji}</div>
              <div className={styles.taskContent}>
                <h3 className={styles.taskTitle}>{task.title}</h3>
                <p className={styles.taskDesc}>
                  {TASK_LABELS[task.type]} — {task.description}
                </p>
              </div>
              <div className={styles.taskReward}>
                {task.rewardStars > 0 && (
                  <span className={`${styles.rewardItem} ${styles.rewardStar}`}>
                    ⭐ {task.rewardStars}
                  </span>
                )}
                {task.rewardExp > 0 && (
                  <span className={`${styles.rewardItem} ${styles.rewardExp}`}>
                    +{task.rewardExp} XP
                  </span>
                )}
              </div>
              <ChevronRightIcon size={16} className={styles.taskArrow} />
            </div>
          );
        })}
      </section>
    </main>
  );
};

export { LessonPage };
