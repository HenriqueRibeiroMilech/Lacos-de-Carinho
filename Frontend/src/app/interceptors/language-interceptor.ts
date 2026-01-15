import { HttpHandlerFn, HttpRequest } from "@angular/common/http";

export const languageInterceptor = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const newReq = req.clone({
    headers: req.headers.set('Accept-Language', 'pt-BR')
  });

  return next(newReq);
}
