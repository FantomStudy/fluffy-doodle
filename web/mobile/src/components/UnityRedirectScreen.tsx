import * as React from "react";
import { useEffect, useState } from "react";
import { ActivityIndicator, Pressable, StyleSheet, Text, View } from "react-native";
import { colors, radii, semantic } from "../theme/tokens";
import { openUnityLevel } from "../utils/unity";

type UnityRedirectScreenProps = {
  onBack?: () => void;
};

const UnityRedirectScreen = ({ onBack }: UnityRedirectScreenProps) => {
  const [hasError, setHasError] = useState(false);

  const handleOpenUnity = async () => {
    try {
      const isOpened = await openUnityLevel();

      setHasError(!isOpened);
    } catch {
      setHasError(true);
    }
  };

  useEffect(() => {
    void handleOpenUnity();
  }, []);

  return (
    <View style={styles.root}>
      <View style={styles.card}>
        <Text style={styles.title}>
          {hasError ? "Не удалось открыть Unity" : "Открываем уровень в Unity"}
        </Text>
        <Text style={styles.description}>
          {hasError
            ? "Проверь EXPO_PUBLIC_UNITY_HOST в mobile/.env и доступность Unity-хоста."
            : "Сейчас откроется случайный уровень Unity WebGL в мобильном режиме."}
        </Text>
        {!hasError && <ActivityIndicator size="large" color={semantic.primary} />}
        {hasError && (
          <Pressable style={styles.primaryButton} onPress={() => void handleOpenUnity()}>
            <Text style={styles.primaryButtonText}>Попробовать снова</Text>
          </Pressable>
        )}
        {onBack && (
          <Pressable style={styles.secondaryButton} onPress={onBack}>
            <Text style={styles.secondaryButtonText}>Назад</Text>
          </Pressable>
        )}
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  root: {
    flex: 1,
    alignItems: "center",
    justifyContent: "center",
    padding: 24,
    backgroundColor: semantic.background,
  },
  card: {
    width: "100%",
    gap: 16,
    padding: 24,
    borderRadius: radii["3xl"],
    backgroundColor: semantic.surface,
    alignItems: "center",
  },
  title: {
    fontSize: 24,
    fontWeight: "700",
    color: semantic.foreground,
    textAlign: "center",
  },
  description: {
    fontSize: 15,
    lineHeight: 22,
    color: semantic.foregroundMuted,
    textAlign: "center",
  },
  primaryButton: {
    width: "100%",
    alignItems: "center",
    borderRadius: radii.full,
    paddingHorizontal: 24,
    paddingVertical: 12,
    backgroundColor: colors.purple600,
  },
  primaryButtonText: {
    fontSize: 15,
    fontWeight: "700",
    color: semantic.foregroundWhite,
  },
  secondaryButton: {
    width: "100%",
    alignItems: "center",
    borderRadius: radii.full,
    paddingHorizontal: 24,
    paddingVertical: 12,
    borderWidth: 2,
    borderColor: semantic.border,
  },
  secondaryButtonText: {
    fontSize: 15,
    fontWeight: "600",
    color: semantic.foreground,
  },
});

export { UnityRedirectScreen };
