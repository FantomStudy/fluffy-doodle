import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/question-code")({
  component: QuestionCodePage,
});

function QuestionCodePage() {
  return (
    <AppLayout
      pageTitle="Напиши код"
      activeNav="/games"
      sideNote={{
        title: "Миниредактор кода",
        text: "Пишешь сам с нуля. Подсказка открывается одним нажатием.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Задание — Код</p>
          <h2>Напиши короткий код</h2>
          <p className="orbita-copy">
            Открой редактор, напечатай команды и запусти. Ошибку покажет
            сразу — без страшных сообщений.
          </p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-5">
          <p className="orbita-kicker">Что нужно сделать</p>
          <h3>Напиши три команды для старта</h3>
          <div className="orbita-list" style={{ marginTop: "16px" }}>
            <div className="orbita-item">
              <strong>1. Запустить робота</strong>
              <p>Вызови команду старт.</p>
            </div>
            <div className="orbita-item">
              <strong>2. Поздороваться</strong>
              <p>Робот должен сказать «Привет!».</p>
            </div>
            <div className="orbita-item">
              <strong>3. Двинуться вперёд</strong>
              <p>Дай команду движения.</p>
            </div>
          </div>
          <div className="orbita-chip-row" style={{ marginTop: "20px" }}>
            <span className="orbita-chip">Модуль 3</span>
            <span className="orbita-chip">Урок 2</span>
          </div>
        </article>

        <article className="orbita-panel orbita-span-7">
          <p className="orbita-kicker">Редактор</p>
          <div className="orbita-code-shell">
            <div className="orbita-code-editor">
              <code>
                {`robot.start()\nrobot.say("Привет!")\nrobot.move("forward")`}
              </code>
            </div>
            <div className="orbita-code-hint">
              <span>Подсказка: используй robot.start(), robot.say(), robot.move()</span>
            </div>
          </div>
          <div className="orbita-action-row" style={{ marginTop: "18px" }}>
            <button
              className="orbita-button orbita-button--primary"
              type="button"
            >
              Запустить
            </button>
            <button
              className="orbita-button orbita-button--secondary"
              type="button"
            >
              Сбросить
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
