export type ChatMessage = {
  id: string;
  author: string;
  createdOn: Date;
  currentText: string;
  isPending?: boolean;
  lastEditedOn?: Date;
  didSendFail?: boolean;
};
