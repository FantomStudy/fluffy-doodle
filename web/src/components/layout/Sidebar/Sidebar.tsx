import { Link } from "@tanstack/react-router";
import s from "./Sidebar.module.css";

const navItems = [
  { icon: "⌂", label: "Главная", href: "/" as const },
  { icon: "◎", label: "Путь", href: "/path" as const },
  { icon: "◫", label: "Уроки", href: "/lessons" as const },
  { icon: "✓", label: "Задания", href: "/tasks" as const },
  { icon: "★", label: "Достижения", href: "/achievements" as const },
  { icon: "◌", label: "Профиль", href: "/profile" as const },
];

export function Sidebar() {
  return (
    <aside className={s.sidebar}>
      <div className={s.logo}>
        <div className={s.logoIcon}>
          <span className={s.logoText}>OK</span>
        </div>
        <div className={s.logoMeta}>
          <span className={s.logoLabel}>Orbita Kids</span>
          <span className={s.logoTitle}>Главная</span>
        </div>
      </div>

      <nav className={s.nav}>
        {navItems.map((item) => (
          <Link
            key={item.href}
            to={item.href}
            activeOptions={{ exact: true }}
            activeProps={{ className: `${s.navLink} ${s.navLinkActive}` }}
            inactiveProps={{ className: s.navLink }}
          >
            <div className={s.navIconWrap}>
              <span className={s.navIcon}>{item.icon}</span>
            </div>
            <span className={s.navLabel}>{item.label}</span>
          </Link>
        ))}
      </nav>

      <div className={s.focusCard}>
        <p className={s.focusLabel}>Сегодня в фокусе</p>
        <p className={s.focusTitle}>Один понятный шаг</p>
        <p className={s.focusDesc}>
          Ребенок сразу видит, что делать дальше: открыть миссию и получить
          награду без перегруза экраном.
        </p>
      </div>
    </aside>
  );
}
