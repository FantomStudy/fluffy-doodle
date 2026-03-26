import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/map")({
  component: MapPage,
});

function MapPage() {
  return (
    <AppLayout
      pageTitle="Путь обучения"
      activeNav="/map"
      sideNote={{
        title: "В центре внимания",
        text: "Путь теперь выглядит как настоящее поле настольной игры: видна текущая клетка, награда и закрытые этапы.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Карта прогресса</p>
          <h2>Путь до 8 уровня</h2>
          <p className="orbita-copy">
            Каждая клетка ведет к следующему шагу: теория, видео, урок,
            практика, квиз или награда.
          </p>
        </div>
        <div className="orbita-chip-row">
          <span className="orbita-chip">Уровень 7</span>
          <span className="orbita-chip">1480 XP</span>
          <span className="orbita-chip">5 дней подряд</span>
        </div>
      </header>

      <section className="orbita-monopoly-shell">
        <div className="orbita-monopoly-board">
          <Link
            to="/"
            className="orbita-monopoly-board__cell is-corner is-complete"
            data-area="c1"
          >
            <div className="orbita-monopoly-icon is-green">⌂</div>
            <span className="orbita-monopoly-step">Старт</span>
            <strong className="orbita-monopoly-title">Главная база</strong>
            <p className="orbita-monopoly-copy">
              Возвращайся сюда за миссией дня и наградой.
            </p>
          </Link>

          <Link
            to="/theory"
            className="orbita-monopoly-board__cell is-complete"
            data-area="t1"
          >
            <div className="orbita-monopoly-icon is-blue">◎</div>
            <span className="orbita-monopoly-step">Шаг 1</span>
            <strong className="orbita-monopoly-title">Теория</strong>
            <p className="orbita-monopoly-copy">
              Короткие карточки и примеры из жизни.
            </p>
            <span className="orbita-monopoly-price">Пройдено</span>
          </Link>

          <Link
            to="/video-lessons"
            className="orbita-monopoly-board__cell is-complete"
            data-area="t2"
          >
            <div className="orbita-monopoly-icon is-purple">▶</div>
            <span className="orbita-monopoly-step">Шаг 2</span>
            <strong className="orbita-monopoly-title">Видеоурок</strong>
            <p className="orbita-monopoly-copy">
              Быстрый разбор темы с персонажем.
            </p>
            <span className="orbita-monopoly-price">Пройдено</span>
          </Link>

          <Link
            to="/lesson"
            className="orbita-monopoly-board__cell is-current"
            data-area="t3"
          >
            <div className="orbita-monopoly-icon is-orange">◨</div>
            <span className="orbita-monopoly-step">Шаг 3</span>
            <strong className="orbita-monopoly-title">Урок</strong>
            <p className="orbita-monopoly-copy">
              Сейчас открыт этот этап. Можно продолжать.
            </p>
            <span className="orbita-monopoly-price">Текущий шаг</span>
          </Link>

          <Link
            to="/tasks"
            className="orbita-monopoly-board__cell is-corner is-reward"
            data-area="c2"
          >
            <div className="orbita-monopoly-icon is-orange">✦</div>
            <span className="orbita-monopoly-step">Угол</span>
            <strong className="orbita-monopoly-title">Сундук XP</strong>
            <p className="orbita-monopoly-copy">
              Забирай дополнительные награды за серию.
            </p>
          </Link>

          <Link
            to="/practice"
            className="orbita-monopoly-board__cell is-locked"
            data-area="r1"
          >
            <div className="orbita-monopoly-icon is-green">✦</div>
            <span className="orbita-monopoly-step">Шаг 4</span>
            <strong className="orbita-monopoly-title">Практика</strong>
            <p className="orbita-monopoly-copy">
              Откроется сразу после урока.
            </p>
            <span className="orbita-monopoly-price">Скоро</span>
          </Link>

          <Link
            to="/quiz"
            className="orbita-monopoly-board__cell is-locked"
            data-area="r2"
          >
            <div className="orbita-monopoly-icon is-blue">?</div>
            <span className="orbita-monopoly-step">Шаг 5</span>
            <strong className="orbita-monopoly-title">Квиз</strong>
            <p className="orbita-monopoly-copy">
              Проверка знаний из 4 коротких вопросов.
            </p>
            <span className="orbita-monopoly-price">Закрыто</span>
          </Link>

          <Link
            to="/result"
            className="orbita-monopoly-board__cell is-reward"
            data-area="r3"
          >
            <div className="orbita-monopoly-icon is-purple">✓</div>
            <span className="orbita-monopoly-step">Финиш</span>
            <strong className="orbita-monopoly-title">Результат</strong>
            <p className="orbita-monopoly-copy">
              XP, советы и новый бейдж недели.
            </p>
            <span className="orbita-monopoly-price">+150 XP</span>
          </Link>

          <Link
            to="/games"
            className="orbita-monopoly-board__cell is-corner is-danger"
            data-area="c3"
          >
            <div className="orbita-monopoly-icon is-red">♟</div>
            <span className="orbita-monopoly-step">Угол</span>
            <strong className="orbita-monopoly-title">Игры</strong>
            <p className="orbita-monopoly-copy">
              Задания другого формата — квизы, код и порядок.
            </p>
          </Link>

          <div className="orbita-monopoly-board__center">
            <p className="orbita-kicker">Центр маршрута</p>
            <strong style={{ fontSize: "28px" }}>Основы программирования</strong>
            <p className="orbita-copy">
              Этот модуль охватывает базовые понятия кодинга, порядок команд и
              первые алгоритмы.
            </p>
            <div className="orbita-chip-row">
              <span className="orbita-monopoly-chip">4 из 7 шагов</span>
              <span className="orbita-monopoly-chip">+350 XP за модуль</span>
            </div>
            <Link to="/module" className="orbita-button orbita-button--primary">
              К модулю
            </Link>
          </div>
        </div>
      </section>
    </AppLayout>
  );
}
