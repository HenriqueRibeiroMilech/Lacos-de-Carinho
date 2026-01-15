import { HttpErrorResponse, HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { inject } from "@angular/core";
import { UserAuthService } from "../services/user-auth";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";

export const authInterceptor = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const _userAuthService = inject(UserAuthService);
  const _router = inject(Router);

  const HAS_TOKEN = _userAuthService.getUserToken();
  if(HAS_TOKEN) {
    const newReq = req.clone({
      headers: req.headers.append('Authorization', `Bearer ${HAS_TOKEN}`)
    });

    return next(newReq).pipe(
      catchError((error) => {
        if (error instanceof HttpErrorResponse && (error.status === 401 || error.status === 403)) {
          _userAuthService.clearUserToken();
          _router.navigate(['/entrar']);
        }
        return throwError(() => error);
      })
    );
  }

  return next(req).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && (error.status === 401 || error.status === 403)) {
        _userAuthService.clearUserToken();
        _router.navigate(['/entrar']);
      }
      return throwError(() => error);
    })
  );
}
