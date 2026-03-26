import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/parent")({
  component: ParentPage,
});

function ParentPage() {
  return (
    <AppLayout
      pageTitle="Родитель"
      activeNav="/parent"
      mobileActiveNav="/profile"
      sideNote={{
        title: "Центр управления",
        text: "Прогресс ребенка, слабые темы, лимиты времени и недельные цели в одном месте.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Панель родителя</p>
          <h2>Прогресс, аналитика и контроль</h2>
          <p className="orbita-copy">
            Здесь собраны все инструменты для наблюдения за обучением: отчёты,
            настройка лимитов и цели на неделю.
          </p>
        </div>
        <div className="orbita-chip-row">
          <span className="orbita-chip">Дима, 11 лет</span>
          <span className="orbita-chip">Уровень 7</span>
        </div>
      </header>

      <section className="orbita-metric-grid">
        <article className="orbita-metric">
          <div className="orbita-metric__icon orbita-metric__icon--green">★</div>
          <div>
            <strong>1480 XP</strong>
            <p>За неделю</p>
          </div>
        </article>
        <article className="orbita-metric">
          <div className="orbita-metric__icon orbita-metric__icon--blue">⬛</div>
          <div>
            <strong>4 урока</strong>
            <p>Завершено</p>
          </div>
        </article>
        <article className="orbita-metric">
          <div className="orbita-metric__icon orbita-metric__icon--orange">✦</div>
          <div>
            <strong>5 дней</strong>
            <p>Серия занятий</p>
          </div>
        </article>
        <article className="orbita-metric">
          <div className="orbita-metric__icon orbita-metric__icon--yellow">◎</div>
          <div>
            <strong>Алгоритмы</strong>
            <p>Лучший модуль</p>
          </div>
        </article>
      </section>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-8">
          <p className="orbita-kicker">Быстрые переходы</p>
          <div className="orbita-list">
            <div className="orbita-item">
              <strong>Аналитика</strong>
              <p>Сильные темы и зоны роста.</p>
              <small>
                <Link to="/analytics">Открыть</Link>
              </small>
            </div>
            <div className="orbita-item">
              <strong>Отчёты</strong>
              <p>История занятий и ошибок.</p>
              <small>
                <Link to="/reports">Открыть</Link>
              </small>
            </div>
            <div className="orbita-item">
              <strong>Контроль</strong>
              <p>Лимиты времени и недельные цели.</p>
              <small>
                <Link to="/control">Настроить</Link>
              </small>
            </div>
          </div>
        </article>

        <aside className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Что идет лучше всего</p>
          <strong>Алгоритмы</strong>
          <p className="orbita-copy" style={{ marginTop: "8px" }}>
            Высокая точность в заданиях на порядок.
          </p>
          <div style={{ marginTop: "18px" }}>
            <p className="orbita-kicker">Следующая награда</p>
            <strong>Бейдж «Молодец»</strong>
            <p className="orbita-copy" style={{ marginTop: "8px" }}>
              Ещё один учебный шаг без пропуска.
            </p>
          </div>
        </aside>
      </section>
    </AppLayout>
  );
}
