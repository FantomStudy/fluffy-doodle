import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/practice")({
  component: PracticePage,
});

function PracticePage() {
  return (
    <AppLayout
      pageTitle="Практика"
      activeNav="/games"
      mobileActiveNav="/games"
      sideNote={{
        title: "Интерактивный шаг",
        text: "Один вопрос, несколько вариантов, сразу видно правильный ответ и объяснение.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Практика после урока</p>
          <h2>Выбери правильный порядок команд</h2>
          <p className="orbita-copy">
            Робот должен сначала включиться, затем получить команду и только
            потом начать движение.
          </p>
        </div>
        <div className="orbita-chip-row">
          <span className="orbita-chip">Задание 1 из 3</span>
          <span className="orbita-chip">Подсказка доступна</span>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-7">
          <div className="orbita-answer-grid">
            <div className="orbita-answer-card is-correct">
              <div className="orbita-answer-card__badge">A</div>
              <div>
                <strong>Включить → дать команду → начать движение</strong>
                <p>
                  Это верный алгоритм: каждый шаг идет в понятном порядке.
                </p>
              </div>
            </div>
            <div className="orbita-answer-card">
              <div className="orbita-answer-card__badge">B</div>
              <div>
                <strong>Начать движение → включить → дать команду</strong>
                <p>
                  Здесь робот пытается действовать до того, как готов.
                </p>
              </div>
            </div>
            <div className="orbita-answer-card">
              <div className="orbita-answer-card__badge">C</div>
              <div>
                <strong>Дать команду → начать движение → включить</strong>
                <p>
                  Команда не сработает, если робот еще не включен.
                </p>
              </div>
            </div>
          </div>
        </article>

        <article className="orbita-panel orbita-span-5">
          <p className="orbita-kicker">Обратная связь</p>
          <h3>Почему это правильно</h3>
          <p className="orbita-copy">
            Алгоритм работает, когда следующий шаг опирается на предыдущий.
            Сначала подготовка, потом действие.
          </p>
          <div className="orbita-action-row" style={{ marginTop: "16px" }}>
            <Link to="/quiz" className="orbita-button orbita-button--primary">
              Проверить себя
            </Link>
            <Link
              to="/lesson"
              className="orbita-button orbita-button--secondary"
            >
              Назад к уроку
            </Link>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
