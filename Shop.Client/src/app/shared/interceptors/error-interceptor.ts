import { HttpInterceptorFn } from '@angular/common/http';
import { catchError } from 'rxjs';
import { ErrorService } from '../services/error.service';
import { inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  /*Class
    └── constructor DI

    Function (interceptor)
    └── inject() DI*/
  const errorService = inject(ErrorService);

  //next(req)가 Observable을 반환해.
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      console.log('status', err.status);
      console.log('error body:', err.error);

      errorService.setHttpError(err);
      
      throw err;
      /*
      Interceptor
      → 공통 에러 UI 처리
      → 원래 호출자에게도 에러 전달*/
    })
  );
};
