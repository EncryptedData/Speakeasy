import {
  Accessor,
  createContext,
  createEffect,
  onMount,
  type ParentComponent,
  useContext,
} from "solid-js";
import { createStore, produce, type SetStoreFunction } from "solid-js/store";

import type { ChatMessage } from "@models/ChatMessage";
import { useAuthContext } from "./authContext";
import {
  getApiV1ChannelByIdMessages,
  MessageDto,
  postApiV1ChannelByIdMessage,
} from "@api";
import { parseDateFromUuid } from "@utilities/uuid";

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
} satisfies ChatContext);

type ContinuationStateValue = { continuationToken: string };
type ContinuationState = Record<string, ContinuationStateValue>;

export const ChatProvider: ParentComponent = (props) => {
  const authContext = useAuthContext();

  const [continuationStore, updateContinuationStore] =
    createStore<ContinuationState>({});
  const [chatState, updateChatState] = createStore<
    Record<string, ChatMessage[]>
  >({});
  const [loadingState, updateLoadingState] = createStore<
    Record<string, boolean>
  >({});

  createEffect(() => {
    if (!authContext.isLoggedIn()) {
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
                createdOn: parseDateFromUuid(d.id!),
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
  const authContext = useAuthContext();

  createEffect(() => {
    if (!channelId()) {
      return;
    }

    chatContext.loadMessages(channelId());
  });

  return {
    messages: () => chatContext.messages()[channelId()] || [],
    isLoading: () => chatContext.loadingState()[channelId()] || true,
    loadMessages: () => chatContext.loadMessages(channelId()),
    sendMessage: async (text: string) => {
      const tempId = Date.now().toString();

      chatContext.updateMessages(channelId(), (innerMessages) => [
        ...innerMessages,
        {
          id: tempId,
          author: authContext.me().id!,
          createdOn: new Date(),
          currentText: text,
          isPending: true,
          lastEditedOn: undefined,
        },
      ]);

      let createdMessage: MessageDto | undefined;
      let didSendFail = false;
      try {
        const sendResponse = await postApiV1ChannelByIdMessage({
          body: text,
          path: { id: channelId() },
        });

        if (sendResponse.error || !sendResponse.data) {
          // TODO: Toast or something for error
          console.error(sendResponse.error);
        }

        createdMessage = sendResponse.data;
      } catch {
        didSendFail = true;
      }

      chatContext.updateMessages(
        channelId(),
        produce((innerMessages) => {
          const matchingMessage = innerMessages.find((m) => m.id === tempId);
          if (matchingMessage) {
            matchingMessage.id = createdMessage?.id ?? matchingMessage.id;
            matchingMessage.isPending = false;
            matchingMessage.didSendFail = didSendFail;
            matchingMessage.createdOn = parseDateFromUuid(createdMessage?.id!);
          }
        }),
      );
    },
  };
}
