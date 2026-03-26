import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/result")({
  component: ResultPage,
});

function ResultPage() {
  return (
    <AppLayout
      pageTitle="Результат"
      activeNav="/games"
      mobileActiveNav="/games"
      sideNote={{
        title: "Финиш шага",
        text: "Результат показывает только главное: награду, сильный момент и логичное продолжение пути.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Поздравляем</p>
          <h2>Задание завершено</h2>
          <p className="orbita-copy">
            Ты правильно собрал алгоритм, прошел квиз и открыл новую клетку
            маршрута.
          </p>
        </div>
        <div className="orbita-action-row">
          <Link to="/" className="orbita-button orbita-button--primary">
            Вернуться на главную
          </Link>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Награда</p>
          <h3>+150 XP</h3>
          <p className="orbita-copy">Очки уже добавлены к прогрессу уровня.</p>
        </article>
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Точность</p>
          <h3>4 из 4</h3>
          <p className="orbita-copy">
            Все ключевые шаги в теме выполнены верно.
          </p>
        </article>
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Новый статус</p>
          <h3>Бейдж «Молодец»</h3>
          <p className="orbita-copy">
            Открыт за прохождение недели без потери темпа.
          </p>
        </article>
      </section>

      <section className="orbita-doodle-strip">
        <article className="orbita-doodle-card">
          <div className="orbita-doodle orbita-doodle--shield" />
          <div>
            <strong>Что получилось особенно хорошо</strong>
            <p>
              Ты не просто выбрал ответ, а понял, почему шаги должны идти
              именно в таком порядке.
            </p>
          </div>
        </article>
        <article className="orbita-doodle-card">
          <div className="orbita-doodle orbita-doodle--book" />
          <div>
            <strong>Что дальше</strong>
            <p>
              Следующий экран ведет обратно на главную, где уже открыта новая
              миссия и обновлен путь.
            </p>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
