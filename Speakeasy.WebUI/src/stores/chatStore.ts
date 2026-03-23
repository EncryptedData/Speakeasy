import { Accessor, createEffect, createSignal } from "solid-js";
import { createStore, produce } from "solid-js/store";

import { ChatMessage } from "../models/ChatMessage";

/// TEMP NONSENSE
const STRINGS = [
  "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
  "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
  "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
  "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
];

async function loadChats(
  channelId: string,
  continuationToken: string | undefined,
) {
  const min = 0;
  const max = 3;

  await new Promise((r) => setTimeout(r, 1000));

  const outChatMessages: ChatMessage[] = [];
  for (let i = 0; i < 100; i++) {
    const userIndex = Math.floor(Math.random() * (max - min + 1)) + min;
    const strIndex = Math.floor(Math.random() * (max - min + 1)) + min;

    outChatMessages.push({
      id: Date.now().toString(),
      author: (userIndex + 1).toString(),
      currentText: STRINGS[strIndex],
      isPending: false,
      createdOn: new Date(),
    });
  }

  return outChatMessages;
}
/////

export type ChatStore = {
  /**
   * ChannelId -> Messages
   */
  messages: Record<string, ChatMessage[]>;
};

/**
 * ChannelId -> Message ContinuationToken
 */
const continuationTokens: Record<string, string> = {};

export type ChatStoreOps = {
  isLoading: () => boolean;

  /** Prepends the next page of messages to the channel's message list. */
  loadNextPage: () => Promise<void>;

  /**
   * Appends an optimistic message to the channel, then confirms it after the send completes.
   * The message is marked as pending until the send resolves.
   */
  sendMessage: (message: string) => Promise<void>;
};

// TODO: Maybe better suited as context at a higher level so that it survives hot reload
const [chatStore, updateChatStore] = createStore<ChatStore>({
  messages: {},
});

/**
 * Returns a reactive accessor for the messages in a channel, and operations for interacting with them.
 * Lazily loads the first page of messages when first called for a given channel.
 */
export function useChatStore(
  channelId: string,
): [Accessor<ChatMessage[]>, ChatStoreOps] {
  const [isLoading, setIsLoading] = createSignal(false);

  // Load the first page of messages if they haven't been loaded yet.
  createEffect(async () => {
    if (!!chatStore.messages[channelId]) {
      return;
    }

    setIsLoading(true);
    try {
      const chats = await loadChats(channelId, undefined);
      updateChatStore("messages", channelId, chats);
      continuationTokens[channelId] = chats[0]?.id || "";
    } finally {
      setIsLoading(false);
    }
  });

  return [
    () => chatStore.messages[channelId] || [],
    {
      isLoading: isLoading,
      loadNextPage: async () => {
        if (isLoading()) {
          return;
        }

        const continuationToken = continuationTokens[channelId];
        if (!continuationToken) {
          return;
        }

        setIsLoading(true);
        try {
          const newMessages = await loadChats(channelId, continuationToken);
          updateChatStore("messages", channelId, (messages) => [
            ...newMessages,
            ...messages,
          ]);
          continuationTokens[channelId] =
            newMessages[0]?.id || continuationTokens[channelId];
        } finally {
          setIsLoading(false);
        }
      },
      sendMessage: async (message: string) => {
        const tempId = Date.now().toString();

        updateChatStore("messages", channelId, (messages) => [
          ...messages,
          {
            id: tempId,
            author: "me",
            createdOn: new Date(),
            currentText: message,
            isPending: true,
            lastEditedOn: undefined,
          },
        ]);

        let didSendFail = false;
        try {
          await new Promise((r) => setTimeout(r, 1500));
        } catch {
          didSendFail = true;
        }

        updateChatStore(
          "messages",
          channelId,
          produce((innerMessages) => {
            const matchingMessage = innerMessages.find((m) => m.id === tempId);
            if (matchingMessage) {
              matchingMessage.isPending = false;
              matchingMessage.didSendFail = didSendFail;
            }
          }),
        );
      },
    },
  ];
}
