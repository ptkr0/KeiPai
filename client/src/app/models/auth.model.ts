export interface IUser {
    id: string;
    email: string;
    username: string;
    role: string;
}

export interface ILogin {
    email: string;
    password: string;
}

export interface IRegisterInfluencer {
    username: string;
    email: string;
    password: string;
    language: string;
    contactEmail: string;
}

export interface IRegisterDeveloper {
    username: string;
    email: string;
    password: string;
    websiteUrl: string;
    contactEmail: string;
}