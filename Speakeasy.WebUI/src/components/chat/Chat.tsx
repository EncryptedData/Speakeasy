import { Component, createSignal, Show } from "solid-js";
import { VList } from "virtua/solid";

import { User } from "../../models/User";
import { useChatStore } from "../../stores/chatStore";
import { TextField } from "../input/textField";
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
    <div class="flex flex-col flex-1 bg-bg-chat">
      <div class="flex-1">
        <VList
          data={chats()}
          shift
          onScroll={async (offset) => {
            if (offset < 400) {
              await loadNextPage();
            }
          }}
        >
          {(data, index) => {
            // No need to show the profile for several messages in a row
            const showProfile =
              index() > 0 && chats()[index() - 1].author != data.author;

            return (
              <div class="px-4 py-0.5 flex gap-4 hover:bg-bg-base-hover transition">
                <div style={{ width: "50px" }}>
                  <Show when={showProfile}>
                    <ChatProfile user={users()[data.author]} />
                  </Show>
                </div>
                <div>
                  {data.currentText} {data.isPending ? "Pending" : ""}
                </div>
              </div>
            );
          }}
        </VList>
      </div>
      <form
        class="p-4 flex"
        onSubmit={async (e) => {
          e.preventDefault();
          const sendMessagePromise = sendMessage(message());
          setMessage("");
          await sendMessagePromise;
        }}
      >
        <TextField
          class="flex-1"
          onChange={(e) => setMessage(e.currentTarget.value)}
          value={message()}
          placeholder="Enter a message..."
        />
        <button type="submit">send</button>
      </form>
    </div>
  );
};
