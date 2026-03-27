import { createFileRoute } from "@tanstack/react-router";
import { lazy, Suspense } from "react";

const QuizChallenge = lazy(() =>
  import("../components/QuizChallenge").then((m) => ({
    default: m.QuizChallenge,
  })),
);

export const Route = createFileRoute("/knowledge")({
  component: KnowledgePage,
});

function KnowledgePage() {
  return (
    <Suspense
      fallback={
        <div
          style={{
            background: "#f5f5f7",
            minHeight: "100vh",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            color: "#9ca3af",
            fontSize: 14,
            fontFamily: "sans-serif",
          }}
        >
          Загрузка…
        </div>
      }
    >
      <QuizChallenge />
    </Suspense>
  );
}
