import {
  createContext,
  createMemo,
  createResource,
  ParentComponent,
  useContext,
} from "solid-js";
import { createStore } from "solid-js/store";
import { makePersisted } from "@solid-primitives/storage";
import { getManageInfo } from "@api";

export type AuthContext = {
  accessToken: () => string;
  authLoading: () => boolean;
  email: () => string | undefined;
  isLoggedIn: () => boolean;
  refreshToken: () => string;
  updateAuth: (newAuth: { accessToken: string; refreshToken: string }) => void;
};

const AuthContext = createContext<AuthContext>({
  accessToken: () => "",
  authLoading: () => true,
  email: () => "",
  isLoggedIn: () => false,
  refreshToken: () => "",
  updateAuth: () => {},
});

export const AuthProvider: ParentComponent = (props) => {
  const [authStore, setAuthStore] = makePersisted(
    createStore({
      accessToken: "",
      refreshToken: "",
    }),
  );

  const manageInfoInput = createMemo(() =>
    !!authStore.accessToken
      ? ({
          headers: {
            Authorization: `Bearer ${authStore.accessToken}`,
          },
        } satisfies Parameters<typeof getManageInfo>[0])
      : null,
  );
  const [{ latest, state }] = createResource(manageInfoInput, getManageInfo);

  const value: AuthContext = {
    accessToken: () => authStore.accessToken,
    email: () => latest?.data?.email,
    isLoggedIn: () => !!latest?.data?.email,
    authLoading: () => state !== "unresolved" && state !== "ready",
    refreshToken: () => authStore.refreshToken,
    updateAuth: ({ accessToken, refreshToken }) =>
      setAuthStore({
        accessToken: accessToken,
        refreshToken: refreshToken,
      }),
  };

  return (
    <AuthContext.Provider value={value}>{props.children}</AuthContext.Provider>
  );
};

export function useAuthContext() {
  return useContext(AuthContext);
}
