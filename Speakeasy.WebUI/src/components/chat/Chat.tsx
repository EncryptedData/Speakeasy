import { FiSend } from "solid-icons/fi";
import {
  Accessor,
  Component,
  createEffect,
  createSignal,
  Show,
} from "solid-js";
import { Virtualizer, VirtualizerHandle } from "virtua/solid";
import "./chat.css";

import { useChatContextForChannel } from "@context/chatContext";
import { TextInput } from "../input/textInput";
import { ChatProfile } from "./ChatProfile";
import { useGroupState } from "@hooks/group";

export type ChatProps = {
  channelId: Accessor<string>;
};

export const Chat: Component<ChatProps> = (props) => {
  const chatContext = useChatContextForChannel(props.channelId);
  const chats = chatContext.messages;
  const groupState = useGroupState();
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

  function groupSelected() {
    return groupState.selectedGroup() !== undefined;
  }

  function channelSelected() {
    return groupState.selectedChannel() !== undefined;
  }

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
              index() === 0 || chats()[index() - 1]?.author != data.author;

            return (
              <div class="px-4 py-0.5 flex gap-4 hover:bg-bg-base-hover transition">
                <div class="profile">
                  <Show when={showProfile}>
                    <ChatProfile userId={data.author} />
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
        <div class="size-full p-2 text-center" hidden={(groupSelected() && channelSelected())}>
            Select a group and channel to begin chatting...
        </div>
        <div class="flex-1 flex chat__input" hidden={!(groupSelected() && channelSelected())}>
          <TextInput
            class="flex-1 pr-12 bg-bg-base active:bg-bg-surface"
            onChange={(e) => setMessage(e.currentTarget.value)}
            value={message()}
            placeholder="Enter a message..."
          />
          <button
            class="p-2 rounded hover:bg-bg-surface-hover active:bg-bg-elevated-hover disabled:bg-bg-surface transition-colors"
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
