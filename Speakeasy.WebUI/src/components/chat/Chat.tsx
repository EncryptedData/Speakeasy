import { Component, createSignal, onMount } from "solid-js";
import { createStore } from "solid-js/store";
import { VList } from "virtua/solid";
import { ChatProfile } from "./ChatProfile";
import { User } from "../../models/User";

const STRINGS = [
  "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
  "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
  "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
  "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
];

const HEIGHTS = [50, 25, 100, 75];

export type ChatMessage = {
  userId: string;
  message: string;
};

const [users] = createSignal<Record<string, User>>({
  ["1"]: { userId: "1", profilePicture: "", username: "User1" },
  ["2"]: { userId: "2", profilePicture: "", username: "User2" },
  ["3"]: { userId: "3", profilePicture: "", username: "User3" },
  ["4"]: { userId: "4", profilePicture: "", username: "User4" },
});

async function loadChats() {
  const min = 0;
  const max = 3;

  await new Promise((r) => setTimeout(r, 1000));

  const outChatMessages: ChatMessage[] = [];
  for (let i = 0; i < 100; i++) {
    const userIndex = Math.floor(Math.random() * (max - min + 1)) + min;
    const strIndex = Math.floor(Math.random() * (max - min + 1)) + min;

    outChatMessages.push({
      userId: (userIndex + 1).toString(),
      message: STRINGS[strIndex],
    });
  }

  return outChatMessages;
}

export type ChatProps = {};

export const Chat: Component<ChatProps> = (props) => {
  const [chats, setChats] = createStore<ChatMessage[]>([]);
  onMount(async () => {
    const chats = await loadChats();
    setChats(chats);
  });

  const [message, setMessage] = createSignal("");
  let loadingChats = false;

  return (
    <div class="p-4 flex flex-col flex-1">
      <div class="flex-1">
        <VList
          class="p-4"
          data={chats}
          shift
          onScroll={async (offset) => {
            if (offset < 400) {
              if (loadingChats) {
                return;
              }
              console.log("loading more");

              loadingChats = true;
              const moreChats = await loadChats();
              setChats((c) => [...moreChats, ...c]);

              // Stop us from infinitely loading a ton of chats as we onScroll
              // TODO: Once we have better pagination, we will need to update this logic
              setTimeout(() => (loadingChats = false), 1000);
            }
          }}
        >
          {(data, index) => {
            return (
              <div class="pb-1 flex gap-4">
                <ChatProfile user={users()[data.userId]} />
                <div>{data.message}</div>
              </div>
            );
          }}
        </VList>
      </div>
      <form
        onSubmit={(e) => {
          e.preventDefault();
          setChats((c) => [...c, { userId: "1", message: message() }]);
          setMessage("");
        }}
      >
        <label>
          Message
          <input
            onChange={(e) => setMessage(e.currentTarget.value)}
            value={message()}
          />
        </label>
        <button type="submit">send</button>
      </form>
    </div>
  );
};
