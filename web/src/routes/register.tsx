import { createFileRoute, Link } from "@tanstack/react-router";
import { AppLayout } from "../components/orbita/AppLayout";

export const Route = createFileRoute("/register")({
  component: RegisterPage,
});

function RegisterPage() {
  return (
    <AppLayout
      pageTitle="Регистрация"
      activeNav="/"
      sideNote={{
        title: "Старт без перегруза",
        text: "Сначала роль и имя, потом возраст и помощник. Экран выглядит как начало приключения.",
      }}
    >
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Новый профиль</p>
          <h2>Создать аккаунт за пару шагов</h2>
          <p className="orbita-copy">
            Простой старт: роль, имя, возраст и пароль. Сразу попадаешь в
            живой первый экран.
          </p>
        </div>
      </header>

      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-7">
          <p className="orbita-kicker">Как это работает</p>
          <h3>После регистрации ребенок сразу попадает в живой первый экран</h3>
          <div className="orbita-list" style={{ marginTop: "16px" }}>
            <div className="orbita-item">
              <strong>1. Выбери роль</strong>
              <p>Профиль для ребенка или взрослого-наставника.</p>
            </div>
            <div className="orbita-item">
              <strong>2. Укажи возраст</strong>
              <p>Задания и тон объяснений подстроятся под уровень.</p>
            </div>
            <div className="orbita-item">
              <strong>3. Выбери помощника</strong>
              <p>Робот или зверёк будут сопровождать в уроках.</p>
            </div>
            <div className="orbita-item">
              <strong>4. Начни путь</strong>
              <p>
                Открывается главная с миссией дня и первым модулем.
              </p>
            </div>
          </div>
        </article>

        <article className="orbita-panel orbita-span-5">
          <p className="orbita-kicker">Шаги регистрации</p>
          <h3>Роль, имя, возраст и пароль</h3>
          <form className="orbita-form">
            <div className="orbita-field">
              <label htmlFor="register-name">Имя</label>
              <input
                className="orbita-input"
                id="register-name"
                name="register-name"
                type="text"
                placeholder="Дима"
                autoComplete="given-name"
              />
            </div>
            <div className="orbita-field">
              <label htmlFor="register-email">Почта родителя</label>
              <input
                className="orbita-input"
                id="register-email"
                name="register-email"
                type="email"
                placeholder="family@orbitakids.ru"
                autoComplete="email"
              />
            </div>
            <div className="orbita-field">
              <label htmlFor="register-password">Пароль</label>
              <input
                className="orbita-input"
                id="register-password"
                name="register-password"
                type="password"
                placeholder="Придумайте пароль"
                autoComplete="new-password"
              />
            </div>
          </form>
          <div className="orbita-action-row" style={{ marginTop: "18px" }}>
            <Link to="/" className="orbita-button orbita-button--primary">
              Зарегистрироваться
            </Link>
          </div>
          <p style={{ marginTop: "18px", color: "#587497", fontSize: "14px" }}>
            Уже есть аккаунт?{" "}
            <Link to="/login" style={{ color: "#4b8dff", fontWeight: 800 }}>
              Войти
            </Link>
          </p>
        </article>
      </section>
    </AppLayout>
  );
}
