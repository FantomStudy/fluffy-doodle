import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/theory")({
  component: TheoryPage,
});

function TheoryPage() {
  return (
    <AppLayout
      pageTitle="Теория"
      activeNav="/modules"
      sideNote={{
        title: "Старт темы",
        text: "Короткие карточки помогают быстро войти в тему и не перегружают ребенка длинными абзацами.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Первый шаг модуля</p>
          <h2>Что такое алгоритм</h2>
          <p className="orbita-copy">
            Алгоритм — это понятный порядок шагов. Сначала понимаем идею,
            потом смотрим видео и только затем идем в урок.
          </p>
        </div>
        <div className="orbita-action-row">
          <Link
            to="/video-lessons"
            className="orbita-button orbita-button--primary"
          >
            Смотреть видео
          </Link>
          <Link to="/module" className="orbita-button orbita-button--secondary">
            Назад к модулю
          </Link>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Карточка 1</p>
          <h3>Порядок действий</h3>
          <p className="orbita-copy">
            Алгоритм помогает не забывать шаги и выполнять задачу по одному
            действию за раз.
          </p>
        </article>
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Карточка 2</p>
          <h3>Пример из жизни</h3>
          <p className="orbita-copy">
            Рецепт, сбор рюкзака или маршрут до школы тоже работают как
            маленькие алгоритмы.
          </p>
        </article>
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Карточка 3</p>
          <h3>Почему это важно</h3>
          <p className="orbita-copy">
            В программировании алгоритмы помогают компьютеру понять, что
            делать и в каком порядке.
          </p>
        </article>
      </section>

      <section className="orbita-doodle-strip">
        <article className="orbita-doodle-card">
          <div className="orbita-doodle orbita-doodle--book" aria-hidden="true" />
          <div>
            <strong>Запомни</strong>
            <p>
              Хороший алгоритм короткий, понятный и идет шаг за шагом без
              лишних действий.
            </p>
          </div>
        </article>
        <article className="orbita-doodle-card">
          <div className="orbita-doodle orbita-doodle--shield" aria-hidden="true" />
          <div>
            <strong>Подсказка от робота</strong>
            <p>
              Если можешь объяснить действие другу по пунктам, значит ты уже
              мыслишь как программист.
            </p>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
