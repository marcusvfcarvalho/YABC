import { Block } from "./block";

export interface NotificationMessage {
    messageType: number;
    block: Block;
}