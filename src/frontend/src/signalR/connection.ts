import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hub')
    .withAutomaticReconnect()
    .build();

export async function startConnection() {
    await connection.start();
}

export { connection };
// build abstraction for simple usage, reconnect and re-authing
