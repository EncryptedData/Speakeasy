import { Component, createEffect, createMemo, createSignal } from "solid-js";
import {
  createForm,
  required,
  email,
  reset,
  setError,
} from "@modular-forms/solid";

import { postApiV1AuthLogin, postApiV1AuthRegister } from "@api";
import { useAuthContext } from "@context/authContext";
import { Button } from "@components/input/button";
import { TextField } from "@components/fields/textField";
import { ValidationError } from "@models/validationError";
import { getErrorMessages } from "@utilities/error";

export type LoginFormProps = {
  mode?: "login" | "register";
  onAuthComplete?: () => void;
};

type LoginFormState = {
  username: string;
  password: string;
};

export const LoginForm: Component<LoginFormProps> = (props) => {
  const { updateAuth } = useAuthContext();

  const [mode, setMode] = createSignal<LoginFormProps["mode"]>(
    props.mode ?? "login",
  );

  const [loginForm, { Form, Field }] = createForm<LoginFormState>();

  // When we change modes, clear errors
  createEffect(() => {
    mode();
    reset(loginForm, {
      keepValues: true,
    });
  });

  return (
    <Form
      class="flex flex-col justify-center gap-2"
      onSubmit={async (state) => {
        if (mode() === "register") {
          const registerResponse = await postApiV1AuthRegister({
            body: {
              email: state.username,
              password: state.password,
            },
          });

          if (registerResponse.error) {
            const typed = registerResponse.error as ValidationError;
            const errorMessages = getErrorMessages(typed, [
              "password",
              "username",
            ]);

            for (const [fieldName, error] of Object.entries(errorMessages)) {
              setError(loginForm, fieldName as keyof LoginFormState, error);
            }
            return;
          }
        }

        const loginResponse = await postApiV1AuthLogin({
          body: {
            email: state.username,
            password: state.password,
          },
        });

        if (loginResponse.error) {
          setError(loginForm, "password", "Invalid credentials.");
        } else if (loginResponse.data) {
          updateAuth(loginResponse.data);
          props.onAuthComplete?.();
        }
      }}
    >
      <Field
        name="username"
        validate={[
          required("Please enter your email."),
          email("Please enter a valid email address."),
        ]}
      >
        {(field, props) => (
          <TextField
            {...props}
            type="email"
            label={"Email"}
            value={field.value}
            error={field.error}
            required
          />
        )}
      </Field>
      <Field name="password" validate={required("Please enter a password.")}>
        {(field, props) => (
          <TextField
            {...props}
            type="password"
            label={"Password"}
            value={field.value}
            error={field.error}
            required
          />
        )}
      </Field>
      <Button class="mt-2" type="submit" disabled={loginForm.submitting}>
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
    </Form>
  );
};
