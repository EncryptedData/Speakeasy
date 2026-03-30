import { Component } from "solid-js";
import Popover from 'corvu/popover'

export type ProfileButtonProps = {
    profilePicture: string,
    name: string,
};

function onClick_button() {
    return;
}

export const ProfileButton: Component<ProfileButtonProps> = (props) => {
    return (
        <button class="flex flex-row">
            <img class="m-1" src={props.profilePicture} style={{ height: "32px", width: "32px" }} />
            <span class="m-1">{props.name}</span>
        </button>
    )
};