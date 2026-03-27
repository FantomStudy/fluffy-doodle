import { createFileRoute } from "@tanstack/react-router";
import { CodingChallenge } from "@/components/CodingChallenge";

export const Route = createFileRoute("/_sidebar/challenge")({
  component: ChallengePage,
});

function ChallengePage() {
  return <CodingChallenge />;
}
