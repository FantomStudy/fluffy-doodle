import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/reports")({
  component: ReportsPage,
});

function ReportsPage() {
  return (
    <AppLayout
      pageTitle="Отчёты"
      activeNav="/parent"
      mobileActiveNav="/profile"
      sideNote={{
        title: "В центре внимания",
        text: "Краткая история занятий и ошибок.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Последние занятия</p>
          <h2>Отчёты</h2>
          <p className="orbita-copy">Краткая история занятий и ошибок.</p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-12">
          <p className="orbita-kicker">История</p>
          <h3>24 марта — алгоритмы</h3>
          <p className="orbita-copy">
            Пройдены урок, практика и квиз. Ошибок: 1.
          </p>
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
