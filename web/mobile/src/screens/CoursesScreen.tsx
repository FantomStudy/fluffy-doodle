import { useNavigation } from "@react-navigation/native";
import * as React from "react";
import { ActivityIndicator, ScrollView, StyleSheet, Text, View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { CourseCard } from "../components/CourseCard";
import { useCourses } from "../hooks/useCourses";
import { semantic } from "../theme/tokens";

export const CoursesScreen = () => {
  const { data: courses, isLoading, error } = useCourses();
  const navigation = useNavigation<any>();

  if (isLoading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color={semantic.primary} />
      </View>
    );
  }

  if (error) {
    return (
      <View style={styles.center}>
        <Text style={styles.error}>Ошибка загрузки курсов</Text>
      </View>
    );
  }

  return (
    <SafeAreaView style={styles.root} edges={["top"]}>
      <ScrollView contentContainerStyle={styles.content}>
        <Text style={styles.title}>Уроки</Text>
        <Text style={styles.subtitle}>Выбери курс и начни обучение</Text>

        {courses && courses.length > 0 ? (
          <View style={styles.courseGrid}>
            {courses.map((course) => (
              <CourseCard
                key={course.id}
                course={course}
                onPress={() => navigation.navigate("CourseDetail", { courseId: course.id })}
              />
            ))}
          </View>
        ) : (
          <Text style={styles.emptyText}>Пока нет доступных курсов</Text>
        )}
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
    gap: 12,
  },
  center: {
    flex: 1,
    alignItems: "center",
    justifyContent: "center",
  },
  title: {
    fontSize: 28,
    fontWeight: "700",
    color: semantic.foreground,
  },
  subtitle: {
    fontSize: 14,
    color: semantic.foregroundMuted,
  },
  courseGrid: {
    gap: 12,
  },
  emptyText: {
    textAlign: "center",
    color: semantic.foregroundMuted,
    fontSize: 16,
    marginTop: 32,
  },
  error: {
    color: semantic.danger,
    fontSize: 16,
  },
});
