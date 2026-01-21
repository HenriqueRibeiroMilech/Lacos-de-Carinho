import { Injectable, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PaymentApprovedEvent {
    preferenceId: string;
    userName: string;
    token: string;
    message: string;
}

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    private hubConnection: signalR.HubConnection | null = null;
    private paymentApproved$ = new Subject<PaymentApprovedEvent>();

    // Observable para componentes ouvirem
    get onPaymentApproved() {
        return this.paymentApproved$.asObservable();
    }

    /**
     * Conecta ao hub de pagamento e se inscreve para receber notificações
     */
    async connectToPaymentHub(preferenceId: string): Promise<void> {
        // Monta URL do hub (remove /api se existir)
        const baseUrl = environment.apiUrl.replace('/api', '');
        const hubUrl = `${baseUrl}/hubs/payment`;

        console.log('SignalR: Connecting to', hubUrl);

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Registra handler para evento de pagamento aprovado
        this.hubConnection.on('PaymentApproved', (data: PaymentApprovedEvent) => {
            console.log('SignalR: PaymentApproved received', data);
            this.paymentApproved$.next(data);
        });

        try {
            await this.hubConnection.start();
            console.log('SignalR: Connected successfully');

            // Entra no grupo específico do pagamento
            await this.hubConnection.invoke('JoinPaymentGroup', preferenceId);
            console.log('SignalR: Joined payment group', preferenceId);
        } catch (err) {
            console.error('SignalR: Connection error', err);
            throw err;
        }
    }

    /**
     * Desconecta do hub
     */
    async disconnect(): Promise<void> {
        if (this.hubConnection) {
            try {
                await this.hubConnection.stop();
                console.log('SignalR: Disconnected');
            } catch (err) {
                console.error('SignalR: Disconnect error', err);
            }
            this.hubConnection = null;
        }
    }

    /**
     * Verifica se está conectado
     */
    isConnected(): boolean {
        return this.hubConnection?.state === signalR.HubConnectionState.Connected;
    }
}
