import { Accessor, createEffect, createMemo, createSignal } from "solid-js";
import { createStore, produce } from "solid-js/store";

export type ChatMessage = {
  id: string;
  author: string;
  createdOn: Date;
  currentText: string;
  isPending?: boolean;
  lastEditedOn?: Date;
};

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
  loadNextPage: () => Promise<void>;
  sendMessage: (message: string) => Promise<void>;
};

const [chatStore, updateChatStore] = createStore<ChatStore>({
  messages: {},
});

export function useChatStore(
  channelId: string,
): [Accessor<ChatMessage[]>, ChatStoreOps] {
  const messages = createMemo(() => chatStore.messages[channelId] || []);
  const [isLoading, setIsLoading] = createSignal(false);

  createEffect(async () => {
    if (!!chatStore.messages[channelId]) {
      return;
    }

    setIsLoading(true);
    try {
      const chats = await loadChats(channelId, undefined);
      console.log("loaded chats", channelId, chats);
      updateChatStore("messages", channelId, chats);
      continuationTokens[channelId] = chats[0]?.id || "";
    } finally {
      setIsLoading(false);
    }
  });

  return [
    messages,
    {
      isLoading: isLoading,
      loadNextPage: async () => {
        if (isLoading()) {
          return;
        }

        const continuationToken = continuationTokens[channelId];

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

        await new Promise((r) => setTimeout(r, 1500));

        updateChatStore(
          "messages",
          channelId,
          produce((innerMessages) => {
            const matchingMessage = innerMessages.find((m) => m.id === tempId);
            if (matchingMessage) {
              matchingMessage.isPending = false;
            }
          }),
        );
      },
    },
  ];
}
