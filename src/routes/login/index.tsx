import { createFileRoute } from "@tanstack/react-router";
import { LoginForm } from "@/components/LoginForm";
import { Card } from "@/components/ui/Card";
import styles from "./index.module.css";

const RouteComponent = () => {
  return (
    <div className={styles.root}>
      <Card className={styles.card}>
        <LoginForm />
      </Card>
    </div>
  );
};

export const Route = createFileRoute("/login/")({
  component: RouteComponent,
});
