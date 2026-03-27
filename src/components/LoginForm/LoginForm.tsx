import { Link, useNavigate } from "@tanstack/react-router";
import { RocketIcon } from "lucide-react";
import { useState } from "react";
import { ApiError, signIn } from "@/api/auth";
import { setAuthenticated } from "@/lib/authSession";
import { Button } from "../ui/Button";
import { Input } from "../ui/Input";
import styles from "./LoginForm.module.css";

export const LoginForm = () => {
  const navigate = useNavigate();
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      await signIn({ login, password });
      setAuthenticated();
      navigate({ to: "/" });
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message);
      } else {
        setError("Произошла ошибка. Попробуй ещё раз.");
      }
    } finally {
      setLoading(false);
    }
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

      <Button type="submit" className={styles.submitBtn} disabled={loading}>
        <RocketIcon size={18} />
        {loading ? "Входим..." : "Начать обучение"}
      </Button>

      {error && <p className={styles.error}>{error}</p>}

      <Link to="/register" className={styles.link}>
        Нет аккаунта? <span>Зарегистрироваться</span>
      </Link>

      <Link to="/" className={styles.forgotLink}>
        Забыли пароль?
      </Link>
    </form>
  );
};
