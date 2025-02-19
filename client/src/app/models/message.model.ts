import { IBasicUserInfo } from "./user.model";

export interface IMessage {
    id: number;
    senderId: string;
    content: string;
    createdAt: string;
}

export interface IMessageUser {
    userId: string;
    username: string;
    lastMessage: string;
    lastMessageDate: string;
}

export interface IMessageResponse {
    user: IBasicUserInfo;
    messages: IMessage[];
}

export interface ISendMessage {
    receiverId: string;
    content: string;
}