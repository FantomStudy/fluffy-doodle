import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/quiz")({
  component: QuizPage,
});

function QuizPage() {
  return (
    <AppLayout
      pageTitle="Квиз"
      activeNav="/games"
      mobileActiveNav="/games"
      sideNote={{
        title: "Финальная проверка",
        text: "Небольшой квиз после практики показывает, закрепилась ли тема.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Финальный вопрос</p>
          <h2>Что такое алгоритм?</h2>
          <p className="orbita-copy">
            Выбери самое точное определение. После ответа откроется экран
            результата с наградой.
          </p>
        </div>
        <div className="orbita-chip-row">
          <span className="orbita-chip">Вопрос 2 из 4</span>
          <span className="orbita-chip">Прогресс 50%</span>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-8">
          <div className="orbita-answer-grid">
            <div className="orbita-answer-card is-correct">
              <div className="orbita-answer-card__badge">A</div>
              <div>
                <strong>Понятный порядок шагов для решения задачи</strong>
                <p>
                  Правильный ответ. Алгоритм описывает, что делать и в каком
                  порядке.
                </p>
              </div>
            </div>
            <div className="orbita-answer-card">
              <div className="orbita-answer-card__badge">B</div>
              <div>
                <strong>Любая красивая картинка на экране</strong>
                <p>
                  Это не связано с программированием и логикой действий.
                </p>
              </div>
            </div>
            <div className="orbita-answer-card">
              <div className="orbita-answer-card__badge">C</div>
              <div>
                <strong>Одна случайная команда без порядка</strong>
                <p>
                  Алгоритм всегда состоит из связанных шагов, а не из одного
                  случайного действия.
                </p>
              </div>
            </div>
          </div>
        </article>

        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">После квиза</p>
          <h3>Что произойдет дальше</h3>
          <p className="orbita-copy">
            Ты получишь XP, увидишь советы и откроешь следующую клетку
            модуля.
          </p>
          <div className="orbita-action-row" style={{ marginTop: "16px" }}>
            <Link to="/result" className="orbita-button orbita-button--primary">
              Завершить
            </Link>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
