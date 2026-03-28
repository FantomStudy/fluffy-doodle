import type { Task } from "@/api/courses";
import { createFileRoute, Link, useParams } from "@tanstack/react-router";
import { ArrowLeftIcon, ChevronRightIcon, ClockIcon } from "lucide-react";
import { useState } from "react";
import { CodingChallenge } from "@/components/CodingChallenge";
import { FlowchartChallenge } from "@/components/FlowchartChallenge";
import { QuizChallenge } from "@/components/QuizChallenge";
import { useCourseLessons, useSubmitTask } from "@/hooks/useCourses";
import styles from "./index.module.css";

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

interface CompletedTaskState {
  taskId: number;
}

interface VictoryScreenProps {
  task: Task;
  hasNextTask: boolean;
  unityWebglUrl: string;
  onBack: () => void;
  onNext: () => void;
}

const VictoryScreen = ({
  task,
  hasNextTask,
  unityWebglUrl,
  onBack,
  onNext,
}: VictoryScreenProps) => {
  return (
    <main className={styles.victoryRoot}>
      <div className={styles.victoryCard}>
        <div className={styles.victoryBadge}>Готово</div>
        <p className={styles.victoryEyebrow}>Ты справился</p>
        <h1 className={styles.victoryTitle}>{task.title}</h1>
        <p className={styles.victoryDescription}>Отличная работа! Попробуешь ещё одно?</p>

        <div className={styles.victoryRewards}>
          {task.rewardStars > 0 && (
            <div className={styles.victoryRewardCard}>
              <span className={styles.victoryRewardValue}>⭐ {task.rewardStars}</span>
              <span className={styles.victoryRewardLabel}>звёзд</span>
            </div>
          )}
          {task.rewardExp > 0 && (
            <div className={styles.victoryRewardCard}>
              <span className={styles.victoryRewardValue}>+{task.rewardExp} XP</span>
              <span className={styles.victoryRewardLabel}>опыта</span>
            </div>
          )}
        </div>

        <div className={styles.victoryActions}>
          <a
            href={unityWebglUrl}
            target="_blank"
            rel="noreferrer"
            className={styles.victoryLinkBtn}
          >
            Дополнительное задание
          </a>
          <button type="button" className={styles.victorySecondaryBtn} onClick={onBack}>
            К заданиям
          </button>
          <button type="button" className={styles.victoryPrimaryBtn} onClick={onNext}>
            {hasNextTask ? "Дальше" : "Продолжить"}
          </button>
        </div>
      </div>
    </main>
  );
};

const RouteComponent = () => {
  const [activeTaskId, setActiveTaskId] = useState<number | null>(null);
  const [completedTask, setCompletedTask] = useState<CompletedTaskState | null>(null);

  const params = useParams({
    from: "/_protected/_sidebar/courses/$courseId/lessons/$lessonId/",
  });

  const courseId = Number(params.courseId);
  const lessonId = Number(params.lessonId);

  const { data: lessons, isLoading } = useCourseLessons(courseId);
  const submitMutation = useSubmitTask(courseId);

  if (isLoading) {
    return (
      <main className={styles.root}>
        <p>Загрузка...</p>
      </main>
    );
  }

  const lesson = lessons?.find((item) => item.id === lessonId);

  if (!lesson) {
    return (
      <main className={styles.root}>
        <p>Урок не найден</p>
      </main>
    );
  }

  const activeTask = lesson.tasks.find((task) => task.id === activeTaskId);
  const completedTaskData = lesson.tasks.find((task) => task.id === completedTask?.taskId) ?? null;
  const activeTaskIndex = activeTask
    ? lesson.tasks.findIndex((task) => task.id === activeTask.id)
    : -1;
  const nextTask = activeTaskIndex >= 0 ? lesson.tasks[activeTaskIndex + 1] : undefined;
  const unityWebglUrl =
    import.meta.env.VITE_UNITY_WEBGL_URL ??
    `/unity-webgl?courseId=${courseId}&lessonId=${lessonId}&taskId=${completedTask?.taskId ?? ""}`;

  const handleQuizComplete = (task: Task, selectedOptionIds: string[] = []) => {
    submitMutation.mutate(
      { lessonId, taskId: task.id, body: { selectedOptionIds } },
      {
        onSuccess: () => {
          setCompletedTask({ taskId: task.id });
        },
      },
    );
  };

  const handleSolvedComplete = (task: Task) => {
    submitMutation.mutate(
      { lessonId, taskId: task.id, body: { solved: true } },
      {
        onSuccess: () => {
          setCompletedTask({ taskId: task.id });
        },
      },
    );
  };

  if (completedTaskData) {
    return (
      <VictoryScreen
        task={completedTaskData}
        hasNextTask={Boolean(nextTask)}
        unityWebglUrl={unityWebglUrl}
        onBack={() => {
          setCompletedTask(null);
          setActiveTaskId(null);
        }}
        onNext={() => {
          setCompletedTask(null);
          setActiveTaskId(nextTask?.id ?? null);
        }}
      />
    );
  }

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
        to="/courses/$courseId"
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
              onKeyDown={(event) => {
                if (event.key === "Enter" || event.key === " ") {
                  setActiveTaskId(task.id);
                }
              }}
            >
              <div className={`${styles.taskIcon} ${icon.className}`}>{icon.emoji}</div>
              <div className={styles.taskContent}>
                <h3 className={styles.taskTitle}>{task.title}</h3>
                <p className={styles.taskDesc}>
                  {TASK_LABELS[task.type]} - {task.description}
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

export const Route = createFileRoute("/_protected/_sidebar/courses/$courseId/lessons/$lessonId/")({
  component: RouteComponent,
});
