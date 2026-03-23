import { Component } from "solid-js";

export type ChannelButtonProps = {
    id: string,
    description: string,
    name: string,
};

function onClick_channel() {
    return;
}

function onClick_channelMenu() {
    return;
}

export const ChannelButton: Component<ChannelButtonProps> = (props) => {
    return (
        <div class="flex flex-row">
            <button type="button" id={props.id} class="flex" onClick={onClick_channel}>
                {props.name}
            </button>
            <button type="button" class="flex" onClick={onClick_channelMenu}>
                ...
            </button>
        </div>
    )
};