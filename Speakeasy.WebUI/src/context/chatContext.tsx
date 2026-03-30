import {
  Accessor,
  createContext,
  createEffect,
  onMount,
  type ParentComponent,
  useContext,
} from "solid-js";
import { createStore, type SetStoreFunction } from "solid-js/store";

import type { ChatMessage } from "@models/ChatMessage";
import { useAuthContext } from "./authContext";
import { getApiV1ChannelByIdMessages } from "@api";

const DEFAULT_TAKE = 75;

export type ChatContext = {
  /**
   * Accessor for ChannelId -> Messages
   */
  messages: () => Record<string, ChatMessage[]>;
  loadingState: () => Record<string, boolean>;
  loadMessages: (channelId: string) => Promise<void>;
  updateMessages: SetStoreFunction<ReturnType<ChatContext["messages"]>>;
};

export const ChatContext = createContext<ChatContext>({
  messages: () => ({}),
  loadingState: () => ({}),
  loadMessages: () => Promise.resolve(),
  updateMessages: () => {},
});

type ContinuationStateValue = { continuationToken: string };
type ContinuationState = Record<string, ContinuationStateValue>;

export const ChatProvider: ParentComponent = (props) => {
  const { isLoggedIn } = useAuthContext();

  const [continuationStore, updateContinuationStore] =
    createStore<ContinuationState>({});
  const [chatState, updateChatState] = createStore<
    Record<string, ChatMessage[]>
  >({});
  const [loadingState, updateLoadingState] = createStore<
    Record<string, boolean>
  >({});

  createEffect(() => {
    if (!isLoggedIn()) {
      updateContinuationStore({});
      updateChatState({});
    }
  });

  const value: ChatContext = {
    loadingState: () => loadingState,

    // TODO: We should probably handle the case where the user navigates off of a channel then navigates back
    // In this case, we probably want to re-retrieve messages they haven't seen
    // Maybe we just let signalr handle this but idk
    loadMessages: async (channelId: string) => {
      if (loadingState[channelId]) {
        return;
      }

      updateLoadingState(channelId, true);

      const continuationValue = continuationStore[channelId];
      if (!!continuationValue && !continuationValue.continuationToken) {
        return;
      }

      try {
        const response = await getApiV1ChannelByIdMessages({
          query: {
            LastMessageId: continuationValue?.continuationToken,
            Take: DEFAULT_TAKE,
          },
          path: {
            id: channelId,
          },
        });

        if (response.error) {
          // TODO: Error toast w/ retry?
          console.error(response.error);
        }

        const data = (response.data || []).reverse();

        updateContinuationStore(channelId, {
          continuationToken:
            data.length !== DEFAULT_TAKE ? undefined : (data[0]?.id ?? ""),
        });

        updateChatState(channelId, (messages) => [
          ...data.map(
            (d) =>
              ({
                id: d.id || "",
                author: d.authorId || "",
                createdOn: new Date(d.createdOn!),
                currentText: d.currentText,
              }) satisfies ChatMessage,
          ),
          ...(messages || []),
        ]);
      } finally {
        updateLoadingState(channelId, false);
      }
    },
    messages: () => chatState,
    updateMessages: updateChatState,
  };

  return (
    <ChatContext.Provider value={value}>{props.children}</ChatContext.Provider>
  );
};

export function useChatContext() {
  return useContext(ChatContext);
}

export function useChatContextForChannel(channelId: Accessor<string>) {
  const chatContext = useChatContext();

  createEffect(() => {
    chatContext.loadMessages(channelId());
  });

  return {
    messages: () => chatContext.messages()[channelId()] || [],
    isLoading: () => chatContext.loadingState()[channelId()] || true,
    loadMessages: () => chatContext.loadMessages(channelId()),
  };
}
