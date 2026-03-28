import { useNavigation } from "@react-navigation/native";
import { useQueryClient } from "@tanstack/react-query";
import * as React from "react";
import { useRef, useState } from "react";
import {
  ActivityIndicator,
  FlatList,
  Image,
  Pressable,
  ScrollView,
  StyleSheet,
  Text,
  View,
} from "react-native";
import { logout } from "../api/auth";
import { uploadAvatar } from "../api/user";
import { useProfile } from "../hooks/useProfile";
import { colors, radii, semantic } from "../theme/tokens";

export const ProfilePage = () => {
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);

  const navigation = useNavigation<any>();
  const queryClient = useQueryClient();
  const { data: profile, isLoading, error } = useProfile();

  const handleAvatarPick = async () => {
    try {
      const ImagePicker = require("expo-image-picker");
      const result = await ImagePicker.launchImageLibraryAsync({
        mediaTypes: ImagePicker.MediaTypeOptions.Images,
        allowsEditing: true,
        aspect: [1, 1],
        quality: 0.8,
      });

      if (result.canceled) return;

      setUploadError(null);
      setUploading(true);
      await uploadAvatar(result.assets[0].uri);
      queryClient.invalidateQueries({ queryKey: ["profile"] });
    } catch (err) {
      setUploadError(err instanceof Error ? err.message : "Ошибка загрузки аватара");
    } finally {
      setUploading(false);
    }
  };

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
          {error instanceof Error ? error.message : "Ошибка загрузки профиля"}
        </Text>
      </View>
    );
  }

  if (!profile) return null;

  const expPercent =
    profile.expToNextLevel > 0 ? Math.min((profile.exp / profile.expToNextLevel) * 100, 100) : 0;
  const expRemaining = Math.max(profile.expToNextLevel - profile.exp, 0);

  return (
    <ScrollView style={styles.root} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Мой профиль</Text>

      {uploadError && <Text style={styles.error}>{uploadError}</Text>}

      <View style={styles.userCard}>
        <Pressable onPress={handleAvatarPick} style={styles.avatarWrap}>
          {profile.avatar ? (
            <Image source={{ uri: profile.avatar }} style={styles.avatarImage} />
          ) : (
            <View style={styles.avatarPlaceholder}>
              {uploading ? (
                <ActivityIndicator color={semantic.foreground} />
              ) : (
                <Text style={styles.avatarPlaceholderText}>📷{"\n"}Загрузить</Text>
              )}
            </View>
          )}
        </Pressable>

        <View style={styles.levelBadge}>
          <Text style={styles.levelNum}>{profile.level}</Text>
          <Text style={styles.levelLabel}>уровень</Text>
        </View>

        <View style={styles.userInfo}>
          <Text style={styles.userName}>{profile.fullName}</Text>
          <Text style={styles.userLogin}>{profile.login}</Text>
          <Text style={styles.userPhone}>{profile.phoneNumber}</Text>

          <View style={styles.codeWrap}>
            <Text style={styles.codeLabel}>Личный код студента</Text>
            <Text style={styles.code}>{profile.studentCode}</Text>
          </View>
        </View>

        <View style={styles.statsRow}>
          <View style={styles.statCard}>
            <Text style={styles.statIcon}>⭐</Text>
            <View>
              <Text style={styles.statValue}>{profile.stars}</Text>
              <Text style={styles.statLabel}>звёзд</Text>
            </View>
          </View>
          <View style={styles.statCard}>
            <Text style={styles.statIcon}>⚡</Text>
            <View>
              <Text style={styles.statValue}>{profile.exp}</Text>
              <Text style={styles.statLabel}>опыт</Text>
            </View>
          </View>
          <View style={styles.statCard}>
            <Text style={styles.statIcon}>🔥</Text>
            <View>
              <Text style={styles.statValue}>{profile.streak}</Text>
              <Text style={styles.statLabel}>серия</Text>
            </View>
          </View>
        </View>

        <View style={styles.expSection}>
          <View style={styles.expHeader}>
            <Text style={styles.expLabel}>До уровня {profile.level + 1}</Text>
            <Text style={styles.expNumbers}>
              {profile.exp} / {profile.expToNextLevel} XP
            </Text>
          </View>
          <View style={styles.expTrack}>
            <View style={[styles.expFill, { width: `${expPercent}%` as any }]} />
          </View>
          <Text style={styles.expHint}>
            {expRemaining > 0
              ? `Ещё ${expRemaining} XP до следующего уровня`
              : "Уровень достигнут! 🎉"}
          </Text>
        </View>

        <Pressable style={styles.logoutButton} onPress={handleLogout}>
          <Text style={styles.logoutText}>Выйти</Text>
        </Pressable>
      </View>

      <Text style={styles.achievementsTitle}>Достижения</Text>

      {!profile.achievements?.length ? (
        <Text style={styles.emptyText}>Пока нет достижений</Text>
      ) : (
        <View style={styles.achievementsGrid}>
          {profile.achievements.map((item) => (
            <View key={item.id} style={styles.achievementCard}>
              <Image source={{ uri: item.icon }} style={styles.achievementImage} />
              <Text style={styles.achievementName}>{item.name}</Text>
              <Text style={styles.achievementDesc}>{item.description}</Text>
            </View>
          ))}
        </View>
      )}
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
  userCard: {
    backgroundColor: semantic.surface,
    borderRadius: radii["3xl"],
    padding: 20,
    marginBottom: 24,
    gap: 16,
  },
  avatarWrap: {
    alignSelf: "center",
  },
  avatarImage: {
    width: 120,
    height: 120,
    borderRadius: radii["3xl"],
  },
  avatarPlaceholder: {
    width: 120,
    height: 120,
    borderRadius: radii["3xl"],
    backgroundColor: colors.purple50,
    alignItems: "center",
    justifyContent: "center",
  },
  avatarPlaceholderText: {
    textAlign: "center",
    fontSize: 12,
    color: semantic.foreground,
  },
  levelBadge: {
    alignSelf: "center",
    alignItems: "center",
    backgroundColor: colors.purple600,
    borderRadius: radii.xl,
    paddingHorizontal: 24,
    paddingVertical: 8,
  },
  levelNum: {
    fontSize: 22,
    fontWeight: "800",
    color: semantic.foregroundWhite,
  },
  levelLabel: {
    fontSize: 11,
    fontWeight: "600",
    color: semantic.foregroundWhite,
    textTransform: "uppercase",
    opacity: 0.85,
  },
  userInfo: {
    gap: 4,
  },
  userName: {
    fontSize: 22,
    fontWeight: "700",
    color: semantic.foreground,
  },
  userLogin: {
    fontSize: 13,
    color: semantic.foregroundMuted,
  },
  userPhone: {
    fontSize: 14,
    color: semantic.foregroundSubtle,
  },
  codeWrap: {
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
    alignSelf: "flex-start",
    fontFamily: "monospace",
    letterSpacing: 1,
    overflow: "hidden",
  },
  statsRow: {
    flexDirection: "row",
    gap: 10,
  },
  statCard: {
    flex: 1,
    flexDirection: "row",
    alignItems: "center",
    gap: 8,
    backgroundColor: semantic.surfaceMuted,
    borderRadius: radii.lg,
    padding: 10,
  },
  statIcon: {
    fontSize: 20,
  },
  statValue: {
    fontSize: 18,
    fontWeight: "700",
    color: semantic.foreground,
  },
  statLabel: {
    fontSize: 12,
    color: semantic.foregroundMuted,
  },
  expSection: {
    gap: 6,
  },
  expHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
  },
  expLabel: {
    fontSize: 13,
    fontWeight: "600",
    color: semantic.foregroundStrong,
  },
  expNumbers: {
    fontSize: 13,
    fontWeight: "600",
    color: semantic.primary,
  },
  expTrack: {
    height: 12,
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
    fontSize: 12,
    color: semantic.foregroundSubtle,
  },
  logoutButton: {
    alignSelf: "center",
    borderWidth: 2,
    borderColor: semantic.danger,
    borderRadius: radii["4xl"],
    paddingHorizontal: 28,
    paddingVertical: 10,
  },
  logoutText: {
    fontSize: 14,
    fontWeight: "600",
    color: semantic.danger,
  },
  error: {
    color: semantic.danger,
    textAlign: "center",
    fontSize: 16,
    marginBottom: 16,
  },
  emptyText: {
    textAlign: "center",
    color: semantic.foregroundMuted,
    fontSize: 16,
  },
  achievementsTitle: {
    fontSize: 24,
    fontWeight: "600",
    color: semantic.accent,
    textAlign: "center",
    marginBottom: 20,
  },
  achievementsGrid: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: 12,
  },
  achievementCard: {
    width: "48%",
    backgroundColor: semantic.surface,
    borderRadius: radii["3xl"],
    padding: 12,
  },
  achievementImage: {
    width: "100%",
    aspectRatio: 200 / 160,
    borderRadius: radii["3xl"],
    marginBottom: 8,
  },
  achievementName: {
    fontSize: 14,
    fontWeight: "600",
    color: semantic.foreground,
    textAlign: "center",
    marginBottom: 4,
  },
  achievementDesc: {
    fontSize: 12,
    color: semantic.foregroundSubtle,
    lineHeight: 18,
  },
});
