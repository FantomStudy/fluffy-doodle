import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/question-match")({
  component: QuestionMatchPage,
});

function QuestionMatchPage() {
  return (
    <AppLayout
      pageTitle="Соедини пары"
      activeNav="/games"
      sideNote={{
        title: "Пары команд",
        text: "Нажми слово слева, потом — подходящее справа.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Задание — Пары</p>
          <h2>Соедини команду с действием</h2>
          <p className="orbita-copy">
            Кликни на команду слева и подходящее описание справа. Пары
            подсветятся одним цветом.
          </p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Команды</p>
          <h3>Выбери команду</h3>
          <div className="orbita-list" style={{ marginTop: "16px" }}>
            <button
              className="orbita-answer-card orbita-answer-card--selected"
              type="button"
              style={{ width: "100%", textAlign: "left", marginBottom: "8px" }}
            >
              robot.start()
            </button>
            <button
              className="orbita-answer-card"
              type="button"
              style={{ width: "100%", textAlign: "left", marginBottom: "8px" }}
            >
              robot.say()
            </button>
            <button
              className="orbita-answer-card"
              type="button"
              style={{ width: "100%", textAlign: "left", marginBottom: "8px" }}
            >
              robot.move()
            </button>
            <button
              className="orbita-answer-card"
              type="button"
              style={{ width: "100%", textAlign: "left" }}
            >
              robot.stop()
            </button>
          </div>
        </article>

        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Действия</p>
          <h3>Выбери описание</h3>
          <div className="orbita-list" style={{ marginTop: "16px" }}>
            <button
              className="orbita-answer-card"
              type="button"
              style={{ width: "100%", textAlign: "left", marginBottom: "8px" }}
            >
              Двигает робота вперёд
            </button>
            <button
              className="orbita-answer-card orbita-answer-card--selected"
              type="button"
              style={{ width: "100%", textAlign: "left", marginBottom: "8px" }}
            >
              Запускает программу
            </button>
            <button
              className="orbita-answer-card"
              type="button"
              style={{ width: "100%", textAlign: "left", marginBottom: "8px" }}
            >
              Произносит фразу вслух
            </button>
            <button
              className="orbita-answer-card"
              type="button"
              style={{ width: "100%", textAlign: "left" }}
            >
              Останавливает робота
            </button>
          </div>
          <div className="orbita-action-row" style={{ marginTop: "20px" }}>
            <button
              className="orbita-button orbita-button--primary"
              type="button"
            >
              Проверить
            </button>
            <Link to="/result" className="orbita-button orbita-button--success">
              Далее
            </Link>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
