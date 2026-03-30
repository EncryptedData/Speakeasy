import { Component } from "solid-js";
import { Channel } from "../../models/Channel";
import Accordion from "corvu/accordion"
import { VList } from "virtua";
import { ChannelButton } from "./ChannelButton";
import Disclosure from 'corvu/disclosure'

export type ChannelGroupProps = {
    id: string,
    name: string,
    channels: Channel[],
    collapsed?: boolean,
};

export const ChannelGroupList: Component<ChannelGroupProps> = (props) => {
    return (
        <div class="mt-8">
            <Disclosure collapseBehavior="hide">
                {(d) => (
                    <>
                        <div class="mb-2 flex items-center justify-between space-x-4">
                            <p class="font-medium text-corvu-text-dark">{props.name}</p>
                            <Disclosure.Trigger class="rounded-lg bg-corvu-100 p-1 transition-all duration-100 hover:bg-corvu-200 active:translate-y-0.5">
                                {d.expanded ? (
                                    <>
                                        <span class="sr-only">Collapse</span>
                                    </>
                                ) : (
                                    <>
                                        <span class="sr-only">Expand</span>
                                    </>
                                )}
                            </Disclosure.Trigger>
                        </div>

                        <Disclosure.Content class="mt-1 space-y-1 overflow-hidden data-expanded:animate-expand data-collapsed:animate-collapse">
                            <div class="grid gap-2">
                                {props.channels.map((ch) => (
                                    <button
                                        type="button"
                                        class="w-full text-left rounded-lg px-3 py-2 hover:bg-corvu-400"

                                        onClick={() => {
                                            /* handle channel click if needed */
                                        }}
                                    >
                                        {ch.name}
                                    </button>
                                ))}
                            </div>
                        </Disclosure.Content>
                    </>
                )}
            </Disclosure>
        </div>
    );
};
