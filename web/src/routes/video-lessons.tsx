import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/video-lessons")({
  component: VideoLessonsPage,
});

function VideoLessonsPage() {
  return (
    <AppLayout
      pageTitle="Видеоуроки"
      activeNav="/modules"
      sideNote={{
        title: "Короткое объяснение",
        text: "Видео занимает 3 минуты и помогает ребенку увидеть тему перед уроком.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Видео перед уроком</p>
          <h2>Как работает алгоритм</h2>
          <p className="orbita-copy">
            В ролике робот показывает, как шаги превращаются в понятный план
            действий и почему порядок важен.
          </p>
        </div>
        <div className="orbita-action-row">
          <Link to="/lesson" className="orbita-button orbita-button--primary">
            Перейти к уроку
          </Link>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-8">
          <p className="orbita-kicker">Главный ролик</p>
          <h3>Алгоритм на примере утреннего сбора</h3>
          <p className="orbita-copy">
            Длительность 3:12. Робот показывает, что будет, если перепутать
            шаги или пропустить важное действие.
          </p>
          <div className="orbita-progress">
            <div className="orbita-progress__top">
              <span>Просмотрено</span>
              <span>82%</span>
            </div>
            <div className="orbita-progress__track">
              <span
                className="orbita-progress__fill"
                style={{ width: "82%" }}
              />
            </div>
          </div>
        </article>

        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Плейлист</p>
          <h3>Что посмотреть дальше</h3>
          <div className="orbita-list">
            <div className="orbita-item">
              <strong>1. Ошибка в шаге</strong>
              <p>Как одна пропущенная команда ломает весь путь.</p>
            </div>
            <div className="orbita-item">
              <strong>2. Команды роботу</strong>
              <p>Почему компьютер понимает только точные инструкции.</p>
            </div>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}
