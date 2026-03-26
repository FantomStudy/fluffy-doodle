import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/onboarding")({
  component: OnboardingPage,
});

function OnboardingPage() {
  return (
    <AppLayout
      pageTitle="Онбординг"
      activeNav="/"
      sideNote={{
        title: "В центре внимания",
        text: "Возраст, аватар и первый модуль.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Первый запуск</p>
          <h2>Онбординг</h2>
          <p className="orbita-copy">Возраст, аватар и первый модуль.</p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-12">
          <p className="orbita-kicker">Настройка</p>
          <h3>Подберем обучение под ребенка</h3>
          <p className="orbita-copy">
            Стартовый маршрут занимает несколько шагов: роль, возраст,
            помощник и первый модуль.
          </p>
          <div className="orbita-action-row" style={{ marginTop: "18px" }}>
            <Link to="/register" className="orbita-button orbita-button--primary">
              Начать регистрацию
            </Link>
            <Link to="/login" className="orbita-button orbita-button--secondary">
              Войти
            </Link>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
