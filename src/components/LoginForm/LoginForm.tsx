import { Link, useNavigate } from "@tanstack/react-router";
import { RocketIcon } from "lucide-react";
import { useState } from "react";
import { useLogin } from "@/hooks/useLogin";
import { Button } from "../ui/Button";
import { Input } from "../ui/Input";
import styles from "./LoginForm.module.css";

export const LoginForm = () => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");

  const navigate = useNavigate();

  const { mutate, isPending, error } = useLogin();

  const errorMessage = error
    ? error instanceof Error
      ? error.message
      : "Произошла ошибка. Попробуй ещё раз."
    : null;

  const handleSubmit = async (e: React.SubmitEvent) => {
    e.preventDefault();

    mutate(
      { login, password },
      {
        onSuccess: () => navigate({ to: "/" }),
      },
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
          <label className={styles.label} htmlFor="login">
            Логин
          </label>
          <Input
            id="login"
            type="text"
            placeholder="Введи свой логин"
            autoComplete="username"
            value={login}
            onChange={(e) => setLogin(e.target.value)}
          />
        </div>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="password">
            Пароль
          </label>
          <Input
            id="password"
            type="password"
            placeholder="Введи пароль"
            autoComplete="current-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
      </div>

      <Button type="submit" className={styles.submitBtn} disabled={isPending}>
        <RocketIcon size={18} />
        {isPending ? "Входим..." : "Начать обучение"}
      </Button>

      {errorMessage && <p className={styles.error}>{errorMessage}</p>}

      <Link to="/register" className={styles.link}>
        Нет аккаунта? <span>Зарегистрироваться</span>
      </Link>

      <Link to="/" className={styles.forgotLink}>
        Забыли пароль?
      </Link>
    </form>
  );
};
