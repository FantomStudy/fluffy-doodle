import React from "react";
import { StyleSheet, Text } from "react-native";
import { colors, semantic } from "../../theme/tokens";

type BadgeVariant = "primary" | "success" | "lock";

interface BadgeProps {
  variant?: BadgeVariant;
  children: React.ReactNode;
}

const variantStyles: Record<BadgeVariant, { bg: string; color: string }> = {
  primary: { bg: colors.purple100, color: colors.purple600 },
  success: { bg: colors.green100, color: colors.green600 },
  lock: { bg: colors.gray600, color: colors.white },
};

export const Badge = ({ variant = "primary", children }: BadgeProps) => {
  const v = variantStyles[variant];

  return <Text style={[styles.badge, { backgroundColor: v.bg, color: v.color }]}>{children}</Text>;
};

const styles = StyleSheet.create({
  badge: {
    alignSelf: "flex-start",
    fontSize: 14,
    fontWeight: "500",
    paddingHorizontal: 14,
    paddingVertical: 5,
    borderRadius: 999,
    overflow: "hidden",
  },
});
