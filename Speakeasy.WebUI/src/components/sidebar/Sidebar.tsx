import { Component, onMount, createUniqueId } from "solid-js";
import { createStore } from "solid-js/store";
import { VList } from "virtua/solid";
import { ChannelGroup } from "../../models/ChannelGroup";
import { User } from "../../models/User";
import { ProfileButton } from "./ProfileButton";
import { ChannelGroupList } from "./ChannelGroupList";

async function loadProfile() {
    const user: User = { 
        userId: createUniqueId(), // this is not what this method is for, but im lazy
        profilePicture: "../../assets/ph_profileIcon.svg",
        username: "someuser"
    }
    return user
}

async function loadChannels() {
    const outChannelGroups: ChannelGroup[] = [];
    outChannelGroups.push(
        { 
            id: createUniqueId(), 
            header: "--General--",
            channels: [
                { 
                    id: createUniqueId(), 
                    name: "General", 
                    description: "Default landing space. Any and all topics which don't fit existing channels.", 
                    order: 0 
                },
                { 
                    id: createUniqueId(), 
                    name: "Gaming", 
                    description: "Boom. Headshot.", 
                    order: 1 
                },
                { 
                    id: createUniqueId(), 
                    name: "Memes", 
                    description: "Darnk.", 
                    order: 2 
                },
            ]
        },
        { 
            id: createUniqueId(), 
            header: "--Tech--",
            channels: [
                { 
                    id: createUniqueId(), 
                    name: "Linux", 
                    description: "All things gray beard would approve of.", 
                    order: 0 
                }
            ]
        }
    )

    return outChannelGroups;
}

function onClick_profile() {
    return;
}

function onClick_channel() {
    return;
}

function onClick_addChannel() {
    
}

export type SidebarProps = {
    collapsed?: boolean
};

export const Sidebar: Component<SidebarProps> = (props) => {
    const [channels, setChannelGroups] = createStore<ChannelGroup[]>([]);
    const [profile, setProfile] = createStore<User>( { userId: "", profilePicture: "", username: "" } );
    
    onMount(async () => {
        const profile = await loadProfile();
        setProfile(profile);

        const channels = await loadChannels();
        setChannelGroups(channels);
    });

    return (
        <div id="sidebar" class="flex flex-col">
            <VList data={channels} class="flex flex-col">
                {(d, i) => (
                    <ChannelGroupList id={d.id} channels={d.channels} name={d.header} />
                )}
            </VList>
            <ProfileButton name={profile.username} profilePicture={profile.profilePicture}/>
        </div>
    );
};