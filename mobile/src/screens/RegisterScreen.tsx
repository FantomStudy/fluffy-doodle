import React from "react";
import { KeyboardAvoidingView, Platform, StyleSheet } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { RegisterForm } from "../components/RegisterForm";
import { Card } from "../components/ui/Card";
import { semantic } from "../theme/tokens";

export const RegisterScreen = () => (
  <SafeAreaView style={styles.root}>
    <KeyboardAvoidingView
      behavior={Platform.OS === "ios" ? "padding" : "height"}
      style={styles.container}
    >
      <Card style={styles.card}>
        <RegisterForm />
      </Card>
    </KeyboardAvoidingView>
  </SafeAreaView>
);

const styles = StyleSheet.create({
  root: {
    flex: 1,
    backgroundColor: semantic.background,
  },
  container: {
    flex: 1,
    justifyContent: "center",
    paddingHorizontal: 20,
  },
  card: {
    paddingHorizontal: 24,
    paddingVertical: 32,
  },
});
