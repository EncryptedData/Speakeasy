import { Component } from "solid-js";
import { Channel } from "../../models/Channel";
import Accordion from "corvu/accordion"
import { VList } from "virtua";
import { ChannelButton } from "./ChannelButton";

export type ChannelGroupProps = {
    id: string,
    name: string,
    channels: Channel[],
    collapsed?: boolean,
};

export const ChannelGroupList: Component<ChannelGroupProps> = (props) => {
    return (
        <div class="flex flex-col">
            <Accordion collapseBehavior="hide">
                <Accordion.Item>
                    <h3>
                        <Accordion.Trigger>
                            {props.name}
                        </Accordion.Trigger>
                    </h3>
                    <Accordion.Content>
                        <VList data={props.channels}>
                            {(d: { id: string; name: string; description: string; }, i: any) => (
                                <ChannelButton id={d.id} name={d.name} description={d.description} />
                            )}
                        </VList>
                    </Accordion.Content>
                </Accordion.Item>
            </Accordion>
        </div>
    )
};
