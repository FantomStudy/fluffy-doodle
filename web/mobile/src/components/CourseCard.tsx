import type { Course } from "../api/courses";
import * as React from "react";
import { Image, Pressable, StyleSheet, Text, View } from "react-native";
import { colors, radii, semantic } from "../theme/tokens";
import { Badge } from "./ui/Badge";

interface CourseCardProps {
  course: Course;
  onPress: () => void;
}

export const CourseCard = ({ course, onPress }: CourseCardProps) => (
  <Pressable style={styles.card} onPress={onPress}>
    <Image source={{ uri: course.imageUrl }} style={styles.image} />
    <Badge>{course.categoryName}</Badge>
    <Text style={styles.title}>{course.title}</Text>
    <Text style={styles.desc} numberOfLines={2}>
      {course.description}
    </Text>
    <View style={styles.meta}>
      <Text style={styles.star}>⭐</Text>
      <Text style={styles.level}>{course.level}</Text>
      <Text style={styles.divider}>•</Text>
      <Text style={styles.lessons}>{course.totalLessons} уроков</Text>
    </View>
  </Pressable>
);

const styles = StyleSheet.create({
  card: {
    backgroundColor: semantic.surface,
    borderRadius: radii["3xl"],
    padding: 16,
    gap: 8,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.06,
    shadowRadius: 16,
    elevation: 3,
  },
  image: {
    width: "100%",
    height: 120,
    borderRadius: radii.xl,
    marginBottom: 4,
  },
  title: {
    fontSize: 16,
    fontWeight: "700",
    color: semantic.foreground,
  },
  desc: {
    fontSize: 13,
    color: semantic.foregroundMuted,
    lineHeight: 18,
  },
  meta: {
    flexDirection: "row",
    alignItems: "center",
    gap: 6,
  },
  star: {
    fontSize: 14,
  },
  level: {
    fontSize: 13,
    fontWeight: "600",
    color: semantic.foreground,
  },
  divider: {
    fontSize: 13,
    color: semantic.foregroundSubtle,
  },
  lessons: {
    fontSize: 13,
    color: semantic.foregroundMuted,
  },
});
