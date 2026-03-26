import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/modules")({
  component: ModulesPage,
});

function ModulesPage() {
  return (
    <AppLayout
      pageTitle="Каталог модулей"
      activeNav="/modules"
      sideNote={{
        title: "Не просто список",
        text: "Каталог выглядит как атлас путешествий: у каждого мира свой паспорт, маршрут и билет на следующий шаг.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Атлас миров</p>
          <h2>Выбери, куда идти дальше</h2>
          <p className="orbita-copy">
            Вместо одинаковых карточек здесь живой каталог с большими
            паспортами тем, маршрутами внутри модуля и боковой колонкой с
            билетами на новые форматы.
          </p>
        </div>
        <div className="orbita-chip-row">
          <span className="orbita-chip">Уровень 7</span>
          <span className="orbita-chip">3 мира открыты</span>
          <span className="orbita-chip">1 мир в прогрессе</span>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-8">
          <p className="orbita-kicker">Текущий мир</p>
          <h3>Основы программирования</h3>
          <p className="orbita-copy">
            Самый важный стартовый модуль: команды, порядок действий, первый
            код и понятные шаги от теории до практики.
          </p>
          <div className="orbita-chip-row" style={{ marginTop: "16px" }}>
            <span className="orbita-chip">35% завершено</span>
            <span className="orbita-chip">+350 XP</span>
            <span className="orbita-chip">7 шагов в маршруте</span>
          </div>
          <div className="orbita-action-row" style={{ marginTop: "18px" }}>
            <Link to="/module" className="orbita-button orbita-button--primary">
              Продолжить модуль
            </Link>
          </div>
        </article>

        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Другие миры</p>
          <div className="orbita-list">
            <div className="orbita-item">
              <strong>Логика и алгоритмы</strong>
              <p>29% завершено</p>
              <small>
                <Link to="/module">Открыть</Link>
              </small>
            </div>
            <div className="orbita-item">
              <strong>Переменные</strong>
              <p>Новый модуль</p>
              <small>
                <Link to="/module">Посмотреть</Link>
              </small>
            </div>
            <div className="orbita-item">
              <strong>Циклы</strong>
              <p>Скоро открывается</p>
            </div>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
