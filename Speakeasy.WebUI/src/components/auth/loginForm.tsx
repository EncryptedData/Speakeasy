import { Component, createSignal } from "solid-js";

import { postLogin, postRegister } from "@api";
import { TextField } from "@components/input/textField";
import { useAuthContext } from "@context/authContext";
import { Label } from "@components/input/label";
import { Button } from "@components/input/button";

export type LoginFormProps = {
  mode?: "login" | "register";
  onAuthComplete?: () => void;
};

export const LoginForm: Component<LoginFormProps> = (props) => {
  const { updateAuth } = useAuthContext();

  const [mode, setMode] = createSignal<LoginFormProps["mode"]>(props.mode ?? "login");

  // TODO: Choose form library, if we want
  const [email, setEmail] = createSignal("");
  const [password, setPassword] = createSignal("");

  return (
    <form
      class="flex flex-col justify-center gap-2"
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
          updateAuth(loginResponse.data);
          props.onAuthComplete?.();
        }
      }}
    >
      <Label>
        Email
        <TextField
          name="email"
          onChange={(e) => setEmail(e.currentTarget.value)}
        />
      </Label>
      <Label>
        Password
        <TextField
          name="password"
          type="password"
          onChange={(e) => setPassword(e.currentTarget.value)}
        />
      </Label>
      <Button class="mt-2" type="submit">
        {mode() === "login" ? "Login" : "Register"}
      </Button>
      <Button
        onClick={() =>
          setMode((current) => (current === "login" ? "register" : "login"))
        }
        type="button"
      >
        {mode() === "login" ? "Register an account" : "I have an account"}
      </Button>
    </form>
  );
};
