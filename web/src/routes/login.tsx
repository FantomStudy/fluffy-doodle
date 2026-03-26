import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/login")({
  component: LoginPage,
});

function LoginPage() {
  return (
    <AppLayout
      pageTitle="Вход"
      activeNav="/"
      sideNote={{
        title: "Быстрый возврат в продукт",
        text: "Ребенок сразу попадает к миссии дня, а родитель к прогрессу, отчётам и настройкам контроля.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">С возвращением</p>
          <h2>Войти в Orbita Kids</h2>
          <p className="orbita-copy">
            Слева история продукта, справа форма входа с ролью, логином и
            паролем.
          </p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-7">
          <p className="orbita-kicker">Один продукт, два сценария</p>
          <h3>Ребенок и родитель входят по-разному, но в одну экосистему</h3>
          <p className="orbita-copy">
            После входа ребенок продолжает путь, уроки и игры, а родитель
            открывает аналитику, отчёты и контроль времени.
          </p>
          <div className="orbita-list" style={{ marginTop: "16px" }}>
            <div className="orbita-item">
              <strong>Ребенок</strong>
              <p>Продолжить модуль и вернуться к последней точке прогресса.</p>
            </div>
            <div className="orbita-item">
              <strong>Родитель</strong>
              <p>Проверить сильные темы, ошибки и время занятий.</p>
            </div>
          </div>
        </article>

        <article className="orbita-panel orbita-span-5">
          <p className="orbita-kicker">Войти</p>
          <h3>Роль и данные</h3>
          <form className="orbita-form">
            <div className="orbita-field">
              <label htmlFor="login-user">Почта или логин</label>
              <input
                className="orbita-input"
                id="login-user"
                name="login-user"
                type="text"
                placeholder="dima@orbitakids.ru"
                autoComplete="username"
              />
            </div>
            <div className="orbita-field">
              <label htmlFor="login-password">Пароль</label>
              <input
                className="orbita-input"
                id="login-password"
                name="login-password"
                type="password"
                placeholder="Введите пароль"
                autoComplete="current-password"
              />
            </div>
          </form>
          <div className="orbita-action-row" style={{ marginTop: "18px" }}>
            <Link to="/" className="orbita-button orbita-button--primary">
              Войти как ребенок
            </Link>
            <Link
              to="/parent"
              className="orbita-button orbita-button--secondary"
            >
              Войти как родитель
            </Link>
          </div>
          <p style={{ marginTop: "18px", color: "#587497", fontSize: "14px" }}>
            Нет аккаунта?{" "}
            <Link to="/register" style={{ color: "#4b8dff", fontWeight: 800 }}>
              Перейти к регистрации
            </Link>
          </p>
        </article>
      </section>
    </AppLayout>
  );
}
