import { useNavigation } from "@react-navigation/native";
import * as React from "react";
import { Image, Pressable, ScrollView, StyleSheet, Text, View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { CourseCard } from "../components/CourseCard";
import { useCourses } from "../hooks/useCourses";
import { useMe } from "../hooks/useMe";
import { colors, radii, semantic } from "../theme/tokens";

const getDayWord = (n: number) => {
  if (n % 100 >= 11 && n % 100 <= 14) return "дней подряд";
  const r = n % 10;
  if (r === 1) return "день подряд";
  if (r >= 2 && r <= 4) return "дня подряд";
  return "дней подряд";
};

export const HomeScreen = () => {
  const { data: courses } = useCourses();
  const { data: me } = useMe();
  const navigation = useNavigation<any>();

  return (
    <SafeAreaView style={styles.root} edges={["top"]}>
      <ScrollView contentContainerStyle={styles.content}>
        <Text style={styles.greeting}>Рад твоему возращению, {me?.fullName ?? "Ученик"}</Text>
        <Text style={styles.subtitle}>Твой учебный процесс</Text>

        <View style={styles.learningCard}>
          <View style={styles.cardHeader}>
            <View style={styles.statBadge}>
              <Text style={styles.statBadgeText}>⭐ {me?.stars ?? 0}</Text>
            </View>
            <View style={styles.streakBadge}>
              <Text style={styles.streakText}>
                🔥 Серия: {me?.streak ?? 0} {getDayWord(me?.streak ?? 0)}
              </Text>
            </View>
          </View>

          <View style={styles.currentLesson}>
            <Text style={styles.lessonLabel}>Текущий урок</Text>
            <Text style={styles.lessonTitle}>Цвет и Материалы</Text>
            <Text style={styles.lessonDesc}>Краткое описание урока</Text>
            <Pressable style={styles.continueBtn}>
              <Text style={styles.continueBtnText}>Продолжить</Text>
            </Pressable>
          </View>

          <View style={styles.progressSection}>
            <Text style={styles.progressLabel}>Твой прогресс</Text>
            <Text style={styles.progressCount}>Урок 4 из 5</Text>
            <View style={styles.progressTrack}>
              <View style={[styles.progressFill, { width: "80%" as any }]} />
            </View>
          </View>
        </View>

        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>Другие курсы</Text>
        </View>

        <View style={styles.courseGrid}>
          {courses?.map((course) => (
            <CourseCard
              key={course.id}
              course={course}
              onPress={() => navigation.navigate("CourseDetail", { courseId: course.id })}
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
  greeting: {
    fontSize: 24,
    fontWeight: "700",
    color: semantic.foreground,
  },
  subtitle: {
    fontSize: 14,
    color: semantic.foregroundMuted,
  },
  learningCard: {
    backgroundColor: semantic.surface,
    borderRadius: radii["3xl"],
    padding: 20,
    gap: 16,
  },
  cardHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  statBadge: {
    backgroundColor: colors.amber50,
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: radii.full,
  },
  statBadgeText: {
    fontSize: 14,
    fontWeight: "700",
    color: colors.amber500,
  },
  streakBadge: {
    backgroundColor: "#fef2f2",
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: radii.full,
  },
  streakText: {
    fontSize: 13,
    fontWeight: "600",
    color: semantic.foreground,
  },
  currentLesson: {
    gap: 6,
  },
  lessonLabel: {
    fontSize: 12,
    fontWeight: "600",
    color: semantic.foregroundMuted,
    textTransform: "uppercase",
  },
  lessonTitle: {
    fontSize: 20,
    fontWeight: "700",
    color: semantic.foreground,
  },
  lessonDesc: {
    fontSize: 14,
    color: semantic.foregroundMuted,
  },
  continueBtn: {
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
    paddingVertical: 12,
    alignItems: "center",
    marginTop: 8,
  },
  continueBtnText: {
    fontSize: 15,
    fontWeight: "700",
    color: semantic.foregroundWhite,
  },
  progressSection: {
    gap: 6,
  },
  progressLabel: {
    fontSize: 12,
    fontWeight: "600",
    color: semantic.foregroundMuted,
    textTransform: "uppercase",
  },
  progressCount: {
    fontSize: 14,
    fontWeight: "600",
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
  sectionHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  sectionTitle: {
    fontSize: 20,
    fontWeight: "700",
    color: semantic.foreground,
  },
  courseGrid: {
    gap: 12,
  },
});
