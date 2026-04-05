import { ParentComponent } from "solid-js";

import { UserContextProvider } from "./userContext";
import { AppContextProvider } from "./appContext";
import { ChatProvider } from "./chatContext";
import { AuthProvider } from "./authContext";

export const Providers: ParentComponent = (props) => {
  return (
    <AuthProvider>
      <ChatProvider>
        <AppContextProvider>
          <UserContextProvider>{props.children}</UserContextProvider>
        </AppContextProvider>
      </ChatProvider>
    </AuthProvider>
  );
};
