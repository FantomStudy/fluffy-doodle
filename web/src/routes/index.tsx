import { createFileRoute } from "@tanstack/react-router";
import { Sidebar } from "../components/layout/Sidebar";
import { HeroSection } from "../components/home/HeroSection";
import { StatsBar } from "../components/home/StatsBar";
import { CurrentCourseCard } from "../components/home/CurrentCourseCard";
import { WeeklyGoalCard } from "../components/home/WeeklyGoalCard";
import { ModulesSection } from "../components/home/ModulesSection";
import s from "./index.module.css";

const RouteComponent = () => {
  return (
    <div className={s.page}>
      <Sidebar />
      <main className={s.main}>
        <HeroSection userName="Дима" />
        <StatsBar />
        <div className={s.bottomRow}>
          <div className={s.bottomLeft}>
            <CurrentCourseCard />
            <ModulesSection />
          </div>
          <WeeklyGoalCard />
        </div>
      </main>
    </div>
  );
};

export const Route = createFileRoute("/")({
  component: RouteComponent,
});
