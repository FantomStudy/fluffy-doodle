import type {ReactNode} from "react";
import { Link } from "@tanstack/react-router";
import {  useEffect } from "react";
import s from "./AppLayout.module.css";

interface SideNote {
  title: string;
  text: string;
}

interface AppLayoutProps {
  pageTitle: string;
  activeNav: "/" | "/map" | "/modules" | "/games" | "/profile" | "/parent";
  sideNote?: SideNote;
  mobileActiveNav?: "/" | "/modules" | "/games" | "/profile" | "/parent";
  children: ReactNode;
}

const NAV_ITEMS = [
  { icon: "⌂", label: "Главная", to: "/" as const },
  { icon: "◉", label: "Путь", to: "/map" as const },
  { icon: "◫", label: "Модули", to: "/modules" as const },
  { icon: "♟", label: "Игры", to: "/games" as const },
  { icon: "◌", label: "Профиль", to: "/profile" as const },
  { icon: "⌘", label: "Родитель", to: "/parent" as const },
];

const MOBILE_NAV = [
  { icon: "⌂", label: "Главная", to: "/" as const },
  { icon: "◫", label: "Модули", to: "/modules" as const },
  { icon: "♟", label: "Игры", to: "/games" as const },
  { icon: "◌", label: "Профиль", to: "/profile" as const },
];

export function AppLayout({
  pageTitle,
  activeNav,
  sideNote,
  mobileActiveNav,
  children,
}: AppLayoutProps) {
  useEffect(() => {
    document.body.className = "orbita-page";
    return () => {
      document.body.className = "";
    };
  }, []);

  return (
    <>
      <div className="orbita-page__glow orbita-page__glow--left" />
      <div className="orbita-page__glow orbita-page__glow--right" />

      <main className="orbita-app">
        <aside className="orbita-sidebar">
          <div className="orbita-brand">
            <div className="orbita-brand__logo">OK</div>
            <div>
              <p className="orbita-brand__eyebrow">Orbita Kids</p>
              <h1>{pageTitle}</h1>
            </div>
          </div>

          <nav className="orbita-nav">
            {NAV_ITEMS.map((item) => (
              <Link
                key={item.to}
                to={item.to}
                className={`orbita-nav__link${activeNav === item.to ? " is-active" : ""}`}
              >
                <span className="orbita-nav__icon">{item.icon}</span>
                <span>{item.label}</span>
              </Link>
            ))}
          </nav>

          {sideNote && (
            <div className="orbita-sidebar__note">
              <strong>{sideNote.title}</strong>
              <p>{sideNote.text}</p>
            </div>
          )}
        </aside>

        <section className="orbita-main">{children}</section>
      </main>

      <nav className={s.mobileNav}>
        {MOBILE_NAV.map((item) => {
          const isActive = mobileActiveNav
            ? item.to === mobileActiveNav
            : item.to === activeNav;
          return (
            <Link
              key={item.to}
              to={item.to}
              className={isActive ? s.mobileNavLinkActive : s.mobileNavLink}
            >
              <span className={s.mobileNavIcon}>{item.icon}</span>
              {item.label}
            </Link>
          );
        })}
      </nav>
    </>
  );
}
