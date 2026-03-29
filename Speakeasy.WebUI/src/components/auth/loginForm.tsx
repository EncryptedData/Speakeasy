import { Component, createSignal } from "solid-js";

import { postLogin, postRegister } from "@api";
import { TextField } from "@components/input/textField";
import { useAuthContext } from "@context/authContext";

export type LoginFormProps = {
  mode?: "login" | "register";
};

export const LoginForm: Component<LoginFormProps> = (props) => {
  const { updateAuth } = useAuthContext();

  const [mode, setMode] = createSignal<LoginFormProps["mode"]>("login");

  // TODO: Choose form library, if we want
  const [email, setEmail] = createSignal("");
  const [password, setPassword] = createSignal("");

  return (
    <form
      onSubmit={async (e) => {
        e.preventDefault();

        if (mode() === "register") {
          const registerResponse = await postRegister({
            body: {
              email: email(),
              password: password(),
            },
          });

          if (registerResponse.error) {
            // TODO: Toast or something for errors
            console.error(registerResponse.error);
            return;
          }
        }

        const loginResponse = await postLogin({
          body: {
            email: email(),
            password: password(),
          },
        });

        if (loginResponse.error) {
          // TODO: Toast or something for errors
          console.error(loginResponse.error);
        } else if (loginResponse.data) {
          updateAuth({
            accessToken: loginResponse.data.accessToken,
            refreshToken: loginResponse.data.refreshToken,
          });
        }
      }}
    >
      <label>
        Email
        <TextField
          name="email"
          onChange={(e) => setEmail(e.currentTarget.value)}
        />
      </label>
      <label>
        Password
        <TextField
          name="password"
          type="password"
          onChange={(e) => setPassword(e.currentTarget.value)}
        />
      </label>
      <button type="submit">{mode() === "login" ? "Login" : "Register"}</button>
      <button
        onClick={() =>
          setMode((current) => (current === "login" ? "register" : "login"))
        }
        type="button"
      >
        {mode() === "login" ? "Register an account" : "I have an account"}
      </button>
    </form>
  );
};
