import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/rating")({
  component: RatingPage,
});

function RatingPage() {
  return (
    <AppLayout
      pageTitle="Рейтинг"
      activeNav="/profile"
      mobileActiveNav="/profile"
      sideNote={{
        title: "В центре внимания",
        text: "Рейтинг показывает место и прогресс.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Топ недели</p>
          <h2>Рейтинг</h2>
          <p className="orbita-copy">Рейтинг показывает место и прогресс.</p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-8">
          <p className="orbita-kicker">Лидеры</p>
          <h3>1. Аня, 2. Дима, 3. Рома</h3>
          <p className="orbita-copy">Топ пользователей за неделю.</p>
        </article>
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Твое место</p>
          <h3>2 место</h3>
          <p className="orbita-copy">До первого места осталось 340 XP.</p>
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
