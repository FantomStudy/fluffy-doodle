import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/question-fill")({
  component: QuestionFillPage,
});

function QuestionFillPage() {
  return (
    <AppLayout
      pageTitle="Заполни пропуск"
      activeNav="/games"
      sideNote={{
        title: "Дополни строку",
        text: "Вводишь слово или команду туда, где есть пробел.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Задание — Пропуск</p>
          <h2>Заполни пропуск в коде</h2>
          <p className="orbita-copy">
            Перед тобой строки кода с пустым местом. Напечатай нужное слово
            и нажми «Проверить».
          </p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Код с пропуском</p>
          <h3>Что здесь написано?</h3>
          <div className="orbita-code-shell" style={{ marginTop: "16px" }}>
            <div className="orbita-code-editor">
              <code>
                {`robot.start()\nrobot.say(___)\nrobot.move("forward")`}
              </code>
            </div>
          </div>
          <p className="orbita-copy" style={{ marginTop: "12px" }}>
            Вместо <strong>___</strong> нужна команда-строка. Посмотри на урок
            «Приветствие» если забыл.
          </p>
          <div className="orbita-field" style={{ marginTop: "16px" }}>
            <label htmlFor="fill-answer">Твой ответ</label>
            <input
              className="orbita-input"
              id="fill-answer"
              name="fill-answer"
              type="text"
              placeholder={`"Привет!"`}
            />
          </div>
        </article>

        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Результат</p>
          <h3>Так выглядит полный код</h3>
          <div className="orbita-code-shell" style={{ marginTop: "16px" }}>
            <div className="orbita-code-editor">
              <code>
                {`robot.start()\nrobot.say("Привет!")\nrobot.move("forward")`}
              </code>
            </div>
          </div>
          <div className="orbita-chip-row" style={{ marginTop: "16px" }}>
            <span className="orbita-chip orbita-chip--green">+15 XP</span>
          </div>
          <div className="orbita-action-row" style={{ marginTop: "18px" }}>
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
