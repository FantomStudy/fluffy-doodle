import { createFileRoute } from "@tanstack/react-router";
import { lazy, Suspense } from "react";

const FlowchartChallenge = lazy(() =>
  import("../components/FlowchartChallenge").then((m) => ({
    default: m.FlowchartChallenge,
  })),
);

export const Route = createFileRoute("/algorithm")({
  component: AlgorithmPage,
});

function AlgorithmPage() {
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
      <FlowchartChallenge />
    </Suspense>
  );
}
