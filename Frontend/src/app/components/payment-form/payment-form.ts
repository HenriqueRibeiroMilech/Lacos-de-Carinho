import { Component, EventEmitter, Input, OnInit, OnDestroy, Output, inject, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentService, IDirectPaymentResponse } from '../../services/payment';
import { UserAuthService } from '../../services/user-auth';
import { environment } from '../../../environments/environment';
import { take } from 'rxjs';

// TypeScript declaration for the MercadoPago SDK loaded via script tag
declare const MercadoPago: any;

@Component({
    selector: 'app-payment-form',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './payment-form.html',
    styleUrl: './payment-form.css'
})
export class PaymentForm implements OnInit, OnDestroy {
    /**
     * Tipo de pagamento: 'new_account' para cadastro, 'upgrade' para upgrade de conta
     */
    @Input() paymentType: 'new_account' | 'upgrade' = 'new_account';

    /**
     * Dados do usuário (para novo cadastro)
     */
    @Input() userName?: string;
    @Input() userEmail?: string;
    @Input() userPassword?: string;

    /**
     * Emitido quando o pagamento for aprovado
     */
    @Output() paymentApproved = new EventEmitter<IDirectPaymentResponse>();

    /**
     * Emitido quando o pagamento for rejeitado ou houver erro
     */
    @Output() paymentError = new EventEmitter<string>();

    /**
     * Emitido se o usuário quiser cancelar/voltar
     */
    @Output() cancelled = new EventEmitter<void>();

    private readonly _paymentService = inject(PaymentService);
    private readonly _userAuthService = inject(UserAuthService);
    private readonly _ngZone = inject(NgZone);

    // Estado do componente
    selectedMethod: 'card' | 'pix' = 'card';
    isProcessing = false;
    errorMessage = '';
    cardFormReady = false;

    // Dados do cartão
    cardNumber = '';
    cardExpiry = '';
    cardCvv = '';
    cardholderName = '';
    cardholderDocument = ''; // CPF
    installments = 1;

    // Parcelas disponíveis
    installmentOptions: {
        quantity: number;
        amount: number; // valor da parcela
        totalAmount: number; // valor total
        label: string;
    }[] = [];
    loadingInstallments = false;
    private lastBinChecked = '';

    // Dados do SDK
    private mp: any;
    private cardFormInstance: any;

    // Dados do Pix (retornados após processamento)
    pixData: {
        qrCode: string;
        qrCodeBase64: string;
        ticketUrl: string;
    } | null = null;

    // Resultado do pagamento
    paymentResult: IDirectPaymentResponse | null = null;

    ngOnInit() {
        this.initMercadoPago();
    }

    ngOnDestroy() {
        this.stopPixPolling();
    }

    private initMercadoPago() {
        try {
            this.mp = new MercadoPago(environment.mercadoPagoPublicKey, {
                locale: 'pt-BR'
            });
            this.cardFormReady = true;
        } catch (error) {
            console.error('Erro ao inicializar MercadoPago SDK:', error);
            this.errorMessage = 'Erro ao carregar sistema de pagamento. Recarregue a página.';
        }
    }

    selectMethod(method: 'card' | 'pix') {
        this.selectedMethod = method;
        this.resetPaymentState();
    }

    resetPaymentState() {
        this.errorMessage = '';
        this.pixData = null;
        this.paymentResult = null;
        this.pixPollingMessage = '';
        this.stopPixPolling();
    }

    async processPayment() {
        this.errorMessage = '';
        this.isProcessing = true;

        try {
            if (this.selectedMethod === 'pix') {
                await this.processPixPayment();
            } else {
                await this.processCardPayment();
            }
        } catch (error: any) {
            this.isProcessing = false;
            this.errorMessage = error.message || 'Erro ao processar pagamento.';
        }
    }

    private async processCardPayment() {
        // Validações básicas
        if (!this.cardNumber || !this.cardExpiry || !this.cardCvv || !this.cardholderName) {
            this.isProcessing = false;
            this.errorMessage = 'Preencha todos os campos do cartão.';
            return;
        }

        // Extrair mês/ano da validade
        const [expMonth, expYear] = this.cardExpiry.split('/').map(s => s.trim());
        if (!expMonth || !expYear) {
            this.isProcessing = false;
            this.errorMessage = 'Data de validade inválida. Use MM/AA.';
            return;
        }

        const fullYear = expYear.length === 2 ? `20${expYear}` : expYear;

        try {
            // Identificar o método de pagamento pelo número do cartão
            const bin = this.cardNumber.replace(/\s/g, '').substring(0, 6);
            const paymentMethodResult = await this.mp.getPaymentMethods({ bin });
            const paymentMethod = paymentMethodResult.results?.[0];

            if (!paymentMethod) {
                this.isProcessing = false;
                this.errorMessage = 'Cartão não reconhecido. Verifique o número.';
                return;
            }

            // Buscar emissor e usar o primeiro
            const issuerResult = await this.mp.getIssuers({
                paymentMethodId: paymentMethod.id,
                bin
            });
            const issuerId = issuerResult[0]?.id?.toString() || '';

            // Criar token do cartão (PCI-compliant)
            const tokenResult = await this.mp.createCardToken({
                cardNumber: this.cardNumber.replace(/\s/g, ''),
                cardholderName: this.cardholderName,
                cardExpirationMonth: expMonth,
                cardExpirationYear: fullYear,
                securityCode: this.cardCvv,
                identificationType: 'CPF',
                identificationNumber: this.cardholderDocument.replace(/\D/g, '')
            });

            if (!tokenResult?.id) {
                this.isProcessing = false;
                this.errorMessage = 'Erro ao processar dados do cartão. Verifique as informações.';
                return;
            }

            // Enviar para o backend
            const email = this.paymentType === 'upgrade'
                ? this._userAuthService.getDecodedToken()?.['email'] || ''
                : this.userEmail || '';

            this._paymentService.processDirectPayment({
                paymentType: this.paymentType,
                token: tokenResult.id,
                paymentMethodId: paymentMethod.id,
                issuerId: issuerId,
                installments: this.installments,
                payerEmail: email,
                name: this.userName,
                password: this.userPassword
            }).pipe(take(1)).subscribe({
                next: (response) => {
                    this._ngZone.run(() => {
                        this.isProcessing = false;
                        this.paymentResult = response;
                        this.handlePaymentResult(response);
                    });
                },
                error: (error) => {
                    this._ngZone.run(() => {
                        this.isProcessing = false;
                        const backendError = error?.error;
                        if (backendError?.errorMessages?.length) {
                            this.errorMessage = backendError.errorMessages[0];
                        } else {
                            this.errorMessage = 'Erro ao processar pagamento. Tente novamente.';
                        }
                        this.paymentError.emit(this.errorMessage);
                    });
                }
            });
        } catch (sdkError: any) {
            this.isProcessing = false;
            console.error('MercadoPago SDK error:', sdkError);

            // Mapear erros comuns do SDK
            if (sdkError?.message?.includes('cardNumber')) {
                this.errorMessage = 'Número do cartão inválido.';
            } else if (sdkError?.message?.includes('securityCode')) {
                this.errorMessage = 'Código de segurança inválido.';
            } else if (sdkError?.message?.includes('expirationDate')) {
                this.errorMessage = 'Data de validade inválida.';
            } else {
                this.errorMessage = 'Erro ao processar dados do cartão.';
            }
        }
    }

    private async processPixPayment() {
        const email = this.paymentType === 'upgrade'
            ? this._userAuthService.getDecodedToken()?.['email'] || ''
            : this.userEmail || '';

        this._paymentService.processDirectPayment({
            paymentType: this.paymentType,
            paymentMethodId: 'pix',
            issuerId: '',
            installments: 1,
            payerEmail: email,
            name: this.userName,
            password: this.userPassword
        }).pipe(take(1)).subscribe({
            next: (response) => {
                this._ngZone.run(() => {
                    this.isProcessing = false;

                    if (response.pixQrCode) {
                        this.pixData = {
                            qrCode: response.pixQrCode,
                            qrCodeBase64: response.pixQrCodeBase64 || '',
                            ticketUrl: response.ticketUrl || ''
                        };
                    }

                    if (response.status === 'approved') {
                        this.pixData = null;
                        this.paymentResult = response;
                        this.handlePaymentResult(response);
                    } else if (response.status === 'rejected') {
                        this.paymentResult = response;
                        this.handlePaymentResult(response);
                    } else if (response.paymentId) {
                        // Status pending: iniciar polling para verificar se o Pix foi pago
                        this.startPixPolling(response.paymentId);
                    }
                });
            },
            error: (error) => {
                this._ngZone.run(() => {
                    this.isProcessing = false;
                    const backendError = error?.error;
                    if (backendError?.errorMessages?.length) {
                        this.errorMessage = backendError.errorMessages[0];
                    } else {
                        this.errorMessage = 'Erro ao gerar Pix. Tente novamente.';
                    }
                    this.paymentError.emit(this.errorMessage);
                });
            }
        });
    }

    private pixPollingInterval: any = null;
    pixPollingMessage = '';

    private startPixPolling(paymentId: number) {
        this.pixPollingMessage = 'Aguardando pagamento...';
        let attempts = 0;
        const maxAttempts = 360; // 30 minutos (5s * 360)

        this.pixPollingInterval = setInterval(() => {
            attempts++;

            if (attempts > maxAttempts) {
                this.stopPixPolling();
                this.pixPollingMessage = 'Pix expirado. Gere um novo.';
                return;
            }

            this._paymentService.checkPixStatus(paymentId)
                .pipe(take(1))
                .subscribe({
                    next: (response) => {
                        this._ngZone.run(() => {
                            if (response.status === 'approved') {
                                this.stopPixPolling();
                                this.pixPollingMessage = '';
                                this.pixData = null;
                                this.paymentResult = response;
                                this.handlePaymentResult(response);
                            } else if (response.status === 'rejected') {
                                this.stopPixPolling();
                                this.pixPollingMessage = '';
                                this.pixData = null;
                                this.paymentResult = response;
                                this.handlePaymentResult(response);
                            }
                        });
                    },
                    error: () => {
                        // Silenciosamente ignora erros de polling
                    }
                });
        }, 5000);
    }

    private stopPixPolling() {
        if (this.pixPollingInterval) {
            clearInterval(this.pixPollingInterval);
            this.pixPollingInterval = null;
        }
    }

    private handlePaymentResult(response: IDirectPaymentResponse) {
        // Tag the response with the selected method
        response.paymentMethod = this.selectedMethod;

        if (response.status === 'approved') {
            this.paymentApproved.emit(response);
        } else if (response.status === 'rejected') {
            this.errorMessage = response.message;
            this.paymentError.emit(response.message);
        }
        // 'pending' / 'in_process' ficam na tela mostrando o Pix QR code
    }

    copyPixCode() {
        if (this.pixData?.qrCode) {
            navigator.clipboard.writeText(this.pixData.qrCode).then(() => {
                this.pixCopied = true;
                setTimeout(() => this.pixCopied = false, 3000);
            });
        }
    }

    pixCopied = false;

    // Formatação do número do cartão (adiciona espaços a cada 4 dígitos)
    formatCardNumber() {
        let value = this.cardNumber.replace(/\D/g, '');
        if (value.length > 16) value = value.substring(0, 16);
        this.cardNumber = value.replace(/(\d{4})(?=\d)/g, '$1 ');

        // Buscar parcelas quando o BIN (6 dígitos) estiver disponível
        const bin = value.substring(0, 6);
        if (bin.length === 6 && bin !== this.lastBinChecked) {
            this.lastBinChecked = bin;
            this.fetchInstallments(bin);
        } else if (bin.length < 6) {
            this.installmentOptions = [];
            this.installments = 1;
            this.lastBinChecked = '';
        }
    }

    private async fetchInstallments(bin: string) {
        if (!this.mp) return;
        this.loadingInstallments = true;

        try {
            const result = await this.mp.getInstallments({
                amount: '39.90',
                bin: bin,
                locale: 'pt-BR'
            });

            this._ngZone.run(() => {
                const plans = result?.[0]?.payer_costs || [];
                this.installmentOptions = plans.map((plan: any) => {
                    const qty = plan.installments;
                    const amount = plan.installment_amount;
                    const total = plan.total_amount;
                    const isNoInterest = total <= 39.91; // tolerância de centavo

                    let label: string;
                    if (qty === 1) {
                        label = `1x de R$ ${amount.toFixed(2).replace('.', ',')} sem juros`;
                    } else if (isNoInterest) {
                        label = `${qty}x de R$ ${amount.toFixed(2).replace('.', ',')} sem juros`;
                    } else {
                        label = `${qty}x de R$ ${amount.toFixed(2).replace('.', ',')} (Total: R$ ${total.toFixed(2).replace('.', ',')})`;
                    }

                    return { quantity: qty, amount, totalAmount: total, label };
                });

                // Default: 1x
                this.installments = 1;
                this.loadingInstallments = false;
            });
        } catch (error) {
            this._ngZone.run(() => {
                this.installmentOptions = [];
                this.installments = 1;
                this.loadingInstallments = false;
            });
        }
    }

    onInstallmentChange(event: Event) {
        const select = event.target as HTMLSelectElement;
        this.installments = parseInt(select.value, 10) || 1;
    }

    // Formatação da validade (MM/AA)
    formatExpiry() {
        let value = this.cardExpiry.replace(/\D/g, '');
        if (value.length > 4) value = value.substring(0, 4);
        if (value.length >= 2) {
            this.cardExpiry = value.substring(0, 2) + '/' + value.substring(2);
        } else {
            this.cardExpiry = value;
        }
    }

    // Formatação do CPF
    formatCpf() {
        let value = this.cardholderDocument.replace(/\D/g, '');
        if (value.length > 11) value = value.substring(0, 11);
        if (value.length > 9) {
            this.cardholderDocument = value.replace(/(\d{3})(\d{3})(\d{3})(\d{1,2})/, '$1.$2.$3-$4');
        } else if (value.length > 6) {
            this.cardholderDocument = value.replace(/(\d{3})(\d{3})(\d{1,3})/, '$1.$2.$3');
        } else if (value.length > 3) {
            this.cardholderDocument = value.replace(/(\d{3})(\d{1,3})/, '$1.$2');
        } else {
            this.cardholderDocument = value;
        }
    }

    goBack() {
        this.cancelled.emit();
    }
}
