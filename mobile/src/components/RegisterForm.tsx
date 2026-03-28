import { useNavigation } from "@react-navigation/native";
import * as React from "react";
import { useState } from "react";
import { Pressable, ScrollView, StyleSheet, Switch, Text, View } from "react-native";
import { useRegister } from "../hooks/useRegister";
import { colors, radii, semantic } from "../theme/tokens";
import { Button } from "./ui/Button";
import { Input } from "./ui/Input";

export const RegisterForm = () => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [fullName, setFullName] = useState("");
  const [phone, setPhone] = useState("");
  const [isParent, setIsParent] = useState(false);
  const [studentCode, setStudentCode] = useState("");

  const navigation = useNavigation<any>();
  const { mutate, isPending, error } = useRegister();

  const errorMessage = error
    ? error instanceof Error
      ? error.message
      : "Произошла ошибка. Попробуй ещё раз."
    : null;

  const handleSubmit = () => {
    mutate(
      {
        login,
        password,
        fullName,
        phoneNumber: phone,
        ...(isParent && studentCode ? { studentCode } : {}),
      },
      { onSuccess: () => navigation.navigate("Login") },
    );
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
          <Text style={styles.label}>ФИО</Text>
          <Input
            placeholder="Иванов Иван Иванович"
            autoComplete="name"
            value={fullName}
            onChangeText={setFullName}
          />
        </View>
        <View style={styles.field}>
          <Text style={styles.label}>Номер телефона</Text>
          <Input
            placeholder="+7 (___) ___-__-__"
            keyboardType="phone-pad"
            autoComplete="tel"
            value={phone}
            onChangeText={setPhone}
          />
        </View>
        <View style={styles.field}>
          <Text style={styles.label}>Логин</Text>
          <Input
            placeholder="Придумай логин"
            autoCapitalize="none"
            autoComplete="username"
            value={login}
            onChangeText={setLogin}
          />
        </View>
        <View style={styles.field}>
          <Text style={styles.label}>Пароль</Text>
          <Input
            placeholder="Придумай пароль"
            secureTextEntry
            autoComplete="password-new"
            value={password}
            onChangeText={setPassword}
          />
        </View>
      </View>

      <View style={styles.checkboxRow}>
        <Switch
          value={isParent}
          onValueChange={setIsParent}
          trackColor={{ true: semantic.primary }}
          thumbColor={semantic.foregroundWhite}
        />
        <Text style={styles.checkboxLabel}>Я родитель</Text>
      </View>

      {isParent && (
        <View style={styles.field}>
          <Text style={styles.label}>Код ребёнка</Text>
          <Input
            placeholder="STU-XXXXXXXX"
            autoCapitalize="characters"
            value={studentCode}
            onChangeText={setStudentCode}
          />
        </View>
      )}

      <Button
        onPress={handleSubmit}
        disabled={isPending}
        loading={isPending}
        style={styles.submitBtn}
      >
        <Text style={styles.submitText}>
          {isPending ? "Регистрируем..." : "✨ Зарегистрироваться"}
        </Text>
      </Button>

      {errorMessage && <Text style={styles.error}>{errorMessage}</Text>}

      <Pressable onPress={() => navigation.navigate("Login")}>
        <Text style={styles.link}>
          Уже есть аккаунт? <Text style={styles.linkAccent}>Войти</Text>
        </Text>
      </Pressable>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  form: {
    gap: 20,
  },
  header: {
    alignItems: "center",
    paddingBottom: 4,
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
    gap: 14,
  },
  field: {
    gap: 6,
  },
  label: {
    fontSize: 13,
    fontWeight: "600",
    color: semantic.foregroundStrong,
  },
  checkboxRow: {
    flexDirection: "row",
    alignItems: "center",
    gap: 10,
  },
  checkboxLabel: {
    fontSize: 14,
    fontWeight: "600",
    color: semantic.foreground,
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
  error: {
    fontSize: 13,
    color: semantic.danger,
    textAlign: "center",
    fontWeight: "500",
  },
});
