import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/lesson")({
  component: LessonPage,
});

function LessonPage() {
  return (
    <AppLayout
      pageTitle="Урок"
      activeNav="/games"
      mobileActiveNav="/games"
      sideNote={{
        title: "Один шаг за экран",
        text: "Сначала понимаем цель урока, потом идем в практику и проверяем себя в квизе.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Основной шаг</p>
          <h2>Помоги роботу дойти до финиша</h2>
          <p className="orbita-copy">
            Твоя цель — понять, почему команды должны идти в правильном
            порядке. Ниже маршрут из четырех клеток.
          </p>
        </div>
        <div className="orbita-chip-row">
          <span className="orbita-chip">Урок 3 из 7</span>
          <span className="orbita-chip">Длительность 5 минут</span>
        </div>
      </header>

      <section className="orbita-board">
        <div className="orbita-board__path" aria-hidden="true" />
        <Link to="/theory" className="orbita-board__cell">
          <div className="orbita-board__badge">1</div>
          <strong>Вспомни правило</strong>
          <p>Алгоритм — это порядок понятных шагов.</p>
        </Link>
        <Link to="/video-lessons" className="orbita-board__cell">
          <div className="orbita-board__badge">2</div>
          <strong>Посмотри пример</strong>
          <p>Робот показывает, что будет при ошибке в шаге.</p>
        </Link>
        <a className="orbita-board__cell is-current">
          <div className="orbita-board__badge">3</div>
          <strong>Собери маршрут</strong>
          <p>Сейчас открыта логическая часть урока.</p>
        </a>
        <Link to="/practice" className="orbita-board__cell">
          <div className="orbita-board__badge">4</div>
          <strong>Перейди к практике</strong>
          <p>После этого экрана откроется задание с ответом.</p>
        </Link>
      </section>

      <section className="orbita-doodle-strip">
        <article className="orbita-doodle-card">
          <div className="orbita-doodle orbita-doodle--book" />
          <div>
            <strong>Что запомнить</strong>
            <p>
              Если поменять местами команды, робот может не дойти до цели.
            </p>
          </div>
        </article>
        <article className="orbita-doodle-card">
          <div className="orbita-doodle orbita-doodle--shield" />
          <div>
            <strong>Подсказка помощника</strong>
            <p>
              Сначала описывай задачу вслух, а потом разбивай ее на маленькие
              шаги.
            </p>
          </div>
        </article>
      </section>

      <div className="orbita-action-row">
        <Link to="/practice" className="orbita-button orbita-button--primary">
          Перейти к практике
        </Link>
        <Link to="/module" className="orbita-button orbita-button--secondary">
          Вернуться к модулю
        </Link>
      </div>
    </AppLayout>
  );
}
