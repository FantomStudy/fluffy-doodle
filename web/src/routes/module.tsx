import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/module")({
  component: ModulePage,
});

function ModulePage() {
  return (
    <AppLayout
      pageTitle="Основы программирования"
      activeNav="/modules"
      sideNote={{
        title: "Маршрут модуля",
        text: "Здесь нет скучного списка уроков. Весь модуль выглядит как поле настольной игры с шагами, наградами и финальной клеткой.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Текущий модуль</p>
          <h2>Основы программирования</h2>
          <p className="orbita-copy">
            Ты проходишь тему через короткие форматы: теория, видео, урок,
            практика, квиз и сундук награды.
          </p>
        </div>
        <div className="orbita-chip-row">
          <span className="orbita-chip">4 из 7 шагов</span>
          <span className="orbita-chip">+350 XP за модуль</span>
          <span className="orbita-chip">Следующий шаг: урок</span>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Прогресс</p>
          <h3>35% завершено</h3>
          <p className="orbita-copy">
            Видно, сколько уже пройдено и где находится текущая клетка.
          </p>
        </article>
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Награда</p>
          <h3>Сундук + бейдж</h3>
          <p className="orbita-copy">
            После финиша откроются XP, значок и новая тема на карте.
          </p>
        </article>
        <article className="orbita-panel orbita-span-4">
          <p className="orbita-kicker">Помощник</p>
          <h3>Робот сопровождает</h3>
          <p className="orbita-copy">
            На каждом этапе есть короткая подсказка, чтобы ребенок не
            терялся.
          </p>
        </article>
      </section>

      <section className="orbita-board orbita-board--four">
        <div className="orbita-board__path" aria-hidden="true" />

        <Link to="/theory" className="orbita-board__cell">
          <div className="orbita-board__badge">◎</div>
          <strong>Теория</strong>
          <p>Короткие карточки вместо длинного текста.</p>
        </Link>

        <Link to="/video-lessons" className="orbita-board__cell">
          <div className="orbita-board__badge">▶</div>
          <strong>Видеоурок</strong>
          <p>Смотрим короткое объяснение с персонажем.</p>
        </Link>

        <Link to="/lesson" className="orbita-board__cell is-current">
          <div className="orbita-board__badge">◨</div>
          <strong>Урок</strong>
          <p>Текущая клетка. Изучаем один небольшой шаг.</p>
        </Link>

        <Link to="/practice" className="orbita-board__cell">
          <div className="orbita-board__badge">✦</div>
          <strong>Практика</strong>
          <p>Проверяем знание на действии.</p>
        </Link>

        <Link to="/quiz" className="orbita-board__cell">
          <div className="orbita-board__badge">?</div>
          <strong>Квиз</strong>
          <p>Четыре коротких вопроса по теме.</p>
        </Link>

        <Link to="/result" className="orbita-board__cell">
          <div className="orbita-board__badge">★</div>
          <strong>Результат</strong>
          <p>XP, советы и открытие следующей темы.</p>
        </Link>
      </section>
    </AppLayout>
  );
}
