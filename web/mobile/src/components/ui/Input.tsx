import React from "react";
import { StyleSheet, TextInput, type TextInputProps, type ViewStyle } from "react-native";
import { semantic, radii } from "../../theme/tokens";

interface InputProps extends TextInputProps {
  style?: ViewStyle;
}

export const Input = ({ style, ...props }: InputProps) => (
  <TextInput
    style={[styles.input, style]}
    placeholderTextColor={semantic.foregroundSubtle}
    {...props}
  />
);

const styles = StyleSheet.create({
  input: {
    width: "100%",
    minHeight: 42,
    fontSize: 16,
    borderWidth: 1,
    borderColor: semantic.border,
    borderRadius: radii.md,
    paddingHorizontal: 12,
    paddingVertical: 8,
    color: semantic.foreground,
    backgroundColor: "transparent",
  },
});
