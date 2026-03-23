import { Component } from "solid-js";
import { Channel } from "../../models/Channel";
import Accordion from "corvu/accordion"

export type ChannelGroupProps = {
    id: string,
    description: string,
    name: string,
    channels: Channel[],
};

export const ChannelGroup: Component<ChannelGroupProps> = (props) => {
    return (
        <Accordion>
            <Accordion.Item>
                <Accordion.Trigger>{props.name}</Accordion.Trigger>
            </Accordion.Item>
        </Accordion>
    )
};