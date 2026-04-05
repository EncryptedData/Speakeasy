import { createForm, required } from "@modular-forms/solid";

import { TextField } from "@components/fields/textField";
import { Button } from "@components/input/button";
import { useGroupState } from "@hooks/group";
import { Component } from "solid-js";

export type CreateGroupFormState = {
  name: string;
};

export type CreateGroupFormProps = {
  onSubmit?: () => void;
};

export const CreateGroupForm: Component<CreateGroupFormProps> = (props) => {
  const [loginForm, { Form, Field }] = createForm<CreateGroupFormState>();
  const groupState = useGroupState();

  return (
    <Form
      class="flex flex-col justify-center gap-6"
      onSubmit={async (values) => {
        await groupState.createGroup(values.name);
        props.onSubmit?.();
      }}
    >
      <Field name="name" validate={[required("Enter a group name")]}>
        {(field, props) => (
          <TextField
            {...props}
            type="text"
            label="Name"
            value={field.value}
            error={field.error}
            required
          />
        )}
      </Field>
      <Button type="submit">Create</Button>
    </Form>
  );
};
