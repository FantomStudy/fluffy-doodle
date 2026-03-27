import { Link, useNavigate } from "@tanstack/react-router";
import { SparklesIcon } from "lucide-react";
import { useState } from "react";
import { ApiError, signUp } from "@/api/auth";
import { Button } from "../ui/Button";
import { Input } from "../ui/Input";
import styles from "./RegisterForm.module.css";

export const RegisterForm = () => {
  const navigate = useNavigate();
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [fullName, setFullName] = useState("");
  const [phone, setPhone] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      await signUp({ login, password, fullName, phoneNumber: phone });
      navigate({ to: "/login" });
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

      <Button type="submit" className={styles.submitBtn} disabled={loading}>
        <SparklesIcon size={18} />
        {loading ? "Регистрируем..." : "Зарегистрироваться"}
      </Button>

      {error && <p className={styles.error}>{error}</p>}

      <Link to="/login" className={styles.link}>
        Уже есть аккаунт? <span>Войти</span>
      </Link>
    </form>
  );
};
