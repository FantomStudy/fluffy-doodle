import { createFileRoute } from "@tanstack/react-router";

const RouteComponent = () => {
  return <div></div>;
};

export const Route = createFileRoute("/")({
  component: RouteComponent,
});
