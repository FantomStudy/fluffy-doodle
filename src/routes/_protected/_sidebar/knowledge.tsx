import { createFileRoute } from "@tanstack/react-router";
import { QuizChallenge } from "@/components/QuizChallenge";

const KnowledgePage = () => {
  return <QuizChallenge />;
};

export const Route = createFileRoute("/_protected/_sidebar/knowledge")({
  component: KnowledgePage,
});
