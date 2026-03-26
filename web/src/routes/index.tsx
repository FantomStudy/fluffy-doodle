import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect } from "react";

export const Route = createFileRoute("/")({
  component: DashboardPage,
});

function DashboardPage() {
  useEffect(() => {
    document.body.className = "dashboard-page";
    return () => {
      document.body.className = "";
    };
  }, []);

  return (
    <>
      <div className="dashboard-page__aurora dashboard-page__aurora--left" />
      <div className="dashboard-page__aurora dashboard-page__aurora--right" />

      <main className="kids-dashboard">
        <aside className="dashboard-sidebar">
          <div className="dashboard-sidebar__brand">
            <div className="dashboard-sidebar__logo">OK</div>
            <div>
              <p className="dashboard-sidebar__eyebrow">Orbita Kids</p>
              <h1>Главная</h1>
            </div>
          </div>

          <nav className="dashboard-sidebar__nav">
            <Link to="/" className="dashboard-sidebar__link is-active">
              <span className="dashboard-sidebar__icon">⌂</span>
              <span>Главная</span>
            </Link>
            <Link to="/map" className="dashboard-sidebar__link">
              <span className="dashboard-sidebar__icon">◉</span>
              <span>Путь</span>
            </Link>
            <Link to="/modules" className="dashboard-sidebar__link">
              <span className="dashboard-sidebar__icon">◫</span>
              <span>Модули</span>
            </Link>
            <Link to="/lesson" className="dashboard-sidebar__link">
              <span className="dashboard-sidebar__icon">⬛</span>
              <span>Урок</span>
            </Link>
            <Link to="/games" className="dashboard-sidebar__link">
              <span className="dashboard-sidebar__icon">♟</span>
              <span>Игры</span>
            </Link>
            <Link to="/profile" className="dashboard-sidebar__link">
              <span className="dashboard-sidebar__icon">◌</span>
              <span>Профиль</span>
            </Link>
          </nav>

          <article className="dashboard-sidebar__tip">
            <p className="dashboard-sidebar__tip-label">Сегодня в фокусе</p>
            <strong>Один понятный шаг</strong>
            <p>
              Ребенок сразу видит миссию дня, текущий прогресс и одну главную
              кнопку старта без перегруза экраном.
            </p>
          </article>
        </aside>

        <section className="dashboard-main">
          <section className="mission-hero">
            <div className="mission-hero__copy">
              <p className="mission-hero__eyebrow">Сегодняшняя миссия</p>
              <h2>
                Привет, Дима. Изучи основы безопасности в интернете и заработай
                +150&nbsp;XP.
              </h2>
              <p className="mission-hero__lead">
                Сегодня ты проходишь короткий урок, выполняешь практику и
                открываешь новый значок на карте пути.
              </p>

              <div className="mission-hero__actions">
                <Link to="/lesson" className="button button--primary">
                  Пройти мини-урок
                </Link>
                <Link to="/map" className="button button--secondary">
                  Открыть путь
                </Link>
              </div>

              <div className="mission-progress">
                <div className="mission-progress__meta">
                  <span>До 8 уровня осталось</span>
                  <strong>1480 / 1630 XP</strong>
                </div>
                <div
                  className="mission-progress__track"
                  aria-label="Прогресс до следующего уровня"
                >
                  <span
                    className="mission-progress__fill"
                    style={{ width: "91%" }}
                  />
                </div>
              </div>
            </div>

            <div className="mission-hero__art" aria-hidden="true">
              <div className="mission-hero__orbit mission-hero__orbit--one" />
              <div className="mission-hero__orbit mission-hero__orbit--two" />
              <div className="mission-hero__cloud mission-hero__cloud--top" />
              <div className="mission-hero__cloud mission-hero__cloud--bottom" />
              <div className="module-card__bot" />
            </div>
          </section>

          <section className="stats-board">
            <article className="stat-pill stat-pill--level">
              <div className="stat-pill__icon">↗</div>
              <div>
                <strong>Уровень 7</strong>
                <p>Еще 150 XP до нового ранга</p>
              </div>
            </article>
            <article className="stat-pill stat-pill--xp">
              <div className="stat-pill__icon">★</div>
              <div>
                <strong>1480 XP</strong>
                <p>Уверенный темп за неделю</p>
              </div>
            </article>
            <article className="stat-pill stat-pill--streak">
              <div className="stat-pill__icon">✦</div>
              <div>
                <strong>5 дней</strong>
                <p>Серия без пропусков</p>
              </div>
            </article>
            <article className="stat-pill stat-pill--lessons">
              <div className="stat-pill__icon">⬛</div>
              <div>
                <strong>4 урока</strong>
                <p>Завершено за последние 7 дней</p>
              </div>
            </article>
          </section>

          <section className="dashboard-grid">
            <div className="dashboard-grid__main">
              <article className="continue-card">
                <div className="continue-card__fox" aria-hidden="true">
                  <div className="continue-card__fox-ear continue-card__fox-ear--left" />
                  <div className="continue-card__fox-ear continue-card__fox-ear--right" />
                  <div className="continue-card__fox-face" />
                </div>
                <div className="continue-card__content">
                  <p className="section-kicker">Продолжить обучение</p>
                  <h3>Основы программирования</h3>
                  <p>
                    Следующий шаг: команды и последовательности. Ты уже прошел
                    вводную теорию и сейчас можешь сразу перейти к уроку и
                    практике.
                  </p>
                  <div className="lesson-progress">
                    <div>
                      <div
                        className="lesson-progress__track"
                        aria-label="Прогресс модуля"
                      >
                        <span
                          className="lesson-progress__fill"
                          style={{ width: "35%" }}
                        />
                      </div>
                    </div>
                    <strong>35%</strong>
                  </div>
                </div>
                <Link to="/module" className="button button--secondary">
                  Продолжить
                </Link>
              </article>

              <section className="module-section">
                <div className="module-section__head">
                  <div>
                    <p className="section-kicker">Каталог модулей</p>
                    <h3>Учебные блоки по интересам</h3>
                  </div>
                  <Link to="/modules" className="button button--secondary">
                    Смотреть все
                  </Link>
                </div>

                <div className="module-grid">
                  <article className="module-card module-card--blue">
                    <div className="module-card__media">
                      <div className="module-card__bot" aria-hidden="true" />
                    </div>
                    <div className="module-card__body">
                      <h4>Основы программирования</h4>
                      <p>
                        Базовые команды, циклы и логика действий на понятных
                        примерах.
                      </p>
                      <div className="module-card__footer">
                        <strong>35% завершено</strong>
                        <Link to="/module">Продолжить</Link>
                      </div>
                    </div>
                  </article>

                  <article className="module-card module-card--orange">
                    <div className="module-card__media">
                      <div className="module-card__fox-mini" aria-hidden="true" />
                    </div>
                    <div className="module-card__body">
                      <h4>Логика и алгоритмы</h4>
                      <p>
                        Учимся строить порядок действий и замечать ошибки до
                        старта.
                      </p>
                      <div className="module-card__footer">
                        <strong>29% завершено</strong>
                        <Link to="/module">Открыть</Link>
                      </div>
                    </div>
                  </article>

                  <article className="module-card module-card--mint">
                    <div className="module-card__media">
                      <div className="module-card__letter" aria-hidden="true">
                        C
                      </div>
                    </div>
                    <div className="module-card__body">
                      <h4>Переменные</h4>
                      <p>
                        Понимаем, как хранятся значения и зачем они нужны в
                        коде.
                      </p>
                      <div className="module-card__footer">
                        <strong>Новый модуль</strong>
                        <Link to="/modules">Посмотреть</Link>
                      </div>
                    </div>
                  </article>
                </div>
              </section>
            </div>

            <aside className="weekly-card">
              <div
                style={{
                  display: "flex",
                  alignItems: "start",
                  justifyContent: "space-between",
                  gap: "12px",
                }}
              >
                <div>
                  <p className="section-kicker">Цель недели</p>
                  <h3>Завершить 5 уроков</h3>
                </div>
                <span className="weekly-card__badge">100 XP</span>
              </div>
              <p className="weekly-card__copy">
                Остался еще один урок, чтобы получить награду и открыть бейдж
                «Молодец».
              </p>
              <div
                className="weekly-card__checks"
                aria-label="Прогресс недельной цели"
              >
                <span className="is-done" />
                <span className="is-done" />
                <span className="is-done" />
                <span className="is-done" />
                <span />
              </div>
              <div className="weekly-card__reward">
                <strong>Награда</strong> Бейдж «Молодец» + 100&nbsp;XP и новая
                клетка на карте пути.
              </div>
              <Link
                to="/tasks"
                className="button button--primary weekly-card__button"
              >
                Посмотреть все
              </Link>
            </aside>
          </section>
        </section>
      </main>

      <nav className="mobile-shortcuts">
        <Link to="/" className="is-active">
          Главная
        </Link>
        <Link to="/map">Путь</Link>
        <Link to="/modules">Модули</Link>
        <Link to="/profile">Профиль</Link>
      </nav>
    </>
  );
}

