import { createFileRoute } from "@tanstack/react-router";
import { CodingChallenge } from "@/components/CodingChallenge";

const ChallengePage = () => {
  return <CodingChallenge />;
};

export const Route = createFileRoute("/_protected/_sidebar/challenge")({
  component: ChallengePage,
});
