import * as signalR from '@microsoft/signalr';
import { useAuthStore } from '@/stores/authStore';

class SignalRConnection {
    private static instance: SignalRConnection;
    private hubConnection!: signalR.HubConnection;
    private authStore = useAuthStore();
    private previousAuthToken: string | null = null;

    constructor() {
        if (SignalRConnection.instance === undefined) {
            this.hubConnection = new signalR.HubConnectionBuilder()
                .withUrl('/chathub', {
                    accessTokenFactory: () => {
                        if (this.authStore.accessToken === this.previousAuthToken) {
                            this.authStore.refreshAccessToken();
                        }
                        if (!this.authStore.accessToken) {
                            throw new Error('Access token is null');
                        }
                        this.previousAuthToken = this.authStore.accessToken;
                        return this.authStore.accessToken;
                    },
                })
                .withAutomaticReconnect()
                .build();

            this.startConnection();
            SignalRConnection.instance = this;
        } else {
            return SignalRConnection.instance;
        }
    }

    private startConnection() {
        try {
            this.hubConnection.start();
        } catch (error) {
            console.log(error);
            // this is sus we should at some point exit this and throw and error instead of trying to reconnect forever I
            setTimeout(() => this.startConnection(), 5000);
        }
    }

    public get connection(): signalR.HubConnection {
        if (!SignalRConnection.instance) {
            throw new Error('SignalRConnection instance is not initialized.');
        }

        if (
            SignalRConnection.instance.hubConnection.state !== signalR.HubConnectionState.Connected
        ) {
            throw new Error('Failed to connect to SignalR hub.');
        }

        return SignalRConnection.instance.hubConnection;
    }
}

export default SignalRConnection;
