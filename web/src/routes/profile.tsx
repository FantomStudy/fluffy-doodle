import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/profile")({
  component: ProfilePage,
});

function ProfilePage() {
  return (
    <AppLayout
      pageTitle="Профиль"
      activeNav="/profile"
      sideNote={{
        title: "Личная карточка игрока",
        text: "Профиль должен чувствоваться как своя комната в игре: кто ты, как растешь и какие награды уже рядом.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Личный кабинет</p>
          <h2>Аватар, уровень и быстрые переходы</h2>
          <p className="orbita-copy">
            Личная витрина прогресса: достижения, статистика и следующая цель
            рядом.
          </p>
        </div>
        <div className="orbita-chip-row">
          <span className="orbita-chip">Дима, 11 лет</span>
          <span className="orbita-chip">Уровень 7</span>
          <span className="orbita-chip">1480 XP</span>
        </div>
      </header>

      <section className="orbita-metric-grid">
        <article className="orbita-metric">
          <div className="orbita-metric__icon orbita-metric__icon--blue">↗</div>
          <div>
            <strong>Уровень 7</strong>
            <p>Текущий прогресс по рангу</p>
          </div>
        </article>
        <article className="orbita-metric">
          <div className="orbita-metric__icon orbita-metric__icon--yellow">★</div>
          <div>
            <strong>1480 XP</strong>
            <p>Уже заработано</p>
          </div>
        </article>
        <article className="orbita-metric">
          <div className="orbita-metric__icon orbita-metric__icon--orange">✦</div>
          <div>
            <strong>5 дней</strong>
            <p>Серия без пропусков</p>
          </div>
        </article>
        <article className="orbita-metric">
          <div className="orbita-metric__icon orbita-metric__icon--green">⬛</div>
          <div>
            <strong>4 урока</strong>
            <p>Завершено за неделю</p>
          </div>
        </article>
      </section>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-8">
          <p className="orbita-kicker">Игрок недели</p>
          <h3>Дима, 11 лет</h3>
          <p className="orbita-copy">
            Лучше всего идут алгоритмы, а следующая награда уже близко, если
            сохранить темп ещё на несколько шагов.
          </p>
          <div className="orbita-chip-row" style={{ marginTop: "16px" }}>
            <span className="orbita-chip">Лучший модуль: алгоритмы</span>
            <span className="orbita-chip">Серия 5 дней</span>
            <span className="orbita-chip">Следующий бейдж близко</span>
          </div>
          <div className="orbita-list" style={{ marginTop: "18px" }}>
            <div className="orbita-item">
              <strong>Достижения</strong>
              <p>Посмотреть все открытые и закрытые бейджи.</p>
              <small>
                <Link to="/achievements">Открыть достижения</Link>
              </small>
            </div>
            <div className="orbita-item">
              <strong>Рейтинг</strong>
              <p>Проверить позицию за неделю.</p>
              <small>
                <Link to="/rating">Перейти в рейтинг</Link>
              </small>
            </div>
          </div>
        </article>

        <aside className="orbita-panel orbita-span-4">
          <article style={{ marginBottom: "16px" }}>
            <strong>Что идет лучше всего</strong>
            <p className="orbita-copy" style={{ marginTop: "8px" }}>
              Лучше всего идут алгоритмы. Это сильная сторона профиля.
            </p>
          </article>
          <article style={{ marginBottom: "16px" }}>
            <strong>Следующая награда</strong>
            <p className="orbita-copy" style={{ marginTop: "8px" }}>
              Следующая награда уже близко. Ещё один уверенный учебный шаг.
            </p>
          </article>
          <div className="orbita-action-row">
            <Link to="/" className="orbita-button orbita-button--primary">
              На главную
            </Link>
            <Link
              to="/settings"
              className="orbita-button orbita-button--secondary"
            >
              Настройки
            </Link>
          </div>
        </aside>
      </section>
    </AppLayout>
  );
}
