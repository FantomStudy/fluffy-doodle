import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/question-order")({
  component: QuestionOrderPage,
});

function QuestionOrderPage() {
  return (
    <AppLayout
      pageTitle="Собери порядок"
      activeNav="/games"
      sideNote={{
        title: "Правильная очерёдность",
        text: "Тащи шаги в нужную очерёдность. Порядок важен!",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Задание — Порядок</p>
          <h2>Собери шаги в правильном порядке</h2>
          <p className="orbita-copy">
            Перетаскивай команды так, чтобы робот сделал всё правильно.
            Сначала старт, потом движение.
          </p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-7">
          <p className="orbita-kicker">Шаги программы</p>
          <h3>Расставь команды в нужном порядке</h3>
          <div className="orbita-list" style={{ marginTop: "16px" }}>
            <div
              className="orbita-item"
              style={{ cursor: "grab", userSelect: "none" }}
            >
              <span
                style={{
                  fontWeight: 800,
                  marginRight: "12px",
                  color: "#4b8dff",
                }}
              >
                1
              </span>
              robot.start()
            </div>
            <div
              className="orbita-item"
              style={{ cursor: "grab", userSelect: "none" }}
            >
              <span
                style={{
                  fontWeight: 800,
                  marginRight: "12px",
                  color: "#4b8dff",
                }}
              >
                2
              </span>
              robot.say("Привет!")
            </div>
            <div
              className="orbita-item"
              style={{ cursor: "grab", userSelect: "none" }}
            >
              <span
                style={{
                  fontWeight: 800,
                  marginRight: "12px",
                  color: "#4b8dff",
                }}
              >
                3
              </span>
              robot.move("forward")
            </div>
            <div
              className="orbita-item"
              style={{ cursor: "grab", userSelect: "none" }}
            >
              <span
                style={{
                  fontWeight: 800,
                  marginRight: "12px",
                  color: "#4b8dff",
                }}
              >
                4
              </span>
              robot.stop()
            </div>
          </div>
        </article>

        <article className="orbita-panel orbita-span-5">
          <p className="orbita-kicker">Проверка</p>
          <h3>Порядок верный!</h3>
          <p className="orbita-copy" style={{ marginTop: "12px" }}>
            Всё по схеме: запуск → приветствие → движение → стоп. Робот
            выполнит задание без ошибок.
          </p>
          <div className="orbita-chip-row" style={{ marginTop: "16px" }}>
            <span className="orbita-chip orbita-chip--green">+25 XP</span>
            <span className="orbita-chip">Серия 3</span>
          </div>
          <div className="orbita-action-row" style={{ marginTop: "24px" }}>
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
