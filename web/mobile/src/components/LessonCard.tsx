import type { Lesson } from "../api/courses";
import * as React from "react";
import { Pressable, StyleSheet, Text, View } from "react-native";
import { colors, radii, semantic } from "../theme/tokens";

type LessonStatus = "completed" | "current" | "locked";

interface LessonCardProps {
  id: number;
  courseId: number;
  number: number;
  title: string;
  description: string;
  status: LessonStatus;
  onPress: () => void;
}

const statusConfig: Record<
  LessonStatus,
  { numBg: string; numColor: string; label: string; badgeBg: string; badgeColor: string }
> = {
  completed: {
    numBg: colors.green600,
    numColor: colors.white,
    label: "Пройдено",
    badgeBg: colors.green50,
    badgeColor: colors.green600,
  },
  current: {
    numBg: colors.purple600,
    numColor: colors.white,
    label: "Текущий урок",
    badgeBg: colors.purple50,
    badgeColor: colors.purple600,
  },
  locked: {
    numBg: colors.gray200,
    numColor: colors.gray500,
    label: "Заблокировано",
    badgeBg: colors.gray100,
    badgeColor: colors.gray500,
  },
};

export const LessonCard = ({ number, title, description, status, onPress }: LessonCardProps) => {
  const cfg = statusConfig[status];

  return (
    <Pressable style={styles.card} onPress={onPress}>
      <View style={[styles.number, { backgroundColor: cfg.numBg }]}>
        <Text style={[styles.numberText, { color: cfg.numColor }]}>{number}</Text>
      </View>
      <View style={styles.content}>
        <Text style={styles.title}>{title}</Text>
        <Text style={styles.desc} numberOfLines={2}>
          {description}
        </Text>
        <View style={[styles.badge, { backgroundColor: cfg.badgeBg }]}>
          <Text style={[styles.badgeText, { color: cfg.badgeColor }]}>
            {status === "completed" ? "✓ " : status === "locked" ? "🔒 " : ""}
            {cfg.label}
          </Text>
        </View>
      </View>
    </Pressable>
  );
};

const styles = StyleSheet.create({
  card: {
    flexDirection: "row",
    alignItems: "flex-start",
    gap: 14,
    backgroundColor: semantic.surface,
    borderRadius: radii.xl,
    padding: 16,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.04,
    shadowRadius: 8,
    elevation: 2,
  },
  number: {
    width: 40,
    height: 40,
    borderRadius: 20,
    alignItems: "center",
    justifyContent: "center",
  },
  numberText: {
    fontSize: 16,
    fontWeight: "800",
  },
  content: {
    flex: 1,
    gap: 4,
  },
  title: {
    fontSize: 16,
    fontWeight: "600",
    color: semantic.foreground,
  },
  desc: {
    fontSize: 13,
    color: semantic.foregroundMuted,
    lineHeight: 18,
  },
  badge: {
    alignSelf: "flex-start",
    paddingHorizontal: 10,
    paddingVertical: 3,
    borderRadius: radii.full,
    marginTop: 4,
  },
  badgeText: {
    fontSize: 12,
    fontWeight: "600",
  },
});
