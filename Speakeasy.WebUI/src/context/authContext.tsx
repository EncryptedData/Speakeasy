import {
  createContext,
  createEffect,
  createMemo,
  createResource,
  onCleanup,
  ParentComponent,
  useContext,
} from "solid-js";
import { createStore } from "solid-js/store";
import { makePersisted } from "@solid-primitives/storage";

import { AccessTokenResponse, getManageInfo, postRefresh } from "@api";
import { client } from "../api/client.gen";

export type StoredCredentials = {
  accessToken: string;
  refreshToken: string;
  expirationMs: number;
};

export type AuthContext = {
  accessToken: () => string;
  authLoading: () => boolean;
  email: () => string | undefined;
  logout: () => void;
  isLoggedIn: () => boolean;
  updateAuth: (newAuth: AccessTokenResponse) => void;
};

const AuthContext = createContext<AuthContext>({
  accessToken: () => "",
  authLoading: () => true,
  email: () => "",
  logout: () => {},
  isLoggedIn: () => false,
  updateAuth: () => {},
});

export const AuthProvider: ParentComponent = (props) => {
  const [authStore, setAuthStore] = makePersisted(
    createStore<StoredCredentials>({
      accessToken: "",
      refreshToken: "",
      expirationMs: 0,
    }),
  );

  // Automatically refresh the token if we can
  createEffect(() => {
    const expirationMs = authStore.expirationMs;
    const token = authStore.refreshToken;

    if (!token) return;

    const msUntilExpiry = expirationMs - Date.now();

    const timeout = setTimeout(async () => {
      const refreshResponse = await postRefresh({
        body: {
          refreshToken: token,
        },
      });

      if (refreshResponse.error) {
        setAuthStore({
          accessToken: "",
          refreshToken: "",
          expirationMs: 0,
        });
      } else if (refreshResponse.data) {
        setAuthStore({
          accessToken: refreshResponse.data.accessToken,
          refreshToken: refreshResponse.data.refreshToken,
          expirationMs: Date.now() + Number(refreshResponse.data.expiresIn) * 1000,
        });
      }
    // Refresh 60s before expiry. If msUntilExpiry < 60_000 (token already close to
    // or past expiry), the delay is negative — browsers clamp this to 0 and fire
    // immediately, which is the correct behavior.
    }, msUntilExpiry - 60_000);

    onCleanup(() => clearTimeout(timeout));
  });

  // Automatically retrieve user info (/manage/info) as our token changes
  const manageInfoInput = createMemo(() =>
    !!authStore.accessToken
      ? ({
          auth: () => authStore.accessToken,
        } satisfies Parameters<typeof getManageInfo>[0])
      : null,
  );
  const [info, { mutate }] = createResource(manageInfoInput, getManageInfo);

  const logout = () => {
    // Remove credentials
    setAuthStore({
      accessToken: "",
      refreshToken: "",
      expirationMs: 0,
    });

    // Clear /manage/info user data
    mutate(undefined);

    // Double check we cleared stored local data
    localStorage.clear();
  };

  createEffect(() => {
    if (!authStore.accessToken) {
      mutate(undefined);
    }
  });

  const value: AuthContext = {
    accessToken: () => authStore.accessToken,
    email: () => info()?.data?.email,
    isLoggedIn: () => !!info()?.data?.email,
    logout: logout,
    authLoading: () => {
      if (!authStore.accessToken) {
        return false;
      }

      if (info.state === "ready") {
        return false;
      }

      return true;
    },
    updateAuth: (response: AccessTokenResponse) =>
      setAuthStore({
        accessToken: response.accessToken,
        refreshToken: response.refreshToken,
        expirationMs: Date.now() + Number(response.expiresIn) * 1000,
      }),
  };

  // Configure our api client to grab our access token
  client.setConfig({
    auth: () => value.accessToken(),
  });

  return (
    <AuthContext.Provider value={value}>{props.children}</AuthContext.Provider>
  );
};

export function useAuthContext() {
  return useContext(AuthContext);
}
