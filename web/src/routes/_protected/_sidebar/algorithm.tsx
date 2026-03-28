import { createFileRoute } from "@tanstack/react-router";
import { FlowchartChallenge } from "@/components/FlowchartChallenge";

const AlgorithmPage = () => {
  return <FlowchartChallenge />;
};

export const Route = createFileRoute("/_protected/_sidebar/algorithm")({
  component: AlgorithmPage,
});
