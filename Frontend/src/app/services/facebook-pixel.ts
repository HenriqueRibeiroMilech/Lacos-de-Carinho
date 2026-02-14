import { Injectable } from '@angular/core';

declare let fbq: Function;

@Injectable({
    providedIn: 'root'
})
export class FacebookPixelService {

    constructor() { }

    public track(eventName: string, properties?: any) {
        if (typeof fbq === 'function') {
            fbq('track', eventName, properties);
        } else {
            console.warn('Facebook Pixel not loaded');
        }
    }

    public trackCustom(eventName: string, properties?: any) {
        if (typeof fbq === 'function') {
            fbq('trackCustom', eventName, properties);
        }
    }
}
