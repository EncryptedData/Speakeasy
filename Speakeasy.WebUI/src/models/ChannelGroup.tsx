import { Channel } from "./Channel"

export type ChannelGroup = {
    id: string;
    header: string;
    channels: Channel[];
};