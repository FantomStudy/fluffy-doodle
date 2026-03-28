import { Link, useNavigate } from "@tanstack/react-router";
import { SparklesIcon } from "lucide-react";
import { useState } from "react";
import { useRegister } from "@/hooks/useRegister";
import { Button } from "../ui/Button";
import { Input } from "../ui/Input";
import styles from "./RegisterForm.module.css";

export const RegisterForm = () => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [fullName, setFullName] = useState("");
  const [phone, setPhone] = useState("");
  const [isParent, setIsParent] = useState(false);
  const [studentCode, setStudentCode] = useState("");

  const navigate = useNavigate();

  const { mutate, isPending, error } = useRegister();

  const errorMessage = error
    ? error instanceof Error
      ? error.message
      : "Произошла ошибка. Попробуй ещё раз."
    : null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    mutate(
      {
        login,
        password,
        fullName,
        phoneNumber: phone,
        ...(isParent && studentCode ? { studentCode } : {}),
      },
      { onSuccess: () => navigate({ to: "/login" }) },
    );
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <div className={styles.header}>
        <div className={styles.brand}>
          <span className={styles.brandPurple}>Fluffy</span>{" "}
          <span className={styles.brandGold}>Doodle</span>
        </div>
      </div>

      <div className={styles.fields}>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="fullName">
            ФИО
          </label>
          <Input
            id="fullName"
            type="text"
            placeholder="Иванов Иван Иванович"
            autoComplete="name"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
          />
        </div>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="phone">
            Номер телефона
          </label>
          <Input
            id="phone"
            type="tel"
            placeholder="+7 (___) ___-__-__"
            autoComplete="tel"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
          />
        </div>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="reg-login">
            Логин
          </label>
          <Input
            id="reg-login"
            type="text"
            placeholder="Придумай логин"
            autoComplete="username"
            value={login}
            onChange={(e) => setLogin(e.target.value)}
          />
        </div>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="reg-password">
            Пароль
          </label>
          <Input
            id="reg-password"
            type="password"
            placeholder="Придумай пароль"
            autoComplete="new-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
      </div>

      <label className={styles.checkboxRow}>
        <input
          type="checkbox"
          className={styles.checkbox}
          checked={isParent}
          onChange={(e) => setIsParent(e.target.checked)}
        />
        <span className={styles.checkboxLabel}>Я родитель</span>
      </label>

      {isParent && (
        <div className={styles.field}>
          <label className={styles.label} htmlFor="student-code">
            Код ребёнка
          </label>
          <Input
            id="student-code"
            type="text"
            placeholder="STU-XXXXXXXX"
            value={studentCode}
            onChange={(e) => setStudentCode(e.target.value)}
          />
        </div>
      )}

      <Button type="submit" className={styles.submitBtn} disabled={isPending}>
        <SparklesIcon size={18} />
        {isPending ? "Регистрируем..." : "Зарегистрироваться"}
      </Button>

      {errorMessage && <p className={styles.error}>{errorMessage}</p>}

      <Link to="/login" className={styles.link}>
        Уже есть аккаунт? <span>Войти</span>
      </Link>
    </form>
  );
};
