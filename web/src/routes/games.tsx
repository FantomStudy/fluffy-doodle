import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/games")({
  component: GamesPage,
});

function GamesPage() {
  return (
    <AppLayout
      pageTitle="Игры"
      activeNav="/games"
      sideNote={{
        title: "Игровой центр",
        text: "Три формата заданий: выбор ответа, написание кода и расстановка порядка.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Игровые форматы</p>
          <h2>Учись через игру</h2>
          <p className="orbita-copy">
            Выбери формат задания. Каждый тренирует другой навык: логику,
            точность или умение строить алгоритмы.
          </p>
        </div>
        <div className="orbita-chip-row">
          <span className="orbita-chip">3 формата доступно</span>
          <span className="orbita-chip">+50 XP за каждое</span>
        </div>
      </header>

      <div className="orbita-game-hub">
        <article className="orbita-game-card">
          <div className="orbita-game-card__icon is-blue">A</div>
          <strong>Выбери ответ</strong>
          <p>Один вопрос, четыре варианта. Мгновенная реакция после клика.</p>
          <div className="orbita-action-row" style={{ marginTop: "14px" }}>
            <Link
              to="/question-choice"
              className="orbita-button orbita-button--primary"
            >
              Начать
            </Link>
          </div>
        </article>

        <article className="orbita-game-card">
          <div className="orbita-game-card__icon is-orange">&lt;/&gt;</div>
          <strong>Напиши код</strong>
          <p>
            Слева описание, справа компилятор. Три строки, чтобы запустить
            робота.
          </p>
          <div className="orbita-action-row" style={{ marginTop: "14px" }}>
            <Link
              to="/question-code"
              className="orbita-button orbita-button--primary"
            >
              Начать
            </Link>
          </div>
        </article>

        <article className="orbita-game-card">
          <div className="orbita-game-card__icon is-purple">1→</div>
          <strong>Собери порядок</strong>
          <p>Расставь шаги в правильной последовательности.</p>
          <div className="orbita-action-row" style={{ marginTop: "14px" }}>
            <Link
              to="/question-order"
              className="orbita-button orbita-button--primary"
            >
              Начать
            </Link>
          </div>
        </article>
      </div>
    </AppLayout>
  );
}
