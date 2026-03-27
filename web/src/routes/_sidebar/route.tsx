import { createFileRoute, Outlet } from "@tanstack/react-router";
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
  { icon: <HomeIcon size={16} />, label: "Главная" },
  { icon: <MapIcon size={16} />, label: "Путь" },
  { icon: <BookOpenIcon size={16} />, label: "Уроки" },
  { icon: <ClipboardListIcon size={16} />, label: "Задания", active: true },
  { icon: <TrophyIcon size={16} />, label: "Достижения" },
  { icon: <UserIcon size={16} />, label: "Профиль" },
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
            <button
              key={item.label}
              type="button"
              className={`${styles.navItem} ${item.active ? styles.navItemActive : ``}`}
            >
              {item.icon}
              {item.label}
            </button>
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
