import type { RegisteredRouter, ValidateLinkOptions } from "@tanstack/react-router";
import { createFileRoute, Link, Outlet, redirect } from "@tanstack/react-router";
import {
  BookOpenIcon,
  ClipboardListIcon,
  HomeIcon,
  MapIcon,
  TrophyIcon,
  UserIcon,
} from "lucide-react";
import { refresh } from "@/api/auth";
import { isAuthenticated, setAuthenticated } from "@/lib/authSession";
import styles from "./route.module.css";

interface NavItem {
  icon: React.ReactNode;
  label: string;
  linkOptions: ValidateLinkOptions<RegisteredRouter>;
}

const NAV: NavItem[] = [
  { icon: <HomeIcon size={16} />, label: "Главная", linkOptions: { to: "/" } },
  { icon: <MapIcon size={16} />, label: "Путь", linkOptions: { to: "/" } },
  { icon: <BookOpenIcon size={16} />, label: "Уроки", linkOptions: { to: "/" } },
  { icon: <ClipboardListIcon size={16} />, label: "Задания", linkOptions: { to: "/" } },
  { icon: <TrophyIcon size={16} />, label: "Достижения", linkOptions: { to: "/" } },
  { icon: <UserIcon size={16} />, label: "Профиль", linkOptions: { to: "/profile" } },
];

const RouteComponent = () => {
  return (
    <div className={styles.layout}>
      <aside className={styles.sidebar}>
        <div className={styles.sidebarTop}>
          <div className={styles.avatar} />
          <span className={styles.brand}>Fluffy Doodle</span>
        </div>

        <nav className={styles.nav}>
          {NAV.map((item) => (
            <Link key={item.label} {...item.linkOptions} className={styles.navItem}>
              {item.icon}
              <p>{item.label}</p>
            </Link>
          ))}
        </nav>
      </aside>

      <Outlet />
    </div>
  );
};

export const Route = createFileRoute("/_sidebar")({
  beforeLoad: async () => {
    if (isAuthenticated()) return;

    try {
      await refresh();
      setAuthenticated();
    } catch {
      throw redirect({ to: "/login" });
    }
  },
  component: RouteComponent,
});
