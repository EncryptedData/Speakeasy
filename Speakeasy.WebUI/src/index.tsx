/* @refresh reload */
import type { RouteSectionProps } from "@solidjs/router";
import { Navigate, Route, Router } from "@solidjs/router";
import "solid-devtools";
import { lazy, Show } from "solid-js";
import { render } from "solid-js/web";
import "./index.css";

import { AuthProvider, useAuthContext } from "@context/authContext";
import { ChatProvider } from "@context/chatContext";

const root = document.getElementById("root");

if (import.meta.env.DEV && !(root instanceof HTMLElement)) {
  throw new Error(
    "Root element not found. Did you forget to add it to your index.html? Or maybe the id attribute got misspelled?",
  );
}

const App = lazy(() => import("./App"));
const LoginPage = lazy(() => import("@components/auth/loginPage"));

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
      <ChatProvider>
        <Router root={AuthLayout}>
          <Route path="/" component={App} />
          <Route path="/login" component={LoginPage} />
          <Route path="*" component={() => <Navigate href={"/"} />} />
        </Router>
      </ChatProvider>
    </AuthProvider>
  ),
  root!,
);
