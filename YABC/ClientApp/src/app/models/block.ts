export interface Block {
    id: number;
    name: string;
    image: string;
    nonce: number;
    hash256: string;
    previousHash256: string;
    personId: number
}