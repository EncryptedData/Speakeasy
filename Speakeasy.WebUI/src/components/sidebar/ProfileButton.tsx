import { Component } from "solid-js";

export type ProfileButtonProps = {
    profilePicture: string,
    name: string,
};

function onClick_button() {
    return;
}

export const ProfileButton: Component<ProfileButtonProps> = (props) => {
    return (
        <>
        <button type="menu" class="flex flex-row" onClick={onClick_button}>
            <img class="flex svg-primary" src={props.profilePicture} style={{ stroke: "var(--color-text-primary)", height: "32px", width: "32px"}}></img>
            <p class="flex">{props.name}</p>
        </button>
        </>
    )
};