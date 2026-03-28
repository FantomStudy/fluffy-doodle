import type { ViewStyle } from "react-native";
import * as React from "react";
import { ActivityIndicator, Pressable, StyleSheet, Text } from "react-native";
import { radii, semantic } from "../../theme/tokens";

type ButtonVariant = "primary" | "ghost" | "danger" | "link";

interface ButtonProps {
  variant?: ButtonVariant;
  onPress?: () => void;
  disabled?: boolean;
  loading?: boolean;
  style?: ViewStyle;
  children: React.ReactNode;
}

const variantConfig: Record<ButtonVariant, { bg: string; color: string; borderColor?: string }> = {
  primary: { bg: semantic.primary, color: semantic.foregroundWhite },
  danger: { bg: semantic.danger, color: semantic.foregroundWhite },
  ghost: { bg: "transparent", color: semantic.foreground },
  link: { bg: "transparent", color: semantic.foreground },
};

export const Button = ({
  variant = "primary",
  onPress,
  disabled = false,
  loading = false,
  style,
  children,
}: ButtonProps) => {
  const v = variantConfig[variant];

  return (
    <Pressable
      onPress={onPress}
      disabled={disabled || loading}
      style={({ pressed }) => [
        styles.button,
        { backgroundColor: v.bg, opacity: pressed ? 0.85 : disabled ? 0.5 : 1 },
        variant === "link" && styles.link,
        style,
      ]}
    >
      {loading ? (
        <ActivityIndicator color={v.color} size="small" />
      ) : typeof children === "string" ? (
        <Text style={[styles.text, { color: v.color }]}>{children}</Text>
      ) : (
        children
      )}
    </Pressable>
  );
};

const styles = StyleSheet.create({
  button: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 8,
    height: 42,
    paddingHorizontal: 16,
    borderRadius: radii.lg,
  },
  text: {
    fontSize: 16,
    fontWeight: "500",
  },
  link: {
    height: "auto" as unknown as number,
    paddingHorizontal: 0,
  },
});
