import { createFileRoute } from "@tanstack/react-router";
import { LoginForm } from "@/components/LoginForm";
import { Card } from "@/components/ui/Card";
import styles from "./auth.module.css";

const RouteComponent = () => {
  return (
    <div className={styles.root}>
      <div className={styles.wrapper}>
        <img src="/assets/mascot.png" alt="" className={styles.mascot} aria-hidden />
        <Card className={styles.card}>
          <LoginForm />
        </Card>
      </div>
    </div>
  );
};

export const Route = createFileRoute("/(auth)/login")({
  component: RouteComponent,
});
