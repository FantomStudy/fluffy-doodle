import type { RegisteredRouter, ValidateLinkOptions } from "@tanstack/react-router";
import { createFileRoute, Link, Outlet } from "@tanstack/react-router";
import { BookOpenIcon, HomeIcon, MessageCircleIcon, UserIcon } from "lucide-react";
import { useMe } from "@/hooks/useMe";
import styles from "./route.module.css";

interface NavItem {
  icon: React.ReactNode;
  label: string;
  linkOptions: ValidateLinkOptions<RegisteredRouter>;
}

const STUDENT_NAV: NavItem[] = [
  { icon: <HomeIcon size={16} />, label: "Главная", linkOptions: { to: "/" } },
  { icon: <BookOpenIcon size={16} />, label: "Уроки", linkOptions: { to: "/courses" } },
  { icon: <MessageCircleIcon size={16} />, label: "Форум", linkOptions: { to: "/forum" } },
  { icon: <UserIcon size={16} />, label: "Профиль", linkOptions: { to: "/profile" } },
];

const PARENT_NAV: NavItem[] = [
  { icon: <UserIcon size={16} />, label: "Профиль ребёнка", linkOptions: { to: "/child" } },
];

const RouteComponent = () => {
  const { data: me } = useMe();
  const isParent = me?.roleName === "parent";
  const nav = isParent ? PARENT_NAV : STUDENT_NAV;

  return (
    <div className={styles.layout}>
      <aside className={styles.sidebar}>
        <div className={styles.sidebarTop}>
          <img
            src="/assets/mascot.png"
            alt="Fluffy Doodle"
            className={styles.logo}
          />
          <span className={styles.brand}>Fluffy Doodle</span>
        </div>

        <nav className={styles.nav}>
          {nav.map((item) => (
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
