import {
  Accessor,
  createContext,
  createEffect,
  createSignal,
  onCleanup,
  ParentComponent,
  useContext,
} from "solid-js";
import { createStore } from "solid-js/store";
import { makePersisted } from "@solid-primitives/storage";

import {
  AccessTokenResponse,
  getApiV1AuthManageInfo,
  getApiV1UserMe,
  postApiV1AuthRefresh,
  UserDto,
} from "@api";
import { client } from "../api/client.gen";

export type StoredCredentials = {
  accessToken: string;
  refreshToken: string;
  expirationMs: number;
};

type AuthState = "loading" | "authenticated" | "unauthenticated";

export type AuthContext = {
  authState: () => AuthState;
  me: Accessor<
    UserDto & {
      email: string;
    }
  >;
  isLoggedIn: () => boolean;
  logout: () => void;
  updateAuth: (newAuth: AccessTokenResponse) => void;
};

const AuthContext = createContext<AuthContext>({
  authState: () => "loading" as AuthState,
  me: () => ({
    email: "",
  }),
  isLoggedIn: () => false,
  logout: () => {},
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

  const [authState, setAuthState] = createSignal<AuthState>("loading");
  const [me, setMe] = createSignal<ReturnType<AuthContext["me"]>>({
    email: "",
  });

  const clearAuth = () => {
    setAuthStore({ accessToken: "", refreshToken: "", expirationMs: 0 });
    setMe({
      email: "",
    });
    setAuthState("unauthenticated");
    localStorage.clear();
  };

  const applyTokens = (response: AccessTokenResponse) => {
    setAuthStore({
      accessToken: response.accessToken,
      refreshToken: response.refreshToken,
      expirationMs: Date.now() + Number(response.expiresIn) * 1000,
    });
  };

  createEffect(() => {
    const token = authStore.accessToken;
    const refreshToken = authStore.refreshToken;
    const expirationMs = authStore.expirationMs;

    if (!token) {
      setMe({
        email: "",
      });
      setAuthState("unauthenticated");
      return;
    }

    setAuthState("loading");

    validateSession(token, refreshToken, expirationMs);
  });

  async function validateSession(
    token: string,
    refreshToken: string,
    expirationMs: number,
  ) {
    // If expired, try to refresh before validating
    if (expirationMs <= Date.now()) {
      if (!refreshToken) {
        clearAuth();
        return;
      }

      const refreshResult = await postApiV1AuthRefresh({
        body: { refreshToken },
      });

      if (refreshResult.error || !refreshResult.data) {
        clearAuth();
        return;
      }

      // Store update re-triggers the effect with the fresh token,
      // which will then proceed to /manage/info
      applyTokens(refreshResult.data);
      return;
    }

    // Token is valid — verify the session with the server
    const [info, me] = await Promise.all([
      getApiV1AuthManageInfo({ auth: () => token }),
      getApiV1UserMe({ auth: () => token }),
    ]);

    if (info.data?.email && me.data?.id) {
      setMe({
        ...me.data,
        email: info.data.email,
      });
      setAuthState("authenticated");
    } else {
      clearAuth();
    }
  }

  // Schedule a proactive refresh before the token expires
  createEffect(() => {
    const expirationMs = authStore.expirationMs;
    const refreshToken = authStore.refreshToken;

    if (!refreshToken || !expirationMs) return;

    const msUntilRefresh = expirationMs - Date.now() - 60_000;

    // Don't schedule if token is already expired — the validation
    // effect above handles that case immediately
    if (msUntilRefresh <= 0) return;

    const timeout = setTimeout(async () => {
      const result = await postApiV1AuthRefresh({
        body: { refreshToken },
      });

      if (result.data) {
        applyTokens(result.data);
      } else {
        clearAuth();
      }
    }, msUntilRefresh);

    onCleanup(() => clearTimeout(timeout));
  });

  // Configure our api client to use the current access token
  client.setConfig({
    auth: () => authStore.accessToken,
    baseUrl: import.meta.env.VITE_API_URL,
  });

  const value: AuthContext = {
    authState,
    me,
    isLoggedIn: () => authState() === "authenticated",
    logout: clearAuth,
    updateAuth: applyTokens,
  };

  return (
    <AuthContext.Provider value={value}>{props.children}</AuthContext.Provider>
  );
};

export function useAuthContext() {
  return useContext(AuthContext);
}
