import { useNavigate } from "@solidjs/router";
import { createEffect } from "solid-js";

import { LoginForm } from "./loginForm";
import { useAuthContext } from "@context/authContext";

export const LoginPage = () => {
  const navigate = useNavigate();
  const authContext = useAuthContext();
  createEffect(() => {
    if (authContext.isLoggedIn()) {
      navigate("/");
    }
  });

  return (
    <div class="login-page flex flex-1 justify-center align-middle">
      <LoginForm onAuthComplete={() => navigate("/")} />
    </div>
  );
};

export default LoginPage;
