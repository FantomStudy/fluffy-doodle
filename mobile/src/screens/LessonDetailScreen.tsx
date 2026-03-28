import type { NativeStackScreenProps } from "@react-navigation/native-stack";
import type { Task } from "../api/courses";
import type { RootStackParamList } from "../navigation/types";
import { useRoute } from "@react-navigation/native";
import * as React from "react";
import { useState } from "react";
import { ActivityIndicator, Pressable, ScrollView, StyleSheet, Text, View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { QuizChallenge } from "../components/QuizChallenge";
import { UnityRedirectScreen } from "../components/UnityRedirectScreen";
import { useCourseLessons, useSubmitTask } from "../hooks/useCourses";
import { colors, radii, semantic } from "../theme/tokens";

const TASK_LABELS: Record<Task["type"], string> = {
  quiz: "📝 Тест",
  flowchart: "🔷 Блок-схема",
  algorithm: "⚙️ Алгоритм",
};

type Props = NativeStackScreenProps<RootStackParamList, "LessonDetail">;

interface CompletedTaskState {
  taskId: number;
}

const VictoryScreen = ({
  task,
  hasNextTask,
  onBack,
  onNext,
}: {
  task: Task;
  hasNextTask: boolean;
  onBack: () => void;
  onNext: () => void;
}) => (
  <View style={styles.victoryRoot}>
    <View style={styles.victoryCard}>
      <Text style={styles.victoryBadge}>✅ Готово</Text>
      <Text style={styles.victoryTitle}>{task.title}</Text>
      <Text style={styles.victoryDesc}>Отличная работа!</Text>
      <View style={styles.victoryRewards}>
        {task.rewardStars > 0 && (
          <View style={styles.rewardCard}>
            <Text style={styles.rewardValue}>⭐ {task.rewardStars}</Text>
            <Text style={styles.rewardLabel}>звёзд</Text>
          </View>
        )}
        {task.rewardExp > 0 && (
          <View style={styles.rewardCard}>
            <Text style={styles.rewardValue}>+{task.rewardExp} XP</Text>
            <Text style={styles.rewardLabel}>опыта</Text>
          </View>
        )}
      </View>
      <Pressable style={styles.victorySecondary} onPress={onBack}>
        <Text style={styles.victorySecondaryText}>К заданиям</Text>
      </Pressable>
      <Pressable style={styles.victoryPrimary} onPress={onNext}>
        <Text style={styles.victoryPrimaryText}>{hasNextTask ? "Дальше" : "Продолжить"}</Text>
      </Pressable>
    </View>
  </View>
);

export const LessonDetailScreen = () => {
  const route = useRoute<Props["route"]>();
  const { courseId, lessonId } = route.params;

  const [activeTaskId, setActiveTaskId] = useState<number | null>(null);
  const [completedTask, setCompletedTask] = useState<CompletedTaskState | null>(null);

  const { data: lessons, isLoading } = useCourseLessons(courseId);
  const submitMutation = useSubmitTask(courseId);

  if (isLoading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color={semantic.primary} />
      </View>
    );
  }

  const lesson = lessons?.find((item) => item.id === lessonId);
  if (!lesson) {
    return (
      <View style={styles.center}>
        <Text>Урок не найден</Text>
      </View>
    );
  }

  const activeTask = lesson.tasks.find((t) => t.id === activeTaskId);
  const completedTaskData = lesson.tasks.find((t) => t.id === completedTask?.taskId) ?? null;
  const activeTaskIndex = activeTask ? lesson.tasks.findIndex((t) => t.id === activeTask.id) : -1;
  const nextTask = activeTaskIndex >= 0 ? lesson.tasks[activeTaskIndex + 1] : undefined;

  const handleQuizComplete = (task: Task, selectedOptionIds: string[] = []) => {
    submitMutation.mutate(
      { lessonId, taskId: task.id, body: { selectedOptionIds } },
      { onSuccess: () => setCompletedTask({ taskId: task.id }) },
    );
  };

  if (completedTaskData) {
    return (
      <VictoryScreen
        task={completedTaskData}
        hasNextTask={Boolean(nextTask)}
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
    if (activeTask.type === "quiz") {
      return (
        <QuizChallenge
          task={activeTask}
          onComplete={(ids) => handleQuizComplete(activeTask, ids)}
        />
      );
    }

    return <UnityRedirectScreen onBack={() => setActiveTaskId(null)} />;
  }

  return (
    <SafeAreaView style={styles.root} edges={["bottom"]}>
      <ScrollView contentContainerStyle={styles.content}>
        <Text style={styles.lessonTitle}>{lesson.title}</Text>
        <Text style={styles.lessonDesc}>{lesson.description}</Text>
        <Text style={styles.lessonMeta}>
          ⏱ {lesson.estimatedMinutes} мин • {lesson.tasks.length} заданий
        </Text>

        <Text style={styles.sectionTitle}>Задания</Text>

        <View style={styles.tasksList}>
          {lesson.tasks.map((task, i) => (
            <Pressable
              key={task.id}
              style={styles.taskCard}
              onPress={() => setActiveTaskId(task.id)}
            >
              <Text style={styles.taskNum}>{i + 1}</Text>
              <View style={styles.taskContent}>
                <Text style={styles.taskType}>{TASK_LABELS[task.type]}</Text>
                <Text style={styles.taskTitle}>{task.title}</Text>
                <Text style={styles.taskReward}>
                  ⭐ {task.rewardStars} • +{task.rewardExp} XP
                </Text>
              </View>
              <Text style={styles.taskArrow}>›</Text>
            </Pressable>
          ))}
        </View>
      </ScrollView>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  root: { flex: 1, backgroundColor: semantic.background },
  content: { padding: 16, paddingBottom: 100, gap: 16 },
  center: { flex: 1, alignItems: "center", justifyContent: "center" },
  lessonTitle: {
    fontSize: 24,
    fontWeight: "700",
    color: semantic.foreground,
  },
  lessonDesc: {
    fontSize: 15,
    color: semantic.foregroundMuted,
    lineHeight: 22,
  },
  lessonMeta: {
    fontSize: 13,
    color: semantic.foregroundSubtle,
  },
  sectionTitle: {
    fontSize: 20,
    fontWeight: "700",
    color: semantic.foreground,
    marginTop: 8,
  },
  tasksList: { gap: 10 },
  taskCard: {
    flexDirection: "row",
    alignItems: "center",
    gap: 14,
    backgroundColor: semantic.surface,
    borderRadius: radii.xl,
    padding: 16,
  },
  taskNum: {
    width: 36,
    height: 36,
    borderRadius: 18,
    backgroundColor: colors.purple50,
    color: colors.purple600,
    fontSize: 16,
    fontWeight: "800",
    textAlign: "center",
    lineHeight: 36,
    overflow: "hidden",
  },
  taskContent: { flex: 1, gap: 2 },
  taskType: {
    fontSize: 12,
    fontWeight: "600",
    color: semantic.foregroundMuted,
  },
  taskTitle: {
    fontSize: 16,
    fontWeight: "600",
    color: semantic.foreground,
  },
  taskReward: {
    fontSize: 12,
    color: semantic.foregroundSubtle,
  },
  taskArrow: {
    fontSize: 24,
    color: semantic.foregroundSubtle,
  },
  victoryRoot: {
    flex: 1,
    backgroundColor: semantic.background,
    alignItems: "center",
    justifyContent: "center",
    padding: 24,
  },
  victoryCard: {
    backgroundColor: semantic.surface,
    borderRadius: radii["3xl"],
    padding: 32,
    alignItems: "center",
    gap: 12,
    width: "100%",
  },
  victoryBadge: {
    fontSize: 16,
    fontWeight: "700",
    color: colors.green600,
    backgroundColor: colors.green50,
    paddingHorizontal: 16,
    paddingVertical: 6,
    borderRadius: radii.full,
    overflow: "hidden",
  },
  victoryTitle: {
    fontSize: 24,
    fontWeight: "700",
    color: semantic.foreground,
    textAlign: "center",
  },
  victoryDesc: {
    fontSize: 15,
    color: semantic.foregroundMuted,
    textAlign: "center",
  },
  victoryRewards: {
    flexDirection: "row",
    gap: 16,
    marginVertical: 8,
  },
  rewardCard: {
    alignItems: "center",
    gap: 2,
  },
  rewardValue: {
    fontSize: 20,
    fontWeight: "800",
    color: semantic.foreground,
  },
  rewardLabel: {
    fontSize: 12,
    color: semantic.foregroundMuted,
  },
  victorySecondary: {
    borderWidth: 2,
    borderColor: semantic.border,
    borderRadius: radii.full,
    paddingHorizontal: 24,
    paddingVertical: 12,
    width: "100%",
    alignItems: "center",
  },
  victorySecondaryText: {
    fontSize: 15,
    fontWeight: "600",
    color: semantic.foreground,
  },
  victoryPrimary: {
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
    paddingHorizontal: 24,
    paddingVertical: 12,
    width: "100%",
    alignItems: "center",
  },
  victoryPrimaryText: {
    fontSize: 15,
    fontWeight: "700",
    color: semantic.foregroundWhite,
  },
});
