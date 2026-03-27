import type { RegisteredRouter, ValidateLinkOptions } from "@tanstack/react-router";
import { createFileRoute, Link, Outlet } from "@tanstack/react-router";
import { BookOpenIcon, ClipboardListIcon, HomeIcon, MapIcon, UserIcon } from "lucide-react";
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

export const Route = createFileRoute("/_protected/_sidebar")({
  component: RouteComponent,
});
