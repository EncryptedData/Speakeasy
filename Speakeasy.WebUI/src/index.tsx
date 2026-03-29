/* @refresh reload */
import "./index.css";
import { render } from "solid-js/web";
import { Navigate, Route, Router } from "@solidjs/router";
import type { RouteSectionProps } from "@solidjs/router";
import { Show } from "solid-js";
import "solid-devtools";

import App from "./App";
import { AuthProvider } from "@context/authContext";
import { LoginPage } from "@components/auth/loginPage";
import { useAuthContext } from "@context/authContext";

const root = document.getElementById("root");

if (import.meta.env.DEV && !(root instanceof HTMLElement)) {
  throw new Error(
    "Root element not found. Did you forget to add it to your index.html? Or maybe the id attribute got misspelled?",
  );
}

const AuthLayout = (props: RouteSectionProps) => {
  const { authState } = useAuthContext();
  return (
    <Show
      when={authState() !== "loading"}
      fallback={
        <div class="flex flex-1 items-center justify-center">Loading...</div>
      }
    >
      {props.children}
    </Show>
  );
};

render(
  () => (
    <AuthProvider>
      <Router root={AuthLayout}>
        <Route path="/" component={App} />
        <Route path="/login" component={LoginPage} />
        <Route path="*" component={() => <Navigate href={"/"} />} />
      </Router>
    </AuthProvider>
  ),
  root!,
);
