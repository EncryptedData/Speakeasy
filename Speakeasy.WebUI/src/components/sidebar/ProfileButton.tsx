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
        <button type="menu" class="" onClick={onClick_button}>
            <img src={props.profilePicture} style={{ height: "32px", width: "32px"}}></img>
            <div>{props.name}</div>
        </button>
        </>
    )
};