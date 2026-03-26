import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/settings")({
  component: SettingsPage,
});

function SettingsPage() {
  return (
    <AppLayout
      pageTitle="Настройки"
      activeNav="/profile"
      sideNote={{
        title: "В центре внимания",
        text: "Язык, звук, помощник и аккаунт.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Параметры приложения</p>
          <h2>Настройки</h2>
          <p className="orbita-copy">Язык, звук, помощник и аккаунт.</p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Интерфейс</p>
          <h3>Звук и уведомления</h3>
          <p className="orbita-copy">Мягкие сигналы за награды.</p>
        </article>
        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Аккаунт</p>
          <h3>Почта и пароль</h3>
          <p className="orbita-copy">Настройки безопасности и отчётов.</p>
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
