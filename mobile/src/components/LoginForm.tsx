import { useNavigation } from "@react-navigation/native";
import * as React from "react";
import { useState } from "react";
import { Pressable, ScrollView, StyleSheet, Text, View } from "react-native";
import { useLogin } from "../hooks/useLogin";
import { colors, radii, semantic } from "../theme/tokens";
import { Button } from "./ui/Button";
import { Input } from "./ui/Input";

export const LoginForm = () => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");

  const navigation = useNavigation<any>();
  const { mutate, isPending, error } = useLogin();

  const errorMessage = error
    ? error instanceof Error
      ? error.message
      : "Произошла ошибка. Попробуй ещё раз."
    : null;

  const handleSubmit = () => {
    mutate({ login, password }, { onSuccess: () => navigation.replace("Main") });
  };

  return (
    <ScrollView contentContainerStyle={styles.form} keyboardShouldPersistTaps="handled">
      <View style={styles.header}>
        <Text style={styles.brand}>
          <Text style={styles.brandPurple}>Fluffy </Text>
          <Text style={styles.brandGold}>Doodle</Text>
        </Text>
      </View>

      <View style={styles.fields}>
        <View style={styles.field}>
          <Text style={styles.label}>Логин</Text>
          <Input
            placeholder="Введи свой логин"
            autoCapitalize="none"
            autoComplete="username"
            value={login}
            onChangeText={setLogin}
          />
        </View>
        <View style={styles.field}>
          <Text style={styles.label}>Пароль</Text>
          <Input
            placeholder="Введи пароль"
            secureTextEntry
            autoComplete="password"
            value={password}
            onChangeText={setPassword}
          />
        </View>
      </View>

      <Button
        onPress={handleSubmit}
        disabled={isPending}
        loading={isPending}
        style={styles.submitBtn}
      >
        <Text style={styles.submitText}>{isPending ? "Входим..." : "🚀 Начать обучение"}</Text>
      </Button>

      {errorMessage && <Text style={styles.error}>{errorMessage}</Text>}

      <Pressable onPress={() => navigation.navigate("Register")}>
        <Text style={styles.link}>
          Нет аккаунта? <Text style={styles.linkAccent}>Зарегистрироваться</Text>
        </Text>
      </Pressable>

      <Pressable>
        <Text style={styles.forgotLink}>Забыли пароль?</Text>
      </Pressable>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  form: {
    gap: 24,
  },
  header: {
    alignItems: "center",
    paddingBottom: 8,
  },
  brand: {
    fontSize: 32,
    fontWeight: "800",
    letterSpacing: 1.2,
  },
  brandPurple: {
    color: semantic.primary,
  },
  brandGold: {
    color: colors.amber500,
  },
  fields: {
    gap: 16,
  },
  field: {
    gap: 6,
  },
  label: {
    fontSize: 13,
    fontWeight: "600",
    color: semantic.foregroundStrong,
  },
  submitBtn: {
    width: "100%",
    height: 48,
    borderRadius: radii.full,
  },
  submitText: {
    fontSize: 15,
    fontWeight: "700",
    color: semantic.foregroundWhite,
  },
  link: {
    textAlign: "center",
    fontSize: 13,
    color: semantic.foregroundMuted,
    fontWeight: "500",
  },
  linkAccent: {
    color: semantic.primary,
    fontWeight: "600",
  },
  forgotLink: {
    textAlign: "center",
    fontSize: 13,
    color: semantic.primary,
    fontWeight: "500",
  },
  error: {
    fontSize: 13,
    color: semantic.danger,
    textAlign: "center",
    fontWeight: "500",
  },
});
