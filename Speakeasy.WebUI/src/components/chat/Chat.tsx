import "./chat.css";
import {
  Accessor,
  Component,
  createEffect,
  createSignal,
  Show,
} from "solid-js";
import { Virtualizer, VirtualizerHandle } from "virtua/solid";
import { FiSend } from "solid-icons/fi";

import { User } from "@models/User";
import { TextInput } from "../input/textInput";
import { ChatProfile } from "./ChatProfile";
import { useChatContextForChannel } from "@context/chatContext";

const [users] = createSignal<Record<string, User>>({
  ["me"]: { userId: "1", profilePicture: "", username: "User1" },
  ["1"]: { userId: "1", profilePicture: "", username: "User1" },
  ["2"]: { userId: "2", profilePicture: "", username: "User2" },
  ["3"]: { userId: "3", profilePicture: "", username: "User3" },
  ["4"]: { userId: "4", profilePicture: "", username: "User4" },
});

export type ChatProps = {
  channelId: Accessor<string>;
};

export const Chat: Component<ChatProps> = (props) => {
  const chatContext = useChatContextForChannel(props.channelId);
  const chats = chatContext.messages;

  const [message, setMessage] = createSignal("");
  const [shift, setShift] = createSignal(false);
  const [virtualizerHandle, setVirtualizerHandle] = createSignal<
    VirtualizerHandle | undefined
  >();
  const [shouldStickToBottom, setShouldStickToBottom] = createSignal(true);

  createEffect(() => {
    const handle = virtualizerHandle();
    if (!handle) return;
    const lastItemIndex = chats().length - 1;
    if (shouldStickToBottom()) {
      handle.scrollToIndex(lastItemIndex, { align: "end" });
    }
  });

  createEffect(() => {
    chats();
    setShift(false);
  });

  return (
    <div class=" bg-bg-chat flex flex-col chat__container">
      <div class="flex flex-col flex-1 chat__virtualizer">
        <div class="grow" />
        <Virtualizer
          ref={setVirtualizerHandle}
          data={chats()}
          shift={shift()}
          onScroll={async (offset) => {
            const handle = virtualizerHandle();
            if (!handle) return;
            setShouldStickToBottom(
              offset - handle.scrollSize + handle.viewportSize >= -1.5,
            );

            if (offset < 400) {
              // todo: manage race conditions here
              setShift(true);
              await chatContext.loadMessages();
            }
          }}
        >
          {(data, index) => {
            // No need to show the profile for several messages in a row
            const showProfile =
              index() > 0 && chats()[index() - 1]?.author != data.author;

            return (
              <div class="px-4 py-0.5 flex gap-4 hover:bg-bg-base-hover transition">
                <div style={{ width: "50px", "min-width": "50px" }}>
                  <Show when={showProfile}>
                    <ChatProfile user={users()[data.author]!} />
                  </Show>
                </div>
                <div>
                  {data.currentText}
                  {data.isPending ? "Pending" : ""}
                </div>
              </div>
            );
          }}
        </Virtualizer>
      </div>
      <form
        class="p-4 flex"
        onSubmit={(e) => {
          e.preventDefault();

          setShouldStickToBottom(true);
          chatContext.sendMessage(message());
          setMessage("");
        }}
      >
        <div class="flex-1 flex chat__input">
          <TextInput
            class="flex-1 pr-12"
            onChange={(e) => setMessage(e.currentTarget.value)}
            value={message()}
            placeholder="Enter a message..."
          />
          <button
            class="p-2 rounded bg-bg-surface hover:bg-bg-surface-hover active:bg-bg-elevated-hover transition-colors"
            disabled={!message()}
            type="submit"
          >
            <FiSend />
          </button>
        </div>
      </form>
    </div>
  );
};
