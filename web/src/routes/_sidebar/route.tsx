import { createFileRoute, Link, Outlet } from "@tanstack/react-router";
import {
  BookOpenIcon,
  ClipboardListIcon,
  HomeIcon,
  MapIcon,
  TrophyIcon,
  UserIcon,
} from "lucide-react";
import styles from "./route.module.css";

const NAV = [
  { icon: <HomeIcon size={16} />, label: "Главная", to: "/" },
  { icon: <MapIcon size={16} />, label: "Путь", href: "/map" },
  { icon: <BookOpenIcon size={16} />, label: "Уроки", href: "/knowledge" },
  { icon: <ClipboardListIcon size={16} />, label: "Задания", href: "/challenge" },
  { icon: <TrophyIcon size={16} />, label: "Достижения", href: "/achievements" },
  { icon: <UserIcon size={16} />, label: "Профиль", href: "/profile" },
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
            <Link key={item.label} to={item.href} className={styles.navItem}>
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
  component: RouteComponent,
});
