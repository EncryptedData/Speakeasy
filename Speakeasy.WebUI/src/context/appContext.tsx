import {
  Accessor,
  createContext,
  createEffect,
  createResource,
  createSignal,
  ParentComponent,
  useContext,
} from "solid-js";
import { createStore } from "solid-js/store";

import { useAuthContext } from "./authContext";
import {
  ChannelDto,
  getApiV1Group,
  getApiV1GroupByIdChannels,
  GroupDto,
} from "@api";

export type AppContext = {
  groups: Accessor<GroupDto[]>;

  /**
   * GroupId -> Channel
   */
  channels: Accessor<Record<string, ChannelDto[]>>;
};

export const AppContext = createContext<AppContext>({
  groups: () => [],
  channels: () => ({}),
} satisfies AppContext);

export const AppContextProvider: ParentComponent = (props) => {
  const { isLoggedIn } = useAuthContext();

  const channelsLoading: Record<string, boolean> = {};
  const [channelStore, updateChannelStore] = createStore<
    Record<string, ChannelDto[]>
  >({});
  const [groups, setGroups] = createSignal<GroupDto[]>([]);

  // Load groups on login. Clear state on logout
  createEffect(async () => {
    if (!isLoggedIn()) {
      setGroups([]);
      updateChannelStore({});
      return;
    }

    const response = await getApiV1Group();
    if (response.error || !response.data) {
      // TODO: Error handling
      console.error(response.error);
      return [];
    }

    setGroups(response.data);
  });

  // Eagerly load channels
  createEffect(async () => {
    const allGroups = groups();
    if (!allGroups?.length) {
      return;
    } else if (!isLoggedIn()) {
      return;
    }

    // Preload channels for the group one at a time. User can specifically request one if they navigate there
    for (const group of allGroups) {
      if (channelsLoading[group.id!]) {
        continue;
      }

      channelsLoading[group.id!] = true;
      try {
        const response = await getApiV1GroupByIdChannels({
          path: { id: group.id! },
        });

        if (response.error || !response.data) {
          // TODO: Error handling. Nothing in this case? This is eager loading
          console.error(response.error);
          continue;
        }

        updateChannelStore(group.id!, response.data);
      } finally {
        channelsLoading[group.id!] = false;
      }
    }
  });

  const value: AppContext = {
    groups: () => groups() || [],
    channels: () => channelStore,
  };

  return (
    <AppContext.Provider value={value}>{props.children}</AppContext.Provider>
  );
};

export function useAppContext() {
  return useContext(AppContext);
}
