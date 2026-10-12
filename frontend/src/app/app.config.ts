import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideZonelessChangeDetection } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { SpanishPaginatorIntl } from './core/i18n/spanish-paginator-intl';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZonelessChangeDetection(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch(), withInterceptors([errorInterceptor])),
    provideAnimationsAsync(),
    { provide: MatPaginatorIntl, useClass: SpanishPaginatorIntl }
  ],
};
