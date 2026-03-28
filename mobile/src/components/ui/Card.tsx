import React from "react";
import { StyleSheet, View, type ViewStyle } from "react-native";
import { semantic, radii } from "../../theme/tokens";

interface CardProps {
  style?: ViewStyle;
  children: React.ReactNode;
}

export const Card = ({ style, children }: CardProps) => (
  <View style={[styles.card, style]}>{children}</View>
);

const styles = StyleSheet.create({
  card: {
    backgroundColor: semantic.surface,
    borderColor: semantic.border,
    borderWidth: 1,
    borderRadius: radii["4xl"],
    padding: 24,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.08,
    shadowRadius: 32,
    elevation: 4,
  },
});
