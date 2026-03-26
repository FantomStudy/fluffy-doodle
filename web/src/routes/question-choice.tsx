import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/question-choice")({
  component: QuestionChoicePage,
});

function QuestionChoicePage() {
  return (
    <AppLayout
      pageTitle="Выбери правильный ответ"
      activeNav="/games"
      sideNote={{
        title: "Выбор с помощью",
        text: "Три варианта ответа. Подсказка видна сразу — никаких ловушек.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Задание — Выбор</p>
          <h2>Выбери правильный ответ</h2>
          <p className="orbita-copy">
            Читай вопрос и выбирай один из трёх вариантов. Подсказчик
            поможет, если нужно.
          </p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-7">
          <p className="orbita-kicker">Вопрос</p>
          <h3>Какая команда двигает робота вперёд?</h3>
          <p className="orbita-copy" style={{ margin: "12px 0 24px" }}>
            Вспомни урок про движение робота по клеткам. Нужна одна команда
            из трёх.
          </p>
          <div className="orbita-answer-grid">
            <button className="orbita-answer-card" type="button">
              <span>robot.left()</span>
            </button>
            <button
              className="orbita-answer-card orbita-answer-card--selected"
              type="button"
            >
              <span>robot.forward()</span>
            </button>
            <button className="orbita-answer-card" type="button">
              <span>robot.spin()</span>
            </button>
          </div>
          <div className="orbita-action-row" style={{ marginTop: "24px" }}>
            <button
              className="orbita-button orbita-button--primary"
              type="button"
            >
              Проверить
            </button>
            <button
              className="orbita-button orbita-button--secondary"
              type="button"
            >
              Подсказка
            </button>
          </div>
        </article>

        <article className="orbita-panel orbita-span-5">
          <p className="orbita-kicker">Реакция системы</p>
          <h3>Всё правильно!</h3>
          <p className="orbita-copy" style={{ marginTop: "12px" }}>
            Команда <strong>robot.forward()</strong> двигает робота вперёд на
            одну клетку. Именно это нам нужно в этом задании.
          </p>
          <div className="orbita-chip-row" style={{ marginTop: "16px" }}>
            <span className="orbita-chip orbita-chip--green">+20 XP</span>
            <span className="orbita-chip">Модуль 2</span>
          </div>
          <div className="orbita-action-row" style={{ marginTop: "24px" }}>
            <Link to="/result" className="orbita-button orbita-button--success">
              Далее
            </Link>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
