import type { NativeStackScreenProps } from "@react-navigation/native-stack";
import type { RootStackParamList } from "../navigation/types";
import { useNavigation, useRoute } from "@react-navigation/native";
import * as React from "react";
import {
  ActivityIndicator,
  Image,
  Pressable,
  ScrollView,
  StyleSheet,
  Text,
  View,
} from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { LessonCard } from "../components/LessonCard";
import { useCourse, useCourseLessons } from "../hooks/useCourses";
import { colors, radii, semantic } from "../theme/tokens";

type LessonStatus = "completed" | "current" | "locked";

const getLessonStatus = (lesson: { tasks: unknown[] }, hasCurrentLesson: boolean): LessonStatus => {
  if (lesson.tasks.length === 0) return "completed";
  if (!hasCurrentLesson) return "current";
  return "locked";
};

type Props = NativeStackScreenProps<RootStackParamList, "CourseDetail">;

export const CourseDetailScreen = () => {
  const route = useRoute<Props["route"]>();
  const navigation = useNavigation<Props["navigation"]>();
  const { courseId } = route.params;

  const course = useCourse(courseId);
  const lessons = useCourseLessons(courseId);

  let hasCurrentLesson = false;
  const displayLessons = (lessons.data ?? [])
    .sort((a, b) => a.order - b.order)
    .map((lesson) => {
      const status = getLessonStatus(lesson, hasCurrentLesson);
      if (status === "current") hasCurrentLesson = true;
      return {
        id: lesson.id,
        courseId,
        number: lesson.order,
        title: lesson.title,
        description: lesson.description,
        tasks: lesson.tasks,
        status,
      };
    });

  const completedCount = displayLessons.filter((l) => l.status === "completed").length;
  const totalCount = displayLessons.length;
  const progressPercent = totalCount > 0 ? (completedCount / totalCount) * 100 : 0;

  if (course.isPending || lessons.isLoading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color={semantic.primary} />
      </View>
    );
  }

  return (
    <SafeAreaView style={styles.root} edges={["bottom"]}>
      <ScrollView contentContainerStyle={styles.content}>
        {course.data?.imageUrl && (
          <Image source={{ uri: course.data.imageUrl }} style={styles.courseImage} />
        )}

        <Text style={styles.courseTitle}>{course.data?.title}</Text>
        <Text style={styles.courseDesc}>{course.data?.description}</Text>

        <View style={styles.metaRow}>
          <Text style={styles.metaBadge}>{course.data?.totalLessons} уроков</Text>
          <Text style={styles.metaBadge}>{course.data?.level}</Text>
          <Text style={styles.metaTime}>⏱ {course.data?.hours} ч</Text>
        </View>

        <View style={styles.progressCard}>
          <Text style={styles.progressTitle}>Твой прогресс</Text>
          <View style={styles.progressTrack}>
            <View style={[styles.progressFill, { width: `${progressPercent}%` as any }]} />
          </View>
          <Text style={styles.progressText}>
            {completedCount} из {totalCount} уроков
          </Text>
        </View>

        <Text style={styles.sectionTitle}>Уроки</Text>

        <View style={styles.lessonsList}>
          {displayLessons.map((lesson) => (
            <LessonCard
              key={lesson.id}
              {...lesson}
              onPress={() =>
                navigation.navigate("LessonDetail", {
                  courseId,
                  lessonId: lesson.id,
                })
              }
            />
          ))}
        </View>
      </ScrollView>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  root: {
    flex: 1,
    backgroundColor: semantic.background,
  },
  content: {
    padding: 16,
    paddingBottom: 100,
    gap: 16,
  },
  center: {
    flex: 1,
    alignItems: "center",
    justifyContent: "center",
  },
  courseImage: {
    width: "100%",
    height: 180,
    borderRadius: radii.xl,
  },
  courseTitle: {
    fontSize: 24,
    fontWeight: "700",
    color: semantic.foreground,
  },
  courseDesc: {
    fontSize: 15,
    color: semantic.foregroundMuted,
    lineHeight: 22,
  },
  metaRow: {
    flexDirection: "row",
    gap: 8,
    alignItems: "center",
  },
  metaBadge: {
    fontSize: 13,
    fontWeight: "600",
    color: colors.purple600,
    backgroundColor: colors.purple50,
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: radii.full,
    overflow: "hidden",
  },
  metaTime: {
    fontSize: 13,
    color: semantic.foregroundMuted,
  },
  progressCard: {
    backgroundColor: semantic.surface,
    borderRadius: radii.xl,
    padding: 16,
    gap: 8,
  },
  progressTitle: {
    fontSize: 16,
    fontWeight: "700",
    color: semantic.foreground,
  },
  progressTrack: {
    height: 8,
    backgroundColor: semantic.borderSubtle,
    borderRadius: radii.full,
    overflow: "hidden",
  },
  progressFill: {
    height: "100%",
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
  },
  progressText: {
    fontSize: 13,
    color: semantic.foregroundMuted,
  },
  sectionTitle: {
    fontSize: 20,
    fontWeight: "700",
    color: semantic.foreground,
  },
  lessonsList: {
    gap: 10,
  },
});
