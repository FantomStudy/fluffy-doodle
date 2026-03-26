import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/achievements")({
  component: AchievementsPage,
});

function AchievementsPage() {
  return (
    <AppLayout
      pageTitle="Достижения"
      activeNav="/profile"
      mobileActiveNav="/profile"
      sideNote={{
        title: "В центре внимания",
        text: "Бейджи, редкость и прогресс.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Коллекция наград</p>
          <h2>Достижения</h2>
          <p className="orbita-copy">Бейджи, редкость и прогресс.</p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Бейдж</p>
          <h3>Молодец</h3>
          <p className="orbita-copy">За 5 уроков подряд.</p>
        </article>
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Бейдж</p>
          <h3>Код-исследователь</h3>
          <p className="orbita-copy">За первый модуль без ошибок.</p>
        </article>
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Бейдж</p>
          <h3>Секретный</h3>
          <p className="orbita-copy">Откроется позже.</p>
        </article>
      </section>

      <div className="orbita-action-row">
        <Link to="/profile" className="orbita-button orbita-button--secondary">
          Назад к профилю
        </Link>
      </div>
    </AppLayout>
  );
}
