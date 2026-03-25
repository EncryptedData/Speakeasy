import { expect, it } from "vitest";

import { useChatStore } from "./chatStore";

it("should have default values", () => {
  const [chats] = useChatStore("myMessageId");

  expect(chats()).not.toBeFalsy();
});
