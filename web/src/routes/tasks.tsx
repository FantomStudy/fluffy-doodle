import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/tasks")({
  component: TasksPage,
});

function TasksPage() {
  return (
    <AppLayout
      pageTitle="Задания"
      activeNav="/"
      sideNote={{
        title: "Ежедневный маршрут",
        text: "Три конкретных шага на сегодня и награда за их выполнение.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Сегодняшние задачи</p>
          <h2>Три шага до награды</h2>
          <p className="orbita-copy">
            Выполни все три шага и получи +120 XP. Каждый шаг занимает не
            более 5 минут.
          </p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Сегодня: три шага</p>
          <div className="orbita-list">
            <div className="orbita-item">
              <strong>1. Теория «Алгоритм»</strong>
              <p>Прочитай 3 коротких карточки о порядке действий.</p>
              <small>
                <Link to="/theory">Начать</Link>
              </small>
            </div>
            <div className="orbita-item">
              <strong>2. Видеоурок (3 мин)</strong>
              <p>Посмотри, как робот использует команды правильно.</p>
              <small>
                <Link to="/video-lessons">Смотреть</Link>
              </small>
            </div>
            <div className="orbita-item">
              <strong>3. Практика</strong>
              <p>Одно интерактивное задание с мгновенной проверкой.</p>
              <small>
                <Link to="/practice">Выполнить</Link>
              </small>
            </div>
          </div>
        </article>

        <article className="orbita-panel orbita-span-6">
          <p className="orbita-kicker">Награда: +120 XP</p>
          <h3>Бейдж + очки</h3>
          <p className="orbita-copy">
            После выполнения всех трёх шагов начисляются XP и открывается
            новая клетка на карте пути.
          </p>
          <div className="orbita-action-row" style={{ marginTop: "16px" }}>
            <Link to="/map" className="orbita-button orbita-button--primary">
              Открыть путь
            </Link>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
