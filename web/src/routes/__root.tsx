import { createRootRoute, Link, Outlet } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools";
import { AppLayout } from "../components/orbita/AppLayout";

export const RootLayout = () => {
  return (
    <>
      <Outlet />
      <TanStackRouterDevtools />
    </>
  );
};

function NotFound() {
  return (
    <AppLayout pageTitle="404" activeNav="/">
      <header className="orbita-header">
        <div className="orbita-header__title">
          <p className="orbita-kicker">Упс</p>
          <h2>404 — Страница улетела с орбиты</h2>
          <p className="orbita-copy">
            Такой страницы нет. Проверь адрес или вернись на главную.
          </p>
        </div>
      </header>
      <section className="orbita-grid">
        <article className="orbita-panel orbita-span-12" style={{ textAlign: "center" }}>
          <p className="orbita-kicker">Что дальше</p>
          <h3>Вернись на главную или выбери раздел</h3>
          <div className="orbita-action-row" style={{ marginTop: "20px", justifyContent: "center" }}>
            <Link to="/" className="orbita-button orbita-button--primary">
              На главную
            </Link>
            <Link to="/modules" className="orbita-button orbita-button--secondary">
              Модули
            </Link>
            <Link to="/map" className="orbita-button orbita-button--secondary">
              Карта
            </Link>
          </div>
        </article>
      </section>
    </AppLayout>
  );
}

export const Route = createRootRoute({
  component: RootLayout,
  notFoundComponent: NotFound,
});
