import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  instance?: string;
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocurrió un error inesperado al procesar la solicitud.';

      if (error.error && typeof error.error === 'object') {
        const problem = error.error as ProblemDetails;
        errorMessage = problem.detail || problem.title || errorMessage;
      } else if (typeof error.error === 'string') {
        errorMessage = error.error;
      }

      console.error(`[HTTP Error ${error.status}]`, errorMessage);
      return throwError(() => new Error(errorMessage));
    })
  );
};
