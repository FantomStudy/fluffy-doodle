import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/control")({
  component: ControlPage,
});

function ControlPage() {
  return (
    <AppLayout
      pageTitle="Контроль"
      activeNav="/parent"
      mobileActiveNav="/profile"
      sideNote={{
        title: "В центре внимания",
        text: "Лимиты времени и цели на неделю.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Настройки контроля</p>
          <h2>Контроль</h2>
          <p className="orbita-copy">Лимиты времени и цели на неделю.</p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Время</p>
          <h3>30 минут в будни</h3>
          <p className="orbita-copy">Подходит для коротких сессий.</p>
        </article>
        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Цели</p>
          <h3>5 уроков и 2 квиза</h3>
          <p className="orbita-copy">Мягкий недельный план.</p>
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
