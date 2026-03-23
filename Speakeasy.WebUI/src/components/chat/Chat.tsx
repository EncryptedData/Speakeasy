import { Component, createEffect, createSignal } from "solid-js";
import { VList } from "virtua/solid";

import { User } from "../../models/User";
import { useChatStore } from "../../stores/chatStore";
import { ChatProfile } from "./ChatProfile";

const [users] = createSignal<Record<string, User>>({
  ["me"]: { userId: "1", profilePicture: "", username: "User1" },
  ["1"]: { userId: "1", profilePicture: "", username: "User1" },
  ["2"]: { userId: "2", profilePicture: "", username: "User2" },
  ["3"]: { userId: "3", profilePicture: "", username: "User3" },
  ["4"]: { userId: "4", profilePicture: "", username: "User4" },
});

export type ChatProps = {};

export const Chat: Component<ChatProps> = (props) => {
  const [chats, { loadNextPage, sendMessage }] = useChatStore("channel1");

  const [message, setMessage] = createSignal("");

  return (
    <div class="p-4 flex flex-col flex-1">
      <div class="flex-1">
        <VList
          class="p-4"
          data={chats()}
          shift
          onScroll={async (offset) => {
            if (offset < 400) {
              await loadNextPage();
            }
          }}
        >
          {(data, index) => {
            return (
              <div class="pb-1 flex gap-4">
                <ChatProfile user={users()[data.author]} />
                <div>
                  {data.currentText} {data.isPending ? "Pending" : ""}
                </div>
              </div>
            );
          }}
        </VList>
      </div>
      <form
        onSubmit={async (e) => {
          e.preventDefault();
          const sendMessagePromise = sendMessage(message());
          setMessage("");
          await sendMessagePromise;
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
