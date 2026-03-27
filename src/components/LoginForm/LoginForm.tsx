import { Link } from "@tanstack/react-router";
import { useState } from "react";
import { Button } from "../ui/Button";
import { Input } from "../ui/Input";
import styles from "./LoginForm.module.css";

export const LoginForm = () => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = (e: React.SubmitEvent) => {
    e.preventDefault();
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <h1 className={styles.title}>Вход</h1>
      <div className={styles.fields}>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="login">
            Логин
          </label>
          <Input
            id="login"
            type="text"
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
            autoComplete="current-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
      </div>
      <Button type="submit">Войти</Button>
      <Link to="/" className={styles.link}>
        Забыли пароль?
      </Link>
    </form>
  );
};
