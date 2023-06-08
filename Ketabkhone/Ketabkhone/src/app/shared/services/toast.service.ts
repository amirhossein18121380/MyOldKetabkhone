import { Injectable } from '@angular/core';
import {Router} from "@angular/router";
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
    
  constructor(private toastr: ToastrService) {}

  showSuccess(title: string, message: string) {
    return this.toastr.success(message, title);
  }

  showSuccess2(message: string) {
    return this.toastr.success(message);
  }

  showWarning(title: string, message: string) {
   return this.toastr.warning(message, title);
  }

  showError(title: string, message: string) {
   return this.toastr.error(message, title);
  }

  showError2(message: string) {
    return this.toastr.error(message);
   }

  showInfo(title: string, message: string) {
   return this.toastr.info(message, title);
  }
}
