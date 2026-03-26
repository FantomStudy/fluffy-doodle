import { createFileRoute } from "@tanstack/react-router";

const RouteComponent = () => {
  return <div style={{ padding: "10px" }}></div>;
};

export const Route = createFileRoute("/")({
  component: RouteComponent,
});
