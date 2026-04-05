import { createEffect } from "solid-js";

import { useUserContext } from "@context/userContext";

export function useUserState(userId: string) {
  const userContext = useUserContext();

  createEffect(() => {
    if (!userContext.users[userId]) {
      userContext.fetchUser(userId);
    }
  });

  return [
    () => userContext.users[userId]?.user || {},
    () => userContext.users[userId]?.loading ?? true,
  ] as const;
}
