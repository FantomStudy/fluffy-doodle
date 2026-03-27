const KEY = "auth_ok";

const setAuthenticated = () => sessionStorage.setItem(KEY, "1");

const clearAuthenticated = () => sessionStorage.removeItem(KEY);

const isAuthenticated = () => sessionStorage.getItem(KEY) === "1";

export { clearAuthenticated, isAuthenticated, setAuthenticated };
