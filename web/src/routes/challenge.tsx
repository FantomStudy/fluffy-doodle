import { createFileRoute } from "@tanstack/react-router";
import { lazy, Suspense } from "react";

const CodingChallenge = lazy(() =>
  import("../components/CodingChallenge").then((m) => ({
    default: m.CodingChallenge,
  })),
);

export const Route = createFileRoute("/challenge")({
  component: ChallengePage,
});

function ChallengePage() {
  return (
    <Suspense
      fallback={
        <div
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            height: "100dvh",
            background: "#0d1117",
            color: "#8b949e",
            fontFamily: "system-ui, sans-serif",
            fontSize: "14px",
          }}
        >
          Загрузка редактора…
        </div>
      }
    >
      <CodingChallenge />
    </Suspense>
  );
}
