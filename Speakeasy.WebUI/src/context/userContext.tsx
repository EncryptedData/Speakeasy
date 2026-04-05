import {
  batch,
  createContext,
  useContext,
  type ParentComponent,
} from "solid-js";
import { createStore } from "solid-js/store";

import { getApiV1UserById, type UserDto } from "@api";

/**
 * UserId -> { UserDto, isLoading }
 */
export type UsersDictionary = Record<
  string,
  {
    user: UserDto;
    loading: boolean;
  }
>;

/**
 * UserId -> IsLoading
 */
export type UserLoadingState = Record<string, boolean>;

export type UserContext = {
  fetchUser: (userId: string) => Promise<void>;
  users: UsersDictionary;
};

export const UserContext = createContext<UserContext>({
  fetchUser: () => Promise.resolve(),
  users: {},
} satisfies UserContext);

export const UserContextProvider: ParentComponent = (props) => {
  const [usersStore, updateUserStore] = createStore<UsersDictionary>({});

  const value: UserContext = {
    fetchUser: async (userId) => {
      if (usersStore[userId]) {
        return;
      }

      if (usersStore[userId]) {
        updateUserStore(userId, "loading", true);
      } else {
        updateUserStore(userId, {
          user: {},
          loading: true,
        });
      }

      try {
        const response = await getApiV1UserById({
          path: {
            id: userId,
          },
        });

        if (response.error || !response.data) {
          console.error(`failed to load user ${userId}`, response.error);
          return;
        }

        updateUserStore(userId, "user", response.data);
      } finally {
        updateUserStore(userId, "loading", false);
      }
    },
    users: usersStore,
  };

  return (
    <UserContext.Provider value={value}>{props.children}</UserContext.Provider>
  );
};

export function useUserContext() {
  return useContext(UserContext);
}
