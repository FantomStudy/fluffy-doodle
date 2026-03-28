import { useNavigation } from "@react-navigation/native";
import * as React from "react";
import { ActivityIndicator, Pressable, ScrollView, StyleSheet, Text, View } from "react-native";
import { logout } from "../api/auth";
import { useChildProgress } from "../hooks/useChildProgress";
import { colors, radii, semantic } from "../theme/tokens";

export const ChildProgressPage = () => {
  const { data: child, isLoading, error } = useChildProgress();
  const navigation = useNavigation<any>();

  const handleLogout = async () => {
    try {
      await logout();
    } finally {
      navigation.reset({ index: 0, routes: [{ name: "Login" }] });
    }
  };

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
        <Text style={styles.error}>
          {error instanceof Error ? error.message : "Ошибка загрузки данных"}
        </Text>
      </View>
    );
  }

  if (!child) return null;

  const expPercent = child.level > 0 ? Math.min((child.exp / 100) * 100, 100) : 0;

  return (
    <ScrollView style={styles.root} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Профиль ребёнка</Text>

      <View style={styles.card}>
        <View style={styles.avatar}>
          <Text style={styles.avatarInitial}>{child.studentName.charAt(0)}</Text>
        </View>

        <View style={styles.info}>
          <Text style={styles.name}>{child.studentName}</Text>
          <Text style={styles.login}>{child.studentLogin}</Text>
          <View style={styles.codeWrap}>
            <Text style={styles.codeLabel}>Личный код студента</Text>
            <Text style={styles.code}>{child.studentCode}</Text>
          </View>
        </View>

        <View style={styles.levelBadge}>
          <Text style={styles.levelNum}>{child.level}</Text>
          <Text style={styles.levelLabel}>уровень</Text>
        </View>

        <Pressable style={styles.logoutButton} onPress={handleLogout}>
          <Text style={styles.logoutText}>Выйти</Text>
        </Pressable>
      </View>

      <View style={styles.statsRow}>
        <View style={styles.statCard}>
          <Text style={styles.statIcon}>⭐</Text>
          <View>
            <Text style={styles.statValue}>{child.stars}</Text>
            <Text style={styles.statLabel}>Звёзд заработано</Text>
          </View>
        </View>
        <View style={styles.statCard}>
          <Text style={styles.statIcon}>⚡</Text>
          <View>
            <Text style={styles.statValue}>{child.exp}</Text>
            <Text style={styles.statLabel}>Очков опыта</Text>
          </View>
        </View>
        <View style={styles.statCard}>
          <Text style={styles.statIcon}>🏆</Text>
          <View>
            <Text style={styles.statValue}>{child.achievements}</Text>
            <Text style={styles.statLabel}>Достижений</Text>
          </View>
        </View>
      </View>

      <View style={styles.expSection}>
        <View style={styles.expHeader}>
          <Text style={styles.expLabel}>До уровня {child.level + 1}</Text>
          <Text style={styles.expNumbers}>{child.exp} / 100 XP</Text>
        </View>
        <View style={styles.expTrack}>
          <View style={[styles.expFill, { width: `${expPercent}%` as any }]} />
        </View>
        <Text style={styles.expHint}>
          {child.exp < 100
            ? `Ещё ${100 - child.exp} XP до следующего уровня`
            : "Уровень достигнут! 🎉"}
        </Text>
      </View>
    </ScrollView>
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
  },
  center: {
    flex: 1,
    alignItems: "center",
    justifyContent: "center",
  },
  title: {
    fontSize: 32,
    fontWeight: "600",
    color: semantic.primary,
    textAlign: "center",
    marginBottom: 24,
  },
  card: {
    backgroundColor: semantic.surface,
    borderRadius: radii["3xl"],
    padding: 20,
    marginBottom: 20,
    alignItems: "center",
    gap: 12,
  },
  avatar: {
    width: 100,
    height: 100,
    borderRadius: radii["3xl"],
    backgroundColor: colors.purple600,
    alignItems: "center",
    justifyContent: "center",
  },
  avatarInitial: {
    fontSize: 40,
    fontWeight: "800",
    color: semantic.foregroundWhite,
    textTransform: "uppercase",
  },
  info: {
    alignItems: "center",
    gap: 4,
  },
  name: {
    fontSize: 22,
    fontWeight: "700",
    color: semantic.foreground,
  },
  login: {
    fontSize: 14,
    color: semantic.foregroundMuted,
  },
  codeWrap: {
    alignItems: "center",
    gap: 2,
    marginTop: 4,
  },
  codeLabel: {
    fontSize: 11,
    fontWeight: "600",
    textTransform: "uppercase",
    letterSpacing: 0.6,
    color: semantic.foregroundMuted,
  },
  code: {
    fontSize: 14,
    fontWeight: "700",
    color: semantic.primary,
    backgroundColor: colors.purple50,
    paddingHorizontal: 12,
    paddingVertical: 4,
    borderRadius: radii.lg,
    fontFamily: "monospace",
    letterSpacing: 1,
    overflow: "hidden",
  },
  levelBadge: {
    alignItems: "center",
    backgroundColor: colors.purple600,
    borderRadius: radii.xl,
    paddingHorizontal: 28,
    paddingVertical: 12,
  },
  levelNum: {
    fontSize: 28,
    fontWeight: "800",
    color: semantic.foregroundWhite,
  },
  levelLabel: {
    fontSize: 12,
    fontWeight: "600",
    color: semantic.foregroundWhite,
    textTransform: "uppercase",
    opacity: 0.85,
  },
  logoutButton: {
    borderWidth: 2,
    borderColor: semantic.danger,
    borderRadius: radii["4xl"],
    paddingHorizontal: 20,
    paddingVertical: 10,
    flexDirection: "row",
    alignItems: "center",
    gap: 6,
  },
  logoutText: {
    fontSize: 14,
    fontWeight: "600",
    color: semantic.danger,
  },
  statsRow: {
    gap: 12,
    marginBottom: 20,
  },
  statCard: {
    flexDirection: "row",
    alignItems: "center",
    gap: 14,
    backgroundColor: semantic.surface,
    borderRadius: radii["3xl"],
    padding: 20,
  },
  statIcon: {
    fontSize: 22,
  },
  statValue: {
    fontSize: 24,
    fontWeight: "800",
    color: semantic.foreground,
  },
  statLabel: {
    fontSize: 13,
    color: semantic.foregroundMuted,
  },
  expSection: {
    backgroundColor: semantic.surface,
    borderRadius: radii["3xl"],
    padding: 20,
    gap: 8,
  },
  expHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
  },
  expLabel: {
    fontSize: 14,
    fontWeight: "600",
    color: semantic.foregroundStrong,
  },
  expNumbers: {
    fontSize: 14,
    fontWeight: "600",
    color: semantic.primary,
  },
  expTrack: {
    height: 14,
    backgroundColor: semantic.borderSubtle,
    borderRadius: radii.full,
    overflow: "hidden",
  },
  expFill: {
    height: "100%",
    borderRadius: radii.full,
    backgroundColor: colors.purple600,
  },
  expHint: {
    fontSize: 13,
    color: semantic.foregroundSubtle,
  },
  error: {
    textAlign: "center",
    color: semantic.danger,
    fontSize: 16,
  },
});
