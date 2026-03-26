import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/analytics")({
  component: AnalyticsPage,
});

function AnalyticsPage() {
  return (
    <AppLayout
      pageTitle="Аналитика"
      activeNav="/parent"
      mobileActiveNav="/profile"
      sideNote={{
        title: "В центре внимания",
        text: "Здесь видны сильные темы и зоны роста.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Сильные и слабые темы</p>
          <h2>Аналитика</h2>
          <p className="orbita-copy">
            Здесь видны сильные темы и зоны роста.
          </p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Сильная тема</p>
          <h3>Алгоритмы</h3>
          <p className="orbita-copy">Высокая точность в заданиях на порядок.</p>
        </article>
        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Зона роста</p>
          <h3>Безопасность</h3>
          <p className="orbita-copy">Стоит добавить больше практики.</p>
        </article>
      </section>

      <div className="orbita-action-row">
        <Link to="/parent" className="orbita-button orbita-button--secondary">
          Назад к панели
        </Link>
      </div>
    </AppLayout>
  );
}
