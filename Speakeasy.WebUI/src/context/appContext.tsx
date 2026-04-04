import {
  Accessor,
  createContext,
  createEffect,
  ParentComponent,
  useContext,
} from "solid-js";
import { createStore, SetStoreFunction } from "solid-js/store";

import { useAuthContext } from "./authContext";
import {
  ChannelDto,
  getApiV1Group,
  getApiV1GroupByIdChannels,
  GroupDto,
} from "@api";

export type GroupDictionary = Record<string, GroupDto>;

export type AppContext = {
  /**
   * GroupId -> Channel
   */
  channels: Accessor<Record<string, ChannelDto[]>>;

  groups: GroupDictionary;

  loadGroups: () => Promise<void>;

  updateGroups: SetStoreFunction<GroupDictionary>;
};

export const AppContext = createContext<AppContext>({
  groups: {},
  channels: () => ({}),
  loadGroups: () => Promise.resolve(),
  updateGroups: () => ({}),
} satisfies AppContext);

export const AppContextProvider: ParentComponent = (props) => {
  const { isLoggedIn } = useAuthContext();

  const channelsLoading: Record<string, boolean> = {};
  const [channelStore, updateChannelStore] = createStore<
    Record<string, ChannelDto[]>
  >({});
  const [groups, setGroups] = createStore<GroupDictionary>({});

  async function loadGroups() {
    const response = await getApiV1Group();
    if (response.error || !response.data) {
      // TODO: Error handling
      console.error(response.error);
      return;
    }

    setGroups(
      response.data.reduce<GroupDictionary>((agg, cur) => {
        if (cur.id) {
          agg[cur.id] = cur;
        }

        return agg;
      }, {}),
    );
  }

  // Load groups on login. Clear state on logout
  createEffect(async () => {
    if (!isLoggedIn()) {
      setGroups({});
      updateChannelStore({});
      return;
    }

    await loadGroups();
  });

  // Eagerly load channels
  createEffect(async () => {
    const allGroups = Object.keys(groups);
    if (!allGroups?.length) {
      return;
    } else if (!isLoggedIn()) {
      return;
    }

    // Preload channels for the group one at a time. User can specifically request one if they navigate there
    for (const groupId of allGroups) {
      if (channelsLoading[groupId]) {
        continue;
      }

      channelsLoading[groupId] = true;
      try {
        const response = await getApiV1GroupByIdChannels({
          path: { id: groupId },
        });

        if (response.error || !response.data) {
          // TODO: Error handling. Nothing in this case? This is eager loading
          console.error(response.error);
          continue;
        }

        updateChannelStore(groupId, response.data);
      } finally {
        channelsLoading[groupId] = false;
      }
    }
  });

  const value: AppContext = {
    groups: groups,
    channels: () => channelStore,
    loadGroups,
    updateGroups: setGroups,
  };

  return (
    <AppContext.Provider value={value}>{props.children}</AppContext.Provider>
  );
};

export function useAppContext() {
  return useContext(AppContext);
}
